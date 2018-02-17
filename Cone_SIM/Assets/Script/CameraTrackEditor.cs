/*
 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Script of track editor
 *	Camera top_down
 *		Move camera in x and z axis with w/s/a/d key
 *		Move camera in y axis with mouse wheel
 * 
 *		Set some limits on map to prevent the user to see off the terrain
 * 
 * dNesvera 
 */

public class CameraTrackEditor : MonoBehaviour {

	// The height we want the camera to be above the ground
	public float initial_height =  16.0f;			


	// Variables of the camera moviment
	public float panSpeed = 20f;
	public Vector2 panLimit;
	public float scrollSpeed = 20f;
	public float minY = 20f;
	public float maxY = 200f;

	public GameObject panel_camera;

	// Use this for initialization
	void Start () {

		// Set initial position of the camera
		transform.position = new Vector3(0, initial_height, 0);

		// Set initial orientation
		transform.eulerAngles = new Vector3(90, 0, 0);

		// Add initial position on the panel
		panel_camera.transform.Find ("InputField_position_x").GetComponent<InputField> ().text = transform.position.x.ToString ("F2");
		panel_camera.transform.Find ("InputField_position_z").GetComponent<InputField> ().text = transform.position.z.ToString ("F2");

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// Camera moved
		bool camera_moved = false;

		// Get current position of the camera
		Vector3 pos = transform.position;

		// Change x and z limit based on the camera height for user never see outside the terrain
		panLimit.x = (-1.36f)*pos.y + (994f);
		panLimit.y = (-0.77f)*pos.y + (992f);

		// Move up (+Z)
		if (Input.GetKey ("w") || Input.GetKey ("up")) {
			pos.z += panSpeed * Time.deltaTime;
			camera_moved = true;
		}

		// Move down (-Z)
		if (Input.GetKey ("s") || Input.GetKey ("down")) {
			pos.z -= panSpeed * Time.deltaTime;
			camera_moved = true;
		}

		// Move left (-X)
		if (Input.GetKey ("a") || Input.GetKey ("left")) {
			pos.x -= panSpeed * Time.deltaTime;
			camera_moved = true;
		}

		// Move right (+X)
		if (Input.GetKey ("d") || Input.GetKey ("right")) {
			pos.x += panSpeed * Time.deltaTime;
			camera_moved = true;
		}

		// Zoom out and zoom with mouse wheel
		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

//		// Limit the position of the camera
		pos.x = Mathf.Clamp (pos.x, -panLimit.x, panLimit.x);
		pos.z = Mathf.Clamp (pos.z, -panLimit.y, panLimit.y);
		pos.y = Mathf.Clamp (pos.y, minY, maxY);

		// Set new position of the camera
		transform.position = pos;

		// update position on the panel if the user changed it
		if(camera_moved){
			panel_camera.transform.Find ("InputField_position_x").GetComponent<InputField> ().text = transform.position.x.ToString ("F2");
			panel_camera.transform.Find ("InputField_position_z").GetComponent<InputField> ().text = transform.position.z.ToString ("F2");
		}
	}

	// Update button on camera position panel
	// Change camera position 
	public void buttonUpdateCamera(){
		// Read values from Inputfields
		Vector3 new_position = new Vector3 ();
		new_position.x = float.Parse(panel_camera.transform.Find ("InputField_position_x").GetComponent<InputField> ().text);
		new_position.x = Mathf.Clamp (new_position.x, -panLimit.x, +panLimit.x);

		new_position.z = float.Parse(panel_camera.transform.Find ("InputField_position_z").GetComponent<InputField> ().text);
		new_position.z = Mathf.Clamp (new_position.z, -panLimit.y, +panLimit.y);

		new_position.y = transform.position.y;

		// Update vehicle position
		transform.position = new_position;

		// Set format
		panel_camera.transform.Find ("InputField_position_x").GetComponent<InputField> ().text = new_position.x.ToString ("F2");
		panel_camera.transform.Find ("InputField_position_z").GetComponent<InputField> ().text = new_position.z.ToString ("F2");
	}
}
