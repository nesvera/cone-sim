using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		if (Input.GetKeyDown (KeyCode.R)) {
			OnRecordPressed ();
		}

		// Read environment
		Raycast ();
				
		// Store data in a string to save later
		StoreTelemetry ();
	

		// Print data on telemetry window
		PrintTelemetry ();
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
		var time = System.DateTime.Now.ToString ("hh.mm");
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
			rec_image.SetActive (false);
			isRecording = false;

			terminal_str += "> Car crashed. Stop recording!" + "\n";
			terminal_text.text = terminal_str;
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

	// Store telemetry in a string 
	void StoreTelemetry(){

		telemetry_str = "";
		telemetry_str += (Time.time - recording_start_time).ToString ("F3") + ";" + 						// [0] - recording time
			vehicle.GetComponent<CarController> ().data_throttle.ToString ("F3") + ";" +					// [1] - throtlle
			vehicle.GetComponent<CarController> ().data_brake.ToString ("F3") + ";" +						// [2] - brake
			vehicle.GetComponent<CarController> ().data_steering.ToString ("F3") + ";" +					// [3] - steering
			vehicle.GetComponent<CarController> ().data_handbrake.ToString ("F3") + ";" +					// [4] - handbrake
			vehicle.GetComponent<CarController> ().data_speed.ToString ("F3") + ";" +						// [5] - speed
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
