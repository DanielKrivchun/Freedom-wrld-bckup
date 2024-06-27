using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankChanger : MonoBehaviour
{
    public int RankUpdater;
    public int _UpdateMyRank()
    {
        RankUpdater++;
        return RankUpdater;
    }

}
