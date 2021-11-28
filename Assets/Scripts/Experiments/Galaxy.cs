using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;


namespace Experiment
{
    public class Galaxy : MonoBehaviour
    {
        [SerializeField] private int _numberOfEntities;

        [SerializeField] private float _startMaxDistance;
        [SerializeField] private float _startMaxVelocity;
        [SerializeField] private float _startMaxMass;
        [SerializeField] private float _gravitationModifier = 0.01f;
        [SerializeField] private float _speed = 0.01f;

        [SerializeField] private GameObject celestialBodyPrefab;

        private TransformAccessArray _transformAccessArray;

        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _velocities;
        private NativeArray<Vector3> _accelerations;
        private NativeArray<float> _masses;

        private void Start()
        {
            _positions = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
            _accelerations = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
            _masses = new NativeArray<float>(_numberOfEntities, Allocator.Persistent);

            Transform[] transforms = new Transform[_numberOfEntities];
            for(int i = 0; i < _numberOfEntities; i++)
            {
                _positions[i] = Random.insideUnitSphere * Random.Range(0, _startMaxDistance);
                _velocities[i] = Random.insideUnitSphere * Random.Range(0, _startMaxVelocity);
                _accelerations[i] = new Vector3();
                _masses[i] = Random.Range(1, _startMaxMass);


                GameObject gameObject = Instantiate(celestialBodyPrefab, _positions[i], Quaternion.identity);
                transforms[i] = gameObject.transform;
                if(i == 25)
                {
                    gameObject.GetComponent<Renderer>().material.color = Color.blue;
                    _masses[i] = 1;
                    _velocities[i] = Random.insideUnitSphere * _startMaxVelocity;
                }
                float scale = _masses[i] / _startMaxMass;
                gameObject.transform.localScale = new Vector3(scale, scale, scale);
            }

            _transformAccessArray = new TransformAccessArray(transforms);
        }

        private void Update()
        {
            GravitationJob gravitationJob = new GravitationJob()
            {
                Positions = _positions,
                Velocities = _velocities,
                Accelerations = _accelerations,
                Masses = _masses,
                GravitationModifier = _gravitationModifier,
            };
            JobHandle gravitationHandle = gravitationJob.Schedule(_numberOfEntities, 0);
            
            MoveJob moveJob = new MoveJob()
            {
                Positions = _positions,
                Velocities = _velocities,
                Accelerations = _accelerations,
                DeltaTime = Time.deltaTime,
                Speed = _speed
            };
            JobHandle moveHandle = moveJob.Schedule(_transformAccessArray, gravitationHandle);
            moveHandle.Complete();
        }

        private void OnDestroy()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _accelerations.Dispose();
            _masses.Dispose();
            _transformAccessArray.Dispose();
        }
    }

    public struct GravitationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> Positions;
        [ReadOnly] public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Accelerations;
        [ReadOnly] public NativeArray<float> Masses;
        [ReadOnly] public float GravitationModifier;
        
        public void Execute(int index)
        {
            for(int i = 0; i < Positions.Length; i++)
            {
                if (i == index) continue;

                float distance = Vector3.Distance(Positions[i], Positions[index]);
                Vector3 dirrection = Positions[i] - Positions[index];
                Vector3 gravitation = (dirrection * Masses[i] * GravitationModifier) / (Masses[index] * Mathf.Pow(distance, 2));
                Accelerations[index] += gravitation;
            }
        }
    }

    public struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Accelerations;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float Speed;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 velocity = Velocities[index] + Accelerations[index] * DeltaTime;
            transform.position += velocity * Speed;

            Positions[index] = transform.position;
            Velocities[index] = velocity;
            Accelerations[index] = Vector3.zero;
        }
    }
}