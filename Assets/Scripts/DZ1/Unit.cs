using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace NikolayTrofimovDZ1
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _healingTime = 3.0f;
        [SerializeField] private float _timeBetweenHealing = 0.5f;
        [SerializeField] private int _healingOverPeriod = 5;
        [SerializeField] private Button _healButton;

        private Coroutine _coroutine;

        private void Start()
        {
            _healButton.onClick.AddListener(ReceiveHealing);
        }

        private void ReceiveHealing()
        {
            if (_coroutine == null) _coroutine = StartCoroutine(Heal());
        }

        private IEnumerator Heal()
        {
            for (float time = 0; time < _healingTime; time += _timeBetweenHealing)
            {
                _health += _healingOverPeriod;
                if (_health >= _maxHealth)
                {
                    _health = _maxHealth;
                    Debug.Log($"Лечение завершено {_health}");
                    yield break;
                }
                Debug.Log($"Лечение в процессе {_health}");
                yield return new WaitForSecondsRealtime(_timeBetweenHealing);
            }
            _coroutine = null;
        }
    }
}