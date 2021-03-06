﻿using UnityEngine;
using System.Collections;

public class PlayerBounceState : IPlayerState {

	private readonly StatePatternPlayer player;
	//public Quaternion xRotation;

	public PlayerBounceState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Transition ();
		player.Look ();
		player.Move ();
		if (player.carrying)
        {
            if (player.CheckThrow())
                return;
            if (player.CheckDrop())
                return;
            player.Carry (player.carriedObject);
		} else {
			player.CheckInteraction ();
		}
	}

	public void ToPlayerIdleState () {
		Debug.Log ("player is now in idle state");
		player.currentState = player.idleState;
	}

	public void ToPlayerWalkState () {
		player.GetComponent<CapsuleCollider> ().material.bounciness = 0f;
		Debug.Log ("player is now in walk state");
		player.currentState = player.walkState;
	}

	public void ToPlayerBounceState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (bounce)");
	}

	public void ToPlayerHookState () {
		Debug.Log ("player is now in hook state");
		player.currentState = player.hookState;
	}

	public void ToPlayerSneakState () {
		Debug.Log ("player is now in sneak state");
		player.transform.localScale -= (Vector3.up * 0.5f);
		player.distToGround -= 0.5f;
		player.currentState = player.sneakState;
	}

    public void ToPlayerTargetState()
    {
        Debug.Log("player is now in target state");
        player.currentState = player.targetState;
    }

    private void Transition () {
		//if (Input.GetKeyDown (KeyCode.C))
		//	ToPlayerSneakState ();
		//if (Input.GetKeyDown (KeyCode.B) || player.collided)
		//	ToPlayerWalkState ();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int x = Screen.width / 2;
            int y = Screen.height / 2;

            Ray ray = player.mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));

            player.targetObject = player.GetTarget(ray);

            if (player.targetObject != null)
            {
                player.targeting = true;
                ToPlayerTargetState();
            }
        }
    }

    //private void Look()
    //{
    //    // Allow the script to clamp based on a desired target value.
    //    var targetOrientation = Quaternion.Euler(player.targetDirection);

    //    // Get raw mouse input for a cleaner reading on more sensitive mice.
    //    var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

    //    // Scale input against the sensitivity setting and multiply that against the smoothing value.
    //    mouseDelta = Vector2.Scale(mouseDelta, new Vector2(player.sensitivity.x * player.smoothing.x, player.sensitivity.y * player.smoothing.y));

    //    // Interpolate mouse movement over time to apply smoothing delta.
    //    player.smoothMouse.x = Mathf.Lerp(player.smoothMouse.x, mouseDelta.x, 1f / player.smoothing.x);
    //    player.smoothMouse.y = Mathf.Lerp(player.smoothMouse.y, mouseDelta.y, 1f / player.smoothing.y);

    //    // Find the absolute mouse movement value from point zero.
    //    player.mouseAbsolute += player.smoothMouse;

    //    // Clamp and apply the local x value first, so as not to be affected by world transforms.
    //    if (player.clampInDegrees.x < 360)
    //        player.mouseAbsolute.x = Mathf.Clamp(player.mouseAbsolute.x, -player.clampInDegrees.x * 0.5f, player.clampInDegrees.x * 0.5f);

    //    xRotation = Quaternion.AngleAxis(player.mouseAbsolute.x, targetOrientation * Vector3.up);
    //    player.transform.localRotation = xRotation;

    //    player.transform.localRotation *= targetOrientation;

    //    if (player.clampInDegrees.y < 360)
    //        player.mouseAbsolute.y = Mathf.Clamp(player.mouseAbsolute.y, -player.clampInDegrees.y * 0.5f, player.clampInDegrees.y * 0.5f);

    //    var yRotation = Quaternion.AngleAxis(-player.mouseAbsolute.y, targetOrientation * Vector3.right);
    //    player.mainCamera.transform.localRotation = yRotation;

    //    player.mainCamera.transform.localRotation *= targetOrientation;
    //}

    //private void Move()
    //{
    //    Vector3 velocity = player.rigidbody.velocity;

    //    RaycastHit hit;
    //    if (Physics.Raycast(player.transform.position, -Vector3.up, out hit, player.distToGround + 0.5f))
    //    {
    //        if (player.canJump && Input.GetButton("Jump"))
    //        {
    //            player.rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
    //        }
    //    }

    //    Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

    //    targetVelocity.Normalize();
    //    if (targetVelocity != Vector3.zero)
    //    {
    //        player.GetComponent<CapsuleCollider>().material.dynamicFriction = 0.5f;
    //        targetVelocity = player.transform.TransformDirection(targetVelocity);

    //        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
    //            player.moveSpeed = 14f;
    //        else
    //            player.moveSpeed = 7f;

    //        targetVelocity *= player.moveSpeed;


    //        // Apply a force that attempts to reach our target velocity
    //        Vector3 velocityChange = (targetVelocity - velocity);
    //        velocityChange.x = Mathf.Clamp(velocityChange.x, -player.maxVelocityChange, player.maxVelocityChange);
    //        velocityChange.z = Mathf.Clamp(velocityChange.z, -player.maxVelocityChange, player.maxVelocityChange);
    //        velocityChange.y = 0;

    //        // move "forward" across object - parallel to face of obstacle based on angle
    //        RaycastHit obstacle;
    //        Vector3 feet = player.transform.position;
    //        feet.y = 0f;
    //        if (Physics.Raycast(feet, velocityChange, out obstacle, 1.25f))
    //            velocityChange = velocityChange - obstacle.normal * Vector3.Dot(velocityChange, obstacle.normal);

    //        velocityChange = Quaternion.Euler(hit.normal) * velocityChange;

    //        player.rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    //    }
    //    else
    //    {
    //        player.GetComponent<CapsuleCollider>().material.dynamicFriction = 1f;
    //    }

    //    if (hit.normal != Vector3.up)
    //    {
    //        player.maxVelocityChange = 1f;
    //    }
    //    else
    //    {
    //        player.maxVelocityChange = 0.5f;
    //    }

    //    // We apply gravity manually for more tuning control
    //    player.rigidbody.AddForce(new Vector3(0, -player.gravity * player.rigidbody.mass, 0));
    //}

    //float CalculateJumpVerticalSpeed()
    //{
    //    // From the jump height and gravity we deduce the upwards speed 
    //    // for the character to reach at the apex.
    //    return Mathf.Sqrt(2 * player.jumpHeight * player.gravity);
    //}

    //private void CheckInteraction()
    //{
    //    int x = Screen.width / 2;
    //    int y = Screen.height / 2;

    //    Ray ray = player.mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));

    //    // within-reach pickup
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        Pickup(ray);
    //    }

    //    // TK pickup
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Pull(ray);
    //    }

    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        Push(ray);
    //    }
    //}

    //private void Pickup(Ray ray)
    //{
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        player.pickup = hit.collider.GetComponent<Pickupable>();

    //        if (player.pickup != null)
    //        {
    //            player.hitColliders = Physics.OverlapSphere(player.mainCamera.transform.position, player.pickupRadius);
    //            foreach (Collider i in player.hitColliders)
    //            {
    //                if (i.gameObject == player.pickup.gameObject)
    //                {
    //                    player.carrying = true;
    //                    player.carriedObject = player.pickup.gameObject;
    //                    player.pickupRigidbody = player.carriedObject.GetComponent<Rigidbody>();
    //                    player.pickupRigidbody.velocity = Vector3.zero;
    //                    player.pickupRigidbody.useGravity = false;
    //                }
    //            }
    //        }
    //    }
    //}

    //private void Pull(Ray ray)
    //{
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        player.pickup = hit.collider.GetComponent<Pickupable>();
    //        //				l = hit.collider.GetComponent<Latchable> ();

    //        if (player.pickup != null)
    //        {
    //            player.carrying = true;
    //            player.carriedObject = player.pickup.gameObject;
    //            player.pickupRigidbody = player.carriedObject.GetComponent<Rigidbody>();
    //            player.pickupRigidbody.velocity = Vector3.zero;
    //            player.pickupRigidbody.useGravity = false;
    //        }

    //        //				if (l != null) {
    //        //					hooked = true;
    //        //					gravity = 0f;
    //        //				}
    //    }
    //}

    //private void Push(Ray ray)
    //{
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        player.pickup = hit.collider.GetComponent<Pickupable>();
    //        if (player.pickup != null)
    //        {
    //            player.pickup.gameObject.GetComponent<Rigidbody>().AddForce(player.transform.forward * player.thrust);
    //        }
    //    }
    //}

    //private void Carry(GameObject o)
    //{
    //    if (player.carrying == true && player.carriedObject != null)
    //    {
    //        o.transform.position = Vector3.Lerp(
    //            o.transform.position,
    //            player.mainCamera.transform.position + (player.mainCamera.transform.forward * player.distance * 2f),
    //            Time.deltaTime * player.smooth
    //        );
    //    }
    //}

    //// Check if item should be dropped
    //private void CheckDrop()
    //{
    //    if (player.carriedObject != null)
    //    {
    //        if (Input.GetKeyDown(KeyCode.E))
    //        {
    //            DropObject();
    //        }
    //    }
    //}

    //// Drop carried object
    //private void DropObject()
    //{
    //    player.carrying = false;
    //    player.pickupRigidbody = player.carriedObject.GetComponent<Rigidbody>();
    //    player.pickupRigidbody.velocity = Vector3.zero;
    //    player.pickupRigidbody.useGravity = true;
    //    player.carriedObject = null;
    //}

    //// check if item should be thrown
    //private void CheckThrow()
    //{
    //    if (player.carrying == true && Input.GetMouseButtonDown(0))
    //    {
    //        player.carriedObject.GetComponent<Rigidbody>().isKinematic = false;
    //        ThrowObject();
    //    }
    //}

    //// Throw carried object
    //private void ThrowObject()
    //{
    //    player.carrying = false;
    //    player.thrownObject = player.carriedObject;
    //    player.carriedObject = null;

    //    player.thrownObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //    player.thrownObject.GetComponent<Rigidbody>().useGravity = true;
    //    player.thrownObject.GetComponent<Rigidbody>().AddForce(player.mainCamera.transform.forward * player.thrust);

    //    player.thrownObject = null;
    //}
}
