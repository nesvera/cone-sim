using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

using System;
using System.IO;
using System.Text;

/* Script of training mode scene
 * This script needs to be added as component of a GameObject (just create an empty one and drag this script)
 * 
 * Read n raycast around the car to show the user and recording on file
 * 
 * dNesvera 
 */

// Access car scripts
using UnityStandardAssets.Vehicles.Car;

public class TrainingDatalogger : MonoBehaviour {

	[Header("Vehicle")]
	// Reference to the car
	public GameObject vehicle;

	private GameObject lidar;

	[Header("Lidar")]
	private int lidar_angle_start = 0;
	private int lidar_angle_end = 360;
	private int lidar_num_ray = 36;
	private int lidar_angle_bt;
	public float lidar_range = 60.0f;

	private float[] lidar_read = new float[36];


	[Header("Recording")]
	public GameObject rec_image;
	private bool isRecording;
	private float recording_start_time;

	[Header("Text Window")]
	public Text telemetry_text;
	public Text terminal_text;

	private String telemetry_str = "";					// Hold current telemetry data
	private String telemetry_file = "";					// Hold all recording telemetry data
	private String terminal_str = "";					// Hold all terminal information

	// Use this for initialization
	void Start () {
		// Create an invisible gameobject to take the axis to raycast
		lidar = GameObject.CreatePrimitive(PrimitiveType.Cube);
		lidar.SetActive (false);
		lidar.GetComponent<BoxCollider> ().enabled = false;
		//lidar.transform.localScale = new Vector3 (3, 3, 6);

		// Angle between which lidar ray
		lidar_angle_bt = (lidar_angle_end - lidar_angle_start) / lidar_num_ray;

		isRecording = false;

		// Reset terminals
		telemetry_str = "";
		telemetry_text.text = telemetry_str;
		terminal_str = "";
		terminal_text.text = terminal_str;

	}
	
	// Update is called once per frame
	void Update () {

		// R = start/stop recording
		if ( CrossPlatformInputManager.GetButtonDown ("Rec") ) {
			OnRecordPressed ();
		}

		// Read environment
		Raycast ();
				
		// Store data in a string to save later
		StoreTelemetry ();
	

		// Print data on telemetry window
		PrintTelemetry ();

		// Lidar - test crash
		/*
		if (lidar_read [9] >= 1) {
			vehicle.transform.GetComponent<CarController> ().Move (0, 1, 0, 0);
		} else {
			vehicle.transform.GetComponent<CarController> ().Move (0, 0, -1, 0);
		}
		*/
	}

	// Read the environment similar lidar
	void Raycast(){
		if (!vehicle)
			return;

		RaycastHit hit;

		lidar_range = GameControl.raycast_range;

		// Get the position of the car and a offset on the Y axis
		lidar.transform.position = new Vector3(vehicle.transform.position.x, vehicle.transform.position.y + 0.2f , vehicle.transform.position.z);

		// Sensor rotate 3 axis (if the car break, the sensor may detect the ground)
		//lidar.transform.eulerAngles = vehicle.transform.eulerAngles;
		//Vector3 car_eulerAngle = vehicle.transform.eulerAngles;

		// Sensor rotate just in the Y axis (stay parallel with the ground)
		lidar.transform.eulerAngles = new Vector3(0, vehicle.transform.eulerAngles.y, 0);

		// Right side of the car 
		int car_right = 90;
	

		int i;
		int j = 0;
		for (i = lidar_angle_start; i > -lidar_angle_end; i-=lidar_angle_bt ) {
			if (Physics.Raycast (lidar.transform.position, Quaternion.AngleAxis ((float)(car_right+i), lidar.transform.up) * lidar.transform.forward, out hit, lidar_range )) {
				Debug.DrawLine (lidar.transform.position, hit.point);
				//Debug.Log (i + " - d: " + hit.distance);

				lidar_read [j++] = hit.distance / lidar_range;

			} else {
				Debug.DrawLine (lidar.transform.position, lidar.transform.position + Quaternion.AngleAxis ((float)(car_right+i), lidar.transform.up)*lidar.transform.forward*lidar_range);
				//Debug.Log ("sl " + Quaternion.AngleAxis ((float)i, vehicle.up) * vehicle.forward * 10f);
			
				lidar_read [j++] = lidar_range / lidar_range;

			}
		}
	}

	// Save the logger in a file
	void SaveFile(){

		// File name based on day and time
		var time = System.DateTime.Now.ToString ("hh.mm.ss");
		var date = System.DateTime.Now.ToString ("yy.MM.dd");
		String file_name = date + "_" + time + ".txt";

		String path = Application.dataPath + "/../Dataset/";

		// Check if the directory exist
		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}

