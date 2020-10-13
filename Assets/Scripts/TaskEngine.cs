using System;
using System.Collections.Generic;
//using UnityEditor.VersionControl;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;
using Assets.Scripts;
using System.Collections;
using UnityEngine.SceneManagement;



// This is the logging for the reversal blocks
/*
 * Log the time at the start of the block
 * Log the time at the start of an attempt
 * Log the time right after they see the x for the reversal
 * Log the time for the check in the reversal block
 * Log the times for any other xs seen in the task (log the attempt number along with the x)
 * Log any triggers seen from the scanner
 * Log the time at the end of the attempt
 * Log the time for the end of the block
 */

// This is the logging for the baseline block
/*
 * Log the time that it switches from the baseline to the box car
 * 
 */

// TODO: Make sure the reversal does not choose the same spot

public class TaskEngine : MonoBehaviour {
	
	private TaskSettings _gameManager;
    private TimeManager _timer;
    private KeyboardHandler _keyStroke;

    #region Public Visual Game Objects
    public GameObject Rect1;
	public GameObject Rect2;
	public GameObject Rect3;
	public GameObject Rect4;
	public GameObject RectOutline1;
	public GameObject RectOutline2;
	public GameObject RectOutline3;
	public GameObject RectOutline4;
	public GameObject Check1;
	public GameObject Check2;
	public GameObject Check3;
	public GameObject Check4;
	public GameObject Cross1;
	public GameObject Cross2;
	public GameObject Cross3;
	public GameObject Cross4;
	public GameObject Hourglass;
	#endregion

	#region Private State Variables (some public variables here as well)

	private IPerfLog _logger = new FileSystemLogger();
	private TaskState _currentTaskState = TaskState.StartTrial;
	private TrialType _trialType = TrialType.Both;
	public BlockType CurrentBlockType = BlockType.Baseline;


																	// DEFINITION OF TERMS//
    // iteration = everytime the paradigm is presented to the subject (every correct response leads to a new iteration of the paradigm)  
    // attempt = everytime the subject makes a response (every iteration will have at least one attempt - a correct attempt will lead to a new iteration)
    // trial = the alloted time period within which to complete a maximum number of iterations/reversal blocks

	private int _scanCount = 0; //varabile to track number of scans that occur
    private int _reversalIterationCount = 0; //variable to keep tabs on the number of times the paradigm is presented 
    private int _attemptCount = 0; //variable to keep total number of attempts (counts both correct and incorrect responses)
    private int _correctAttemptCount = 0; // compiles the number of correct asnwers (everytime a check is presented)
    private int _incorrectAttemptCount = 0; //compiles the number of incorrect answers (everytime an x is presented) 
    private int _incorrectAttemptOnReversalCount = 0; // compiles the number of incorrect answers in a reversal condition 
    private int _choiceAttempt = 0; //variable to track number of attempts 
    private int _interReversalIterationCount = 0; // number of iterations in between reversal blocks
    private int _interReversalIterationNumber = 4; // randomly set the number of iterations within each reversal block
    private int _baselineAttemptCount = 0;
    private int baselineCorrectAttemptCount = 0;
    private int baselineIncorrectAttemptCount = 0;
    private int baselineIterationCount = 0;
    private float baselineResponseTime;
    private float _totalBaselineResponseTime = 0;
    private float averageBaselineResponseTime = 0;
    private float _iterationStartTime; //takes time point at the presentation of every new iteration. 
    private float _iterationEndTime; // takes time following correct response (end of an iteration). 
    private float _responseTime; // equal to = (_iterationEndTime - _iterationlStartTime). calculates response time between correct responses 
    private float _totalResponseTime = 0; // complies total  _responseTime to calculate average
    private float _reversalResponseTime = 0; // the _responseTime which corresponds to time in between responses in a reversal condition
    private float _reversalFlag; // marker for when reversal occurs
    private float _totalReversalResponseTime = 0; // compiles _reversalResponseTime to get total response time in the reversal condition
    private float _averageReversalResponseTime = 0; // average response time to reversal condition  
    private float _averageResponseTime = 0; // average response time over course of the whole trial
    private int _reversalCount = 0; //variable to keep track of total number of reversals

