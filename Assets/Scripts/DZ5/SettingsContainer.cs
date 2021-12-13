using UnityEngine;


namespace NikolayTrofimovDZ5
{
    public class SettingsContainer : Singleton<SettingsContainer>
    {
        [SerializeField] private SpaceShipSettings _spaceShipSettings;

        public SpaceShipSettings SpaceShipSettings => _spaceShipSettings;
    }
}