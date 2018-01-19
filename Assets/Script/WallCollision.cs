/* 
 * Script to traffic cones and walls (gameobject), when the car collide with this object
 * stop recording.
 * 
 * This prevent bad drivers dataset 
 * 
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallCollision : MonoBehaviour {

	// Stop recording on collision
	void OnCollisionEnter(Collision col){

		Scene scene = SceneManager.GetActiveScene ();

		if (scene.name == "Training Mode") {
			GameObject.Find ("Datalogger").GetComponent<TrainingDatalogger> ().OnHallHit ();
		
		} else if (scene.name == "Autonomous Mode") {
			GameObject.Find ("Autonomous Mode Control").GetComponent<AutonomousModeInterface> ().OnHallHit ();
		}
	}
}
