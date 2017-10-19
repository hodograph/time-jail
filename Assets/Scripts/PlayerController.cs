using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speedMultiplier;

	// Use this for initialization
	void Start () {

		speedMultiplier = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		float moveVert = Input.GetAxis ("Vertical");
		float moveHoriz = Input.GetAxis ("Horizontal");

		Vector3 Move = new Vector3 (moveHoriz, 0.0f, moveVert);

		transform.Translate(Move * speedMultiplier);
		
	}
}
