using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointWall : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col){
		// Stop recording
		//GameObject.Find ("Datalogger").GetComponent<TrainingDatalogger>().OnHallHit();
		Debug.Log(col.transform.parent);
	}
}
