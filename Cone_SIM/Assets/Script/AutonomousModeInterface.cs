using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Text;

// Udp communication
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// Access car scripts
using UnityStandardAssets.Vehicles.Car;

/* Script handles the udp communication
 * This script needs to be added as component of a GameObject (just create an empty one and drag this script)
 * 
 * dNesvera 
 */

public class AutonomousModeInterface : MonoBehaviour {

	[Header("Vehicle")]
	// Reference to the car
	public GameObject vehicle;
	private CarController vehicle_control;

	// Lidar 
	private GameObject lidar;
	private int lidar_angle_start = 0;
	private int lidar_angle_end = 360;
	private int lidar_num_ray = 36;
	private int lidar_angle_bt;
	private float[] lidar_read = new float[36];

	[Header("Text Window")]
	public Text telemetry_text;
	public Text terminal_text;

	private String telemetry_str = "";					// Hold current telemetry data
	private String terminal_str = "";					// Hold all terminal information

	[Header("Self-Driving")]
	public GameObject self_driving_image;
	private bool is_self_driving_on = false;

	private float start_time;
	private float udp_timeout;

	// Udp variables

	// Client variables
	private string send_ip;
	private int send_port;

	IPEndPoint remoteEndPoint;
	UdpClient send_udp_client;

	// Server variables
	private Thread receive_thread;
	UdpClient receive_udp_client;

	// public string IP = "127.0.0.1"; 		// default local
	private int receive_port;

	private string last_received_udp_msg = "";
	private bool new_command = false;
	private float[] input_command;

	// Thread variables
	bool thread_running = false;


	// Use this for initialization
	void Start () {
		// Script to control car
		//vehicle_control = vehicle.GetComponent<CarController>();

		// Create an invisible gameobject to take the axis to raycast
		lidar = GameObject.CreatePrimitive(PrimitiveType.Cube);
		lidar.SetActive (false);
		lidar.GetComponent<BoxCollider> ().enabled = false;
		//lidar.transform.localScale = new Vector3 (3, 3, 6);

		// Angle between which lidar ray
		lidar_angle_bt = (lidar_angle_end - lidar_angle_start) / lidar_num_ray;

		// Reset terminals
		telemetry_str = "";
		telemetry_text.text = telemetry_str;
		terminal_str = "";
		terminal_text.text = terminal_str;


		// Set udp communication
		send_ip = "127.0.0.1";
		send_port = 5000;

		receive_port = 5001;

		remoteEndPoint = new IPEndPoint (IPAddress.Parse (send_ip), send_port);
		send_udp_client = new UdpClient ();

		thread_running = true;
		receive_thread = new Thread(new ThreadStart(ReceiveThread));
		receive_thread.Start ();

		input_command = new float[4];


		udp_timeout = 0;
	}

	// Function to map a range to another
	float map(float value, float in_min, float in_max, float out_min, float out_max){
		return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}
		
	// Update is called once per frame
	void Update () {

		// Read environment
		Raycast ();

		// Autonomous mode is enabled
		if (is_self_driving_on) {

			// Send car state via udp
			SendTelemetry ();

			int i = 0;
			while (i < 10000000 ) {

				if (new_command) {
					new_command = false;
					break;
				}

				i++;
			}

			// Throttle receive (0->1) expect (0->1)
			float throttle = input_command [0];
			input_command [0] = 0;		// Reset value

			// Brake receive (0->1) expect (-1->0)
			float brake = (-1)*input_command [1];
			input_command [1] = 0;		// Reset value

			// Steering receive (0->1) expect (-1->1)
			float steering = map(input_command[2], 0, 1, -1, 1);
			input_command [2] = 0.5f;	// Reset value

			// Handbrake receive (0->1) expect (0->1)
			float handbrake = input_command[3];
			input_command [3] = 0;		// Reset value

			vehicle.GetComponent<CarController> ().Move(steering, throttle, brake, handbrake);
		}

		// Print data on telemetry window
		PrintTelemetry();
	}

