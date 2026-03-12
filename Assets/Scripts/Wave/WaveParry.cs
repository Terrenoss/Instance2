using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.iOS;

public class WaveParry : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(2f, 2f, 2f);
    
    //mauvaise touche = perd une vie
    public void Parry(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        Collider[] hits = Physics.OverlapBox(transform.position, boxSize / 2, Quaternion.identity);

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out WaveType waveType)) return;
            Debug.Log("wave type: " + waveType);
            InputControl control = context.control;
            if (control is KeyControl keyControl)
            {
                Key keyPressed = keyControl.keyCode;

                if (keyPressed == Key.Space /*key de la wave*/ )
                {
                    //score ++
                }
                else
                {
                    //perd vie
                    return;
                }
            }
            hit.gameObject.SetActive(false);
            //pulling system
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}