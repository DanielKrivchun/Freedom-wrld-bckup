using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabHolder", menuName = "ScriptableObject/PrefabHolder", order = 100)]
public class PrefabHolder : ScriptableObject
{
    public List<_HolderData> PetPrefabs;

    //Give pet prefabe GameObject using PetID
    public GameObject _GetMyPrefab(string _pet_id)
    {
        return PetPrefabs.Find(asd => asd.PrefabId == _pet_id).PetPrefab;
    }
}

[System.Serializable]
public class _HolderData
{
    public GameObject PetPrefab;
    public string PrefabId;
}
