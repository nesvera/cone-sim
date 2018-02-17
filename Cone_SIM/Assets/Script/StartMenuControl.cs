 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.IO;
using TMPro;

public class StartMenuControl : MonoBehaviour {

	[Header("ScrollView")]
	public GameObject ItemPrefab;
	public GameObject ScrollView_Autonomous;
	public GameObject ScrollView_Training;
	public GameObject ScrollView_Editor;

	// Search and show all track saved on the Tracks folder
	void Awake(){

		// Reset the track+name from the Game control
		GameControl.track_name = "";

		GameControl.current_mode = "Start Menu";

		if (ItemPrefab == null && ScrollView_Editor == null) {
			return;
		}

		// Search in the Track folder for saved tracks
		var info = new DirectoryInfo (Application.dataPath + "/../Track/");
		var fileInfo = info.GetFiles ();
		foreach (var file in fileInfo) {

			// Check extension of the file
			if (Path.GetExtension (file.ToString ()) == ".dat") {

				// Get just the name of the file
				string file_name = Path.GetFileNameWithoutExtension(file.ToString ());

				// Put the list cells in the 3 scrollviews

				// Autonomous
				var instance = GameObject.Instantiate(ItemPrefab.gameObject) as GameObject;
				var instance_content = ScrollView_Autonomous.transform.Find("Viewport/Content").gameObject.transform;

				// Set the parent of the cell list
				instance.transform.SetParent (instance_content, false);

				var title = instance.transform.Find("Title").GetComponent<Text> ();
				title.text = file_name;

				// Training
				instance = GameObject.Instantiate(ItemPrefab.gameObject) as GameObject;
				instance_content = ScrollView_Training.transform.Find("Viewport/Content").gameObject.transform;

				// Set the parent of the cell list
				instance.transform.SetParent (instance_content, false);

				title = instance.transform.Find("Title").GetComponent<Text> ();
				title.text = file_name;


				// Track editor
				instance = GameObject.Instantiate(ItemPrefab.gameObject) as GameObject;
				instance_content = ScrollView_Editor.transform.Find("Viewport/Content").gameObject.transform;

				// Set the parent of the cell list
				instance.transform.SetParent (instance_content, false);

				title = instance.transform.Find("Title").GetComponent<Text> ();
				title.text = file_name;


			}
		}

		// Reload top speed
		GameControl.setCarSpeed (PlayerPrefs.GetFloat ("top_speed"));
	}

	public void TrackEditorButton(){




	}

	public void AutonomousMode(){
		// Check that there is a name to create the track
		if (GameControl.track_name.Length > 0) {

			// Load the Track Editor Scene
			SceneManager.LoadScene("Autonomous Mode");
		}
	}

	public void TrainingMode(){
		// Check that there is a name to create the track
		if (GameControl.track_name.Length > 0) {

			// Load the Track Editor Scene
			SceneManager.LoadScene("Training Mode");
		}
	}

	// Load the track creator scene when edit track button is pressed
	public void EditTrackButton(){
		
		// Check that there is a name to create the track
		if (GameControl.track_name.Length > 0) {

			// Load the Track Editor Scene
			SceneManager.LoadScene("Track Editor");
		}

	}

	// Load the track creator scene when create button is pressed
	public void NewTrackButton(){
		// Reference to the inputfield 
		InputField new_track_name = GameObject.Find ("InputField_new_track_name").GetComponent<InputField> ();

		// Check that there is a name to create the track
		if (new_track_name.text.Length > 0) {

			if (new_track_name.text == "topzera") {
				GameObject.Find ("Canvas/inf_panel/Panel_top").gameObject.SetActive (true);
			}

			//Save the name of the new track
			GameControl.track_name = new_track_name.text;

			// Load the Track Editor Scene
			SceneManager.LoadScene("Track Editor");
		}

	}

	public void Settings(){

	}
}
