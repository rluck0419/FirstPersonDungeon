﻿using UnityEngine;
using System.Collections;

public class WalkState : IPlayerState {

	private readonly StatePatternPlayer player;
	public Quaternion xRotation;

	public WalkState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Look ();
		Jump ();
		Move ();
	}

	public void ToIdleState () {
		Debug.Log ("player is now in idle state");
		player.currentState = player.idleState;
	}

	public void ToWalkState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (walk)");
	}

	public void ToHookState () {
		Debug.Log ("player is now in hook state");
		player.currentState = player.hookState;
	}

	private void Look () {
		if (player.rigidbody.velocity == Vector3.zero)
			ToIdleState ();
		
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

	private void Jump () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			player.rigidbody.velocity = Vector3.up * 10;
		}
	}

	private void Move () {
		if (player.grounded) {
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
			targetVelocity = player.transform.TransformDirection(targetVelocity);
			targetVelocity *= player.moveSpeed;

			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = player.rigidbody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -player.maxVelocityChange, player.maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -player.maxVelocityChange, player.maxVelocityChange);
			velocityChange.y = 0;
			player.rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

			// Jump
			if (player.canJump && Input.GetButton("Jump")) {
				player.rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
			}
		}

		// We apply gravity manually for more tuning control
		player.rigidbody.AddForce(new Vector3 (0, -player.gravity * player.rigidbody.mass, 0));

		player.grounded = false;
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * player.jumpHeight * player.gravity);
	}
}
