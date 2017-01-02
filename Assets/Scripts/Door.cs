using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	public float duration = 2f;

	// set this to -1 or 1 (left or right, respectively)
	public float direction = 1f;
	public bool zaxis = false;

	private Vector3 openPosition;
	private Vector3 closedPosition;
	private Coroutine lastRoutine = null;

	void Start () {
		closedPosition = transform.position;
		if (zaxis) {
			openPosition = closedPosition + (Vector3.forward * direction * 5f);
		} else {
			openPosition = closedPosition + (Vector3.right * direction * 5f);
		}
	}

	void OnTriggerEnter () {
		if (lastRoutine != null) {
			StopCoroutine (lastRoutine);
		}
		lastRoutine = StartCoroutine (MoveDoor (transform.position, openPosition));
	}

	void OnTriggerExit () {
		if (lastRoutine != null) {
			StopCoroutine (lastRoutine);
		}
		lastRoutine = StartCoroutine (MoveDoor (transform.position, closedPosition));
	}

	IEnumerator MoveDoor (Vector3 start, Vector3 end) {
		float elapsed = 0f;
		while (elapsed <= duration) {
			transform.position = Vector3.Lerp (start, end, (elapsed / duration));
			elapsed += Time.deltaTime;
			yield return null;
		}
	}
}
