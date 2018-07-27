using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetState : IPlayerState {

    private readonly StatePatternPlayer player;

    public PlayerTargetState(StatePatternPlayer statePatternPlayer)
    {
        player = statePatternPlayer;
    }

    public void UpdateState()
    {
        Transition();
        player.Look(player.targetObject);
        player.Move();
        if (player.carrying)
        {
            if (player.CheckThrow())
                return;
            if (player.CheckDrop())
                return;

            if (player.carriedObject.layer == 11)
            {
                player.Platform(player.carriedObject);
            }
            else
            {
                player.Carry(player.carriedObject);
            }
        }
        else
        {
            player.CheckInteraction();
        }
    }

    public void ToPlayerIdleState()
    {
        Debug.Log("player is now in idle state");
        player.currentState = player.idleState;
    }

    public void ToPlayerWalkState()
    {
        Debug.Log("player is now in walk state");
        player.currentState = player.walkState;
        player.targetObject = null;
    }

    public void ToPlayerHookState()
    {
        Debug.Log("player is now in hook state");
        player.currentState = player.hookState;
    }

    public void ToPlayerBounceState()
    {
        Debug.Log("player is now in bounce state");
        player.collided = false;
        player.GetComponent<CapsuleCollider>().material.bounciness = 1f;
        player.currentState = player.bounceState;
    }

    public void ToPlayerSneakState()
    {
        Debug.Log("player is now in sneak state");
        player.transform.localScale -= (Vector3.up * 0.5f);
        player.distToGround -= 0.5f;
        player.currentState = player.sneakState;
    }

    public void ToPlayerTargetState()
    {
        Debug.Log("Whoops... You can't go from one state to the same state (walk)");
    }

    private void Transition()
    {
        //if (Input.GetKeyDown(KeyCode.B))
        //    ToPlayerBounceState();
        //if (Input.GetKeyDown(KeyCode.C))
        //    ToPlayerSneakState();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int x = Screen.width / 2;
            int y = Screen.height / 2;

            Ray ray = player.mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));

            var newTarget = player.GetTarget(ray);

            if (newTarget == null || newTarget == player.targetObject)
            {
                player.skipFrame = true;
                player.settingTarget = false;
                player.targeting = false;
                ToPlayerWalkState();
            }
            else
            {
                player.targetObject = newTarget;
                player.settingTarget = true;
            }
        }
        //if (player.rigidbody.velocity == Vector3.zero)
        //    ToPlayerIdleState();
    }
}
