using System.Collections;
using System.Collections.Generic;
using Beamable;
using System.Threading.Tasks;
using Beamable.Server.Clients;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Web3Manager : MonoBehaviour
{

    private ExtraPlayerDataServiceClient _ExtraPlayerDataServiceClient = null;

    private class Web3Data
    {
        public string playerId;
        public string walletAddress;
        public bool nftOwned;
    }
    // Start is called before the first frame update
    async void Start()
    {
        _ExtraPlayerDataServiceClient = new ExtraPlayerDataServiceClient();

        // get wallet address
        await WalletService();
    }

    private async Task<Web3Data> WalletService()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        string _playerId = beamContext.PlayerId.ToString();

        Debug.Log($"beamContext.PlayerId = {_playerId}");

        // Call Microservice method
        string jsonAddress = await _ExtraPlayerDataServiceClient.GetAddress(_playerId);

        // Parse json string to object
        JObject parsedJson = JObject.Parse(jsonAddress);

        string walletAddress = (string)parsedJson["walletAddress"];
        string playerId = beamContext.PlayerId.ToString();

        Web3Data playerData = new Web3Data();

        playerData.playerId = playerId;
        playerData.walletAddress = walletAddress;

        Debug.Log(playerData.walletAddress.GetType());

        Debug.Log(playerData.walletAddress);

        return playerData;

    }
}
