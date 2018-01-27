using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Script to handle click on an item of the scrollView
 * This script is a component of the TrackItemPrefab (Asset/ScrollView)
 * 
 * Get name and image of the clicked item
 * 
 * dNesvera
 */

public class TrackItemScript : MonoBehaviour {

	public static Image last_image_clicked;

	public void OnImageClicked(){
		// Get track_name from the list cell
		var track_name = GetComponent<Image> ().transform.Find ("Title").GetComponent<Text> ().text;
		GameControl.track_name = track_name;

		if (last_image_clicked == null) {
			last_image_clicked = GetComponent<Image> ();
			last_image_clicked.color = new Color32 (0xc8, 0xc8, 0xc8, 0xff);

		} else {
			last_image_clicked.color = new Color32 (0xff, 0xff, 0xff, 0xff);
			last_image_clicked = GetComponent<Image> ();
			last_image_clicked.color = new Color32 (0xc8, 0xc8, 0xc8, 0xff);
		}

		
	}
}
