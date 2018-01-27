using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Script to detect car collision 
 * This script needs to be added as component of a traffic cone or wall(cube)
 * When the car collide execute some function
 * 
 * This prevent bad drivers dataset :P
 * 
 * dNesvera
 */

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
