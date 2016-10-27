using UnityEngine;
using System.Collections;

public class PlayerHookState : IPlayerState {

	private readonly StatePatternPlayer player;

	public PlayerHookState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {

	}

	public void ToPlayerIdleState () {
		Debug.Log ("player is now in idle state");
		player.currentState = player.idleState;
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
		Debug.Log ("Whoops... You can't go from one state to the same state (hook)");
	}
}
