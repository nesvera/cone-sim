using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;

public class TrainingModeControl : MonoBehaviour {

	// Prefab reference
	public GameObject cone_prefab;		

	// Cone spawn properties
	private Quaternion cone_orientation = Quaternion.Euler(-90, 0, 0);
	private float cone_spawn_height = 0.24f;

	// Store all cones in the track
	private Dictionary<string, GameObject> cones_placed = new Dictionary<string, GameObject>();

	// Cone id (increase)
	private int last_cone_id = 0;

	[Header("Prefabs")]
	// Car initial position
	private Vector3 car_init_position;
	private Vector3 car_init_rotation;

	[Header("Camera")]
	// Camera references
	public GameObject main_camera;
	public GameObject drive_camera;
	private int current_camera = 0;

	void Awake(){
		GameControl.current_mode = "Autonomous Mode";

		BuildLoadedTrack ();
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		// V = Change camera
		if (Input.GetKeyDown(KeyCode.V)) {

			if (current_camera == 0) {
				main_camera.SetActive (false);
				drive_camera.SetActive (true);

				current_camera = 1;
			
			} else {
				main_camera.SetActive (true);
				drive_camera.SetActive (false);

				current_camera = 0;
			}
		}

		// R = Autonomous mode
		if (Input.GetKeyDown(KeyCode.R)) {


		}

		// If the car goes out of the map
		GameObject car = GameObject.Find ("Car").transform.GetChild(0).gameObject;
		if (car.transform.position.y < -10) {
			car.transform.position = car_init_position;
			car.transform.eulerAngles = car_init_rotation;
		}
			
	}

	public void buttonReset(){
		GameObject car = GameObject.Find ("Car").transform.GetChild(0).gameObject;
		car.transform.position = car_init_position;
		car.transform.eulerAngles = car_init_rotation;
	}

	public void buttonTrackEditor(){
		// Check that there is a name to create the track
		if (GameControl.track_name.Length > 0) {

			// Load the Track Editor Scene
			SceneManager.LoadScene("Track Editor");
		}
	}
		
	// Save and go back to the start menu
	public void buttonExit(){

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

		// Store initial position
		car_init_position = new Vector3 (track.car.position [0], track.car.position [1], track.car.position [2]);
		car_init_rotation = new Vector3 (track.car.rotation [0], track.car.rotation [1], track.car.rotation [2]);

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

