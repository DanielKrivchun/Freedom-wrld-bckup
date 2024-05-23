using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabHolder", menuName = "ScriptableObject/PrefabHolder", order = 100)]
public class PrefabHolder : ScriptableObject
{
    public List<_HolderData> PetPrefabs;


    public GameObject _GetMyPrefab(string _pet_id)
    {
        return null;
    }

}

[System.Serializable]
public class _HolderData
{
    public GameObject PetPrefab;
    public string PrefabId;
}
