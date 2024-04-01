using System.Collections.Generic;
using System.IO;
using Game.Hub;
using Game.Hub.Interactables;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager: MonoBehaviour{
    [SerializeField] private PlayerData playerData;
    private string savePath;
    public static SaveManager instance;
    [SerializeField] private List<Paragon> paragons;
    private readonly int PARAGON_COUNT = 4;

    void Start(){
        savePath = Application.dataPath + Path.AltDirectorySeparatorChar + "/Saves/SaveData.json";
        playerData = PlayerData.Instance;
        instance = this;
        LoadData();
        PlayerCharacter pc = FindObjectOfType<PlayerCharacter>();
        pc.SetParagonInfo(playerData.currentParagon);
        
        List<ParagonInfo> paragonsOwned = playerData.paragonsOwned;
        int paragonIndex = 0;
        
        paragons.ForEach(p => p.gameObject.SetActive(false));

        foreach (ParagonInfo pInfo in paragonsOwned){
            Debug.Log(pInfo);
            if (pInfo == playerData.currentParagon){
                Debug.Log("Found owned paragon unit!");
                continue;
            }
            Paragon p = paragons[paragonIndex];
            p.gameObject.SetActive(true);
            p.SetParagonInfo(pInfo);
            paragonIndex++;
        }
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
    public bool SpendCopperCoins(int amount){
        if (amount < playerData.copperCoins){
            return false;
        }
        playerData.copperCoins += amount;
        SaveData();
        return true;
    }
    public void BuyParagon(ParagonInfo paragon){
        playerData.paragonsOwned.Add(paragon);
        SaveData();
    }
    public void SetCurrentParagon(ParagonInfo paragonInfo){
        playerData.currentParagon = paragonInfo;
        SaveData();
    }
}