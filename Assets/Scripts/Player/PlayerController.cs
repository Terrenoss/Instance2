using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public UnityEvent<Vector2> PlayerMovementEvent;
    public UnityEvent<Guid> WaveParryEvent;
    public UnityEvent PauseEvent;

    public void CallPlayerMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            try
            {
                Vector2 mvmntInput = ctx.ReadValue<Vector2>();
                PlayerMovementEvent.Invoke(mvmntInput);
            }
            catch (InvalidOperationException)
            {
                Debug.LogError("Erreur : La fonction de mouvement a essayé de lire un bouton ! Vérifiez vos événements (Events) dans le Player Input (Inspecteur Unity) : vous avez très probablement assigné CallPlayerMovement à l'action Pause.");
            }
        }
    }

    public void CallCurentWaveParry(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
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
