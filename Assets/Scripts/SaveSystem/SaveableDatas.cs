using System;
using System.Collections.Generic;

[Serializable]
public class SaveableDatas
{
    public string _fileName;

    public SerializableDictionary<string, int> SavedInts = new();
    public SerializableDictionary<string, float> SavedFloats = new();
    public SerializableDictionary<string, string> SavedStrings = new();
    public SerializableDictionary<string, bool> SavedBools = new();

    public SerializableDictionary<string, int[]> SavedIntArrays = new();
    public SerializableDictionary<string, float[]> SavedFloatArrays = new();
    public SerializableDictionary<string, string[]> SavedStringArrays = new();
    public SerializableDictionary<string, bool[]> SavedBoolArrays = new();

    public SaveableDatas(string fileName)
    {
        _fileName = fileName;
    }

    public void SaveInt(string name, int value)
    {
        if (SavedInts.ContainsKey(name))
        {
            SavedInts[name] = value;
            return;
        }
        SavedInts.Add(name, value);
    }

    public int GetSavedInt(string name) {  return SavedInts[name]; }

    public void SaveFloat(string name, float value)
    {
        if (SavedFloats.ContainsKey(name))
        {
            SavedFloats[name] = value;
            return;
        }
        SavedFloats.Add(name, value);
    }

    public float GetSavedFloat(string name) { return SavedFloats[name]; }

    public void SaveString(string name, string value)
    {
        if (SavedStrings.ContainsKey(name)) 
        { 
            SavedStrings[name] = value; 
            return; 
        }
        SavedStrings.Add(name, value);
    }

    public string GetSavedString(string name) { return SavedStrings[name]; }

    public void SaveBool(string name, bool value)
    {
        if (SavedBools.ContainsKey(name))
        {
            SavedBools[name] = value;
            return;
        }
        SavedBools.Add(name, value);
    }

    public bool GetSavedBool(string name) { return SavedBools[name]; }

    public void SaveIntArray(string name, int[] value)
    {
        if (SavedIntArrays.ContainsKey(name))
        {
            SavedIntArrays[name] = value;
            return;
        }
        SavedIntArrays.Add(name, value);
    }

    public int[] GetSavedIntArray(string name) { return SavedIntArrays[name]; }

    public void SaveFloatArray(string name, float[] value)
    {
        if (SavedFloatArrays.ContainsKey(name))
        {
            SavedFloatArrays[name] = value;
            return;
        }
        SavedFloatArrays.Add(name, value);
    }

    public float[] GetSavedFloatArray(string name) { return SavedFloatArrays[name]; }

    public void SaveStringArray(string name, string[] value)
    {
        if (SavedStringArrays.ContainsKey(name))
        {
            SavedStringArrays[name] = value;
            return;
        }
        SavedStringArrays.Add(name, value);
    }

    public string[] GetSavedStringArray(string name) { return SavedStringArrays[name]; }

    public void SaveBoolArray(string name, bool[] value)
    {
        if (SavedBoolArrays.ContainsKey(name))
        {
            SavedBoolArrays[name] = value;
            return;
        }
        SavedBoolArrays.Add(name, value);
    }

    public bool[] GetSavedBoolArray(string name) { return SavedBoolArrays[name]; }

    public string GetFileName()
    {
        return _fileName;
    }
}

[Serializable]
public class SerializableDictionary<TKey, UValue>
{
    public List<TKey> Keys = new List<TKey>();
    public List<UValue> Values = new List<UValue>();

    public int Count { get { return Keys.Count; } }

    public void Add(TKey key, UValue val)
    {
        if (Keys.Contains(key))
        {
            throw new Exception("Tried to add a key that already exists.");
        }

        Keys.Add(key);
        Values.Add(val);
    }

    public bool ContainsKey(TKey key)
    {
        return Keys.Contains(key);
    }

    public UValue GetValue(TKey key)
    {
        return Values[Keys.IndexOf(key)];
    }

    public UValue this[TKey key]
    {
        get => GetValue(key);
        set
        {
            if (!Keys.Contains(key))
            {
                throw new Exception("The key you're trying to access was not found.");
            }
            else
            {
                Values[Keys.IndexOf(key)] = value;
            }
        }
    }
}