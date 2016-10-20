using UnityEngine;
using System.Collections;

public class HookState : IPlayerState {

	private readonly StatePatternPlayer player;

	public HookState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {

	}

	public void ToIdleState () {
		Debug.Log ("player is now in idle state");
		player.currentState = player.idleState;
	}

	public void ToWalkState () {
		Debug.Log ("player is now in walk state");
		player.currentState = player.walkState;
	}

	public void ToHookState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (hook)");
	}
}
