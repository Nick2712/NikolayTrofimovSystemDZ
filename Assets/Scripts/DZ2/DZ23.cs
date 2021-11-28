using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace NikolayTrofimovDZ2
{
    public class DZ23 : MonoBehaviour
    {
        [SerializeField] private GameObject _rotatingObject;
        [SerializeField] private int _objectsCount;
        [SerializeField] private float _objectsSpawnDistance;
        [SerializeField] private float _minRotationSpeed;
        [SerializeField] private float _maxRotationSpeed;

        private NativeArray<float> _rotationSpeeds;
        private TransformAccessArray _rotationObjects;

        private void Start()
        {
            _rotationSpeeds = new NativeArray<float>(_objectsCount, Allocator.Persistent);
            var transforms = new Transform[_objectsCount];
            for(int i = 0; i < _objectsCount; i++)
            {
                var position = Random.insideUnitSphere * Random.Range(0, _objectsSpawnDistance);
                transforms[i] = Instantiate(_rotatingObject, position, Quaternion.identity).transform;
                _rotationSpeeds[i] = Random.Range(_minRotationSpeed, _maxRotationSpeed);
            }
            _rotationObjects = new TransformAccessArray(transforms);
        }

        private void Update()
        {
            var rotateJob = new RotateJob()
            {
                RotationSpeeds = _rotationSpeeds,
                DeltaTime = Time.deltaTime
            };
            JobHandle job = rotateJob.Schedule(_rotationObjects);
            job.Complete();
        }

        private void OnDestroy()
        {
            _rotationSpeeds.Dispose();
            _rotationObjects.Dispose();
        }
    }

    public struct RotateJob : IJobParallelForTransform
    {
        public NativeArray<float> RotationSpeeds;
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            Quaternion rotation = Quaternion.AngleAxis(RotationSpeeds[index] * DeltaTime, Vector3.up);
            Quaternion objectRotation = transform.rotation;
            objectRotation.eulerAngles += rotation.eulerAngles;

            transform.rotation = objectRotation;
        }
    }
}