using System;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "PetCareData", menuName = "PetCare/PetCareData", order = 100)]
public class PetCareData : ScriptableObject
{
    public _PetData m_petdata;
    [Space]
    public string m_genratedjson;

    public void _GetMyJsonData()
    {
        Debug.Log(m_petdata.lastTimeHappy);
        Debug.Log(m_petdata.lastTimeClean);
        Debug.Log(m_petdata.lastTimeFeed);
        LocalDatabase d = new LocalDatabase();
        m_genratedjson = d.SaveData(m_petdata);

        DateTime parsedDate = DateTime.Parse(m_petdata.lastTimeHappy);
        Debug.Log(parsedDate);
    }

    public void _SetData()
    {
        m_petdata.lastTimeFeed = (DateTime.Now).ToString();
        m_petdata.lastTimeClean = (DateTime.Now).ToString();
        m_petdata.lastTimeHappy = (DateTime.Now).ToString();
    }


    public void _GenrateFromString()
    {

    }

}

[System.Serializable]
public class _PetData
{
    public string lastTimeHappy, lastTimeFeed, lastTimeClean;
    public int happiness, hunger, cleanliness, energy;
}
