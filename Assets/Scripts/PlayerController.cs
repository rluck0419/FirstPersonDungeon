using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private GameObject playerObject;
	private CharacterController controller;
	private GameObject mainCamera;
	private Vector3 currentPosition;
	private GameObject[] latchables;

	private bool hooked = false;
	private bool crouching = false;
	private bool sprinting = false;
	private bool carrying = false;
	private bool dJumped = false;
	private float mouseX = 0.0f;
	private float mouseY = 0.0f;
	private Vector3 rotateDirection = Vector3.zero;
	private Vector3 moveDirection = Vector3.zero;

	private GameObject carriedObject;
	private GameObject thrownObject;


	public float pickupRadius = 1.0f;
	private Pickupable p;
	private Latchable l;
	private Rigidbody r;
	private Collider[] hitColliders;
	public float distance = 1.0f;


	public float standingHeight = 0.75f;
	public float crouchHeight = 0.1f;
	public float cameraCrouch = 1.0f;

	private float currentSpeed;
	public float speed = 4.0f;
	public float sprintSpeed = 6.0f;
	public float crouchSpeed = 1.0f;
	public float climbSpeed = 16.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 17.0f;
	public float rotateSpeed = 8.0f;
	public float thrust = 512.0f;
	public float smooth = 7.0f;
	public float rotation = 2.0f;


	// Use this for initialization
	void Start () {
		playerObject = GameObject.Find("Player");
		controller = GetComponent<CharacterController>();
		mainCamera = GameObject.FindWithTag("MainCamera");
		currentSpeed = speed;
		currentPosition = transform.position;
		latchables = GameObject.FindGameObjectsWithTag ("Latchable");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
			if (crouching == true) {
				crouching = !crouching;
				mainCamera.transform.position = mainCamera.transform.position + (mainCamera.transform.up * cameraCrouch);
				currentSpeed = speed;
			}
			crouching = false;
			sprinting = true;
		}

		if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
			sprinting = false;
		}

		// Crouching
		if (Input.GetKeyDown(KeyCode.C)) {
			crouching = !crouching;
			if (crouching == true) {				
				controller.height = crouchHeight;
				mainCamera.transform.position = mainCamera.transform.position - (mainCamera.transform.up * cameraCrouch);
				currentSpeed = crouchSpeed;
			} else {
				controller.height = standingHeight;
				mainCamera.transform.position = mainCamera.transform.position + (mainCamera.transform.up * cameraCrouch);
				currentSpeed = speed;
			}
		}

		if (carrying == true) {
			if (sprinting == true) {
				crouching = false;
				currentSpeed = sprintSpeed;
			} else if (crouching == true) {
				currentSpeed = crouchSpeed;
			} else {
				currentSpeed = speed;
			}
			if (carriedObject != null) {
				Carry(carriedObject);
				CheckThrow();
				CheckDrop();
			} else if (carriedObject != null) {
				// precautionary measure - possibly unnecessary ("if (carriedObject) != null)" above, "carrying = false" below)
				carrying = false;
			}
		} else if (carrying == false) {
			if (sprinting == true) {
				crouching = false;				
				currentSpeed = sprintSpeed;
			} else if (crouching == true) {
				currentSpeed = crouchSpeed;
			} else {
				currentSpeed = speed;
			}
			Pickup();
		} 

		// Walking / Looking around
		if (controller.isGrounded == true) {
			moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0f, Input.GetAxis ("Vertical"));
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= currentSpeed;
			if (Input.GetButtonDown ("Jump")) {
				moveDirection.y = jumpSpeed;
				dJumped = false;
			}
		} else if (Input.GetButtonDown ("Jump") && !dJumped) {
			moveDirection.y = jumpSpeed;
			dJumped = true;
		}
		// else { } >> Once properly implemented, a final condition should allow for movement in the air

		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);

		mouseX += rotateSpeed * Input.GetAxis("Mouse X");
		mouseY += rotateSpeed * Input.GetAxis("Mouse Y");

		mouseY = Mathf.Clamp(mouseY, -90f, 90f);

		while (mouseX < 0f) {
			mouseX += 360f;
		}

		while (mouseX >= 360f) {
			mouseX -= 360f;
		}

		rotateDirection = new Vector3(-mouseY, mouseX, 0f);

		controller.transform.eulerAngles = rotateDirection;

		if (hooked == true) {
			currentPosition = playerObject.transform.position;
			if (l != null) {
				playerObject.transform.position = Vector3.Lerp (currentPosition, l.gameObject.transform.position, 0.05f);
				foreach (GameObject latchable in latchables) {
					hitColliders = Physics.OverlapSphere (latchable.transform.position, 1.0f);
					foreach (Collider hitCollider in hitColliders) {
						if (hitCollider.gameObject == playerObject) {
							hooked = false;
							l = null;
							gravity = 17.0f;
						}
					}
				}
			}
		}
	}

	public void Pickup () {
		int x = Screen.width / 2;
		int y = Screen.height / 2;
		
		Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x,y));
		RaycastHit hit;

		if (Input.GetMouseButtonDown(0)) {
			if (Physics.Raycast(ray, out hit)) {
				p = hit.collider.GetComponent<Pickupable>();
				l = hit.collider.GetComponent<Latchable> ();

				if (p != null) {
					carrying = true;
					carriedObject = p.gameObject;
					r = carriedObject.GetComponent<Rigidbody>();
					r.useGravity = false;
				}

				if (l != null) {
					hooked = true;
					gravity = 0f;
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			if (Physics.Raycast(ray, out hit)) {
				p = hit.collider.GetComponent<Pickupable>();
				
				if (p != null) {
					hitColliders = Physics.OverlapSphere(mainCamera.transform.position, pickupRadius);
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
					p.gameObject.GetComponent<Rigidbody> ().AddForce (transform.forward * thrust);
				}
			}
		}
	}

	public void Carry (GameObject o) {
		if (carrying==true && carriedObject!=null) {
			o.transform.position = Vector3.Lerp (
				o.transform.position,
				mainCamera.transform.position + (mainCamera.transform.forward * distance),
				Time.deltaTime * smooth
				);
			o.transform.Rotate(Vector3.right * rotation);
		}
	}

	// Check if item should be dropped after pickup
	public void CheckDrop () {
		if (carriedObject != null) {
			if(Input.GetKeyDown (KeyCode.E)) {
				DropObject();
			}
		}
	}

	// Drop objects that have been picked up
	public void DropObject () {
		carrying = false;
		carriedObject.GetComponent<Rigidbody>().useGravity = true;
		carriedObject = null;
	}

	public void CheckThrow () {
		if(carrying == true && Input.GetMouseButtonDown(0)) {
			carriedObject.GetComponent<Rigidbody>().isKinematic = false;
			ThrowObject();
		}
	}

	public void ThrowObject () {
		carrying = false;
		thrownObject = carriedObject;
		carriedObject = null;

		thrownObject.GetComponent<Rigidbody>().useGravity = true;
		thrownObject.GetComponent<Rigidbody>().AddForce(transform.forward * thrust);

		thrownObject = null;
	}	
}
