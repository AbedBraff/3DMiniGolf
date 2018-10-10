using UnityEngine;
using System.Collections;

public class CameraOrbitScript : MonoBehaviour {

	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;


	private Rigidbody rigidbody;

	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start () 
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		rigidbody = GetComponent<Rigidbody>();

		// Make the rigid body not change rotation
		if (rigidbody != null)
		{
			rigidbody.freezeRotation = true;
		}
	}

	void LateUpdate () 
	{
		if (target) 
		{
			x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			/*RaycastHit hit;
			if (Physics.Linecast (target.position, transform.position, out hit)) 
			{
				distance -=  hit.distance;
			}*/
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;

			transform.rotation = rotation;
			transform.position = position;
		}
	}
}