    public DateTime BlockTimerStart = DateTime.Now;//timer for block design condition. 
	//public DateTime blockTimerEnd = DateTime.Now;//timer for block design condition. 
	private DateTime _scanTime = DateTime.Now;//time tag for each scan
    private DateTime _scannerDelayStart = DateTime.Now; //Time stamp soon as RUN is pressed
    private DateTime _scannerDelayEnd = DateTime.Now;//Time stamp soon as first scanner input  
    private float _currentTrialLength = 0;//current trial length (sec). Taking the time since RUN press minus scannerdelay to get actual task time length
    public float ScannerDelay = 0; //scannerdelaystart - scannerdelay end to get length of scanner delay
    private int _baselineBlockLength = 0; //the length in sec of the baseline blocks (set by user)
    private int _reversalBlockLength = 0; //the length in sec of the reversal blocks (set by user)
    private int _blockLengthLimit = 30; //variable that holds current block length, set by _baselineBlockLength or _reversalBlockLength
    private int _trialLengthLimit = 540;// number of seconds the trial will span. can be changed by user on config screen

    private ItemChoice _currentChoice = ItemChoice.None;
	private ItemChoice _correctChoice = ItemChoice.None;
	private ItemChoice _previousCorrectChoice = ItemChoice.None;
	private List<ItemChoice> _previousChoices;

	private GameObject[] _rectsFilled;
	private GameObject[] _rectsOutlined;
	private GameObject[] _checks;
	private GameObject[] _crosses;
	
    #endregion

	void Awake()
	{
        _gameManager = TaskSettingsManager.TaskSettings;
        int.TryParse(_gameManager.SetBaselineBlockLength, out _baselineBlockLength);
        int.TryParse(_gameManager.SetReversalBlockLength, out _reversalBlockLength);
        int.TryParse(_gameManager.TrialLengthLimit, out _trialLengthLimit);

        if (_gameManager.EnableBaselineBlock && _gameManager.EnableReversalBlock)
	{
			_trialType = TrialType.Both;
	}
				
	if (_gameManager.EnableBaselineBlock && !_gameManager.EnableReversalBlock)
	{
			_trialType = TrialType.Baseline;
	}

	if (_gameManager.EnableReversalBlock && !_gameManager.EnableBaselineBlock)
	{
			_trialType = TrialType.Reversal;
	}

	if (_gameManager.EnableBaselineBlock == false && _gameManager.EnableReversalBlock == false)
	{
			_trialType = TrialType.Reversal;
	}

		Random.seed = (int)( Time.time * 10000 );
		
		_rectsFilled = new GameObject[] {Rect1, Rect2, Rect3, Rect4};
		_rectsOutlined = new GameObject[] {RectOutline1, RectOutline2, RectOutline3, RectOutline4};
		_checks = new GameObject[]{Check1, Check2, Check3, Check4};
		_crosses = new GameObject[]{Cross1, Cross2, Cross3, Cross4};
		
		_currentTaskState = TaskState.StartTrial;

		if (_gameManager.EnableTriggerOnScanner == true)
		{
			_currentTaskState = TaskState.WaitForTrigger;
            _scannerDelayStart = DateTime.Now;
		}
	
		Debug.Log(TaskSettingsManager.TaskSettings.SubjectId);
	}

	#region Screen Object Management

	void StartBaseline ()
	{
		foreach(var outline in _rectsOutlined)
			outline.SetActive(true);
		foreach(var filled in _rectsFilled)
			filled.SetActive(false);
	}

	void StartReversal ()
	{
		foreach (var outline in _rectsOutlined)
			outline.SetActive(false);
		foreach (var filled in _rectsFilled)
			filled.SetActive(true);
	}

	void ClearFeedback ()
	{
		foreach (var check in _checks)
			check.SetActive(false);
		foreach (var cross in _crosses)
			cross.SetActive(false);
	}
	
	void ClearScreen ()
	{
		foreach (var outline in _rectsOutlined)
			outline.SetActive(false);
		foreach (var filled in _rectsFilled)
			filled.SetActive(false);
		foreach (var check in _checks)
			check.SetActive(false);
		foreach (var cross in _crosses)
			cross.SetActive(false);

		_choiceAttempt = 1;
	}

	void DisplayHint()
	{
			var choiceIdx = ((int) _correctChoice);
			_rectsFilled[choiceIdx].SetActive(true);
	}

	void UpdateDisplayForChoice(ItemChoice chosenItem, ChoiceResult result)
	{
		var choiceIdx = ((int)chosenItem);

		if(result == ChoiceResult.Correct)
			_checks[choiceIdx].SetActive(true);
		else
			_crosses[choiceIdx].SetActive(true);
	}

    #endregion

