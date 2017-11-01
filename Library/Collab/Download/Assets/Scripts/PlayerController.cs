using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speedMultiplier;
	public Rigidbody rb;
	Matrix4x4 baseMatrix = Matrix4x4.identity;

	// Used to calibrate the Input.acceleration
	void CalibrateAccelerometer()
	{
		/*Currently only works in quadrants 2&4
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
		Vector3 fixedAcceleration = this.baseMatrix.MultiplyVector (Input.acceleration);
		Debug.Log ("Fixed: " + fixedAcceleration);
		Debug.Log ("Original: " + Accel);
		rb.velocity = fixedAcceleration * speedMultiplier;
	}

	void OnGUI(){
		//Calibrate button
		if (GUI.Button(new Rect(Screen.width/10*9, Screen.height/20*18, Screen.width/10, Screen.width/20), "Calibrate")) {
			CalibrateAccelerometer ();
			Debug.Log ("Calibrating");
		}

		//Sprint button (repeat does on hold)
		if(GUI.RepeatButton(new Rect(Screen.width/10*9, Screen.height/20*16, Screen.width/10, Screen.width/20), "Sprint")){
			speedMultiplier = 80f;
		}else{
			speedMultiplier = 20f;
		}
	}
}