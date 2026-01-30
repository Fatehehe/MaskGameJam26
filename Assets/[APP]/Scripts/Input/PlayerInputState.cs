using Modules.StatePatterns;
using UnityEngine;

public class PlayerInputState : IState
{
    protected PlayerInput input;

    public PlayerInputState() { }

    public override void OnEnter()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log($"Entering state:{GetType().Name}");
        }
    }

    public override void OnExit()
    {

    }

    public override void Tick()
    {

    }
}