    public void TriggerOn()
    {
        if (_currentTaskState == TaskState.WaitForTrigger)
        {
            _scannerDelayEnd = DateTime.Now;
            ScannerDelay = (_scannerDelayEnd - _scannerDelayStart).Seconds;

            Hourglass.SetActive(false);
            _currentTaskState = TaskState.StartTrial;
        }

        if (KeyboardHandler.Trigger)
        {
            _scanCount++;
            _scanTime = DateTime.Now;

            string _sT = _scanTime.ToString("yyyyMMdd-HHmmss.fff");

            _logger.LogScan(_scanCount + " , " + _sT);
        }

    }

    public void AbortTrial()
    {
        _averageResponseTime = _totalResponseTime / _reversalIterationCount; // average response time to get correct answer over course of the whole trial
        _averageReversalResponseTime = _totalReversalResponseTime / _reversalCount;// average response time to reversal condition
        averageBaselineResponseTime = _totalBaselineResponseTime / baselineIterationCount; // baseline block metrics

        _logger.LogReversalSummary("ReversalIterationCount , trialLength , averageResponseTime , reversalCount , averageReversalResponseTime" + Environment.NewLine + _reversalIterationCount + " , " + _timer.CurrentTrialLength + " , " + _averageResponseTime + " , " + _reversalCount + " , " + _averageReversalResponseTime); // summery metrics for whole trial
        _logger.LogBaselineSummary("BaselineIterationCount , trialLength , baselineCorrectCount , baselineIncorrectCount , averageBaselineResponseTime" + Environment.NewLine + baselineIterationCount + " , " + _timer.CurrentTrialLength + " , " + baselineCorrectAttemptCount + " , " + baselineIncorrectAttemptCount + " , " + averageBaselineResponseTime);
        _logger.Term();
    }

    // Use this for initialization
    void Start () 
	{
        // TODO: If missing event id, subject id, etc then go back to config screen
        // Get the task parameters setup here...
		ClearScreen();
        _timer = FindObjectOfType<TimeManager>();

	}

	public void ItemChosen(ItemChoice selectedItem)
	{
		if(_currentTaskState == TaskState.ProcessChoice)
			_currentChoice = selectedItem;
	}

	void SetCorrectAnswerToPrevious()
	{
		_correctChoice = _previousCorrectChoice;
	}

	void SetCorrectAnswerRandomly()
	{
		var itms = Enum.GetValues(typeof (ItemChoice));
		_correctChoice = (ItemChoice)itms.GetValue( Random.Range(0, 4) );  // Yes, 1, 4 is correct.  Idx 0 is none.
	}

	void SetCorrectAnswerBasedOnChoiceProb(ItemChoice choice)
	{	
		var probability = 0;

		// Don't count the previous item as an incorrect attempt
		if (_currentChoice == _previousCorrectChoice)
		{
			_correctChoice = ItemChoice.None;
			return;
		}
		
		// Process a new choice and use a distribution to determine they were correct
		switch(_choiceAttempt)
		{
		case 0:
			probability = 15; // For the first response 15% of the time it will be correct
			break;
		case 1:
			probability = 33; // For the second response it is 33%
			break;
		case 2:
			probability = 100; // For the third choice it is always correct
			break;
		}

		if (Random.Range(1, 100) <= probability)
		{
			_correctChoice = choice;
			_choiceAttempt = 0;
			_interReversalIterationCount = 0;
		}
		else
		{
			_choiceAttempt++;
			_correctChoice = ItemChoice.None;
		}
	}

