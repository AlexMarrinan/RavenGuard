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
    [SerializeField] private List<Paragon> fountainParagons;
    [SerializeField] private List<Paragon> shopParagons;
    [SerializeField] private ParagonInfo startingPInfo;

    private readonly int PARAGON_COUNT = 4;

    void Start(){
        savePath = Application.persistentDataPath + "/SaveData.json";
        playerData = PlayerData.Instance;
        instance = this;
        LoadData();
        PlayerCharacter pc = FindObjectOfType<PlayerCharacter>();
        if (pc != null){
            pc.SetParagonInfo(playerData.currentParagon);
        }
        
        List<ParagonInfo> paragonsOwned = playerData.paragonsOwned;
        int paragonIndex = 0;
        
        fountainParagons.ForEach(p => p.gameObject.SetActive(false));
        shopParagons.ForEach(p => p.gameObject.SetActive(true));

        if (fountainParagons.Count <= 0){
            return;
        }
        foreach (ParagonInfo pInfo in paragonsOwned){
            Debug.Log(pInfo);
            Debug.Log(playerData.currentParagon);

            foreach (Paragon shopP in shopParagons){
                if (shopP.GetParagonInfo() == pInfo){
                    shopP.gameObject.SetActive(false);
                    break;
                }
            }
            if (pInfo == playerData.currentParagon){
                continue;
            }
            Paragon p = fountainParagons[paragonIndex];
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
            playerData = new();
            playerData.currentParagon = startingPInfo;
            playerData.paragonsOwned.Add(startingPInfo);
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
    public int GetCoins(){
        return playerData.copperCoins;
    }
    public void AddCopperCoins(int amount){
        playerData.copperCoins += amount;
        SaveData();
    }
    public bool SpendCopperCoins(int amount){
        if (amount > playerData.copperCoins){
            return false;
        }
        playerData.copperCoins -= amount;
        SaveData();
        return true;
    }
    public void BuyParagon(ParagonInfo pInfo){
        playerData.paragonsOwned.Add(pInfo);
        foreach (Paragon p in fountainParagons){
            if (p.GetParagonInfo() == null){
                p.SetParagonInfo(pInfo);
                p.gameObject.SetActive(true);
                break;
            }
        }
        SaveData();
    }
    public void SetCurrentParagon(ParagonInfo paragonInfo){
        playerData.currentParagon = paragonInfo;
        SaveData();
    }
    
    public ParagonInfo GetCurrentParagon(){
        return playerData.currentParagon;
    }
    public BaseSkill GetSkill(SkillProgressionGroup skillProgressionGroup){
        var upgrades = playerData.skillUpgrades;
        if (!upgrades.ContainsKey(skillProgressionGroup)){
            return skillProgressionGroup.skillProgression[0].skill;
        }
        int index = upgrades[skillProgressionGroup];
        return skillProgressionGroup.skillProgression[index].skill;
    }
    public int GetSkillLevel(SkillProgressionGroup skillProgressionGroup){
        var upgrades = playerData.skillUpgrades;
        if (!upgrades.ContainsKey(skillProgressionGroup)){
            return 0;
        }
        int index = upgrades[skillProgressionGroup];
        return index;
    }
    public void UpgradeSkill(SkillProgressionGroup skillProgressionGroup, int level){
        var upgrades = playerData.skillUpgrades;
        upgrades[skillProgressionGroup] = level-1;
        SpendCopperCoins(skillProgressionGroup.skillProgression[level-1].cost);
    }
}