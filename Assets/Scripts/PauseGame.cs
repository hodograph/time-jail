using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour {
	public Transform canvas;	

	// Update is called once per frame
	void OnGUI(){
		GUIStyle pauseStyle = new GUIStyle ("button");
		pauseStyle.fontSize = 15;

		//Pause button
		if (GUI.Button(new Rect(Screen.width/10*9, Screen.height/20*18, Screen.width/10, Screen.width/20), "Pause", pauseStyle)) {
			Pause ();
		}
	}

	//Pause/resume
	public void Pause(){ 
		if (canvas.gameObject.activeInHierarchy == false) {
			canvas.gameObject.SetActive (true);
			Time.timeScale = 0;
		} else {
			canvas.gameObject.SetActive (false);
			Time.timeScale = 1;
		}
	}

	//Main menu 
	public void LoadMenu(){
		SceneManager.LoadScene (0);
		Time.timeScale = 1;
	}

	//Calibrate
	public void CalibrateAccelerometer()
	{
		PlayerController PC =  GameObject.FindObjectOfType(typeof(PlayerController)) as PlayerController;
		Time.timeScale = 1;
		PC.CalibrateAccelerometer ();
		Time.timeScale = 0;

	}
}