using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardCollider : MonoBehaviour {

	private PlayerController PlayerController;

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Player") {
			DeathScreen DS =  GameObject.FindObjectOfType(typeof(DeathScreen)) as DeathScreen;
			DS.Pause ();
		}
	}

}
