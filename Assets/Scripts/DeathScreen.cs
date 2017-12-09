using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour {
	public Transform canvas;
	public GameObject player;

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
	public void Restart()
	{
		if (SceneManager.GetActiveScene ().buildIndex == 5) {
			player.transform.position = new Vector3 (1, -.37f, 1);
			canvas.gameObject.SetActive (false);

		} else {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
		Time.timeScale = 1;

	}
}
