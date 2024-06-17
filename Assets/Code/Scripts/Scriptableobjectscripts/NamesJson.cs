using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NamesJson", menuName = "ScriptableObject/NamesJson", order = 100)]
public class NamesJson : ScriptableObject
{
    public _FakeUsers RandomNames;

    private List<int> usedNames;

    public void _SetMyData()
    {
        TextAsset s = Resources.Load("Names") as TextAsset;
        RandomNames = JsonUtility.FromJson<_FakeUsers>(s.ToString());
    }

    public string _GetNames()
    {
        int a = _GetRandomNumber();

        if (usedNames.Contains(a))
        {
            a = _GetRandomNumber();
        }

        return RandomNames.Names[a];
    }

    private int _GetRandomNumber()
    {
        return Random.Range(0, RandomNames.Names.Count);
    }

}

[System.Serializable]
public class _FakeUsers
{
    public List<string> Names;
}


