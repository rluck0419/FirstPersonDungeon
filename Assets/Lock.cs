using UnityEngine;
using System.Collections;

public class Lock : MonoBehaviour {
	private MeshRenderer keyMesh;
	private MeshRenderer energyMesh;
	private GameObject door;
	private Vector3 doorPos;
	private Vector3 finalPos;
	private bool open;

	// Use this for initialization
	void Start () {
		keyMesh = GameObject.Find ("KeyIn").GetComponent<MeshRenderer> ();
		energyMesh = GameObject.Find ("Energy").GetComponent<MeshRenderer> ();
		door = GameObject.Find ("Door");
		doorPos = door.transform.position;
		finalPos = doorPos + (Vector3.down * 15.0f);
	}

	void Update () {
		if (open == true) {
			Debug.Log ("door has been opened");
			doorPos = door.transform.position;
			door.transform.position = Vector3.Lerp (doorPos, finalPos, 0.03f);
		}
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Key") {
			Destroy (other.gameObject);
			keyMesh.enabled = true;
			energyMesh.enabled = true;
			open = true;
		}
	}
}
