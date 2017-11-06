using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speedMultiplier;
	Matrix4x4 baseMatrix = Matrix4x4.identity;
	public static bool timeFrozen = false;
	[HideInInspector]
	public Rigidbody rb;



	// Used to calibrate the Input.acceleration
	void CalibrateAccelerometer()
	{
		/*Currently only works if calibrate is clicked in quadrant 2
		 * if clicked in 2, zeros in 2
		 * if clicked in 3, zeros in 1
		 * if clicked in 1, zeros flipped in 1 (up is down, down is up)
		 * 
		 *                  UP
		 *                  ^
		                    |
		             1      |       2
		                    |
		                    |
		Back <--------------+------------- -> Person/Screen facing
		                    |
		                    |
		             4      |      3
		                    |
		                    V
		                   Down
		  Not sure how or why this works.
		*/
		Vector3 Accel = Input.acceleration;
		Accel.z *= -1;
		Quaternion rotate = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), Accel);
		Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotate, new Vector3(1.0f, 1.0f, 1.0f));
		this.baseMatrix = matrix.inverse;
	}

	// Use this for initialization
	void Start () {

		speedMultiplier = 20f;

		rb = GetComponent<Rigidbody>();
		CalibrateAccelerometer ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 Move = Vector3.zero;
		Vector3 fixedAcceleration = this.baseMatrix.MultiplyVector (Input.acceleration);
		//Debug.Log ("Fixed: " + fixedAcceleration);
		//Debug.Log ("Original: " + Input.acceleration);
		rb.velocity = fixedAcceleration * speedMultiplier;

		float moveVert = Input.GetAxis ("Vertical");
		float moveHoriz = Input.GetAxis ("Horizontal");
		if (moveVert != 0.0f || moveHoriz != 0.0f) {

			Move = new Vector3 (moveHoriz, 0.0f, moveVert);

			rb.velocity = Move * speedMultiplier;
			//rb.AddForce (Move * speedMultiplier,ForceMode.VelocityChange);
		}
		if (fixedAcceleration == Vector3.zero && Move == Vector3.zero)
			timeFrozen = true;
		else
			timeFrozen = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "PickUp")
		{
			other.gameObject.SetActive(false);
		}
	}
	void OnGUI(){
		GUIStyle calibrateStyle = new GUIStyle ("button");
		calibrateStyle.fontSize = 15;
		//Calibrate button
		if (GUI.Button(new Rect(Screen.width/10*9, Screen.height/20*18, Screen.width/10, Screen.width/20), "Calibrate", calibrateStyle)) {
			CalibrateAccelerometer ();
			Debug.Log ("Calibrating");
		}

		GUIStyle sprintStyle = new GUIStyle ("button");
		sprintStyle.fontSize = 15;
		//Sprint button (repeat does on hold)
		if(GUI.RepeatButton(new Rect(Screen.width/10*9, Screen.height/20*16, Screen.width/10, Screen.width/20), "Sprint", sprintStyle)){
			speedMultiplier = 80f;
		}else{
			speedMultiplier = 20f;
		}
	}
}