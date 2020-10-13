
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskSettingsManager : MonoBehaviour {

    public static TaskSettings TaskSettings = new TaskSettings();

	void Awake () 
    {
	}

	void Start () 
	{
		DontDestroyOnLoad(this);
	}

	void Update () 
	{
	}
}

public class TaskSettings
{
    public bool EnableBaselineBlock = false;
	public bool EnableReversalBlock = true;
    public bool EnableTriggerOnScanner = false;
    public string TrialLengthLimit = "5";
    public string SetReversalBlockLength = "39";
	public string SetBaselineBlockLength = "19";
    public string SubjectId;
    public string EventId;
	public string Item01KeyVal;
	public string Item02KeyVal;
	public string Item03KeyVal;
	public string Item04KeyVal;
    public string AbortTrialKeyVal = "x";
	public string TriggerKeyVal = "5";
}