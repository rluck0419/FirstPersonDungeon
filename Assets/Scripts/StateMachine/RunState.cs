using UnityEngine;
using System.Collections;

public class RunState : IPlayerState {

	private readonly StatePatternPlayer player;

	public RunState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Look ();
		Move ();
	}

	public void ToIdleState () {

	}

	public void ToWalkState () {

	}

	public void ToRunState () {

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
	}

	private void Move () {

	}
}
