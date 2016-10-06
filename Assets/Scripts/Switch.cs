using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Switch : MonoBehaviour {
	private bool pressed = false;
	private List <GameObject> objsOnSwitch = new List <GameObject> ();
	private GameObject tileSwitch;
	private Vector3 position;
	private Vector3 origPosition;
	private Vector3 pressedPosition;

	private GameObject window;
	private Vector3 windowPosition;
	private Vector3 origWindowPos;
	private Vector3 pressedWindowPos;


	// Use this for initialization
	void Start () {
		tileSwitch = GameObject.Find("Switch");
		position = tileSwitch.transform.position;
		origPosition = position;
		pressedPosition = position + Vector3.down;

		window = GameObject.Find ("Window");
		windowPosition = window.transform.position;
		origWindowPos = windowPosition;
		pressedWindowPos = windowPosition + (Vector3.left * 5f);
	}
	
	// Update is called once per frame
	void Update () {
		position = tileSwitch.transform.position;
		windowPosition = window.transform.position;
		if (pressed == true) {
			tileSwitch.transform.position = Vector3.Lerp (position, pressedPosition, 0.1f);
			window.transform.position = Vector3.Lerp (windowPosition, pressedWindowPos, 0.1f);
		} else {
			tileSwitch.transform.position = Vector3.Lerp (position, origPosition, 0.1f);
			window.transform.position = Vector3.Lerp (windowPosition, origWindowPos, 0.1f);
		}
	}

	void OnTriggerEnter (Collider other) {
		objsOnSwitch.Add (other.gameObject); 
		pressed = true; 
	}

	void OnTriggerExit (Collider other) {
		objsOnSwitch.Remove (other.gameObject);
		if (objsOnSwitch.Count == 0) {
			pressed = false;
		}
	}
}