	void Update () 
	{
        //Asking TimeManger if we are at end of trial...
        if (_timer.EndTrial == true)
        {
            _currentTaskState = TaskState.EndTrial;
        }

        //asking TimeManager if we are at end of a block...
        if(_trialType == TrialType.Both)
        {
            if (_timer.SwitchBlock == true)
            {
                if (CurrentBlockType == BlockType.Baseline)
                {
                    CurrentBlockType = BlockType.Reversal;
                    _currentTaskState = TaskState.StartBlock;
                }
                else if (CurrentBlockType == BlockType.Reversal)
                {
                    CurrentBlockType = BlockType.Baseline;
                    _currentTaskState = TaskState.StartBlock;
                }
            }
        }
 

        // MAIN STATE MACHINE
        switch (_currentTaskState)
		{
			case TaskState.WaitForTrigger:

				if (!Hourglass.activeInHierarchy)
                {
                    Hourglass.SetActive(true);
                }
					
				break;

			case TaskState.StartTrial:
				
				ClearFeedback ();
                _logger.Init();

				switch (_trialType)
				{
					case TrialType.Both:
						CurrentBlockType = BlockType.Baseline;
						break;
					case TrialType.Baseline:
						CurrentBlockType = BlockType.Baseline;
						break;
					case TrialType.Reversal:
						CurrentBlockType = BlockType.Reversal;
						break;
				}

				_currentTaskState = TaskState.StartBlock;

				break;

			case TaskState.StartBlock:

				switch (CurrentBlockType)
				{
					case BlockType.Baseline:
						StartBaseline();
						_blockLengthLimit = _baselineBlockLength;
                        break;

					case BlockType.Reversal:
						StartReversal();
						_blockLengthLimit = _reversalBlockLength;
						break; 
				}

                //start the block timer here, check end in Timer script
				BlockTimerStart = DateTime.Now;
				_currentTaskState = TaskState.StartIteration;

				break;

			case TaskState.StartIteration:

				if (CurrentBlockType == BlockType.Baseline)
				{
					ClearFeedback();
					StartBaseline();
				}

				if (CurrentBlockType == BlockType.Reversal)
				{
					ClearFeedback();
					StartReversal();
				}


                _previousChoices = new List<ItemChoice>();

				if(CurrentBlockType == BlockType.Baseline || _previousCorrectChoice == ItemChoice.None)
					SetCorrectAnswerRandomly();
				else
					SetCorrectAnswerToPrevious();

				if (CurrentBlockType == BlockType.Baseline)
					DisplayHint();

				_iterationStartTime = Time.time; //start time of the iteration 
                _currentTaskState = TaskState.ProcessChoice;
                
				break;


			case TaskState.ProcessChoice:

				// Filter out no choice or repeated choices
				if (_currentChoice != ItemChoice.None && !_previousChoices.Contains(_currentChoice))
				{
					_previousChoices.Add(_currentChoice);

                    if (_previousCorrectChoice == ItemChoice.None) 
                    {
                    }

					if (CurrentBlockType == BlockType.Reversal && _interReversalIterationCount > _interReversalIterationNumber)// **RANDOMIZED # OF ITERATIONS BETWEEN REVERSAL BLOCKS**
                    {
                        SetCorrectAnswerBasedOnChoiceProb(_currentChoice);
                        _reversalFlag = 1; //marks occurance of reversals
                    }
						
					if (_currentChoice == _correctChoice)
					{
						UpdateDisplayForChoice(_currentChoice, ChoiceResult.Correct);

						if (CurrentBlockType == BlockType.Reversal)
						{
							_attemptCount++; //tally for total number of responses
							_correctAttemptCount++; //tally for totaly number of CORRECT responses 
						}

						if (CurrentBlockType == BlockType.Baseline)
						{
							_baselineAttemptCount++;
							baselineCorrectAttemptCount++;
						}
							_iterationEndTime = Time.time;
							_currentTaskState = TaskState.ReinforceIteration;
					}

					else
					{
						UpdateDisplayForChoice(_currentChoice, ChoiceResult.Incorrect);

						if (CurrentBlockType == BlockType.Reversal)
						{
							_attemptCount++; //tally for total number of responses
							_incorrectAttemptCount++; //tally for totaly number of INCORRECT responses

							if (_reversalFlag == 1)
							{
								_incorrectAttemptOnReversalCount++; // tally for the number of INCORRECT responses in a REVERSAL condition 
							}
						}

						if (CurrentBlockType == BlockType.Baseline)
						{
							_baselineAttemptCount++;
							baselineIncorrectAttemptCount++;
						}

						_currentTaskState = TaskState.ProcessChoice;
					}

					_currentChoice = ItemChoice.None;
				}
				break;

			case TaskState.ReinforceIteration:

				var tmpTime = Time.time;
				_currentTaskState = tmpTime - _iterationEndTime >= 1.5 ? TaskState.EndIteration : TaskState.ReinforceIteration; //FIX THIS : MAKE THE DELAY CONFIGURABLE 

				break;

			case TaskState.EndIteration:

				//blockTimerEnd = DateTime.Now;
                _currentTrialLength = Time.timeSinceLevelLoad - ScannerDelay;

                _previousCorrectChoice = _correctChoice;
                _interReversalIterationCount++;

                _logger.LogRaw(_reversalIterationCount + " , " + _attemptCount + " , " + _correctAttemptCount + " , " + _incorrectAttemptCount + " , " + _incorrectAttemptOnReversalCount + " , " + _responseTime +
                    " , " + _reversalCount + " , " + _reversalResponseTime + " , " + baselineIterationCount + " , " + baselineResponseTime + " , " + _baselineAttemptCount + " , " + baselineCorrectAttemptCount + " , "
                    + baselineIncorrectAttemptCount);

                if (CurrentBlockType == BlockType.Reversal)
				{
					_reversalIterationCount++; //tally of the number of iterations presented
					_responseTime = _iterationEndTime - _iterationStartTime; //calculating time in between CORRECT responses (response time)
					_totalResponseTime = _totalResponseTime + _responseTime; //total response time compiler. compiles the time it takes to get the correct answer (the time taken to complete iteration) --> used to calculate average

					if (_reversalFlag == 1) // checking if reversal has occured and pulling that time frame from _responseTime
					{
						_interReversalIterationNumber = Random.Range(4, 7);
						_reversalResponseTime = _responseTime;
						_totalReversalResponseTime = _totalReversalResponseTime + _reversalResponseTime; //compiling total response time in reversal situation
						_reversalCount++; //compiles number of reversals as they occur
						_reversalFlag = 0; //resets reversal marker
					}
				}

				if (CurrentBlockType == BlockType.Baseline) //baseline block metrics 
				{
					baselineIterationCount++;
					baselineResponseTime = _iterationEndTime - _iterationStartTime;
					_totalBaselineResponseTime = _totalBaselineResponseTime + baselineResponseTime;
				}

				if (_trialType != TrialType.Both)
				{
					_currentTaskState = TaskState.StartIteration;
				}

				//if (_trialType == TrialType.All && ((blockTimerEnd - blockTimerStart).Seconds > blockLengthLimit))
				//{
				//	_currentTaskState = TaskState.EndBlock;
				//}

				else _currentTaskState = TaskState.StartIteration;

    //            if (_trialLength > _trialLengthLimit) //**SET THIS TO A GIVEN TIME PERIOD (9 MIN WAS DISCUSSED - 540 sec)** 
				//{
				//	_currentTaskState = TaskState.EndTrial;
				//}

				break;
            
			case TaskState.EndBlock:

				if (CurrentBlockType == BlockType.Baseline)
				{
					CurrentBlockType = BlockType.Reversal;
				}

				else if (CurrentBlockType == BlockType.Reversal)
				{
					CurrentBlockType = BlockType.Baseline;
				}

				_currentTaskState = TaskState.StartBlock;
				break;

			case TaskState.EndTrial:

				_averageResponseTime = _totalResponseTime / _reversalIterationCount; // average response time to get correct answer over course of the whole trial
				_averageReversalResponseTime = _totalReversalResponseTime / _reversalCount;// average response time to reversal condition
				averageBaselineResponseTime = _totalBaselineResponseTime / baselineIterationCount; // baseline block metrics

                _logger.LogReversalSummary("ReversalIterationCount , trialDuration , averageResponseTime , reversalCount , averageReversalResponseTime" + Environment.NewLine + _reversalIterationCount + " , " + _timer.CurrentTrialLength + " , " + _averageResponseTime + " , " + _reversalCount + " , " + _averageReversalResponseTime); // summery metrics for whole trial
                _logger.LogBaselineSummary("BaselineIterationCount , trialDuration , baselineCorrectCount , baselineIncorrectCount , averageBaselineResponseTime" + Environment.NewLine + baselineIterationCount + " , " + _timer.CurrentTrialLength + " , " + baselineCorrectAttemptCount + " , " + baselineIncorrectAttemptCount + " , " + averageBaselineResponseTime);
                _logger.Term();

                SceneManager.LoadScene("GameEndScreen_Stimulation"); 

				break;
		}
	}

    public string SubjectID { get; set; }
}

public enum TaskState
{
	WaitForTrigger,
	StartTrial,
	StartBlock,
	StartIteration,
	ProcessChoice,
	ReinforceIteration,
	EndIteration,
	EndBlock,
	EndTrial,
	Term
}
// Make sure none is always first otherwise the random correct choice will break!!!
public enum ItemChoice
{
		None = -1, 
		Item1, 
		Item2, 
		Item3, 
		Item4
}
public enum ChoiceResult
{
	Correct, Incorrect
}
public enum BlockType
{
	Baseline,
	Reversal,
}
public enum TrialType
{
	Baseline,
	Reversal,
	Both
}