using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;

public class KeyboardHandler : MonoBehaviour {

    private TaskEngine _taskEngine;
    private ConfigureViewModel _configView;
    private TimeManager _timer;

    public static bool Trigger;

	// Use this for initialization
	void Start ()
    {
        _taskEngine = FindObjectOfType<TaskEngine>();
        _configView = FindObjectOfType<ConfigureViewModel>();
    }

	void ProcessKeyboardChoice()  // TODO: Really need to use the input manager to help with this; may consider using rewired instead
	{
		try
		{
			//wait for choice
			var choice1 = Input.GetKeyDown(TaskSettingsManager.TaskSettings.Item01KeyVal);
			if (choice1)
				_taskEngine.ItemChosen(ItemChoice.Item1);

			var choice2 = Input.GetKeyDown(TaskSettingsManager.TaskSettings.Item02KeyVal);
			if (choice2)
				_taskEngine.ItemChosen(ItemChoice.Item2);

			var choice3 = Input.GetKeyDown(TaskSettingsManager.TaskSettings.Item03KeyVal);
			if (choice3)
				_taskEngine.ItemChosen(ItemChoice.Item3);

			var choice4 = Input.GetKeyDown(TaskSettingsManager.TaskSettings.Item04KeyVal);
			if (choice4)
				_taskEngine.ItemChosen(ItemChoice.Item4);

			Trigger = Input.GetKeyDown(TaskSettingsManager.TaskSettings.TriggerKeyVal);
			if (Trigger && TaskSettingsManager.TaskSettings.EnableTriggerOnScanner)
            {
                _taskEngine.TriggerOn();
            }
				
            var abortTrial = Input.GetKeyDown(TaskSettingsManager.TaskSettings.AbortTrialKeyVal);
            if (abortTrial)
            {
                _configView.Abort();
                _taskEngine.AbortTrial();
            }

		}
	
		catch (System.Exception e)
		{
			int i = 0;
			Debug.Log(e);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		ProcessKeyboardChoice();
	}
}
