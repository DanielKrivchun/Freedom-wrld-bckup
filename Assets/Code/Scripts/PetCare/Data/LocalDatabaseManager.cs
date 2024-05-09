using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDatabaseManager : MonoBehaviour
{
    public static LocalDatabaseManager instance;

    private LocalDatabase database;
    public PetCareStateManager petCareStateManager;

    private void Awake()
    {
        database = new LocalDatabase();
        if (instance == null)
        {
            instance = this;
        }
        else Debug.LogWarning("More than one DatabaseManager In the Scene");
    }

    /*private void Update()
    {
        if (TimingManager.gameHourTimer < 0)
        {
            PetCareData petCareData = new PetCareData
                (petCareStateManager.lastTimeHappy, petCareStateManager.lastTimeFeed, petCareStateManager.lastTimeClean,
                petCareStateManager.happiness, petCareStateManager.hunger, petCareStateManager.cleanliness, petCareStateManager.energy);
            SavePet(petCareData);
        }
    }*/

    private void Start()
    {
        PetCareData pet = LoadPet();

        if (pet != null) Debug.Log(LoadPet().energy);
    }

    public void SavePetCareData()
    {
        PetCareData petCareData = new PetCareData
                (petCareStateManager.lastTimeHappy, petCareStateManager.lastTimeFeed, petCareStateManager.lastTimeClean,
                petCareStateManager.happiness, petCareStateManager.hunger, petCareStateManager.cleanliness, petCareStateManager.energy);

        database.SaveData("PetCareData", petCareData);
    }

    public void GetPetCareData()
    {
        Debug.Log("Pet Care Data - " + LoadPet());
    }

    public PetCareData LoadPet()
    {
        PetCareData returnValue = null;
        database.LoadData<PetCareData>("PetCareData", (pet) =>
        {
            returnValue = pet;
        });
        return returnValue;
    }
}
