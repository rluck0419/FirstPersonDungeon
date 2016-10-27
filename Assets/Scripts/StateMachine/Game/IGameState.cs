using UnityEngine;
using System.Collections;

public interface IGameState {

	void UpdateState ();

	void ToGamePlayState ();

	void ToGamePauseState ();
}