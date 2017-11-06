

using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public enum Direction {
	Clockwise,
	CounterClockwise
};


public class Patrol : MonoBehaviour {

	public PathHolder[] Pathwalls;
	public Vector3[] pointsForCurrentWall;
	public int destPoint = 0;
	public int currentWall = 0;
	public NavMeshAgent agent;
	public float movingspeed;
	public float distfromwall;

	public Direction myDirection;


	void Start () {
		agent = GetComponent<NavMeshAgent>();
		movingspeed = agent.speed;
		agent.autoBraking = false;


		currentWall = Pathwalls.Length-1;
		GoToNextWall();
	}

	void GoToNextWall() {
		if (Pathwalls.Length == 0)
			return;

		GeneratePointsForWall ();
		currentWall = (currentWall + 1);
		if (currentWall == Pathwalls.Length) {
			currentWall = 0;
	}

	}


	void GotoNextPoint() {


		agent.destination = pointsForCurrentWall[destPoint];

		destPoint = destPoint + 1;
		if (destPoint == pointsForCurrentWall.Length-1) {
			destPoint = 0;
			GoToNextWall();
		}
	}

	void GeneratePointsForWall() {
		Side[] currentDirs = Pathwalls [currentWall].mySides;
		Transform theCurrentWall = Pathwalls [currentWall].myTransform;
		BoxCollider theCurrentWall1 = Pathwalls [currentWall].myCollider;

		Side[] NewDirs = Pathwalls [((currentWall + 1) % Pathwalls.Length)].mySides;
		Transform theNewWall = Pathwalls [((currentWall + 1) % Pathwalls.Length)].myTransform;
		BoxCollider theNewWall1 = Pathwalls [((currentWall + 1) % Pathwalls.Length)].myCollider;


		Vector3 closestpointCurrentWall = GetComponent<Collider>().ClosestPointOnBounds(theCurrentWall.position);
		Vector3 closestpointNewWall = GetComponent<Collider> ().ClosestPointOnBounds (theNewWall.position);

		if (Mathf.Abs (closestpointCurrentWall.z - gameObject.transform.position.z)
			> Mathf.Abs (closestpointCurrentWall.x - gameObject.transform.position.x)) {

			if (gameObject.transform.position.z > closestpointCurrentWall.z) {
				if (myDirection == Direction.CounterClockwise) {
					pointsForCurrentWall [0] = theNewWall.position + new Vector3 (-distfromwall, 0.0f, +theNewWall1.size.z);
					pointsForCurrentWall [1] = theNewWall.position + new Vector3 (-distfromwall, 0.0f, -theNewWall1.size.z);
				} else {
					pointsForCurrentWall [0] = theNewWall.position + new Vector3 (distfromwall, 0.0f, +theNewWall1.size.z);
					pointsForCurrentWall [1] = theNewWall.position + new Vector3 (distfromwall, 0.0f, -theNewWall1.size.z);
				}

			} else {
				if (myDirection == Direction.CounterClockwise) {
					pointsForCurrentWall [0] = theNewWall.position + new Vector3 (distfromwall, 0.0f, -theNewWall1.size.z);
					pointsForCurrentWall [1] = theNewWall.position + new Vector3 (distfromwall, 0.0f, +theNewWall1.size.z);
				} else {
					pointsForCurrentWall [0] = theNewWall.position + new Vector3 (-distfromwall, 0.0f, -theNewWall1.size.z);
					pointsForCurrentWall [1] = theNewWall.position + new Vector3 (-distfromwall, 0.0f, +theNewWall1.size.z);
				}
			}


		} else {

			if (gameObject.transform.position.x > closestpointCurrentWall.x) {
				if (myDirection == Direction.CounterClockwise) {
					pointsForCurrentWall [0] = theNewWall.position + new Vector3 (+theNewWall1.size.x, 0.0f, distfromwall);
					pointsForCurrentWall [1] = theNewWall.position + new Vector3 (-theNewWall1.size.x, 0.0f, distfromwall);
				} else {
					pointsForCurrentWall [0] = theNewWall.position + new Vector3 (+theNewWall1.size.x, 0.0f, -distfromwall);
					pointsForCurrentWall [1] = theNewWall.position + new Vector3 (-theNewWall1.size.x, 0.0f, -distfromwall);
				}
			} else {
				if (myDirection == Direction.CounterClockwise) {
					pointsForCurrentWall [0] = theNewWall.position + new Vector3 (-theNewWall1.size.x, 0.0f, -distfromwall);
					pointsForCurrentWall [1] = theNewWall.position + new Vector3 (+theNewWall1.size.x, 0.0f, -distfromwall);
				} else {
					pointsForCurrentWall [0] = theNewWall.position + new Vector3 (-theNewWall1.size.x, 0.0f, distfromwall);
					pointsForCurrentWall [1] = theNewWall.position + new Vector3 (+theNewWall1.size.x, 0.0f, distfromwall);
				}
			}

		}
	}








	void Update () {
		if (PlayerController.timeFrozen)
			agent.speed = 0;
		else 
			agent.speed = movingspeed;

		if (!agent.pathPending && agent.remainingDistance < 0.5f)
			GotoNextPoint();
	}
}