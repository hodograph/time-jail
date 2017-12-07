using UnityEngine;
using System.Collections;
 
public class GuardTestMovement : MonoBehaviour
{
    public Transform endPoint;
	public float speed;
	private Vector3 pointB;
	public float rotateSpeed;
	
    IEnumerator Start()
    {
		pointB = endPoint.position;
        Vector3 pointA = transform.position;
        while(true)
        {
			yield return StartCoroutine(MoveObject(transform, pointA, pointB, 3.0f));
			yield return StartCoroutine(Rotate(180));
			yield return StartCoroutine(MoveObject(transform, pointB, pointA, 3.0f));
			yield return StartCoroutine(Rotate(180));
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