using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    private Database database;
    public PetCareStateManager petCareSateManager;

    private void Awake()
    {
        database = new Database();
        if (instance == null)
        {
            instance = this;
        }
        else Debug.LogWarning("More than one DatabaseManager In the Scene");
    }

    /*private void Update()
    {
        if(TimingManager.gameHourTimer < 0)
        {
            Pet petCareData = new Pet(petCareSateManager.lastTimeHappy.ToString(), petCareSateManager.lastTimeFeed.ToString(), petCareSateManager.lastTimeClean.ToString(),
                                        petCareSateManager.happiness, petCareSateManager.hunger, petCareSateManager.cleanliness, petCareSateManager.energy);
            
            SavePet(petCareData);
        }
    }*/

    /*private void Start()
    {
        Pet pet = LoadPet();

        if (pet != null) Debug.Log(LoadPet().energy);
    }*/

    public void SavePet(PetData petCareData)
    {
        database.SaveData("pet", petCareData);
    }

    public PetData LoadPet()
    {
        PetData returnValue = null;
        database.LoadData<PetData>("pet", (petCareData) =>
        {
            returnValue = petCareData;
        });
        return returnValue;
    }
}
