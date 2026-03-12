using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerController : MonoBehaviour
{
    public UnityEvent<Vector2> PlayerMovementEvent;
    public UnityEvent<Guid> WaveParryEvent;
    public UnityEvent PauseEvent;

    public void CallPlayerMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Vector2 mvmntInput = ctx.ReadValue<Vector2>();
            PlayerMovementEvent.Invoke(mvmntInput);
        }
    }

    public void CallCurentWaveParry(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Guid parryID;
            int index = 0;
            
            foreach (InputControl inputControl in ctx.action.controls)
            {
                if (inputControl.displayName == ctx.control.displayName)
                {
                    break;
                }
                index++;
            }
            WaveParryEvent.Invoke(ctx.action.bindings[index].id);
        }
    }

    public void CallPauseEvent(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            PauseEvent.Invoke();
        }
    }
}
