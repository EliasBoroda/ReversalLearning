using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;
using Assets.Scripts;
using System.Collections;


//want to start running this soon as that first scan is sent...or soon as run is pressed?
public class TimeManager : MonoBehaviour {

    private TaskSettings _gameManager;
    private TaskEngine _taskEngine;

    private int _baselineBlockLength;
    private int _reversalBlockLength;
    private int _trialLengthLimit;
    private DateTime _blockTimerEnd;
    private float _currentBlockLength;
    private int _currentBlockLengthLimit = 0;

    public float CurrentTrialLength = 0;
    public bool EndTrial = false;
    public bool SwitchBlock = false;

    void Awake ()
    {
        _gameManager = TaskSettingsManager.TaskSettings;
        int.TryParse(_gameManager.SetBaselineBlockLength, out _baselineBlockLength);
        int.TryParse(_gameManager.SetReversalBlockLength, out _reversalBlockLength);
        int.TryParse(_gameManager.TrialLengthLimit, out _trialLengthLimit);

        EndTrial = false;
        SwitchBlock = false;

        _currentBlockLengthLimit = _baselineBlockLength;
    }

    // Use this for initialization
    void Start()
    {
        _taskEngine = FindObjectOfType<TaskEngine>();
    }

    void Update ()
    {
        TrialLengthManager();
        BlockLengthManager();
    }

    //method that tracks Trial Length
    public void TrialLengthManager()
    {
        CurrentTrialLength = (Time.timeSinceLevelLoad - _taskEngine.ScannerDelay);

        if (CurrentTrialLength > _trialLengthLimit)
        {
            EndTrial = true;
        }
    }

    //method to control block timing and trigger switch 
    public void BlockLengthManager()
    {
        SwitchBlock = false;

        if (_taskEngine.CurrentBlockType == BlockType.Baseline)
        {
            _currentBlockLengthLimit = _baselineBlockLength;
        }
        if (_taskEngine.CurrentBlockType == BlockType.Reversal)
        {
            _currentBlockLengthLimit = _reversalBlockLength;
        }


        _blockTimerEnd = DateTime.Now;
        _currentBlockLength = (_blockTimerEnd - _taskEngine.BlockTimerStart).Seconds;

        if (_currentBlockLength > _currentBlockLengthLimit)
        {
            SwitchBlock = true;
        }
    }
}
