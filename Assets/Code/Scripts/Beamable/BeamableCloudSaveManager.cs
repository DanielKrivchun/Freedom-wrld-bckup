using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Beamable;
using Beamable.Api.CloudSaving;


namespace Beamable.CloudSavingService
{
    [Serializable]
    public class RefreshedUnityEvent : UnityEvent<BeamableCloudSavingData> { }

    [Serializable]
    public class BeamableCloudSavingData
    {
        public bool IsFirstFrame = true;
        public bool IsDataFoundFirstFrame = false;

        public PetData petDataCloud = null;
        public PetData petDataLocal = null;

        //public List<string> InstructionLogs = new List<string>();
        public DataState DataState = DataState.Initializing;
    }

    public enum DataState
    {
        Initializing,
        Pending,
        Synced,
        Unsynced
    }


    public class BeamableCloudSaveManager : MonoBehaviour
    {
        public static BeamableCloudSaveManager instance;

        //  Events  ---------------------------------------
        [HideInInspector] public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();

        //  Fields  ---------------------------------------
        private BeamContext _beamContext;
        private Api.CloudSaving.CloudSavingService _cloudSavingService;
        private readonly BeamableCloudSavingData beamableCloudSavingData = new BeamableCloudSavingData();

        public SimpleGameEvent loadGameData;

        /// <summary>
        /// Dynamically build the local storage for the Cloud Saving Data object
        /// </summary>
        private string FilePath
        {
            get
            {
                // Suggested format
                string fileName = "myPetCareData.json";

                // Required format
                return $"{_cloudSavingService.LocalCloudDataFullPath}{Path.DirectorySeparatorChar}{fileName}";
            }
        }


        //  Unity Methods  --------------------------------
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                DestroyImmediate(instance);
            }

            DontDestroyOnLoad(gameObject);
        }

        protected void Start()
        {
            SetupBeamable();
        }


        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;

            Debug.Log($"_beamContext.PlayerId = {_beamContext.PlayerId}");

            _cloudSavingService = _beamContext.Api.CloudSavingService;

            // Subscribe to the UpdatedReceived event to handle when data on disk does not yet exist and is pulled from the server
            _cloudSavingService.UpdateReceived += CloudSavingService_OnUpdateReceived;

            // Subscribe to the OnError event to handle when the service fails
            _cloudSavingService.OnError += CloudSavingService_OnError;

            // Check isInitializing, as best practice
            if (!_cloudSavingService.isInitializing)
            {
                // Init the service, which will first download content that the server may have, that the client does not.
                // The client will then upload any content that it has, that the server is missing
                await _cloudSavingService.Init();
            }
            else
            {
                throw new Exception("Cannot call Init() when " + $"isInitializing = {_cloudSavingService.isInitializing}");
            }


            // Check isInitializing, as best practice
            /*if (!_cloudSavingService.isInitializing)
            {
                // Resets the local cloud data to match the server cloud data
                // IF DESIRED, UNCOMMENT THIS SECTION
                await _cloudSavingService.ReinitializeUserData();
            }
            else
            {
                throw new Exception("Cannot call Init() when " + $"isInitializing = {_cloudSavingService.isInitializing}");
            }*/


            loadGameData.Raise();
            Refresh();
        }


        private void LoadAndSave()
        {
            beamableCloudSavingData.DataState = DataState.Pending;
            beamableCloudSavingData.petDataLocal = LoadData();

            // Determines if the data was found on the very first checking of the scene
            // Useful for demonstration purposes only
            if (beamableCloudSavingData.IsFirstFrame == true)
            {
                beamableCloudSavingData.IsFirstFrame = false;
                beamableCloudSavingData.IsDataFoundFirstFrame =
                    beamableCloudSavingData.petDataLocal != null;
            }

            if (beamableCloudSavingData.petDataLocal == null)
            {
                // Create Data - Default
                beamableCloudSavingData.petDataLocal = new PetData
                {
                    lastTimeHappy = DateTime.Now.ToString(),
                    lastTimeFeed = DateTime.Now.ToString(),
                    lastTimeClean = DateTime.Now.ToString(),
                    lastTimeEnergy = DateTime.Now.ToString(),
                    sleepStartTime = DateTime.Now.ToString(),
                    isSleeping = false,
                    happiness = 100,
                    hunger = 100,
                    cleanliness = 100,
                    energy = 100
                };

                SaveDataInternal(beamableCloudSavingData.petDataLocal);
            }

        }

        public PetData LoadData()
        {
            beamableCloudSavingData.DataState = DataState.Pending;
            var loaded = LoadDataInternal();

            beamableCloudSavingData.petDataCloud = loaded;
            beamableCloudSavingData.petDataLocal = loaded;

            Refresh();

            Debug.Log("Loaded Data - " + beamableCloudSavingData.petDataCloud);
            return beamableCloudSavingData.petDataCloud;
        }

        private PetData LoadDataInternal()
        {
            if (!Directory.Exists(_cloudSavingService.LocalCloudDataFullPath))
            {
                Directory.CreateDirectory(_cloudSavingService.LocalCloudDataFullPath);
            }

            PetData myPetData = null;

            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                myPetData = JsonUtility.FromJson<PetData>(json);
            }

            return myPetData;
        }

        public void SaveData(PetData myPetData)
        {
            beamableCloudSavingData.DataState = DataState.Pending;

            SaveDataInternal(myPetData);

            Refresh();
        }

        private void SaveDataInternal(PetData myPetData)
        {
            var json = JsonUtility.ToJson(myPetData);

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            }

            // Once the data is written to disk, the service will
            // automatically upload the contents to the cloud
            File.WriteAllText(FilePath, json);

            beamableCloudSavingData.petDataCloud = myPetData;
            Debug.Log("Data Saved!");
        }

        public void Refresh()
        {
            // Use DataState to display info to the user via UI
            if (beamableCloudSavingData.petDataCloud == null &&
                beamableCloudSavingData.petDataLocal == null)
            {
                // Connecting
                beamableCloudSavingData.DataState = DataState.Initializing;
            }
            else if (beamableCloudSavingData.petDataCloud == null ||
                beamableCloudSavingData.petDataLocal == null)
            {
                // Data transfer pending
                beamableCloudSavingData.DataState = DataState.Pending;
            }
            else if (beamableCloudSavingData.petDataCloud ==
                beamableCloudSavingData.petDataLocal)
            {
                // Local and Cloud are Synced
                beamableCloudSavingData.DataState = DataState.Synced;
            }
            else
            {
                // Local and Cloud are Not Synced
                beamableCloudSavingData.DataState = DataState.Unsynced;
            }

            //Debug.Log("Refresh()");

            OnRefreshed.Invoke(beamableCloudSavingData);
        }


        //  Event Handlers  -------------------------------
        private void CloudSavingService_OnUpdateReceived(ManifestResponse manifest)
        {
            Debug.Log("CloudSavingService_OnUpdateReceived()");

            // If the settings are changed by the server...
            // Reload the scene or something project-specific to reload your game
            //SceneManager.LoadScene(0);
        }


        private void CloudSavingService_OnError(CloudSavingError cloudSavingError)
        {
            Debug.Log($"CloudSavingService_OnError() Message = {cloudSavingError.Message}");
        }
    }
}
