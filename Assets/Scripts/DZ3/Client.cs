using System.Text;
using UnityEngine;
using UnityEngine.Networking;


namespace NikolayTrofimovDZ3
{
    public class Client : MonoBehaviour
    {
        public delegate void OnMessageRecive(object message);
        public event OnMessageRecive OnMessageReciveEvent;

        private const int MAX_CONNECTION = 10;

        private int _port = 0;
        private int _serverPort = 5805;

        private int _hostID;

        private int _reliableChannel;
        private int _connectionID;

        private bool _isConnected = false;
        private byte _error;

        private string _playerName;

        private void Update()
        {
            if (!_isConnected) return;

            int recHostId;
            int connectionId;
            int channelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
#pragma warning disable 618
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out _error);
#pragma warning restore 618

            while (recData != NetworkEventType.Nothing)
            {
                switch (recData)
                {
                    case NetworkEventType.Nothing:
                        break;

                    case NetworkEventType.ConnectEvent:
                        string messageCE = "You have been connected to server.";
                        OnMessageReciveEvent?.Invoke(messageCE);
                        Debug.Log(messageCE);
                        SendMessage(_playerName);
                        break;

                    case NetworkEventType.DataEvent:
                        string messageDE = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        OnMessageReciveEvent?.Invoke(messageDE);
                        Debug.Log(messageDE);
                        break;

                    case NetworkEventType.DisconnectEvent:
                        _isConnected = false;
                        string messageDisE = "You have been disconnected from server.";
                        OnMessageReciveEvent?.Invoke(messageDisE);
                        Debug.Log(messageDisE);
                        break;

                    case NetworkEventType.BroadcastEvent:
                        break;
                }

#pragma warning disable 618
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out _error);
            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out _error);
#pragma warning restore 618
        }

        public void Connect(string playerName)
        {
#pragma warning disable 618
            NetworkTransport.Init();
            ConnectionConfig connectionConfig = new ConnectionConfig();
#pragma warning restore 618

            _reliableChannel = connectionConfig.AddChannel(QosType.Reliable);

#pragma warning disable 618
            HostTopology topology = new HostTopology(connectionConfig, MAX_CONNECTION);

            _hostID = NetworkTransport.AddHost(topology, _port);
            _connectionID = NetworkTransport.Connect(_hostID, "127.0.0.1", _serverPort, 0, out _error);
#pragma warning restore 618

            if ((NetworkError)_error == NetworkError.Ok)
            {
                _isConnected = true;
                _playerName = playerName;
            }
            else
                Debug.Log((NetworkError)_error);
        }

        public void Disconnect()
        {
            if (!_isConnected) return;

#pragma warning disable 618
            NetworkTransport.Disconnect(_hostID, _connectionID, out _error);
#pragma warning restore 618
            _isConnected = false;
        }

        public void SendMessage(string message)
        {
#pragma warning disable 618
            if (NetworkTransport.IsStarted)
#pragma warning restore 618
            {
                byte[] buffer = Encoding.Unicode.GetBytes(message);
#pragma warning disable 618
                NetworkTransport.Send(_hostID, _connectionID, _reliableChannel, buffer, message.Length * sizeof(char), out _error);
#pragma warning restore 618
                if ((NetworkError)_error != NetworkError.Ok) Debug.Log((NetworkError)_error);
            }
            else Debug.Log("Not connected");
        }
    }
}