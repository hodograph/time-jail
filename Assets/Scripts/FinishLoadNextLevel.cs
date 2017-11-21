using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLoadNextLevel : MonoBehaviour {

	
	void Start () {
		Debug.Log("Finish Script Started");

	}
	
	void OnTriggerEnter(Collider other)
    {

		if (other.gameObject.tag == "Player") 
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}

}
