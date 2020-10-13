using System;
using System.Collections.Generic;
//using UnityEditor.VersionControl;
using UnityEngine;
using System.IO;
using Assets.Scripts;
using UnityEngine.SceneManagement;

public class SetupDisplayManager : MonoBehaviour
{
	public void Return()
	{
		SceneManager.LoadScene("Configure_Scanner");
	}
}
