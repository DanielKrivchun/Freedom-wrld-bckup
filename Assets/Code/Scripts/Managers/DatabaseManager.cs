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

    private void Update()
    {
        if(TimingManager.gameHourTimer < 0)
        {
            Pet pet = new Pet
                (petCareSateManager.lastTimeHappy,
                petCareSateManager.lastTimeFeed,
                petCareSateManager.lastTimeClean,
                petCareSateManager.happiness,
                petCareSateManager.hunger,
                petCareSateManager.cleanliness,
                petCareSateManager.energy
                );
            SavePet( pet );
        }
    }

    /*private void Start()
    {
        Pet pet = LoadPet();

        if (pet != null) Debug.Log(LoadPet().energy);
    }*/

    public void SavePet(Pet pet)
    {
        database.SaveData("pet", pet);
    }

    public Pet LoadPet()
    {
        Pet returnValue = null;
        database.LoadData<Pet>("pet", (pet) =>
        {
            returnValue = pet;
        });
        return returnValue;
    }
}
