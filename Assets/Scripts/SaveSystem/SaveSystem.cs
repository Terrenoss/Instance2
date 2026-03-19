using UnityEngine;
using System.IO;

public class SaveSystem
{
    public static void SaveData(SaveableDatas datas)
    {
        string path = Application.persistentDataPath + $"/{datas.GetFileName()}.wad";
        string dataToStore = JsonUtility.ToJson(datas, true);

        if (File.Exists(path))
        {
            File.WriteAllText(path, dataToStore );
            return;
        }

        FileStream stream = new FileStream(path, FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(dataToStore);
        writer.Close();
        stream.Close();
    }

    public static SaveableDatas LoadDatas(string fileName)
    {
        string path = Application.persistentDataPath + $"/{fileName}.wad";
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string dataToLoad = reader.ReadToEnd();
            SaveableDatas datas = JsonUtility.FromJson<SaveableDatas>(dataToLoad);
            stream.Close();
            return datas;
        }
        else
        {
            Debug.LogWarning("Save file not found in :" + path);
            return null;
        }
    }
}
