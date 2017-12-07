using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuccessScreen : MonoBehaviour {
	public Transform canvas;	

	//Pause/resume
	public void Pause(){ 
		string key = "Level" + SceneManager.GetActiveScene().buildIndex;
		PlayerPrefs.SetInt (key, 1);
		PlayerPrefs.Save ();
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
	public void NextLevel()
	{
		if (SceneManager.GetActiveScene().buildIndex == 5)
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		else
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
		
		Time.timeScale = 1;

	}
}
