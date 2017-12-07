using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLoadNextLevel : MonoBehaviour {
	
	void Start () {
		//Debug.Log("Finish Script Started");

	}
	
	void OnTriggerEnter(Collider other)
    {

		if (other.gameObject.tag == "Player") 
		{
			SuccessScreen SS =  GameObject.FindObjectOfType(typeof(SuccessScreen)) as SuccessScreen;
			SS.Pause ();
		}
	}

}
