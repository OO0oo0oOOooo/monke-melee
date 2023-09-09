using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GibbonRefrences _playerPrefab;

    private static List<Transform> _spawnPoints = new List<Transform>();

    public static void AddSpawnPoint(Transform transform) => _spawnPoints.Add(transform);
    public static void RemoveSpawnPoint(Transform transform) => _spawnPoints.Remove(transform);

    public override void OnNetworkSpawn()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        SpawnPlayerServerRpc(clientId);
        EventManager.Instance.OnPlayerRespawn += RespawnPlayer;
    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.OnPlayerRespawn -= RespawnPlayer;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
        GibbonRefrences player = Instantiate(_playerPrefab, RandomSpawnPoint().position, RandomSpawnPoint().rotation);
        player.NetworkObject.SpawnAsPlayerObject(clientId);
    }

    private Transform RandomSpawnPoint()
    {
        int value = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[value];
    }

    private void RespawnPlayer(ulong clientId)
    {
        if(!IsServer) return;

        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.Despawn();
        SpawnPlayerServerRpc(clientId);
    }
}
