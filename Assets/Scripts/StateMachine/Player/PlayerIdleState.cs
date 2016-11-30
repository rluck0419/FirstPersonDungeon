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
		if (player.carrying) {
			CheckThrow ();
			CheckDrop ();
			Carry (player.carriedObject);
		} else {
			CheckInteraction ();
		}
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

	public void ToPlayerSneakState () {
		Debug.Log ("player is now in sneak state");
		player.transform.localScale -= (Vector3.up * 0.5f);
		player.currentState = player.sneakState;
	}

	private void Transition () {
//		if (Input.GetKeyDown (KeyCode.LeftCommand) || Input.GetKeyDown (KeyCode.RightCommand))
//			ToPlayerBounceState ();
		if (Input.GetKeyDown (KeyCode.C))
			ToPlayerSneakState ();
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

	private void CheckInteraction () {
		int x = Screen.width / 2;
		int y = Screen.height / 2;

		Ray ray = player.mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x,y));

		// within-reach pickup
		if (Input.GetKeyDown (KeyCode.E)) {
			Pickup (ray);
		}

		// TK pickup
		if (Input.GetMouseButtonDown(0)) {
			Pull (ray);
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			Push (ray);
		}
	}

	private void Pickup (Ray ray) {
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit)) {
			player.pickup = hit.collider.GetComponent<Pickupable>();

			if (player.pickup != null) {
				player.hitColliders = Physics.OverlapSphere(player.mainCamera.transform.position, player.pickupRadius);
				foreach (Collider i in player.hitColliders) {
					if (i.gameObject == player.pickup.gameObject) {
						player.carrying = true;
						player.carriedObject = player.pickup.gameObject;
						player.pickupRigidbody = player.carriedObject.GetComponent<Rigidbody>();
						player.pickupRigidbody.velocity = Vector3.zero;
						player.pickupRigidbody.useGravity = false;
					}
				}
			}
		}
	}

	private void Pull (Ray ray) {
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit)) {
			player.pickup = hit.collider.GetComponent<Pickupable>();
			//				l = hit.collider.GetComponent<Latchable> ();

			if (player.pickup != null) {
				player.carrying = true;
				player.carriedObject = player.pickup.gameObject;
				player.pickupRigidbody = player.carriedObject.GetComponent<Rigidbody>();
				player.pickupRigidbody.velocity = Vector3.zero;
				player.pickupRigidbody.useGravity = false;
			}

			//				if (l != null) {
			//					hooked = true;
			//					gravity = 0f;
			//				}
		}
	}

	private void Push (Ray ray) {
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			player.pickup = hit.collider.GetComponent<Pickupable> ();
			if (player.pickup != null) {
				player.pickup.gameObject.GetComponent<Rigidbody> ().AddForce (player.transform.forward * player.thrust);
			}
		}
	}

	private void Carry (GameObject o) {
		if (player.carrying==true && player.carriedObject!=null) {
			o.transform.position = Vector3.Lerp (
				o.transform.position,
				player.mainCamera.transform.position + (player.mainCamera.transform.forward * player.distance * 2f),
				Time.deltaTime * player.smooth
			);
		}
	}

	// Check if item should be dropped
	private void CheckDrop () {
		if (player.carriedObject != null) {
			if(Input.GetKeyDown (KeyCode.E)) {
				DropObject();
			}
		}
	}

	// Drop carried object
	private void DropObject () {
		player.carrying = false;
		player.pickupRigidbody = player.carriedObject.GetComponent<Rigidbody> ();
		player.pickupRigidbody.velocity = Vector3.zero;
		player.pickupRigidbody.useGravity = true;
		player.carriedObject = null;
	}

	// check if item should be thrown
	private void CheckThrow () {
		if(player.carrying == true && Input.GetMouseButtonDown(0)) {
			player.carriedObject.GetComponent<Rigidbody>().isKinematic = false;
			ThrowObject();
		}
	}

	// Throw carried object
	private void ThrowObject () {
		player.carrying = false;
		player.thrownObject = player.carriedObject;
		player.carriedObject = null;

		player.thrownObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		player.thrownObject.GetComponent<Rigidbody> ().useGravity = true;
		player.thrownObject.GetComponent<Rigidbody> ().AddForce(player.mainCamera.transform.forward * player.thrust);

		player.thrownObject = null;
	}	
}
