using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem
{
    public static void SaveData(SaveableDatas datas)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + $"/{datas.GetFileName()}.wad";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, datas);
        stream.Close();
    }

    public static SaveableDatas LoadDatas(string fileName)
    {
        string path = Application.persistentDataPath + $"/{fileName}.wad";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveableDatas datas = formatter.Deserialize(stream) as SaveableDatas;
            stream.Close();
            return datas;
        }
        else
        {
            Debug.LogError("Save file not found in :" + path);
            return null;
        }
    }
}
