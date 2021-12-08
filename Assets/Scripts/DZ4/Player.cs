using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace NikolayTrofimovDZ4
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        private GameObject _playerCharacter;

        private void Start()
        {
            SpawnCharacter();
        }

        private void SpawnCharacter()
        {
            if(!isServer)
            {
                return;
            }

            _playerCharacter = Instantiate(_playerPrefab, transform);

            NetworkServer.SpawnWithClientAuthority(_playerCharacter, connectionToClient);
        }
    }
}