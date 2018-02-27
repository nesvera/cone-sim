using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * 
 * dNesvera 
 */

public class ConeClass{
	private static int id_counter = 0;

	public GameObject obj;
	public int id;
	public Vector3 position;
	public Quaternion rotation;
	public List<ConeData> neighboor;

	public ConeClass(){
		id = id_counter;			// The object receives an unique id
		id_counter++;				// Increment the global id counter
	}

	public ConeClass(GameObject obj, Vector3 position ){
		id = id_counter;			// The object receives an unique id
		id_counter++;				// Increment the global id counter

		obj = obj;
		position = position;
		rotation = Quaternion.Euler (-90, 0, 0);
	}

}
