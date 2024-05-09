using System.IO;
using UnityEngine;

public class LocalDatabase
{
    private string path = Application.dataPath + "/Resources/Data/";

    public string SaveData<T>(T saveData)
    {
        return JsonUtility.ToJson(saveData);
        //File.WriteAllText(path + saveName + ".json", jsonTxt);

    }

    public void LoadData<T>(string saveName, System.Action<T> callback)
    {
        if (File.Exists(path + saveName + ".json"))
        {
            string loadedJSON = File.ReadAllText(path + saveName + ".json");
            callback(JsonUtility.FromJson<T>(loadedJSON));
        }
        else
        {
            Debug.Log("File not found!");
        }
    }
}
