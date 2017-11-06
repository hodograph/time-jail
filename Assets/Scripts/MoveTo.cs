using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour {

	public Transform goal;
	public NavMeshAgent agent;
	private float movingspeed;
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		movingspeed = agent.speed; 
		agent.destination = goal.position; 
	}

	void Update () {
		if (PlayerController.timeFrozen)
			agent.speed = 0;
		else
			agent.speed = movingspeed;
			

	}
}