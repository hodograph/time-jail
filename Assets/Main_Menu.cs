using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){
		GUIStyle titleStyle = new GUIStyle ();
		titleStyle.fontSize = 40;

		if(GUI.Button(new Rect(Screen.width/2-Screen.width/20, Screen.height/2-Screen.height/40, Screen.width/10, Screen.height/20), "START"))
			SceneManager.LoadScene ("Level One", LoadSceneMode.Single);
		GUI.Label (new Rect (Screen.width / 2 - Screen.width / 10, Screen.height / 4, Screen.width / 5, Screen.height / 10), "Time-Jail", titleStyle);

	}
}
