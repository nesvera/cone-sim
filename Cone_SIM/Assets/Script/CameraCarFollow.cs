using UnityEngine;
using System.Collections;

/* Script to camera that follow the car from behind
 * 
 * dNesvera 
 */

public class CameraCarFollow : MonoBehaviour {


	public Transform target;				// The target we are following
	public float distante = 10.0f;			// The distance in the x-z plane to the target
	public float height =  5.0f;			// The height we want the camera to be above the target

	public float heightDamping = 2.0f;	
	public float rotationDamping = 3.0f;


	void FixedUpdate(){

		if (!target)
			return;

		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;

		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping*Time.deltaTime);

		// Damp the height
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping*Time.deltaTime);

		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

		// Set the position of the camera on the x-z plane to distance meters behind target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distante;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

		// Always look at the target
		transform.LookAt(target);

	}
}
