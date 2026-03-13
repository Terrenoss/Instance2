using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public enum WaveTypeEnum
{
    wave1,
    wave2,
    wave3,
    wave4,
}

public class WaveData
{
    public Color color;
    public Key key;
}

public class WaveType : MonoBehaviour
{    
    public WaveTypeEnum waveType;
    public InputAction inputAction;
    private Renderer rend;
    

    private Dictionary<WaveTypeEnum, WaveData> waveColor = new()
    {
        //rajouter sons associés à chaque ondes
        { WaveTypeEnum.wave1, new WaveData() {color = Color.red, key = new Key()} },
        { WaveTypeEnum.wave2, new WaveData() {color = Color.green, key = new Key()} },
        { WaveTypeEnum.wave3, new WaveData() {color = Color.blue, key = new Key()} },
        { WaveTypeEnum.wave4, new WaveData() {color = Color.yellow, key = new Key()} }
        
    };
    
    void OnValidate()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }

        rend.sharedMaterial.color = waveColor[waveType].color;
    }
}
