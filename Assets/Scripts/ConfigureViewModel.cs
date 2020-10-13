using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class ConfigureViewModel : MonoBehaviour
{
    public GameObject ConfigForm;

    public InputField SubjectId;
    public InputField EventId;
    //public InputField ReversalBlockLength;
    //public InputField BaselineBlockLength;
    //public InputField TrialLengthLimit;
    public InputField Button1;
    public InputField Button2;
    public InputField Button3;
    public InputField Button4;
    //public InputField TriggerVal;
    public InputField AbortTrialKeyVal;
    //public Toggle ReversalBlock;
    //public Toggle BaselineBlock;
    //public Toggle TriggerOnScanner;

    public void Awake() //method to preserve user input into the configure screen
    {
        var subId = PlayerPrefs.GetString("SubjectId");
        var eventId = PlayerPrefs.GetString("EventId");
        //var reversalBlockLength = PlayerPrefs.GetString("ReversalBlockLength");
        //var baselineBlockLength = PlayerPrefs.GetString("BaselineBlockLength");
        //var trialLengthLimit = PlayerPrefs.GetString("TrialLengthLimit");
        var button1 = PlayerPrefs.GetString("Button1");
        var button2 = PlayerPrefs.GetString("Button2");
        var button3 = PlayerPrefs.GetString("Button3");
        var button4 = PlayerPrefs.GetString("Button4");
        //var trigger = PlayerPrefs.GetString("TriggerVal");
        var abortTrialKeyVal = PlayerPrefs.GetString("AbortTrialKeyVal");

        if (!string.IsNullOrEmpty(subId))
            SubjectId.text = subId;
        else
            SubjectId.text = "Null";

        if (!string.IsNullOrEmpty(eventId))
            EventId.text = eventId;
        else
            EventId.text = "Null";

        //if (!string.IsNullOrEmpty(reversalBlockLength))
        //    ReversalBlockLength.text = reversalBlockLength;
        //else
        //    ReversalBlockLength.text = "40";

        //if (!string.IsNullOrEmpty(baselineBlockLength))
        //    BaselineBlockLength.text = baselineBlockLength;
        //else
        //    BaselineBlockLength.text = "20";

        //if (!string.IsNullOrEmpty(trialLengthLimit))
        //    TrialLengthLimit.text = trialLengthLimit;
        //else
        //    TrialLengthLimit.text = "540";

        if (!string.IsNullOrEmpty(button1))
            Button1.text = button1;
        else
            Button1.text = "1";

        if (!string.IsNullOrEmpty(button2))
            Button2.text = button2;
        else
            Button2.text = "2";

        if (!string.IsNullOrEmpty(button3))
            Button3.text = button3;
        else
            Button3.text = "3";

        if (!string.IsNullOrEmpty(button4))
            Button4.text = button4;
        else
            Button4.text = "4";

        //if (!string.IsNullOrEmpty(trigger))
        //    TriggerVal.text = trigger;
        //else
        //    TriggerVal.text = "5";

        if (!string.IsNullOrEmpty(abortTrialKeyVal))
            AbortTrialKeyVal.text = abortTrialKeyVal;
        else
            AbortTrialKeyVal.text = "x";
    }

    public void Run()
    {

        Debug.Log("Run Called");
        SceneManager.LoadScene("TaskMain");

        Destroy(ConfigForm);
    }

    public void SetSubjectId(string subjectId)
    {
        TaskSettingsManager.TaskSettings.SubjectId = subjectId;
        PlayerPrefs.SetString("SubjectId", subjectId);
    }

    public void SetEventId(string eventId)
    {
        TaskSettingsManager.TaskSettings.EventId = eventId;
        PlayerPrefs.SetString("EventId", eventId);
    }

    //public void EnableReversalBlock(bool val)
    //{
    //    TaskSettingsManager.TaskSettings.EnableReversalBlock = val;
    //}

    //public void SetReversalBlockLength(string reversalBlockLength)
    //{
    //    TaskSettingsManager.TaskSettings.SetReversalBlockLength = reversalBlockLength;
    //    PlayerPrefs.SetString("ReversalBlockLength", reversalBlockLength);
    //}

    //public void EnableBaselineBlock(bool val)
    //{
    //    TaskSettingsManager.TaskSettings.EnableBaselineBlock = val;
    //}

    //public void SetBaselineBlockLength(string baselineBlockLength)
    //{
    //    TaskSettingsManager.TaskSettings.SetBaselineBlockLength = baselineBlockLength;
    //    PlayerPrefs.SetString("BaselineBlockLength", baselineBlockLength);
    //}

    //public void EnableTriggerOnScanner(bool val)
    //{
    //    TaskSettingsManager.TaskSettings.EnableTriggerOnScanner = val;     
    //}

    //public void SetTrialLength(string trialLengthLimit)
    //{
    //    TaskSettingsManager.TaskSettings.TrialLengthLimit = trialLengthLimit;
    //    PlayerPrefs.SetString("TrialLength", trialLengthLimit);
    //}

    public void SetItem01KeyVal(string item01KeyVal)
    {
        TaskSettingsManager.TaskSettings.Item01KeyVal = item01KeyVal;
        PlayerPrefs.SetString("Button1", item01KeyVal);
    }

    public void SetItem02KeyVal(string item02KeyVal)
    {
        TaskSettingsManager.TaskSettings.Item02KeyVal = item02KeyVal;
        PlayerPrefs.SetString("Button2", item02KeyVal);
    }

    public void SetItem03KeyVal(string item03KeyVal)
    {
        TaskSettingsManager.TaskSettings.Item03KeyVal = item03KeyVal;
        PlayerPrefs.SetString("Button3", item03KeyVal);
    }

    public void SetItem04KeyVal(string item04KeyVal)
    {
        TaskSettingsManager.TaskSettings.Item04KeyVal = item04KeyVal;
        PlayerPrefs.SetString("Button4", item04KeyVal);
    }

    //public void SetTriggerKeyVal(string triggerKeyVal) //scanner trigger value
    //{
    //    TaskSettingsManager.TaskSettings.TriggerKeyVal = triggerKeyVal;
    //    PlayerPrefs.SetString("TriggerVal", triggerKeyVal);
    //}

    public void SetAbortTrialKeyVal(string abortTrialKeyVal)
    {
        TaskSettingsManager.TaskSettings.AbortTrialKeyVal = abortTrialKeyVal;
        PlayerPrefs.SetString("AbortTrialKeyVal", abortTrialKeyVal);
    }

    public void Quit()
    {
        Debug.Log("Quit Called");
        Application.Quit();
    }

    public void SetupDisplay()
    {
        Debug.Log("Setup Screen Called");
        UnityEngine.SceneManagement.SceneManager.LoadScene("SetupDisplayScreen");
    }

    public void Return()
    {
        Debug.Log("Return to Config");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Configure_Scanner");
    }

    public void Abort()
    {
        Debug.Log("Aborting Trial...Return to Config");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Configure_Scanner");
    }
}
