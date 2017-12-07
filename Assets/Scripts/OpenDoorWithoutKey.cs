using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OpenDoorWithoutKey : MonoBehaviour {
 
    public GameObject Door;
	private PlayerController playerController;
	
	private bool playerNearby;
	private bool isOpen;
 
    void Start() {
 
		//Debug.Log("Door Script Started");
		playerNearby = false;
		isOpen = false;
    }
 
    void OnTriggerEnter(Collider other)
    {
		//Debug.Log("Player near door");
		
		if (other.gameObject.tag == "Player") 
		{
			playerNearby = true;
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
		if(playerNearby && !isOpen)
		{
			isOpen = true;
			//Debug.Log("Door Opening");
			
			transform.Rotate (0, 90, 0);
		}
	}	
}


