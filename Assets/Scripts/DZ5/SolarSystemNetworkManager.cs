using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace NikolayTrofimovDZ5
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private string _playerName;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            player.GetComponent<ShipController>().PlayerName = _playerName;

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        

        public void OnPlayerNameChanged(string value)
        {
            _playerName = value;
        }
    }
}