using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public UnityEvent<Vector2> PlayerMovementEvent;
    public UnityEvent<Guid> WaveParryEvent;
    public UnityEvent PauseEvent;
    [SerializeField] private Animator animator;
    private int currentDirection = 0;
    public void CallPlayerMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            try
            {
                Vector2 mvmntInput = ctx.ReadValue<Vector2>();

                if (mvmntInput.x > 0) currentDirection = Mathf.Min(currentDirection + 1, 1);
                else if (mvmntInput.x < 0) currentDirection = Mathf.Max(currentDirection - 1, -1);
                else return; 

                animator.SetInteger("Direction", currentDirection);
                PlayerMovementEvent.Invoke(new Vector2(currentDirection, mvmntInput.y));
                Debug.Log(currentDirection);
            }
            catch (InvalidOperationException)
            {
                Debug.LogError("Erreur : La fonction de mouvement a essayé de lire un bouton !");
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