	// Read the environment similar lidar
	void Raycast(){
		if (!vehicle)
			return;

		RaycastHit hit;

		float lidar_range = GameControl.raycast_range;

		// Get the position of the car and a offset on the Y axis
		lidar.transform.position = new Vector3(vehicle.transform.position.x, vehicle.transform.position.y + 0.2f , vehicle.transform.position.z);

		// Sensor rotate 3 axis (if the car break, the sensor may detect the ground)
		//lidar.transform.eulerAngles = vehicle.transform.eulerAngles;

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


				lidar_read [j++] = hit.distance/lidar_range;

			} else {
				Debug.DrawLine (lidar.transform.position, lidar.transform.position + Quaternion.AngleAxis ((float)(car_right+i), lidar.transform.up)*lidar.transform.forward*lidar_range);
				//Debug.Log ("sl " + Quaternion.AngleAxis ((float)i, vehicle.up) * vehicle.forward * 10f);


				lidar_read [j++] = lidar_range/lidar_range;

			}
		}
	}
		
	// When the user hit the wall, stop recording and discard the logger
	public void OnHallHit(){
		//Debug.Log ("bateu");
	}

	// Print data on the telemetry scrollview window
	void PrintTelemetry(){

		// Print datalogger information on the telemetry window
		if (telemetry_text != null && vehicle != null ) {

			//float current_speed = (vehicle.GetComponent<CarController> ().data_speed / GameControl.car_top_speed);
			float current_speed = vehicle.GetComponent<CarController> ().data_speed;

			telemetry_str = "";
			telemetry_str +=	"    Input " +  "\n" +
				"Throttle: " + vehicle.GetComponent<CarController>().data_throttle + "\n" +
				"Brake: " + vehicle.GetComponent<CarController>().data_brake + "\n" +
				"Steering: " + vehicle.GetComponent<CarController>().data_steering + "\n" +
				"Hand brake: " + vehicle.GetComponent<CarController>().data_handbrake + "\n" +
				"\n" +
				"    Output " + "\n" +
				"Speed: " + current_speed + "\n" +
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

	// Send car state via udp 
	void SendTelemetry(){

		float top_speed = GameControl.car_top_speed;

		telemetry_str = "";
		telemetry_str += (Time.time - start_time).ToString ("F3") + ";" + 									// [0] - Time since enable self-driving
			vehicle.GetComponent<CarController> ().data_throttle.ToString ("F3") + ";" +					// [1] - throtlle
			vehicle.GetComponent<CarController> ().data_brake.ToString ("F3") + ";" +						// [2] - brake
			vehicle.GetComponent<CarController> ().data_steering.ToString ("F3") + ";" +					// [3] - steering
			vehicle.GetComponent<CarController> ().data_handbrake.ToString ("F3") + ";" +					// [4] - handbrake
			(vehicle.GetComponent<CarController> ().data_speed/top_speed).ToString ("F3") + ";" +						// [5] - speed
			"00.0" + ";" +																					// [6] - acceleration x
			"00.0" + ";" +																					// [7] - acceleration z
			vehicle.transform.position.x.ToString ("F3") + ";" +											// [8] - latitude x
			vehicle.transform.position.z.ToString ("F3") + ";";												// [9] - longitude z

		for (int i = 0; i < 36; i++) {
			telemetry_str += lidar_read [i].ToString ("F3") + ";";											// [10 - 45] - 36 raycast distance
		}

		// Just to end :)
		telemetry_str += "0";

		try{
			byte[] data = Encoding.UTF8.GetBytes(telemetry_str);
			send_udp_client.Send(data, data.Length, remoteEndPoint);

		}catch(Exception err){
			//print (err.ToString ());
		}
	}

	public void EnableSelfDriving(){
		self_driving_image.SetActive (true);
		is_self_driving_on = true;
		start_time = Time.time;
	}

	public void DisableSelfDriving(){
		self_driving_image.SetActive (false);
		is_self_driving_on = false;
	}

	public void EnableCommunication(){

	}
		
	private void OnDestroy(){
		DisableCommunication ();
	}

	private void OnApplicationQuit(){
		DisableCommunication ();
	}

	// You need to close the communication
	public void DisableCommunication(){
		thread_running = false;
		receive_udp_client.Close ();
		receive_thread.Abort ();
	}

	// Send current car state via udp
	private void sendString(string message){
		try{
			byte[] data = Encoding.UTF8.GetBytes(message);
			send_udp_client.Send(data, data.Length, remoteEndPoint);

		}catch(Exception err){
			//print (err.ToString ());
		}
	}

	// Thread that listen the inputs via udp
	private void ReceiveThread(){
		receive_udp_client = new UdpClient(receive_port);
		IPEndPoint anyIP = new IPEndPoint (IPAddress.Any, 0);

		while (thread_running) {
			try{
				// Bytes recebidos
				byte[] data = receive_udp_client.Receive(ref anyIP);
				string udp_msg = Encoding.UTF8.GetString(data);
				//Debug.Log(udp_msg);


				// Break csv
				String[] csv_msg = (udp_msg.Trim()).Split(";"[0]);


				for( int i = 0 ; i<4 ; i++ ){
					input_command[i] = float.Parse(csv_msg[i]);

				}

				new_command = true;

			}catch(Exception err){
				//Debug.Log(err.ToString());
			}

		}
	}
}

