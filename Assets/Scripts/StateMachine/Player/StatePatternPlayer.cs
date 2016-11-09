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
	[HideInInspector] public Vector3 crouch = new Vector3  (0, 0.75f, 0);

	// camera movement
	public Vector2 clampInDegrees = new Vector2(360, 180);
	public Vector2 sensitivity = new Vector2(2, 2);
	public Vector2 smoothing = new Vector2(3, 3);

	// walking & speed-based variables
	public float moveSpeed = 8.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 3.0f;

	[HideInInspector] public List<Collider> collidedPlats;
	[HideInInspector] public int bounces = 0;
	[HideInInspector] public int score = 0;
	[HideInInspector] public Vector2 targetDirection;
	[HideInInspector] public Vector2 mouseAbsolute;
	[HideInInspector] public Vector2 smoothMouse;
	[HideInInspector] public Vector3 lookDirection;

	[HideInInspector] public float distToGround;
	[HideInInspector] public Vector3 moveDirection;

	// carry & throw variables
	[HideInInspector] public Pickupable pickup;
	[HideInInspector] public float pickupRadius = 4.0f;
	[HideInInspector] public Rigidbody pickupRigidbody;
	[HideInInspector] public GameObject carriedObject;
	[HideInInspector] public GameObject thrownObject;
	[HideInInspector] public Collider[] hitColliders;
	[HideInInspector] public float distance = 3.0f;
	[HideInInspector] public float thrust = 1024.0f;
	[HideInInspector] public float smooth = 7.0f;


	[HideInInspector] public IPlayerState currentState;
	[HideInInspector] public PlayerIdleState idleState;
	[HideInInspector] public PlayerWalkState walkState;
	[HideInInspector] public PlayerBounceState bounceState;
	[HideInInspector] public PlayerHookState hookState;

	private void Awake () {
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
		score = mainCamera.GetComponent<Gui> ().score_count;
		idleState = new PlayerIdleState (this);
		walkState = new PlayerWalkState (this);
		bounceState = new PlayerBounceState (this);
		hookState = new PlayerHookState (this);
		distToGround = GetComponent<Collider> ().bounds.extents.y;
	}

	void Start () {
		currentState = idleState;
		targetDirection = transform.localRotation.eulerAngles;
	}

	void Update () {
		currentState.UpdateState ();
	}

	void OnCollisionEnter (Collision collision) {
		if (currentState == bounceState) {
			foreach (ContactPoint contact in collision.contacts) {
				if (contact.normal == Vector3.up) {
					collided = true;
					bounces += 1;
					if (!collidedPlats.Contains (contact.otherCollider)) {
						collidedPlats.Add (contact.otherCollider);
						score += 100;
						scoreText.text = "Score: " + score;
						Debug.Log ("Total bounces: " + bounces + ". Unique surface bounces (score): " + (score / 100)  + ". Last bounced on: " + contact.otherCollider.gameObject);
						if (score == 2300) {
							Debug.Log ("You hit all the platforms!");
							if (bounces == 23)
								Debug.Log("You only bounced once on each platform! Holy cow!");
						}
					}
				}
			}
		}
	}
}
