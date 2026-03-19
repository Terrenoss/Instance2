using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class RebindSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private string _actionName;
    [SerializeField] private List<Button> _buttonList;
    [Header("Tick this if the binding is part of a composite.")]
    [SerializeField] private bool _isPartOfComposite = false;
    [SerializeField] private int _compositeIndex = 0;
    private InputAction _action;
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
    private SaveableDatas _savedDatas;

    private void Awake()
    {
        if (SaveSystem.LoadDatas("rebinds") != null)
        {
            _savedDatas = SaveSystem.LoadDatas("rebinds");
            string rebinds = _savedDatas.GetSavedString("rebinds");
            _inputActions.LoadBindingOverridesFromJson(rebinds);
        }
        else
        {
            _savedDatas = new SaveableDatas("rebinds");
        }
    }

    private void Start()
    {
        _action = _inputActions.FindAction(_actionName);
    }

    public void Rebind()
    {
        _inputActions.FindActionMap("Player").Disable();
        foreach (Button button in _buttonList)
        {
            button.enabled = false;
        }
        _rebindingOperation = _action.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted());
        if (_isPartOfComposite)
        {
            _rebindingOperation.WithTargetBinding(_compositeIndex);
        }
        _rebindingOperation.Start();
    }

    private void RebindCompleted()
    {
        _rebindingOperation.Dispose();
        _inputActions.FindActionMap("Player").Enable();
        foreach (Button button in _buttonList)
        {
            button.enabled = true;
        }
        string rebinds = _inputActions.SaveBindingOverridesAsJson();
        _savedDatas.SaveString("rebinds", rebinds);
        SaveSystem.SaveData(_savedDatas);
    }
}