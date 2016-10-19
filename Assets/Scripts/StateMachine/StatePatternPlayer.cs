using UnityEngine;
using System.Collections;

public class StatePatternPlayer : MonoBehaviour {

	public GameObject mainCamera;

	public float moveSpeed = 2.0f;

	public Vector2 clampInDegrees = new Vector2(360, 180);
	public Vector2 sensitivity = new Vector2(2, 2);
	public Vector2 smoothing = new Vector2(3, 3);
	[HideInInspector] public Vector2 targetDirection;
	[HideInInspector] public Vector2 mouseAbsolute;
	[HideInInspector] public Vector2 smoothMouse;

	[HideInInspector] public Vector3 lookDirection;
	[HideInInspector] public Vector3 moveDirection;
	[HideInInspector] public IPlayerState currentState;
	[HideInInspector] public IdleState idleState;
	[HideInInspector] public WalkState walkState;

	private void Awake () {
		idleState = new IdleState (this);
		walkState = new WalkState (this);
	}

	void Start () {
		currentState = idleState;
		targetDirection = transform.localRotation.eulerAngles;
	}

	void Update () {
		currentState.UpdateState ();
	}
}
