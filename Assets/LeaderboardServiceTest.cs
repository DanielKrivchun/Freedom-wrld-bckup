using System.Collections;
using System.Collections.Generic;
using Beamable.Common.Leaderboards;
using Beamable;
using UnityEngine;

public class LeaderboardServiceTest : MonoBehaviour
{
    //  Fields  ---------------------------------------
    [SerializeField] private LeaderboardRef _leaderboardRef = null;
    [SerializeField] private double _score = 100;

    //  Unity Methods  --------------------------------
    protected void Start()
    {
        Debug.Log($"Start()");

        LeaderboardServiceSetScore(_leaderboardRef.Id, _score);
    }

    //  Methods  --------------------------------------
    private async void LeaderboardServiceSetScore(string id, double score)
    {
        var beamContext = BeamContext.Default; 
        await beamContext.OnReady;

        Debug.Log($"beamContext.PlayerId = {beamContext.PlayerId}");

        await beamContext.Api.LeaderboardService.SetScore(id, score);

        Debug.Log($"LeaderboardService.SetScore({id},{score})");
    }
}
