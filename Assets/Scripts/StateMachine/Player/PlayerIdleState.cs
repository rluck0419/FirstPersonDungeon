using UnityEngine;
using System.Collections;

public class PlayerIdleState : IPlayerState {

	private readonly StatePatternPlayer player;
	private Pickupable p;
	private float pickupRadius = 2.0f;
	private Rigidbody r;
	private GameObject carriedObject;
	private GameObject thrownObject;
	private Collider[] hitColliders;
	private float distance = 3.0f;
	private float thrust = 1024.0f;
	private float smooth = 7.0f;
	private float rotation = 2.0f;
	private bool carrying = false;

	public PlayerIdleState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Transition ();
		Look ();
		Jump ();
		if (carrying) {
			CheckThrow ();
			CheckDrop ();
			Carry (carriedObject);
		} else {
			Pickup ();
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

	private void Pickup () {
		int x = Screen.width / 2;
		int y = Screen.height / 2;

		Ray ray = player.mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x,y));
		RaycastHit hit;

		// TK pickup
		if (Input.GetMouseButtonDown(0)) {
			if (Physics.Raycast(ray, out hit)) {
				p = hit.collider.GetComponent<Pickupable>();
//				l = hit.collider.GetComponent<Latchable> ();

				if (p != null) {
					carrying = true;
					carriedObject = p.gameObject;
					r = carriedObject.GetComponent<Rigidbody>();
					r.useGravity = false;
				}

//				if (l != null) {
//					hooked = true;
//					gravity = 0f;
//				}
			}
		}

		// within-reach pickup
		if (Input.GetKeyDown (KeyCode.E)) {
			if (Physics.Raycast(ray, out hit)) {
				p = hit.collider.GetComponent<Pickupable>();

				if (p != null) {
					hitColliders = Physics.OverlapSphere(player.mainCamera.transform.position, pickupRadius);
					foreach (Collider i in hitColliders) {
						if (i.gameObject == p.gameObject) {
							carrying = true;
							carriedObject = p.gameObject;
							r = carriedObject.GetComponent<Rigidbody>();
							r.useGravity = false;
						}
					}
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			if (Physics.Raycast (ray, out hit)) {
				p = hit.collider.GetComponent<Pickupable> ();
				if (p != null) {
					p.gameObject.GetComponent<Rigidbody> ().AddForce (player.transform.forward * thrust);
				}
			}
		}
	}

	private void Carry (GameObject o) {
		if (carrying==true && carriedObject!=null) {
			o.transform.position = Vector3.Lerp (
				o.transform.position,
				player.mainCamera.transform.position + (player.mainCamera.transform.forward * distance),
				Time.deltaTime * smooth
			);
			o.transform.Rotate(Vector3.right * rotation);
		}
	}

	// Check if item should be dropped
	private void CheckDrop () {
		if (carriedObject != null) {
			if(Input.GetKeyDown (KeyCode.E)) {
				DropObject();
			}
		}
	}

	// Drop carried object
	private void DropObject () {
		carrying = false;
		carriedObject.GetComponent<Rigidbody>().useGravity = true;
		carriedObject = null;
	}

	// check if item should be thrown
	private void CheckThrow () {
		if(carrying == true && Input.GetMouseButtonDown(0)) {
			carriedObject.GetComponent<Rigidbody>().isKinematic = false;
			ThrowObject();
		}
	}

	// Throw carried object
	private void ThrowObject () {
		carrying = false;
		thrownObject = carriedObject;
		carriedObject = null;

		thrownObject.GetComponent<Rigidbody>().useGravity = true;
		thrownObject.GetComponent<Rigidbody>().AddForce(player.transform.forward * thrust);

		thrownObject = null;
	}	
}
