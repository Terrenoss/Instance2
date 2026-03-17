using System.Collections.Generic;

public class SaveableDatas
{
    private string _fileName;

    private Dictionary<string, int> _savedInts;
    private Dictionary<string, float> _savedFloats;
    private Dictionary<string, string> _savedStrings;
    private Dictionary<string, bool> _savedBools;

    private Dictionary<string, int[]> _savedIntArrays;
    private Dictionary<string, float[]> _savedFloatArrays;
    private Dictionary<string, string[]> _savedStringArrays;
    private Dictionary<string, bool[]> _savedBoolArrays;

    public SaveableDatas(string fileName)
    {
        _fileName = fileName;
    }

    public void SaveInt(string name, int value)
    {
        _savedInts.Add(name, value);
    }

    public int GetSavedInt(string name) {  return _savedInts[name]; }

    public void SaveFloat(string name, float value)
    {
        _savedFloats.Add(name, value);
    }

    public float GetSavedFloat(string name) { return _savedFloats[name]; }

    public void SaveString(string name, string value)
    {
        _savedStrings.Add(name, value);
    }

    public string GetSavedString(string name) { return _savedStrings[name]; }

    public void SaveBool(string name, bool value)
    {
        _savedBools.Add(name, value);
    }

    public bool GetSavedBool(string name) { return _savedBools[name]; }

    public void SaveIntArray(string name, int[] value)
    {
        _savedIntArrays.Add(name, value);
    }

    public int[] GetSavedIntArray(string name) { return _savedIntArrays[name]; }

    public void SaveFloatArray(string name, float[] value)
    {
        _savedFloatArrays.Add(name, value);
    }

    public float[] GetSavedFloatArray(string name) { return _savedFloatArrays[name]; }

    public void SaveStringArray(string name, string[] value)
    {
        _savedStringArrays.Add(name, value);
    }

    public string[] GetSavedStringArray(string name) { return _savedStringArrays[name]; }

    public void SaveBoolArray(string name, bool[] value)
    {
        _savedBoolArrays.Add(name, value);
    }

    public bool[] GetSavedBoolArray(string name) { return _savedBoolArrays[name]; }

    public string GetFileName()
    {
        return _fileName;
    }
}
