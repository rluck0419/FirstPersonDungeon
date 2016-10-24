using UnityEngine;
using System.Collections;

public class SneakState : IPlayerState {

	private readonly StatePatternPlayer player;
	public Quaternion xRotation;

	public SneakState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Transition ();
		Look ();
		Move ();
	}

	public void ToIdleState () {
		Debug.Log ("player is now in idle state");
		player.currentState = player.idleState;
	}

	public void ToWalkState () {
		player.transform.localScale += player.crouch;
		player.transform.position += player.crouch;
		Debug.Log ("player is now in walk state");
		player.currentState = player.walkState;
	}

	public void ToHookState () {
		Debug.Log ("player is now in hook state");
		player.currentState = player.hookState;
	}

	public void ToSneakState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (sneak)");
	}

	public void ToBounceState () {
		Debug.Log ("player is now in bounce state");
		player.GetComponent<CapsuleCollider> ().material.bounciness = 1f;
		player.currentState = player.bounceState;
	}

	private void Transition () {
		if (Input.GetKeyDown (KeyCode.C) || Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift))
			ToWalkState ();
		if (Input.GetKeyDown (KeyCode.LeftCommand) || Input.GetKeyDown (KeyCode.RightCommand))
			ToBounceState ();
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

		xRotation = Quaternion.AngleAxis(player.mouseAbsolute.x, targetOrientation * Vector3.up);
		player.transform.localRotation = xRotation;

		player.transform.localRotation *= targetOrientation;

		if (player.clampInDegrees.y < 360)
			player.mouseAbsolute.y = Mathf.Clamp(player.mouseAbsolute.y, -player.clampInDegrees.y * 0.5f, player.clampInDegrees.y * 0.5f);

		var yRotation = Quaternion.AngleAxis (-player.mouseAbsolute.y, targetOrientation * Vector3.right);
		player.mainCamera.transform.localRotation = yRotation;

		player.mainCamera.transform.localRotation *= targetOrientation;
	}

	private void Move () {
		RaycastHit hit;
		if (Physics.Raycast (player.transform.position, -Vector3.up, out hit, player.distToGround + 2f)) {
			// Calculate how fast we should be moving
			Vector3 velocity = player.rigidbody.velocity;
			Vector3 targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			targetVelocity.Normalize ();
			if (targetVelocity != Vector3.zero) {
				targetVelocity = player.transform.TransformDirection (targetVelocity);

				player.moveSpeed = 4f;

				targetVelocity *= player.moveSpeed;


				// Apply a force that attempts to reach our target velocity
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp (velocityChange.x, -player.maxVelocityChange, player.maxVelocityChange);
				velocityChange.z = Mathf.Clamp (velocityChange.z, -player.maxVelocityChange, player.maxVelocityChange);
				velocityChange.y = 0;
				velocityChange = Quaternion.Euler (hit.normal) * velocityChange;
				player.rigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
			}


			if (hit.normal != Vector3.up) {
				player.maxVelocityChange = 1f;
			} else {
				player.maxVelocityChange = 0.5f;
			}

			if (player.canJump && Input.GetButton ("Jump")) {
				player.rigidbody.velocity = new Vector3 (velocity.x, CalculateJumpVerticalSpeed (), velocity.z);
			}
		}

		// We apply gravity manually for more tuning control
		player.rigidbody.AddForce(new Vector3 (0, -player.gravity * player.rigidbody.mass, 0));
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * player.jumpHeight * player.gravity);
	}
}
