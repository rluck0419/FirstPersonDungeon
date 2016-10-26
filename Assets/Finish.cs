using UnityEngine;
using System.Collections;

public class Finish : MonoBehaviour {

	void OnTriggerEnter () {
		Debug.Log("Finished! Your Time for level 1: " + Time.realtimeSinceStartup);
	}
}
