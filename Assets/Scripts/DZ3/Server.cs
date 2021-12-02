using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


namespace NikolayTrofimovDZ3
{
    public class Server : MonoBehaviour
    {
        private const int MAX_CONNECTION = 10;

        private int _port = 5805;

        private int _hostID;
        private int _reliableChannel;

        private bool _isStarted = false;
        private byte _error;

        private Dictionary<int, string> _players = new Dictionary<int, string>();

        private void Update()
        {
            if (!_isStarted) return;

            int recHostId;
            int connectionId;
            int chanelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, bufferSize, out dataSize, out _error);
            while(recData != NetworkEventType.Nothing)
            {
                switch(recData)
                {
                    case NetworkEventType.Nothing:
                        break;

                    case NetworkEventType.ConnectEvent:
                        _players.Add(connectionId, "");

                        Debug.Log($"Player {connectionId} has connected.");
                        break;

                    case NetworkEventType.DataEvent:
                        if (_players.ContainsKey(connectionId))
                        {
                            if (_players[connectionId] == "")
                            {
                                _players[connectionId] = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                                string messgeDE = $"{_players[connectionId]} has connected.";
                                SendMessageToAll(messgeDE);
                                Debug.Log(messgeDE);
                            }
                            else
                            {
                                string messgeDE = $"{_players[connectionId]}: {Encoding.Unicode.GetString(recBuffer, 0, dataSize)}";
                                SendMessageToAll(messgeDE);
                                Debug.Log(messgeDE);
                            }
                        }
                        else Debug.Log("player not found");
                        break;

                    case NetworkEventType.DisconnectEvent:
                        string messageDisE = $"{_players[connectionId]} has disconnected.";
                        _players.Remove(connectionId);
                        SendMessageToAll(messageDisE);
                        Debug.Log(messageDisE);
                        break;

                    case NetworkEventType.BroadcastEvent:
                        break;
                }

                recData = NetworkTransport.Receive(out recHostId, out connectionId, out chanelId, recBuffer, bufferSize, out dataSize, out _error);
            }

        }

        public void StartServer()
        {
            NetworkTransport.Init();

            ConnectionConfig connectionConfig = new ConnectionConfig();
            _reliableChannel = connectionConfig.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(connectionConfig, MAX_CONNECTION);
            _hostID = NetworkTransport.AddHost(topology, _port);

            _isStarted = true;
        }

        public void ShutDownServer()
        {
            if (!_isStarted) return;

            NetworkTransport.RemoveHost(_hostID);
            NetworkTransport.Shutdown();
            _isStarted = false;
        }

        public void SendMessage(string message, int connectionID)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(_hostID, connectionID, _reliableChannel, buffer, message.Length * sizeof(char), out _error);
            if ((NetworkError)_error != NetworkError.Ok) Debug.Log((NetworkError)_error);
        }

        public void SendMessageToAll(string message)
        {
            foreach(var player in _players)
            {
                SendMessage(message, player.Key);
            }
        }
    }
}