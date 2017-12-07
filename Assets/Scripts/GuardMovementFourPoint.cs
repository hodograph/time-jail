using UnityEngine;
using System.Collections;

public class GuardMovementFourPoint : MonoBehaviour
{
	public Transform onePoint;
	public Transform twoPoint;
	public Transform threePoint;
	public Transform startPoint;
	public float speed;
	private Vector3 pointB;
	private Vector3 pointC;
	private Vector3 pointD;
	public float rotateSpeed;
	public int degreesToRotate;

	IEnumerator Start()
	{
		pointD = threePoint.position;
		pointC = twoPoint.position;
		pointB = onePoint.position;
		Vector3 pointA = startPoint.position;

		while(true)
		{
			yield return StartCoroutine(MoveObject(transform, pointA, pointB, 3.0f));
			yield return StartCoroutine(Rotate(degreesToRotate));
			yield return StartCoroutine(MoveObject(transform, pointB, pointC, 3.0f));
			yield return StartCoroutine(Rotate(degreesToRotate));
			yield return StartCoroutine(MoveObject(transform, pointC, pointD, 3.0f));
			yield return StartCoroutine(Rotate(degreesToRotate));
			yield return StartCoroutine(MoveObject(transform, pointD, pointA, 3.0f));
			yield return StartCoroutine(Rotate(degreesToRotate));
		}
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