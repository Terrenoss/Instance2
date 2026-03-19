using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class RebindSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private string _actionName;
    [SerializeField] private List<Button> _buttonList;
    [SerializeField] private TextMeshProUGUI _bindingDisplay;
    [Header("Tick this if the binding is part of a composite.")]
    [SerializeField] private bool _isPartOfComposite = false;
    [SerializeField] private int _compositeIndex = 0;
    private InputAction _action;
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
    private SaveableDatas _savedDatas;
    private Dictionary<string, string> _specialCharacters = new Dictionary<string, string>();

    private void Awake()
    {
        _specialCharacters.Add("UPARROW", "\u2191");
        _specialCharacters.Add("DOWNARROW", "\u2193");
        _specialCharacters.Add("LEFTARROW", "\u2190");
        _specialCharacters.Add("RIGHTARROW", "\u2192");
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
        UpdateBindingDisplay();
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
        UpdateBindingDisplay();
        foreach (Button button in _buttonList)
        {
            button.enabled = true;
        }
        string rebinds = _inputActions.SaveBindingOverridesAsJson();
        _savedDatas.SaveString("rebinds", rebinds);
        SaveSystem.SaveData(_savedDatas);
    }

    private void UpdateBindingDisplay()
    {
        string bindingText = _action.bindings[_compositeIndex].effectivePath;
        for (int i = 0; i < bindingText.Length; i++)
        {
            if (bindingText[i] == '/')
            {
                bindingText = bindingText.Remove(0, i + 1).ToUpper();
                break;
            }
        }
        if (_specialCharacters.ContainsKey(bindingText))
        {
            bindingText = _specialCharacters[bindingText];
        }
        _bindingDisplay.text = bindingText;
    }
}