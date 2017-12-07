using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardTestMovement1 : MonoBehaviour
{
	public Transform endPoint;
	public float speed;
	private Vector3 pointB;
	public float rotateSpeed;
	public MazeGenerator1 MazeGen;
	private List<Vector2> offsets = new List<Vector2> { new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1) };

	IEnumerator Start()
	{
		int dir1 = 0;
		while (true) {
			int olddir = dir1;
			Vector2 mypos = new Vector2 (transform.position.x, transform.position.z);
			Vector2 endpos = mypos;
			dir1 = UnityEngine.Random.Range (0, 4);
			Vector2 dir2 = offsets [dir1];
			int dist = 0;

			while (true) {
				dir1 = UnityEngine.Random.Range (0, 4);
				dir2 = offsets [dir1];
				dist = 0;
				while (true) {
					Vector2 temppos = endpos + dir2;
					if (MazeGen.Maze [(int)temppos.x, (int)temppos.y] >= 1) {
						break;
					}
					dist++;
					endpos = temppos;
				}
				if (dist > 0) {
					break;
				}
			}
			int turnMulti = dir1 - olddir;
			switch (turnMulti) {
			case -1:
				turnMulti = 3;
				break;
			case -2:
				turnMulti = 2;
				break;
			case -3:
				turnMulti = 1;
				break;
			}
			yield return StartCoroutine (Rotate(90*turnMulti));
			yield return StartCoroutine(MoveObject(transform, new Vector3(mypos.x,transform.position.y,mypos.y), new Vector3(endpos.x,transform.position.y,endpos.y), 2f/dist));


		}






		//pointB = endPoint.position;
		//Vector3 pointA = transform.position;
		//while(true)
		//{
		//	yield return StartCoroutine(MoveObject(transform, pointA, pointB, 3.0f));
		//	yield return StartCoroutine(Rotate(180));
		//	yield return StartCoroutine(MoveObject(transform, pointB, pointA, 3.0f));
		//	yield return StartCoroutine(Rotate(180));
		//}
	}

	IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
	{
		var i= 0.0f;
		var rate= speed;
		while(i < 1.0f)
		{
			if (!PlayerController.timeFrozen){
				i += Time.deltaTime * rate;
				thisTransform.position = Vector3.Lerp(startPos, endPos, i);
			}
			yield return null;
		}
	}

	IEnumerator Rotate(int toRotate){
		float startAngle = transform.rotation.eulerAngles.y;
		float endAngle = transform.rotation.eulerAngles.y + toRotate;
		float checkAngle = transform.rotation.eulerAngles.y;
		while (endAngle > checkAngle) {
			if (!PlayerController.timeFrozen) {
				transform.Rotate (0, rotateSpeed * Time.deltaTime, 0);
				checkAngle = transform.rotation.eulerAngles.y;
				if (checkAngle < startAngle)
					checkAngle += 360;
			}

			yield return null;
		}
		transform.Rotate (0, endAngle - transform.rotation.eulerAngles.y, 0);
	}

}