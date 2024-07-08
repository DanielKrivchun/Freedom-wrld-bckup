using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NamesJson", menuName = "ScriptableObject/NamesJson", order = 100)]
public class NamesJson : ScriptableObject
{
    public _FakeUsers AINames;

    [HideInInspector]
    public List<int> usedNames;

    public async void _SetMyData()
    {
        TextAsset s = Resources.Load("Names") as TextAsset;
        AINames = JsonUtility.FromJson<_FakeUsers>(s.ToString());
        await Utils._Waiter(500);
    }

    public string _GetNames()
    {
        int a = _GetRandomNumber();

        Debug.Log("Random Number " + a);
        if (usedNames.Contains(a))
        {
            a = _GetRandomNumber();
        }

        return AINames.Names[a];
    }

    private int _GetRandomNumber()
    {
        Debug.Log("Name list count " + AINames.Names.Count);
        return Random.Range(0, AINames.Names.Count);
    }

}

[System.Serializable]
public class _FakeUsers
{
    public List<string> Names;
}


