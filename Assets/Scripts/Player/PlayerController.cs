using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public UnityEvent<Vector2> PlayerMovementEvent;
    public UnityEvent<string> WaveParryEvent;

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
            WaveParryEvent.Invoke(ctx.control.displayName);
        }
    }
}
