using UnityEngine;
using System.Collections;

public class PlayerIdleState : IPlayerState {

	private readonly StatePatternPlayer player;

	public PlayerIdleState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Transition ();
		Look ();
		Jump ();
	}

	public void ToPlayerIdleState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (idle)");
	}

	public void ToPlayerWalkState () {
		Debug.Log ("player is now in walk state");
		player.currentState = player.walkState;
	}
		
	public void ToPlayerBounceState () {
		Debug.Log ("player is now in bounce state");
		player.collided = false;
		player.GetComponent<CapsuleCollider> ().material.bounciness = 1f;
		player.currentState = player.bounceState;
	}

	public void ToPlayerHookState () {
		Debug.Log ("player is now in hook state");
		player.currentState = player.hookState;
	}

	private void Transition () {
		if (Input.GetKeyDown (KeyCode.LeftCommand) || Input.GetKeyDown (KeyCode.RightCommand))
			ToPlayerBounceState ();
		if (Input.GetAxis ("Vertical") != 0 || Input.GetAxis ("Horizontal") != 0 || player.rigidbody.velocity != Vector3.zero)
			ToPlayerWalkState ();
	}

	private void Look () {		
		// Allow the script to clamp based on a desired target value.
		var targetOrientation = Quaternion.Euler(player.targetDirection);

		// Get raw mouse input for a cleaner reading on more sensitive mice.
		var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		// Scale input against the sensitivity setting and multiply that against the smoothing value.
		mouseDelta = Vector2.Scale(mouseDelta, new Vector2(player.sensitivity.x * player.smoothing.x, player.sensitivity.y * player.smoothing.y));

		// Interpolate mouse movement over time to apply smoothing delta.
		player.smoothMouse.x = Mathf.Lerp(player.smoothMouse.x, mouseDelta.x, 1f / player.smoothing.x);
		player.smoothMouse.y = Mathf.Lerp(player.smoothMouse.y, mouseDelta.y, 1f / player.smoothing.y);

		// Find the absolute mouse movement value from point zero.
		player.mouseAbsolute += player.smoothMouse;

		// Clamp and apply the local x value first, so as not to be affected by world transforms.
		if (player.clampInDegrees.x < 360)
			player.mouseAbsolute.x = Mathf.Clamp(player.mouseAbsolute.x, -player.clampInDegrees.x * 0.5f, player.clampInDegrees.x * 0.5f);

		var xRotation = Quaternion.AngleAxis(player.mouseAbsolute.x, targetOrientation * Vector3.up);
		player.transform.localRotation = xRotation;

		player.transform.localRotation *= targetOrientation;

		if (player.clampInDegrees.y < 360)
			player.mouseAbsolute.y = Mathf.Clamp(player.mouseAbsolute.y, -player.clampInDegrees.y * 0.5f, player.clampInDegrees.y * 0.5f);

		var yRotation = Quaternion.AngleAxis (-player.mouseAbsolute.y, targetOrientation * Vector3.right);
		player.mainCamera.transform.localRotation = yRotation;

		player.mainCamera.transform.localRotation *= targetOrientation;
	}

	bool Grounded () {
		return Physics.Raycast(player.transform.position, -Vector3.up, player.distToGround + 0.5f);
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * player.jumpHeight * player.gravity);
	}

	private void Jump () {
		if (Grounded() && Input.GetKeyDown (KeyCode.Space)) {
			player.rigidbody.velocity = new Vector3(player.rigidbody.velocity.x, CalculateJumpVerticalSpeed(), player.rigidbody.velocity.z);
		}
	}
}
