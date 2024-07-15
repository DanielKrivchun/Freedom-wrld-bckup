using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RunnerHandller : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner networkRunnerInstance;

    public RaceManager raceManager;
    public Vector2 m_input;
    #region INetworkRunnerCallbacks ALSO SETTING INPUT OVER HERE

    //FOR INPUTx
    public void _InputSet(InputAction.CallbackContext context)
    {
        m_input = context.ReadValue<Vector2>();

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        //CHECK OF ROOM IS FULL IF FULL THEN DESPWAN PLAYER
        RaceManager.instance._SpawnPlayer(player);
        //CHECKING FOR AI PLAYER COUNT
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        RaceManager.instance._DespawnPlayer(player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        data.direction = RaceManager.instance.m_input;
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
        NetworkEventManager._EventNetworkErrors(_Strings.ServerError);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("OnDisconnectedFromServer");
        NetworkEventManager._EventNetworkErrors(_Strings.ServerError);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed");
        NetworkEventManager._EventNetworkErrors(_Strings.ConnectuonFailed);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");
        NetworkEventManager._EventNetworkErrors(_Strings.HostMigrated);
        return;
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);
        FindObjectOfType<RunnerHandller>()._StartHostMigration(hostMigrationToken);

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }


    #endregion


    public void _StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        networkRunnerInstance = Instantiate(RaceManager.instance.NetworkRunnerPrefab);
        networkRunnerInstance.name = "Migrated Ruunner";
        //networkRunnerInstance.AddCallbacks(this);
        Debug.Log("Host migration started");
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var clienttask = _InitilizeNetworkRunnerHostMigration(networkRunnerInstance, hostMigrationToken);
    }

    protected virtual Task _InitilizeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostmigretiontoken)
    {
        runner.ProvideInput = true;

        return networkRunnerInstance.StartGame(new StartGameArgs
        {
            SceneManager = networkRunnerInstance.GetComponent<NetworkSceneManagerDefault>(),
            HostMigrationToken = hostmigretiontoken,
            HostMigrationResume = _HostmigrationResume
        });

    }

    void _HostmigrationResume(NetworkRunner runner)
    {
        Debug.Log("_HostmigrationResume Started");


        foreach (var resumeobject in runner.GetResumeSnapshotNetworkObjects())
        {
            if (resumeobject.TryGetBehaviour<NavmeshMultiplayer>(out var player))
            {
                runner.Spawn(resumeobject, position: player.MyPos, rotation: player.transform.rotation, onBeforeSpawned: (runner, newtnetworkobject) =>
                {
                    Debug.Log("Worked");
                    newtnetworkobject.CopyStateFrom(resumeobject);
                });
            }

            if (resumeobject.TryGetBehaviour<NetworkAIPlayer>(out var netorkai))
            {
                runner.Spawn(resumeobject, position: netorkai.transform.position, rotation: netorkai.transform.rotation, onBeforeSpawned: (runner, newtnetworkobject) =>
                {
                    Debug.Log("Worked");
                    newtnetworkobject.CopyStateFrom(resumeobject);
                });
            }
        }

        Debug.Log("_HostmigrationResume Completed");
        NetworkEventManager._EventHostMigrationDone();
    }

}
