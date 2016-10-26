using UnityEngine;
using System.Collections;

public interface IPlayerState {

	void UpdateState ();

	void ToIdleState ();

	void ToWalkState ();

	void ToBounceState ();

	void ToHookState ();
}
