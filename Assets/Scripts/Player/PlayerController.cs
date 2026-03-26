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
    [Tooltip("Temps de recharge entre deux déplacements (Cooldown)")]
    [SerializeField] private float movementCooldown = 0.1f;
    
    private int currentDirection = 0;
    private float nextMoveTime = 0f;

    public void CallPlayerMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            try
            {
                if (Time.time < nextMoveTime) return;

                Vector2 mvmntInput = ctx.ReadValue<Vector2>();

                int previousDirection = currentDirection;

                if (mvmntInput.x > 0) currentDirection = Mathf.Min(currentDirection + 1, 1);
                else if (mvmntInput.x < 0) currentDirection = Mathf.Max(currentDirection - 1, -1);
                else return; 

                int delta = currentDirection - previousDirection;
                
                if (delta == 0) return; 

                nextMoveTime = Time.time + movementCooldown;

                if (animator != null)
                {
                    animator.SetInteger("Direction", currentDirection);
                }
                
                PlayerMovementEvent.Invoke(new Vector2(delta, mvmntInput.y));
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
