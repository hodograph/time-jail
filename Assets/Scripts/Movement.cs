using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Gyroscope controller that works with any device orientation.

public class Movement : MonoBehaviour 
{
	#region [Private fields]

	private bool gyroEnabled = true;
	private const float lowPassFilterFactor = 0.2f;

	private readonly Quaternion baseIdentity =  Quaternion.Euler(90, 0, 0);
	private readonly Quaternion landscapeRight =  Quaternion.Euler(0, 0, 90);
	private readonly Quaternion landscapeLeft =  Quaternion.Euler(0, 0, -90);
	private readonly Quaternion upsideDown =  Quaternion.Euler(0, 0, 180);

	private Quaternion cameraBase =  Quaternion.identity;
	private Quaternion calibration =  Quaternion.identity;
	private Quaternion baseOrientation =  Quaternion.Euler(90, 0, 0);
	private Quaternion baseOrientationRotationFix =  Quaternion.identity;



	private Quaternion referanceRotation = Quaternion.identity;
	private bool debug = true;

	#endregion

	#region [Unity events]

	protected void Start () 
	{
		AttachGyro();
	}

	protected void Update() 
	{
		if (!gyroEnabled)
			return;
		transform.rotation = Quaternion.Slerp(transform.rotation,
			cameraBase * ( ConvertRotation(referanceRotation * Input.gyro.attitude) * GetRotFix()), lowPassFilterFactor);
	}

	protected void OnGUI()
	{
		if (!debug)
			return;

		GUILayout.Label("Orientation: " + Screen.orientation);
		GUILayout.Label("Calibration: " + calibration);
		GUILayout.Label("Camera base: " + cameraBase);
		GUILayout.Label("input.gyro.attitude: " + Input.gyro.attitude);
		GUILayout.Label("transform.rotation: " + transform.rotation);

		if (GUILayout.Button("On/off gyro: " + Input.gyro.enabled, GUILayout.Height(100)))
		{
			Input.gyro.enabled = !Input.gyro.enabled;
		}

		if (GUILayout.Button("On/off gyro controller: " + gyroEnabled, GUILayout.Height(100)))
		{
			if (gyroEnabled)
			{
				DetachGyro();
			}
			else
			{
				AttachGyro();
			}
		}

		if (GUILayout.Button("Update gyro calibration (Horizontal only)", GUILayout.Height(80)))
		{
			UpdateCalibration(true);
		}


		if (GUILayout.Button("Reset base orientation", GUILayout.Height(80)))
		{
			ResetBaseOrientation();
		}

		if (GUILayout.Button("Reset camera rotation", GUILayout.Height(80)))
		{
			transform.rotation = Quaternion.identity;
		}

		if (GUILayout.Button("Update camera base rotation (Horizontal only)", GUILayout.Height(80)))
		{
			UpdateCameraBaseRotation(true);
		}

		if (GUILayout.Button("Close", GUILayout.Height(80)))
		{
			debug = false;
		}
	}

	#endregion

	#region [Public methods]
	// Attaches gyro controller to the transform.
	private void AttachGyro()
	{
		gyroEnabled = true;
		ResetBaseOrientation();
		UpdateCalibration(true);
		UpdateCameraBaseRotation(true);
		RecalculateReferenceRotation();
	}

	// Detaches gyro controller from the transform
	private void DetachGyro()
	{
		gyroEnabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "PickUp")
		{
			other.gameObject.SetActive(false);
			hasKey = true;
			//Debug.Log("Has Key");
		}
	}

	#endregion

	#region [Private methods]

	// Update the gyro calibration.

	private void UpdateCalibration(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = (Input.gyro.attitude) * (-Vector3.forward);
			fw.z = 0;
			if (fw == Vector3.zero)
			{
				calibration = Quaternion.identity;
			}
			else
			{
				calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
			}
		}
		else
		{
			calibration = Input.gyro.attitude;
		}
	}

	/// Update the camera base rotation.

	/// <param name='onlyHorizontal'>
	/// Only y rotation.

	private void UpdateCameraBaseRotation(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = transform.forward;
			fw.y = 0;
			if (fw == Vector3.zero)
			{
				cameraBase = Quaternion.identity;
			}
			else
			{
				cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
			}
		}
		else
		{
			cameraBase = transform.rotation;
		}
	}

	private static Quaternion ConvertRotation(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);	
	}

	// Recalculates reference system.
	private void ResetBaseOrientation()
	{
		baseOrientationRotationFix = GetRotFix();
		baseOrientation = baseOrientationRotationFix * baseIdentity;
	}

	// Recalculates reference rotation.
	private void RecalculateReferenceRotation()
	{
		referanceRotation = Quaternion.Inverse(baseOrientation)*Quaternion.Inverse(calibration);
	}

	#endregion
}