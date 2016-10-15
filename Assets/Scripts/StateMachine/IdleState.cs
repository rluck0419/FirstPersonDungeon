using UnityEngine;
using System.Collections;

public class IdleState : IPlayerState {

	private readonly StatePatternPlayer player;

	public IdleState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Look ();
	}

	public void ToIdleState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (idle)");
	}

	public void ToWalkState () {
		player.currentState = player.walkState;
	}

	public void ToRunState () {
		player.currentState = player.runState;
	}

	private void Look () {
		float mouseHorizontal = Input.GetAxis("Mouse X");
		float mouseVertical = Input.GetAxis ("Mouse Y");

		mouseHorizontal *= player.rotateSpeed;
		mouseVertical *= player.rotateSpeed;
		mouseVertical = Mathf.Clamp(mouseVertical, -90f, 90f);

		Vector3 newRotation = player.gameObject.transform.localEulerAngles;
		newRotation.z = 0f;
		player.lookDirection =  new Vector3 (-1 * mouseVertical, mouseHorizontal, 0f);
		player.gameObject.transform.eulerAngles = player.lookDirection + newRotation;

		if (Input.GetAxis ("Horizontal") > 0 || Input.GetAxis ("Vertical") > 0)
			ToWalkState ();
	}
}
