using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

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
    public Guid keyId;
}

public class WaveType : MonoBehaviour
{    
    public WaveTypeEnum waveTypeEnum;
    [SerializeField] private PlayerInput playerInput;
    private List<Guid> bindingList = new();
    private Renderer rend;
    

    public Dictionary<WaveTypeEnum, WaveData> waveParam = new()
    {
        //rajouter sons associés à chaque ondes
        { WaveTypeEnum.wave1, new WaveData() {color = Color.red} },
        { WaveTypeEnum.wave2, new WaveData() {color = Color.green} },
        { WaveTypeEnum.wave3, new WaveData() {color = Color.blue} },
        { WaveTypeEnum.wave4, new WaveData() {color = Color.yellow} }
        
    };

    private void Start()
    {
        InputAction action = playerInput.actions["Parry"];
        ReadOnlyArray<InputBinding> bindings = action.bindings;

        foreach (InputBinding binding in bindings)
        {
            Guid key = binding.id;
            bindingList.Add(key);
            Debug.Log(key);
        }
        waveParam[WaveTypeEnum.wave1].keyId = bindingList[0];
        waveParam[WaveTypeEnum.wave2].keyId = bindingList[1];
        waveParam[WaveTypeEnum.wave3].keyId = bindingList[2];
        waveParam[WaveTypeEnum.wave4].keyId = bindingList[3];
    }

    void OnValidate()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }

        rend.sharedMaterial.color = waveParam[waveTypeEnum].color;
    }
}
