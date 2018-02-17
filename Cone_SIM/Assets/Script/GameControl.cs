using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Static class to store some data between scenes 
 * 
 * dNesvera
 */

public static class GameControl{

	// Car speed
	public static float min_speed = 50;
	public static float max_speed = 200;
	public static float car_top_speed = 100;
	public static float raycast_range = 50;

	public static void setCarSpeed(float new_speed){
		car_top_speed = new_speed;
		raycast_range = Mathf.Ceil (new_speed * 0.8375f - 25.875f);
	}

	// Store the name of the track between the scenes
	public static string track_name = "";

	// Store the current scene
	public static string current_mode = "";


}
