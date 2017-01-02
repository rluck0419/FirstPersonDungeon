using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	public float duration = 2f;
	public float direction = 1f;
	private Vector3 openPosition;
	private Vector3 closedPosition;

	void Start () {
		closedPosition = transform.position;
		openPosition = closedPosition + (Vector3.right * direction * 5f);
	}

	void OnTriggerEnter () {
		StartCoroutine ("Open");
	}

	void OnTriggerExit () {
		StartCoroutine ("Close");
	}

	IEnumerator Open() {
		float elapsed = 0f;
		while (elapsed < duration) {
			transform.position = Vector3.Lerp (closedPosition, openPosition, (elapsed / duration));
			elapsed += Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator Close() {
		float elapsed = 0f;
		while (elapsed < duration) {
			transform.position = Vector3.Lerp (openPosition, closedPosition, (elapsed / duration));
			elapsed += Time.deltaTime;
			yield return null;
		}
	}
}
