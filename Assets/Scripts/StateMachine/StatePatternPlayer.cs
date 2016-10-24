using UnityEngine;
using System.Collections;

public class StatePatternPlayer : MonoBehaviour {

	// player objects / components
	public GameObject mainCamera;
	public Rigidbody rigidbody;

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

	[HideInInspector] public Vector2 targetDirection;
	[HideInInspector] public Vector2 mouseAbsolute;
	[HideInInspector] public Vector2 smoothMouse;
	[HideInInspector] public Vector3 lookDirection;

	[HideInInspector] public float distToGround;
	[HideInInspector] public Vector3 moveDirection;

	[HideInInspector] public IPlayerState currentState;
	[HideInInspector] public IdleState idleState;
	[HideInInspector] public WalkState walkState;
	[HideInInspector] public HookState hookState;
	[HideInInspector] public SneakState sneakState;
	[HideInInspector] public BounceState bounceState;

	private void Awake () {
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
		idleState = new IdleState (this);
		walkState = new WalkState (this);
		hookState = new HookState (this);
		sneakState = new SneakState (this);
		bounceState = new BounceState (this);
		distToGround = GetComponent<Collider> ().bounds.extents.y;
	}

	void Start () {
		currentState = idleState;
		targetDirection = transform.localRotation.eulerAngles;
	}

	void Update () {
		currentState.UpdateState ();
	}
}
