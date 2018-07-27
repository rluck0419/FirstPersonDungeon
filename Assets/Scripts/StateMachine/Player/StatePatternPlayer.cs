using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StatePatternPlayer : MonoBehaviour {

    // player objects / components
    public GameObject mainCamera;
    public Text scoreText;
    public Rigidbody rigidbody;
    public bool collided = true;

    // crouch amount
    //[HideInInspector] public Vector3 crouch = new Vector3(0, 0.75f, 0);

    // camera movement
    public Vector2 clampInDegrees = new Vector2(360, 180);
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);

    // walking & speed-based variables
    public float crouchSpeed = 4.0f;
    public float walkSpeed = 7.0f;
    public float runSpeed = 14.0f;
    float moveSpeed = 7.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;

    [HideInInspector] public bool jumping = false;

    //[HideInInspector] public bool firstJump = false;
    //[HideInInspector] public bool secondJump = false;
    [HideInInspector] public List<Collider> collidedPlats;
    [HideInInspector] public int bounces = 0;
    [HideInInspector] public int score = 0;
    [HideInInspector] public Vector2 targetDirection;
    [HideInInspector] public Vector2 mouseAbsolute;
    [HideInInspector] public Vector2 smoothMouse;
    //[HideInInspector] public Vector3 lookDirection;

    [HideInInspector] public float distToGround;
    //[HideInInspector] public Vector3 moveDirection;

    // target variables
    [HideInInspector] public GameObject targetObject;
    [HideInInspector] public bool targeting = false;
    [HideInInspector] public bool settingTarget = false;
    [HideInInspector] public bool skipFrame = false;
    [HideInInspector] public int currentTargetFrames = 0;
    public int settingTargetDamping = 15;

	// carry & throw variables
	[HideInInspector] public Pickupable pickup;
	[HideInInspector] public float pickupRadius = 5.0f;
	[HideInInspector] public Rigidbody pickupRigidbody;
	[HideInInspector] public GameObject carriedObject;
	[HideInInspector] public GameObject thrownObject;
	[HideInInspector] public Collider[] hitColliders;
	[HideInInspector] public float distance = 5.0f;
	[HideInInspector] public float thrust = 1024.0f;
	[HideInInspector] public float smooth = 10.0f;
	[HideInInspector] public float rotation = 0.5f;
	[HideInInspector] public bool carrying = false;

	[HideInInspector] public IPlayerState currentState;
	[HideInInspector] public PlayerIdleState idleState;
	[HideInInspector] public PlayerWalkState walkState;
	[HideInInspector] public PlayerSneakState sneakState;
	[HideInInspector] public PlayerBounceState bounceState;
	[HideInInspector] public PlayerHookState hookState;
    [HideInInspector] public PlayerTargetState targetState;

	private void Awake ()
    {
        rigidbody = GetComponent<Rigidbody>();
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
		score = mainCamera.GetComponent<Gui> ().score_count;
		idleState = new PlayerIdleState (this);
		walkState = new PlayerWalkState (this);
		bounceState = new PlayerBounceState (this);
		sneakState = new PlayerSneakState (this);
		hookState = new PlayerHookState (this);
        targetState = new PlayerTargetState (this);
		distToGround = GetComponent<Collider> ().bounds.extents.y;
	}

	void Start ()
    {
		currentState = idleState;
		targetDirection = transform.localRotation.eulerAngles;
	}

	void Update ()
    {
		currentState.UpdateState ();
	}

	void OnCollisionEnter (Collision collision)
    {
		if (currentState == bounceState)
        {
			foreach (ContactPoint contact in collision.contacts)
            {
				if (contact.normal == Vector3.up)
                {
					collided = true;
					bounces += 1;
					if (!collidedPlats.Contains (contact.otherCollider))
                    {
						collidedPlats.Add (contact.otherCollider);
						score += 100;
						scoreText.text = "Score: " + score;
						Debug.Log ("Total bounces: " + bounces + ". Unique surface bounces (score): " + (score / 100)  + ". Last bounced on: " + contact.otherCollider.gameObject);
						if (score == 2300)
                        {
							Debug.Log ("You hit all the platforms!");
							if (bounces == 23)
								Debug.Log("You only bounced once on each platform! Holy cow!");
						}
					}
				}
			}
		}
	}

    public void Look(GameObject target = null)
    {
        if (target != null/* && carriedObject != target*/)
        {
            if (settingTarget)
            {
                // get the target points (slightly different rotations for player and camera)
                var targetPlayerRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                var targetCameraRotation = Quaternion.LookRotation(target.transform.position - mainCamera.transform.position);

                // Smoothly rotate the player & camera towards the target point until locked
                // adjust the damping by the player's velocity to ensure rotation reaches lock
                var magnitude = Mathf.Max(rigidbody.velocity.magnitude, 1);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetPlayerRotation, settingTargetDamping * magnitude * Time.deltaTime);
                mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetCameraRotation, settingTargetDamping * magnitude * Time.deltaTime);


                if (mainCamera.transform.rotation == targetCameraRotation)
                {
                    settingTarget = false;
                }
            }
            else
            {
                transform.LookAt(target.transform, Vector3.up);
                mainCamera.transform.LookAt(target.transform, Vector3.up);
            }
            return;
            // to do:
            // should player be able to pickup targets?
            // if so, player should come out of target mode
            // more testing needed
        }

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        mouseAbsolute += smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        var xRotation = Quaternion.AngleAxis(mouseAbsolute.x, targetOrientation * Vector3.up);
        transform.localRotation = xRotation;

        transform.localRotation *= targetOrientation;

        if (clampInDegrees.y < 360)
            mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        var yRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right);
        mainCamera.transform.localRotation = yRotation;

        mainCamera.transform.localRotation *= targetOrientation;
    }

    public GameObject GetTarget(Ray ray)
    {
        GameObject target = null;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var t = hit.collider.GetComponent<Target>();

            if (t != null)
            {
                target = t.gameObject;
            }
        }

        return target;
    }

    public void Move()
    {
        Vector3 velocity = rigidbody.velocity;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.5f))
        {
            if (currentState == sneakState)
            {
                moveSpeed = crouchSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                // create a run state later?
                moveSpeed = runSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }
               

            if (canJump && Input.GetButton("Jump"))
            {
                rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                jumping = true;
            }
            else
            {
                jumping = false;
            }
        }

        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        targetVelocity.Normalize();
        if (targetVelocity != Vector3.zero)
        {
            GetComponent<CapsuleCollider>().material.dynamicFriction = 0.5f;
            targetVelocity = transform.TransformDirection(targetVelocity);

            targetVelocity *= moveSpeed;


            // Apply a force that attempts to reach our target velocity
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            // move "forward" across object - parallel to face of obstacle based on angle
            RaycastHit obstacle;
            if (Physics.Raycast(transform.GetChild(0).transform.position, velocityChange, out obstacle, 1.25f))
                velocityChange = velocityChange - obstacle.normal * Vector3.Dot(velocityChange, obstacle.normal);

            velocityChange = Quaternion.Euler(hit.normal) * velocityChange;

            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 currentVelocity = rigidbody.velocity;
            float yVelocity = currentVelocity.y;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f) && jumping == false && currentState != bounceState)
            {
                yVelocity = 0f;
                //firstJump = false;
                //secondJump = false;
            }
            rigidbody.velocity = new Vector3(currentVelocity.x * 0.2f, yVelocity, currentVelocity.z * 0.2f);
            GetComponent<CapsuleCollider>().material.dynamicFriction = 1f;
        }

        if (hit.normal != Vector3.up)
        {
            maxVelocityChange = 1f;
        }
        else
        {
            maxVelocityChange = 0.5f;
        }

        // We apply gravity manually for more tuning control
        rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0));
    }

    bool Grounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.5f);
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Jump attempt registered");
            //if (firstJump && !secondJump)
            //{
            //    secondJump = true;
            //    rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
            //}

            if (Grounded())
            {
                //firstJump = true;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
            }
        }
    }

    public void CheckInteraction()
    {
        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));

        // within-reach pickup
        if (Input.GetKeyDown(KeyCode.E))
        {
            Pickup(ray);
        }

        // TK pickup
        if (Input.GetMouseButtonDown(0))
        {
            Pull(ray);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Push(ray);
        }
    }

    public void Pickup(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            pickup = hit.collider.GetComponent<Pickupable>();

            if (pickup != null)
            {
                hitColliders = Physics.OverlapSphere(mainCamera.transform.position, pickupRadius);
                foreach (Collider i in hitColliders)
                {
                    if (i.gameObject == pickup.gameObject)
                    {
                        carrying = true;
                        carriedObject = pickup.gameObject;
                        pickupRigidbody = carriedObject.GetComponent<Rigidbody>();
                        pickupRigidbody.velocity = Vector3.zero;
                        pickupRigidbody.useGravity = false;
                    }
                }
            }
        }
    }

    public void Pull(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            pickup = hit.collider.GetComponent<Pickupable>();
            //				l = hit.collider.GetComponent<Latchable> ();

            if (pickup != null)
            {
                carrying = true;
                carriedObject = pickup.gameObject;
                pickupRigidbody = carriedObject.GetComponent<Rigidbody>();
                pickupRigidbody.velocity = Vector3.zero;
                pickupRigidbody.useGravity = false;
            }

            //				if (l != null) {
            //					hooked = true;
            //					gravity = 0f;
            //				}
        }
    }

    public void Push(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            pickup = hit.collider.GetComponent<Pickupable>();
            if (pickup != null)
            {
                pickup.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * thrust);
            }
        }
    }

    public void Carry(GameObject o)
    {
        if (carrying == true && carriedObject != null)
        {
            o.transform.position = Vector3.Lerp(
                o.transform.position,
                mainCamera.transform.position + (mainCamera.transform.forward * distance * 2f),
                Time.deltaTime * smooth
            );
        }
    }

    public void Platform(GameObject o)
    {
        if (carrying == true && o != null)
        {
            o.transform.rotation = Quaternion.Euler(o.transform.rotation.eulerAngles.x, 0, 0);
            o.transform.position = Vector3.Lerp(
                o.transform.position,
                new Vector3(
                    mainCamera.transform.position.x + mainCamera.transform.forward.x * distance * 2f,
                    o.transform.position.y,
                    mainCamera.transform.position.z + mainCamera.transform.forward.z * distance * 2f
                ),
                Time.deltaTime * smooth
            );
        }
    }

    // Check if item should be dropped
    public bool CheckDrop()
    {
        if (carriedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropObject();
                return true;
            }
        }
        return false;
    }

    // Drop carried object
    public void DropObject()
    {
        carrying = false;
        pickupRigidbody = carriedObject.GetComponent<Rigidbody>();
        pickupRigidbody.velocity = Vector3.zero;
        pickupRigidbody.useGravity = true;
        carriedObject = null;
    }

    // check if item should be thrown
    public bool CheckThrow()
    {
        if (carrying == true && Input.GetMouseButtonDown(0))
        {
            carriedObject.GetComponent<Rigidbody>().isKinematic = false;
            ThrowObject();
            return true;
        }
        return false;
    }

    // Throw carried object
    public void ThrowObject()
    {
        carrying = false;
        thrownObject = carriedObject;
        carriedObject = null;

        thrownObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        thrownObject.GetComponent<Rigidbody>().useGravity = true;
        thrownObject.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * thrust);

        thrownObject = null;
    }
}