		// Include file name
		path += file_name;

		// Check if the file exist
		if (!File.Exists (path)) {
			File.WriteAllText (path, telemetry_file);
		}

		terminal_str += "> Stop recording!" + "\n" + "Save as " + file_name + "\n";
		terminal_text.text = terminal_str;
	}

	// When the record button or R was clicked
	public void OnRecordPressed(){

		if (!isRecording) {
			rec_image.SetActive (true);
			recording_start_time = Time.time;
			telemetry_file = "";							// Reset string that hold all the recording data
			isRecording = true;
		
		} else {
			rec_image.SetActive (false);
			SaveFile ();
			isRecording = false;
		}

	}

	// When the user hit the wall, stop recording and discard the logger
	public void OnHallHit(){

		if (isRecording) {
			//rec_image.SetActive (false);
			//isRecording = false;

			terminal_str += "> Car crashed. Stop recording!" + "\n";
			terminal_text.text = terminal_str;

			// Delete the last N seconds and save the record
			int n_secs = 2;
			int n_frames = 50 * n_secs;

			// split content of the text variable into lines
			var lines = telemetry_file.Split('\n');
			string aux_telemetry = "";

			// Has delete some seconds of the recording
			if (lines.Length > n_frames) {

				for (int i = 0; i < (lines.Length-1-n_frames) ; i++) {
					aux_telemetry += lines [i] + "\n";
				}

				//Debug.Log (aux_telemetry);

				// Save telemetry without n last seconds
				telemetry_file = aux_telemetry;

				// Save file
				SaveFile ();
		
			} 

			// Start another recording
			recording_start_time = Time.time;
			telemetry_file = "";							// Reset string that hold all the recording data

		}
	}

	void PrintTelemetry(){

		// Print datalogger information on the telemetry window
		if (telemetry_text != null && vehicle != null ) {

			telemetry_str = "";
			telemetry_str +=	"    Input " +  "\n" +
				"Throttle: " + vehicle.GetComponent<CarController>().data_throttle + "\n" +
				"Brake: " + vehicle.GetComponent<CarController>().data_brake + "\n" +
				"Steering: " + vehicle.GetComponent<CarController>().data_steering + "\n" +
				"Hand brake: " + vehicle.GetComponent<CarController>().data_handbrake + "\n" +
				"\n" +
				"    Output " + "\n" +
				"Speed: " + vehicle.GetComponent<CarController>().data_speed + "\n" +
				"Accel(x): " + "00.0" + "\n" +
				"Accel(z): " + "00.0" + "\n" +
				"Latitude(x): " + vehicle.transform.position.x + "\n" +
				"Longitude(z): " + vehicle.transform.position.z + "\n";

			for (int i = 0; i < 36; i++) {
				telemetry_str += "Lidar " + i + ": " + lidar_read [i] + "\n";
			}

			telemetry_text.text = telemetry_str;
		}
	}

	// Function to map a range to another
	float map(float value, float in_min, float in_max, float out_min, float out_max){
		return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}

	// Store telemetry in a string 
	void StoreTelemetry(){

		// data data from the vehicle
		float throttle = vehicle.GetComponent<CarController> ().data_throttle;

		float brake = vehicle.GetComponent<CarController> ().data_brake;

		float steering = vehicle.GetComponent<CarController> ().data_steering;
		steering = map (steering, -GameControl.max_steering, GameControl.max_steering, 0, 1);

		float handbrake = vehicle.GetComponent<CarController> ().data_handbrake;

		float speed = vehicle.GetComponent<CarController> ().data_speed;
		speed = map (speed, -GameControl.max_speed, GameControl.max_speed, 0, 1);

		telemetry_str = "";
		telemetry_str += (Time.time - recording_start_time).ToString ("F3") + ";" + 						// [0] - recording time
			throttle.ToString ("F3") + ";" +					// [1] - throtlle
			brake.ToString ("F3") + ";" +						// [2] - brake
			steering.ToString ("F3") + ";" +					// [3] - steering
			handbrake.ToString ("F3") + ";" +					// [4] - handbrake
			speed.ToString ("F3") + ";" +						// [5] - speed
			"00.0" + ";" +																					// [6] - acceleration x
			"00.0" + ";" +																					// [7] - acceleration z
			vehicle.transform.position.x.ToString ("F3") + ";" +											// [8] - latitude x
			vehicle.transform.position.z.ToString ("F3") + ";";												// [9] - longitude z
	
		for (int i = 0; i < 36; i++) {
			telemetry_str += lidar_read [i].ToString ("F3") + ";";											// [10 - 45] - 36 raycast distance
		}
			
		// Just to end :)
		telemetry_str += "0";

		telemetry_file += telemetry_str + "\n";

	}
}
