using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Static class to store some data between scenes 
 * 
 * dNesvera
 */

public static class GameControl{

	// Car speed
	public static float max_speed = 100;
	public static float max_steering = 1;

	public static float car_top_speed = max_speed;
	public static float raycast_range = 48;

	// Controller type (keyboard, joystick, racing wheel)
	public static int controller_type = 0;

	// Store the name of the track between the scenes
	public static string track_name = "";

	// Store the current scene
	public static string current_mode = "";


}
