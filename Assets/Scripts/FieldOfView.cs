using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;
	[Range(0,358)]
	public float startAngle;
	[Range(0,358)]
	public float endAngle;
	public float speed;
	public bool changeDir = true;
	public bool isCamera = false;
	private bool going = true;
	
	public LayerMask targetMask;
	public LayerMask obstacleMask;
	public int offset = 1;
	
	public bool noRotate;
	
	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();
	
	public float meshResolution;
	
	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	
	void Start()
	{
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;
		StartCoroutine("FindTargetsWithDelay", 0.2f);
		transform.rotation = Quaternion.Euler (0.0f, startAngle, 0.0f);

		if (!isCamera) {
			GameObject lightGameObject = new GameObject ("Flash Light");
			Light light = lightGameObject.gameObject.AddComponent<Light> ();
			lightGameObject.transform.parent = this.transform;
			light.type = LightType.Spot;
			light.shadows = LightShadows.Soft;
			light.transform.Rotate (0, startAngle - light.transform.rotation.eulerAngles.y, 0);
			light.range = viewRadius;
			light.spotAngle = viewAngle;
			light.transform.position = this.transform.position;
			light.intensity = viewRadius / 15 * 1000;
			light.color = Color.yellow;
			light.renderMode = LightRenderMode.ForcePixel;
		}
	}
	
	void Update()
	{
		DrawFieldOfView();
		Rotate();
	}

	void Rotate(){
		if (!PlayerController.timeFrozen && !noRotate) {
			if (going) {
				if(changeDir)
					transform.Rotate (0, speed * Time.deltaTime, 0);
				else
					transform.Rotate (0, -speed * Time.deltaTime, 0);
				if (transform.rotation.eulerAngles.y >= endAngle && transform.rotation.eulerAngles.y <= endAngle+offset)
					going = false;
			} else {
				if(changeDir)
					transform.Rotate (0, -speed * Time.deltaTime, 0);
				else
					transform.Rotate (0, speed * Time.deltaTime, 0);				if (transform.rotation.eulerAngles.y <= startAngle && transform.rotation.eulerAngles.y >= startAngle-offset)
					going = true;
			}
		}

	}

	IEnumerator FindTargetsWithDelay(float delay)
	{
		while(true)
		{
			yield return new WaitForSeconds (delay);
			FindVisibleTargets();
			
		}

	}
	
	void FindVisibleTargets()
	{
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
		
		for(int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			
			if (Vector3.Angle(transform.forward, dirToTarget) < (viewAngle / 2))
			{
				float distToTarget = Vector3.Distance (transform.position, target.position);
				
				if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
				{
					visibleTargets.Add(target);
					DeathScreen DS =  GameObject.FindObjectOfType(typeof(DeathScreen)) as DeathScreen;
					DS.Pause ();
				}
			}
		}
	}
	
	public Vector3 DirFromAngle(float angleDeg, bool isAngleGlobal)
	{
		if (!isAngleGlobal)
		{
			angleDeg += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleDeg * Mathf.Deg2Rad));
	}

	void DrawFieldOfView()
	{
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3> ();

		for(int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - viewAngle/2 + stepAngleSize*i;
			ViewCastInfo newViewCast = ViewCast(angle);
			viewPoints.Add(newViewCast.point);
		}	
		
		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];
		
		vertices[0] = Vector3.zero;
		
		for(int i = 0; i < vertexCount - 1; i++)
		{
			vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);
			
			if(i < vertexCount - 2)
			{
				triangles[i*3] = 0;
				triangles[i*3+1] = i + 1;
				triangles[i*3+2] = i + 2;
			}
		}
		
		viewMesh.Clear();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}
	
	ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;
		
		if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
		{
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
		}
		else
		{
			return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
		
	}
	
	public struct ViewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float dist;
		public float angle;
		
		public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
		{
			hit = _hit;
			point = _point;
			dist = _dist;
			angle = _angle;


		}		
		
		
		
	}

}
