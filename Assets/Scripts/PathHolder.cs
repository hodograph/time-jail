using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side {
	ZPlus,
	ZMinus,
	XPlus,
	XMinus
};

public class PathHolder : MonoBehaviour {

	public Side[] mySides;
	[HideInInspector]
	public Transform myTransform;
	public BoxCollider myCollider;


	// Use this for initialization
	void Start () {

		myTransform = gameObject.transform;
		myCollider = GetComponent<BoxCollider>();

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
