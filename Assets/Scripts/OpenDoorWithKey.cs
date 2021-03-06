﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OpenDoorWithKey : MonoBehaviour {
 
    public GameObject Door;
	public int level = 0;
	private PlayerController playerController;
	
	private bool isLocked;
	private bool playerNearby;
	private bool isOpen;
	private bool showLockedBool;
	private bool showLockedGUI;
	private bool showLockedLevelGUI;

 
 
	private float currentTime = 0.0f;
	private float executedTime = 0.0f;
	private float timeToWait = 3.0f;
 
    void Start() {
 
		//Debug.Log("Door Script Started");
		isLocked = true;
		playerNearby = false;
		isOpen = false;
		showLockedBool = false;
		showLockedGUI = false;
		showLockedLevelGUI = false;

    }
 
    void OnTriggerEnter(Collider other)
    {
		//Debug.Log("Player near door");
		
		if (other.gameObject.tag == "Player") 
		{
			playerNearby = true;
			executedTime = Time.time;
		}
	}
	
	void OnTriggerExit(Collider other)
    {
		//Debug.Log("Player left door");
		
		if (other.gameObject.tag == "Player") 
		{
			playerNearby = false;
		}
	}
	
	void Update()
	{
		if (showLockedBool) {
			if (level > PlayerController.keyLevel) {
				showLockedLevelGUI = true;
				showLockedGUI = false;

			} else {
				showLockedGUI = true;
				showLockedLevelGUI = false;
			}
		}

		else {
			showLockedGUI = false;
			showLockedLevelGUI = false;
			}

		if(executedTime != 0.0f && !PlayerController.hasKey){
			showLockedBool = true;
			if (currentTime - executedTime >= timeToWait) {
				executedTime = 0.0f;
				showLockedBool = false;
			}
		}

		currentTime = Time.time;
		if(playerNearby && !isOpen)
		{
			if (PlayerController.hasKey) {
				if (PlayerController.keyLevel >= level) {
					isLocked = false;
				}
			}

			
			if (!isLocked) {
				isOpen = true;
				//Debug.Log("Door Opening");
			
				transform.Rotate (0, -90, 0);
			}
		}	
	}

	void OnGUI(){
		GUIStyle keyNeeded = new GUIStyle("Label");
		keyNeeded.fontSize = 40;
		keyNeeded.alignment = TextAnchor.UpperCenter;
		if (showLockedLevelGUI) {
			GUI.Label (new Rect (Screen.width / 5, Screen.height / 10, Screen.width / 5 * 4, Screen.height / 5), string.Concat("Need Level ", level.ToString(), " Key To Open Door"), keyNeeded);
		}
		else if(showLockedGUI)
			GUI.Label (new Rect (Screen.width/5, Screen.height/10, Screen.width/5*4, Screen.height/5), "Need Key To Open Door", keyNeeded);

	}
}
