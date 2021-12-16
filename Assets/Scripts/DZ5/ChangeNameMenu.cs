using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace NikolayTrofimovDZ5
{
    public class ChangeNameMenu : MonoBehaviour
    {
        [SerializeField] private InputField _text;
        private NetworkManagerHUD _networkManagerHUD;
        private SolarSystemNetworkManager _solarSystemNetworkManager;

        private void Start()
        {
            _networkManagerHUD = GetComponent<NetworkManagerHUD>();
            _solarSystemNetworkManager = GetComponent<SolarSystemNetworkManager>();
            _text.onEndEdit.AddListener(_solarSystemNetworkManager.OnPlayerNameChanged);
        }

        private void Update()
        {
            if (_text.isFocused && _networkManagerHUD.showGUI)
            {
                _networkManagerHUD.showGUI = false;
            }
            if (!_text.isFocused && !_networkManagerHUD.showGUI)
            {
                _networkManagerHUD.showGUI = true;
            }
            if(NetworkServer.active || NetworkClient.active)
            {
                _text.gameObject.SetActive(false);
            }
        }
    }
}