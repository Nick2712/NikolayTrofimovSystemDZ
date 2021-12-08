using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace NikolayTrofimovDZ4
{
    public abstract class FireAction : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private int _startAmunition = 20;

        protected string _countBullet = string.Empty;
        protected Queue<GameObject> _bullets = new Queue<GameObject>();
        protected Queue<GameObject> _ammunition = new Queue<GameObject>();
        protected bool _reloading = false;

        public string BulletCount => _countBullet;

        protected virtual void Start()
        {
            for (var i = 0; i < _startAmunition; i++)
            {
                GameObject bullet;
                if(_bulletPrefab == null)
                {
                    bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                else
                {
                    bullet = Instantiate(_bulletPrefab);
                }
                bullet.SetActive(false);
                _ammunition.Enqueue(bullet);
            }
        }

        public virtual async void Reloading()
        {
            _bullets = await Reload();
        }

        protected virtual void Shooting()
        {
            if(_bullets.Count == 0)
            {
                Reloading();
            }
        }

        private async Task<Queue<GameObject>> Reload()
        {
            if (!_reloading)
            {
                _reloading = true;
                StartCoroutine(ReloadingAnim());
                return await Task.Run(delegate
                {
                    var cage = 10;
                    if (_bullets.Count < cage)
                    {
                        Thread.Sleep(3000);
                        var bullets = this._bullets;
                        while (bullets.Count > 0)
                        {
                            _ammunition.Enqueue(bullets.Dequeue());
                        }
                        cage = Mathf.Min(cage, _ammunition.Count);
                        if (cage > 0)
                        {
                            for (var i = 0; i < cage; i++)
                            {
                                var sphere = _ammunition.Dequeue();
                                _bullets.Enqueue(sphere);
                            }
                        }
                    }
                    _reloading = false;
                    return _bullets;
                });
            }
            else
            {
                return _bullets;
            }
        }

        private IEnumerator ReloadingAnim()
        {
            while(_reloading)
            {
                _countBullet = " | ";
                yield return new WaitForSeconds(0.1f);
                _countBullet = @" \ ";
                yield return new WaitForSeconds(0.1f);
                _countBullet = "---";
                yield return new WaitForSeconds(0.1f);
                _countBullet = " / ";
                yield return new WaitForSeconds(0.1f);

            }
            _countBullet = _bullets.Count.ToString();
            yield return null;
        }
    }
}