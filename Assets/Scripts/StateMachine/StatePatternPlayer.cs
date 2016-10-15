using UnityEngine;
using System.Collections;

public class StatePatternPlayer : MonoBehaviour {

	public GameObject mainCamera;
	public float rotateSpeed = 8.0f;
	public float moveSpeed = 8.0f;
	public float jumpSpeed = 4.0f;
	public float gravity = 3.0f;

	[HideInInspector] public Vector3 lookDirection;
	[HideInInspector] public Vector3 moveDirection;
	[HideInInspector] public IPlayerState currentState;
	[HideInInspector] public IdleState idleState;
	[HideInInspector] public WalkState walkState;
	[HideInInspector] public RunState runState;

	private void Awake () {
		idleState = new IdleState (this);
		walkState = new WalkState (this);
		runState = new RunState (this);
	}

	void Start () {
		currentState = idleState;
	}

	void Update () {
		currentState.UpdateState ();
	}
}
