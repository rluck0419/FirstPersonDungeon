using UnityEngine;
using System.Collections;

public class StatePatternGame : MonoBehaviour {
	[HideInInspector] public IGameState currentState;
	[HideInInspector] public GamePlayState playState;
	[HideInInspector] public GamePauseState pauseState;

	private void Awake () {
		playState = new GamePlayState (this);
		pauseState = new GamePauseState (this);
	}

	void Start () {
		currentState = playState;
	}

	void Update () {
		currentState.UpdateState ();
	}
}
