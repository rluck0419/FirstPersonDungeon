using UnityEngine;
using System.Collections;

public class GamePauseState : IGameState {

	private readonly StatePatternGame game;

	public GamePauseState (StatePatternGame statePatternGame) {
		game = statePatternGame;
	}

	public void UpdateState () {

	}

	public void ToGamePlayState () {
		Debug.Log ("game is now in play state");
		game.currentState = game.playState;
	}

	public void ToGamePauseState () {
		Debug.Log ("Whoops... You can't go from one state to the same state (game - play)");
	}
}
