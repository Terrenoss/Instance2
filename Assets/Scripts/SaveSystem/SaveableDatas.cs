using System.Collections.Generic;

public class SaveableDatas
{
    private string _fileName;

    private Dictionary<string, int> _savedInts = new();
    private Dictionary<string, float> _savedFloats = new();
    private Dictionary<string, string> _savedStrings = new();
    private Dictionary<string, bool> _savedBools = new();

    private Dictionary<string, int[]> _savedIntArrays = new();
    private Dictionary<string, float[]> _savedFloatArrays = new();
    private Dictionary<string, string[]> _savedStringArrays = new();
    private Dictionary<string, bool[]> _savedBoolArrays = new();

    public SaveableDatas(string fileName)
    {
        _fileName = fileName;
    }

    public void SaveInt(string name, int value)
    {
        if (_savedInts.ContainsKey(name))
        {
            _savedInts[name] = value;
            return;
        }
        _savedInts.Add(name, value);
    }

    public int GetSavedInt(string name) {  return _savedInts[name]; }

    public void SaveFloat(string name, float value)
    {
        if (_savedFloats.ContainsKey(name))
        {
            _savedFloats[name] = value;
            return;
        }
        _savedFloats.Add(name, value);
    }

    public float GetSavedFloat(string name) { return _savedFloats[name]; }

    public void SaveString(string name, string value)
    {
        if (_savedStrings.ContainsKey(name)) 
        { 
            _savedStrings[name] = value; 
            return; 
        }
        _savedStrings.Add(name, value);
    }

    public string GetSavedString(string name) { return _savedStrings[name]; }

    public void SaveBool(string name, bool value)
    {
        if (_savedBools.ContainsKey(name))
        {
            _savedBools[name] = value;
            return;
        }
        _savedBools.Add(name, value);
    }

    public bool GetSavedBool(string name) { return _savedBools[name]; }

    public void SaveIntArray(string name, int[] value)
    {
        if (_savedIntArrays.ContainsKey(name))
        {
            _savedIntArrays[name] = value;
            return;
        }
        _savedIntArrays.Add(name, value);
    }

    public int[] GetSavedIntArray(string name) { return _savedIntArrays[name]; }

    public void SaveFloatArray(string name, float[] value)
    {
        if (_savedFloatArrays.ContainsKey(name))
        {
            _savedFloatArrays[name] = value;
            return;
        }
        _savedFloatArrays.Add(name, value);
    }

    public float[] GetSavedFloatArray(string name) { return _savedFloatArrays[name]; }

    public void SaveStringArray(string name, string[] value)
    {
        if (_savedStringArrays.ContainsKey(name))
        {
            _savedStringArrays[name] = value;
            return;
        }
        _savedStringArrays.Add(name, value);
    }

    public string[] GetSavedStringArray(string name) { return _savedStringArrays[name]; }

    public void SaveBoolArray(string name, bool[] value)
    {
        if (_savedBoolArrays.ContainsKey(name))
        {
            _savedBoolArrays[name] = value;
            return;
        }
        _savedBoolArrays.Add(name, value);
    }

    public bool[] GetSavedBoolArray(string name) { return _savedBoolArrays[name]; }

    public string GetFileName()
    {
        return _fileName;
    }
}
