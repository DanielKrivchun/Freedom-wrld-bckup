using System.Collections;
using System.Collections.Generic;
using Beamable;
using System.Threading.Tasks;
using Beamable.Server.Clients;
using Newtonsoft.Json.Linq;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;
using UnityEngine;
using NanoSockets;
using System;
using PlayFab.ServerModels;

public class Web3Manager : MonoBehaviour
{
    string rpc = "https://eth-mainnet.g.alchemy.com/v2/8g1sURXwDAEYdIHw7q6F5prdQ77C6y-7";
    string contractAddress = "0x616300b0f9db555cb2c645a943104035b4dbb347";
    string deployerAddress = "0x6e7dE08F9dC987d881D84456970581d047520b99";
    private walletServiceClient _walletServiceClient = null;

    [SerializeField] private PetCareUIManager _PetCareUIManager;

    private class Web3Data
    {
        public string playerId;
        public string walletAddress;
        public bool nftOwned;
    }

    public void Start()
    {
        _walletServiceClient = new walletServiceClient();

        /*await CheckAccount();*/
    }

    /*public async Task CreateEntry(string _address)
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        string _playerId = beamContext.PlayerId.ToString();

        var response = await _walletServiceClient.CreateEntry(_playerId, _address, false);
        Debug.Log(response);


    }*/

    // return Web3Data object
    private async Task<Web3Data> WalletService(string _playerId)
    {
        // Get address from microstorage as json string
        string jsonAddress = await _walletServiceClient.GetEntryByPlayerId(_playerId);

        // Parse into class object
        JObject parsedJson = JObject.Parse(jsonAddress);

        // query by name
        string walletAddress = (string)parsedJson["walletAddress"];
        string playerId = (string)parsedJson["playerId"];
        bool nftOwned = (bool)parsedJson["nftOwned"];

        Web3Data playerData = new Web3Data();
        playerData.playerId = playerId;
        playerData.walletAddress = walletAddress;
        playerData.nftOwned = nftOwned;

        //return as playerData object
        return playerData;

    }

    // Define contract balanceOf function as a class
    [Function("balanceOf", "uint256")]
    private class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public string Owner { get; set; }
    }

    // Check NFT balance and store bool
    public async Task<bool> CheckAccount()
    {
        // Initiate Beamable and playerId
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        string _playerId = beamContext.PlayerId.ToString();

        Web3Data _data = await WalletService(_playerId);

        try
        {
            // Create Web3 instance and check balance
            var web3 = new Web3(rpc);
            var balanceOfFunctionMessage = new BalanceOfFunction()
            {
                Owner = _data.walletAddress
            };

            var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var balance = await balanceHandler.QueryAsync<BigInteger>(contractAddress, balanceOfFunctionMessage);

            // Convert balance to decimal and divide by 10^18
            decimal formattedBalance = (decimal)balance / (decimal)BigInteger.Pow(10, 18);
            int nftCount = (int)formattedBalance;  // Truncate to the nearest whole number

            Debug.Log($"Formatted balance: {formattedBalance}, nftCount: {nftCount}");  // Log the balance for debugging

            // Check if the wallet has more than 1 NFT
            if (nftCount > 1)
            {
                // call method to store bool in microstorage
                Debug.Log($"The owner of {_data.walletAddress} owns {nftCount} NFT(s) from this contract.");
                await _walletServiceClient.UpdateNftOwned(_playerId, true);

                // Activate fourth pet if user owns nft
                await _PetCareUIManager.ActivateFourthPet();
                return true;
            }
            else
            {
                // call method to store bool in microstorage
                Debug.Log($"The owner of {_data.walletAddress} owns less than 1 NFT from this contract.");
                await _walletServiceClient.UpdateNftOwned(_playerId, false);

                // Keep fourth pet token-gated
                await _PetCareUIManager.ActivateFourthPet();
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching NFT balance: {ex.Message}");
            return false;
        }
    }

}
