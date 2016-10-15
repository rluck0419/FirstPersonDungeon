using UnityEngine;
using System.Collections;

public class WalkState : IPlayerState {

	private readonly StatePatternPlayer player;

	public WalkState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Look ();
		Move ();
	}

	public void ToIdleState () {
		player.currentState = player.idleState;
	}

	public void ToWalkState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (walk)");
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

		Vector3 currentRotation = player.gameObject.transform.localEulerAngles;
		currentRotation.z = 0f;
		player.lookDirection = new Vector3 (-1 * mouseVertical, mouseHorizontal, 0f);
		player.lookDirection += currentRotation;
		player.gameObject.transform.eulerAngles = player.lookDirection;
	}

	private void Move () {
		CharacterController controller = player.gameObject.GetComponent<CharacterController> ();

		player.moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		player.moveDirection = player.gameObject.transform.TransformDirection(player.moveDirection);
		player.moveDirection *= player.moveSpeed;
		player.moveDirection.y = 0f;

		controller.Move(player.moveDirection * Time.deltaTime);
	}
}
