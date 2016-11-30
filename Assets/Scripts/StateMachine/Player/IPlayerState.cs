using UnityEngine;
using System.Collections;

public interface IPlayerState {

	void UpdateState ();

	void ToPlayerIdleState ();

	void ToPlayerWalkState ();

	void ToPlayerBounceState ();

	void ToPlayerHookState ();

	void ToPlayerSneakState ();
}
