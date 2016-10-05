using UnityEngine;
using System.Collections;

public class Deathbox : MonoBehaviour {
	public Vector3 playerReset;
	public Vector3 keyReset;

	void Start () {
	
	}

	void OnTriggerEnter (Collider deadObject) {
		deadObject.gameObject.transform.position = playerReset;
	}
}
