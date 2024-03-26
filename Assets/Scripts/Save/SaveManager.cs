using System.IO;
using UnityEngine;

public class SaveManager: MonoBehaviour{
    private PlayerData playerData;
    private string savePath;
    public static SaveManager instance;
    void Start(){
        savePath = Application.dataPath + Path.AltDirectorySeparatorChar + "/Saves/SaveData.json";
        playerData = PlayerData.Instance;
        instance = this;
        LoadData();
    }

    public void SaveData(){
        string json = JsonUtility.ToJson(playerData);
        Debug.Log(json);

        using(StreamWriter writer = new(savePath))
        {
            writer.Write(json);
        }
    }

    public void LoadData(){
        if (!File.Exists(savePath)){
            SaveData();
            return;
        }
        string json = "";
        using(StreamReader reader = new (savePath))
        {
            json = reader.ReadToEnd();
        }

        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        playerData.SetData(data);
    }

    public PlayerData GetData(){
        return playerData;
    }
    public void AddCopperCoins(int amount){
        playerData.copperCoins += amount;
        SaveData();
    }

    public void BuyParagon(UnitClass unitClass){
        playerData.paragonsOwned.Add(unitClass);
        SaveData();
    }
}