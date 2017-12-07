using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float speedMultiplier;
	private float speedMultiplierPrivate;
	public float sprintMultiplier;
	Matrix4x4 baseMatrix;
	public static bool timeFrozen = false;
	[HideInInspector]
	public Rigidbody rb;
	public static bool hasKey = false;
	public static int keyLevel = 0;

	private Vector3 LKposition = new Vector3 (0.0f, 0.0f, 0.0f);

	void OnEnable(){
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable(){
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
		
	void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		hasKey = false;
	}

	// Used to calibrate the Input.acceleration
	public void CalibrateAccelerometer()
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
		baseMatrix = matrix.inverse;
	}

	// Use this for initialization
	void Start () {
		speedMultiplierPrivate = speedMultiplier;
		rb = GetComponent<Rigidbody>();
		CalibrateAccelerometer ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 Move = Vector3.zero;
		Vector3 fixedAcceleration = this.baseMatrix.MultiplyVector (Input.acceleration);
		//Debug.Log ("Fixed: " + fixedAcceleration);
		//Debug.Log ("Original: " + Input.acceleration);
		//fixedAcceleration.z *= -1;
		rb.velocity = fixedAcceleration * speedMultiplier;

		float moveVert = Input.GetAxis ("Vertical");
		float moveHoriz = Input.GetAxis ("Horizontal");
		if (moveVert != 0.0f || moveHoriz != 0.0f) {

			Move = new Vector3 (moveHoriz, 0.0f, moveVert);

			rb.velocity = Move * speedMultiplier;
			//rb.AddForce (Move * speedMultiplier,ForceMode.VelocityChange);
		}
		if (LKposition == gameObject.transform.position)
			timeFrozen = true;
		else
			timeFrozen = false;
		
		LKposition = gameObject.transform.position;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "PickUp")
		{
			other.gameObject.SetActive(false);
			hasKey = true;
			if (other.GetComponent<KeyKey> () != null) {
				KeyKey temp = other.GetComponent(typeof(KeyKey)) as KeyKey;
				keyLevel = temp.Key;
			}

			//Debug.Log("Has Key");
		}
	}
	void OnGUI(){
		GUIStyle sprintStyle = new GUIStyle ("button");
		sprintStyle.fontSize = 15;
		//Sprint button (repeat does on hold)
		if(GUI.RepeatButton(new Rect(Screen.width/10*9, Screen.height/20*16, Screen.width/10, Screen.width/20), "Sprint", sprintStyle)){
			speedMultiplier = sprintMultiplier;
		}else{
			speedMultiplier = speedMultiplierPrivate;
		}
	}
}