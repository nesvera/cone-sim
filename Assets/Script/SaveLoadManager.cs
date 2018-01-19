using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager{

	public static void SaveTrack( Dictionary<string, GameObject> cones ){

		// Get track name
		string track_name = GameControl.track_name + ".dat";

		// Create the "savable" class to the track
		TrackData track = new TrackData();

		// Get car reference
		GameObject car_object = GameObject.Find ("Car").transform.GetChild(0).gameObject;

		// Create a "savable" car
		CarData car_data = new CarData (car_object.transform.position, car_object.transform.eulerAngles);

		// Save on the track class
		track.car = car_data;

		// Loop throgh the dictionary of cones
		foreach (KeyValuePair<string, GameObject> cone in cones) {

			// Create a new "savable" cone class and copy the data from the cone of the game
			ConeData cone_class = new ConeData(int.Parse(cone.Value.name), cone.Value.transform.position, cone.Value.transform.eulerAngles);

			// Gambiarra to get the neighbors of the cone
			foreach (KeyValuePair<string, GameObject> cone_neighbor in cone.Value.GetComponent<neighbor>().neighbors) {

				cone_class.neighbor.Add (int.Parse (cone_neighbor.Value.name));
			}

			// Add the cone to the list on track class
			track.cone_list.Add (cone_class);
		}
			
		var path = Application.dataPath + "/../Track/";

		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream stream = new FileStream (Application.dataPath + "/../Track/" + track_name, FileMode.Create);

		// Save track on file
		bf.Serialize (stream, track);
		stream.Close ();
	}

	public static TrackData LoadTrack(){

		string track_name = GameControl.track_name + ".dat";

		// Check if the track exist
		if (File.Exists (Application.dataPath + "/../Track/" + track_name)) {

			// Read the track saved
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream stream = new FileStream (Application.dataPath + "/../Track/" + track_name, FileMode.Open);

			// Load the "savable" class to the track
			TrackData track = bf.Deserialize (stream) as TrackData;

			return track;

			stream.Close ();
		
		} 

		return null;
	}
		
}

// Class that store all the data of a track
[Serializable]
public class TrackData{
	public CarData car;
	public List<ConeData> cone_list;

	// Constructor
	public TrackData(){
		cone_list = new List<ConeData> ();
	}
}

// Cone properties
[Serializable]
public class ConeData{

	public int id;
	public float[] position = new float[3];
	public float[] rotation = new float[3];
	public List<int> neighbor;

	// Constructor
	public ConeData( int _id, Vector3 _position, Vector3 _rotation){
		id = _id;
		position [0] = _position.x;
		position [1] = _position.y;
		position [2] = _position.z;

		rotation [0] = _rotation.x;
		rotation [1] = _rotation.y;
		rotation [2] = _rotation.z;

		neighbor = new List<int> ();
	}


}

// Car properties
[Serializable]
public class CarData{

	public float[] position = new float[3];
	public float[] rotation = new float[3];

	// Constructor
	public CarData( Vector3 _position, Vector3 _rotation ){
		position [0] = _position.x;
		position [1] = _position.y;
		position [2] = _position.z;

		rotation [0] = _rotation.x;
		rotation [1] = _rotation.y;
		rotation [2] = _rotation.z;
	}

}