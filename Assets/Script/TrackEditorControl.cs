using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;

public class TrackEditorControl : MonoBehaviour {

	// Prefab reference
	public GameObject cone_prefab;			
	public GameObject main_camera;
	public GameObject drive_camera;

	// Cone spawn properties
	private Quaternion cone_orientation = Quaternion.Euler(-90, 0, 0);
	private float cone_spawn_height = 0.24f;

	// Current object (cone or car) selected
	private GameObject object_selected;		

	// Store all cones in the track
	private Dictionary<string, GameObject> cones_placed = new Dictionary<string, GameObject>();

	// Cone id (increase)
	private int last_cone_id = 0;

	[Header("Panels")]
	public GameObject panel_info;
	public GameObject panel_cone;
	public GameObject panel_car;
	public GameObject panel_checkpoint;


	void Awake(){
		GameControl.current_mode = "Track Editor";

		BuildLoadedTrack ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// On right click and check if the cursor is not over the UI
		if (Input.GetMouseButtonDown (0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {

			// Get click position
			Vector3 clickPosition = new Vector3 (3000, 0, 3000);				// Initiate out of the map
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			// Initiate object hited as null
			GameObject object_clicked = null;
			string object_clicked_type = ""; 

			// Check if any objected was clicked
			if (Physics.Raycast (ray, out hit)) {
				clickPosition = hit.point;	
				object_clicked = hit.transform.gameObject;
				object_clicked_type = object_clicked.transform.parent.name;	

				// Panel with rotation and position appers just to car, cones and checkpoint
				if (object_clicked_type != "Cones" && object_clicked_type != "Car" && object_clicked_type != "Checkpoint") {
					panel_info.gameObject.SetActive (true);
					panel_cone.gameObject.SetActive (false);
					panel_car.gameObject.SetActive (false);
				}


			}

			// SHIFT+RIGHT_CLICK = SPAWN A NEW CONE
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {

				// Instantiate a new cone prefab
				GameObject new_clone = Instantiate (cone_prefab, new Vector3 (clickPosition.x, cone_spawn_height, clickPosition.z), cone_orientation) as GameObject;
				new_clone.transform.parent = GameObject.Find ("Cones").transform;

				// Rename the cone object with ID
				new_clone.name = last_cone_id.ToString();
				last_cone_id++;

				// Save coneclass reference to save track after
				if (!cones_placed.ContainsKey (new_clone.name)) {
					cones_placed.Add (new_clone.name, new_clone);
				}

				// (arrumar) tem que colocar a referencia do cone_class em algum lugar
				// e criar uma funcao
			
				// Select the new cone (Turn on the projector)
				if (object_selected == null) {
					selectObject (new_clone);

				} else {
					deselectObject (object_selected);
					selectObject (new_clone);
				}
			}

			// CTRL+RIGHT_CLICK = ADD NEIGHBOR (if another cone is selected)
			else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {

				// Clicked and selected objects needs to be CONES
				if (object_clicked_type == "Cones" && object_selected.transform.parent.name == "Cones") {
					object_selected.GetComponent<neighbor> ().AddNeighboor (object_clicked);
					object_clicked.GetComponent<neighbor> ().AddNeighboor (object_selected);
				}
			}

			// ALT+RIGHT_CLICK = ADD A NEW CONE AND CONECT WITH THE LAST SELECTED
			else if (Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt)) {

				// Instantiate a new cone prefab
				GameObject new_clone = Instantiate (cone_prefab, new Vector3 (clickPosition.x, cone_spawn_height, clickPosition.z), cone_orientation) as GameObject;
				new_clone.transform.parent = GameObject.Find ("Cones").transform;

				// Rename the cone object with ID
				new_clone.name = last_cone_id.ToString();
				last_cone_id++;
			
				// Save coneclass reference to save track after
				if (!cones_placed.ContainsKey (new_clone.name)) {
					cones_placed.Add (new_clone.name, new_clone);
				}

				// Create a line between the last selected_cone and this new
				if (object_selected != null && object_selected.transform.parent.name == "Cones") {
					object_selected.GetComponent<neighbor> ().AddNeighboor (new_clone);
					new_clone.GetComponent<neighbor> ().AddNeighboor (object_selected);
				}

				// Select the new cone (Turn on the projector)
				if (object_selected == null) {
					selectObject (new_clone);

				} else {
					deselectObject (object_selected);
					selectObject (new_clone);
				}
					
			}

			// RIGHT_CLICK = SELECT OBJECT
			else {

				// Just select car and cones
				if (object_clicked_type == "Cones" || object_clicked_type == "Car") {

					if (object_clicked != object_selected) {
						deselectObject (object_selected);	
						selectObject (object_clicked);

					}

				} else {
					deselectObject (object_selected);
					object_selected = null;
				}
			}
		
		// Delete a cone and lines
		} else if (Input.GetKey (KeyCode.Delete)) {
			
			// Just delete CONES
			if (object_selected != null && object_selected.transform.parent.name == "Cones") {

				// Delete of the dictionary 
				if (cones_placed.ContainsKey (object_selected.name)) {
					cones_placed.Remove (object_selected.name);
				}

				object_selected.GetComponent<neighbor> ().DeleteCone ();
				deleteObject (object_selected);
			}
		}
			
		// If the car goes out of the map
		GameObject car = GameObject.Find ("Car").transform.GetChild(0).gameObject;
		if (car.transform.position.y < -10) {
			car.transform.position = new Vector3 (0, 0, 0);
		}
	}

	private void selectObject(GameObject obj){
						
		if (obj != null) {
			GameObject child = obj.transform.Find ("Projector").gameObject;
			child.SetActive (true);

			object_selected = obj;

			var object_selected_type = obj.transform.parent.name;
			if (object_selected_type == "Cones") {
				panel_info.gameObject.SetActive (false);
				panel_cone.gameObject.SetActive (true);
				panel_car.gameObject.SetActive (false);

			} else if (object_selected_type == "Car") {
				panel_info.gameObject.SetActive (false);
				panel_cone.gameObject.SetActive (false);
				panel_car.gameObject.SetActive (true);

			} else if (object_selected_type == "Checkpoint") {
				//					panel_info.SetActive (false);
				//					panel_cone.SetActive(false);
				//					panel_car.SetActive(false);
				//					panel_checkpoint.SetActive(true);

			}
		}
	}

	private void deselectObject(GameObject obj){
		if (obj != null) {
			GameObject child = obj.transform.Find ("Projector").gameObject;
			child.SetActive (false);

			object_selected = null;
		}
	}

	private void deleteObject(GameObject obj){
		if (obj != null) {
			object_selected = null;
		}
	}

	public void buttonDrive(){

		// Change the camera
		main_camera.SetActive(false);
		drive_camera.SetActive (true);

		// Active the script to drive the car
		GameObject car = GameObject.Find ("Car").transform.GetChild(0).gameObject;
		car.GetComponent<CarUserControl> ().enabled = true;

	}

	public void buttonEdit(){

		// Change the camera
		main_camera.SetActive(true);
		drive_camera.SetActive (false);

		// Active the script to drive the car
		GameObject car = GameObject.Find ("Car").transform.GetChild(0).gameObject;
		car.GetComponent<CarUserControl> ().enabled = false;
	}

	public void buttonTest(){
		// Call function to save the track
		SaveLoadManager.SaveTrack (cones_placed);

		// Check that there is a name to create the track
		if (GameControl.track_name.Length > 0) {

			// Load the Track Editor Scene
			SceneManager.LoadScene("Training Mode");
		}
	}

	// Just save the track
	public void buttonSave(){
		// Call function to save the track
		SaveLoadManager.SaveTrack (cones_placed);
	}

	// Save and go back to the start menu
	public void buttonExit(){
		// Call function to save the track
		SaveLoadManager.SaveTrack (cones_placed);

		// Load the Start Menu Scene
		SceneManager.LoadScene("Start Menu");
	}

	public void BuildLoadedTrack(){

		// Load the track 
		TrackData track = SaveLoadManager.LoadTrack ();

		if (track == null) {
			//Debug.Log ("Where is the track?");

			// Load the Start Menu Scene
			//SceneManager.LoadScene("Start Menu");
			return;
		}


		// Set car properties
		GameObject car = GameObject.Find ("Car").transform.GetChild(0).gameObject;
		car.transform.position = new Vector3 (track.car.position [0], track.car.position [1], track.car.position [2]);
		car.transform.eulerAngles = new Vector3 (track.car.rotation [0], track.car.rotation [1], track.car.rotation [2]);

		// First place all cones
		foreach (ConeData cone in track.cone_list) {

			// Position and rotation of the cone
			Vector3 _position = new Vector3 (cone.position [0], cone.position [1], cone.position [2]);
			Quaternion _orientation = Quaternion.Euler(cone.rotation [0], cone.rotation [1], cone.rotation [2]);

			// Instantiate a new cone prefab
			GameObject new_clone = Instantiate (cone_prefab, _position, _orientation) as GameObject;
			new_clone.transform.parent = GameObject.Find ("Cones").transform;

			// Disable the projector
			new_clone.transform.Find("Projector").gameObject.SetActive(false);

			// Rename the prefab
			new_clone.name = cone.id.ToString();
			last_cone_id = cone.id + 1;							// save the last id to continue to change the track


			// Save coneclass reference to save track after
			if (!cones_placed.ContainsKey (new_clone.name)) {
				cones_placed.Add (new_clone.name, new_clone);
			}
		}

		// After place connect cones with neighbors
		foreach (ConeData cone in track.cone_list) {

			// Search the cone in the dictionary
			if (cones_placed.ContainsKey (cone.id.ToString())) {

				// Get the cone (gameobject) from de dictionary
				GameObject cone_obj = cones_placed[cone.id.ToString ()];

				// Loop trough the neighbors of the cone
				foreach (int neighbor in cone.neighbor) {

					// Search the neighbor in the dictionary
					if (cones_placed.ContainsKey (neighbor.ToString())) {

						// Get the cone (gameobject) from de dictionary
						GameObject cone_neighbor = cones_placed[neighbor.ToString()];

						// Connect the cones
						cone_obj.GetComponent<neighbor>().AddNeighboor(cone_neighbor);
						cone_neighbor.GetComponent<neighbor> ().AddNeighboor (cone_obj);
					}
				}
			}

		}

	}

}

