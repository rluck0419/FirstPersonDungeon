using UnityEngine;
using System.Collections;

public class GamePlayState : IGameState {

	private readonly StatePatternGame game;

	public GamePlayState (StatePatternGame statePatternGame) {
		game = statePatternGame;
	}

	public void UpdateState () {

	}

	public void ToGamePlayState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (game - play)");
	}

	public void ToGamePauseState () {
		Debug.Log ("game is now in pause state");
		game.currentState = game.pauseState;
	}
}
