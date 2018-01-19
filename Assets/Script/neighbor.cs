using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class neighbor : MonoBehaviour {

	public GameObject wall_prefab;

	public Dictionary<string, GameObject> neighbors = new Dictionary<string, GameObject> ();
	private Dictionary<string, GameObject> walls = new Dictionary<string, GameObject> ();

	// Add a traffic cone neighbor to this cone and create a wall between them
	public void AddNeighboor(GameObject obj){
		if (obj != null) {
				
			// Check if already exist the walll before create a new
			if (!neighbors.ContainsKey (obj.name)) {
				neighbors.Add (obj.name, obj);
				CreateWall (this.gameObject, obj);
			}
		}
	}

	// Cone_1 = this
	// Cone_2 = neighboor
	void CreateWall(GameObject cone_1, GameObject cone_2){

		Vector3 pos_cone1 = new Vector3 (cone_1.transform.position.x, 0, cone_1.transform.position.z);
		Vector3 pos_cone2 = new Vector3 (cone_2.transform.position.x, 0, cone_2.transform.position.z);
		Vector3 direction = pos_cone1 - pos_cone2;

		float center_x = (cone_1.transform.position.x + cone_2.transform.position.x)/2;
		float center_z = (cone_1.transform.position.z + cone_2.transform.position.z)/2;
		float center_y = (cone_1.transform.position.y + cone_2.transform.position.y)/2;


		float lenght = Mathf.Sqrt (	Mathf.Pow (cone_1.transform.position.x - cone_2.transform.position.x, 2) + 
									Mathf.Pow (cone_1.transform.position.z - cone_2.transform.position.z, 2));

		// Instantiate a cube inside Walls GameObject
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.parent = GameObject.Find ("Walls").transform;

		// Rename the wall to the cone_2 ID
		cube.name = cone_2.name;

		cube.transform.position = new Vector3 (center_x, 0, center_z);

		cube.transform.localScale = new Vector3(0.3f, 0.01f, direction.magnitude);
		cube.transform.rotation = Quaternion.LookRotation (direction);

		cube.GetComponent<Renderer> ().material.color = new Color(255, 255, 255, 1f);

		walls.Add (cube.name, cube);

		// Create walls between the traffic cones
		if (GameControl.current_mode == "Training Mode" || GameControl.current_mode == "Autonomous Mode") {

			if (wall_prefab == null) {
				return;
			}

			Vector3 wall_position = new Vector3 (center_x, center_y, center_z);
			Quaternion wall_orientation = Quaternion.LookRotation (direction);

			GameObject new_wall = Instantiate (wall_prefab, wall_position, wall_orientation) as GameObject;

			float size = new_wall.transform.localScale.y;

			new_wall.transform.localScale = new Vector3(0.001f, 0.5f, direction.magnitude);
		
			new_wall.transform.rotation = Quaternion.LookRotation (direction);

		}
	}

	// Delete line between this cone and cone_2
	public void DeleteLine(GameObject cone){
		
		if (cone != null) {
			// Get reference from the line/wall stored on dictionary
			if (walls.ContainsKey (cone.name)) {
				GameObject line = walls[cone.name]; 

				// Remove from the dictionary and delete from game
				neighbors.Remove(cone.name);
				walls.Remove(cone.name);
				Destroy (line);
			}
		}

	}

	// Delete this cone and delete lines connected to this
	public void DeleteCone(){

		// Look throgh the walls dictionary and delete the walls connected to them
		foreach ( KeyValuePair<string, GameObject> wall in walls) {

			// Delete line connected to cones of the dictionary
			GameObject line = wall.Value;

			// Send message to them to delete the line to this cone too
			if (neighbors.ContainsKey (line.name)) {

				// Get the reference to the other cone connected
				GameObject other_cone = neighbors [line.name];

				// Send message to the other cone to delete the line
				other_cone.GetComponent<neighbor> ().DeleteLine (this.gameObject);
			}
				
			// Delete line of cone_1 to cone_2
			//walls.Remove (wall.Key);
			Destroy (line);
		}

		// Delete the cone
		Destroy (this.gameObject);
	}
}
	