using UnityEngine;
using System.Collections;

public class WalkState : IPlayerState {

	private readonly StatePatternPlayer player;

	public WalkState (StatePatternPlayer statePatternPlayer) {
		player = statePatternPlayer;
	}

	public void UpdateState () {
		Look ();
		Move ();
	}

	public void ToIdleState () {
		
	}

	public void ToWalkState () {

	}

	public void ToRunState () {

	}

	private void Look () {
		
	}

	private void Move () {

	}
}
