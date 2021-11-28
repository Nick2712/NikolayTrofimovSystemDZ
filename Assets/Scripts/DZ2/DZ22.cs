using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace NikolayTrofimovDZ2
{
    public class DZ22 : MonoBehaviour
    {
        private void Start()
        {
            var positions = new NativeArray<Vector3>(10, Allocator.TempJob);
            var velocities = new NativeArray<Vector3>(10, Allocator.TempJob);
            var finalPositions = new NativeArray<Vector3>(10, Allocator.TempJob);

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = Random.insideUnitSphere * Random.Range(0, 10);
                velocities[i] = Random.insideUnitSphere * Random.Range(0, 10);
                Debug.Log($"position[{i}] = {positions[i]}");
                Debug.Log($"velocity[{i}] = {velocities[i]}");
            }

            SomeParalelJob job = new SomeParalelJob()
            {
                Positions = positions,
                Velocities = velocities,
                FinalPositions = finalPositions
            };

            JobHandle handle = job.Schedule(10, 0);
            handle.Complete();

            for(int i = 0; i < finalPositions.Length; i++)
            {
                Debug.Log($"finalPosition[{i}] = {finalPositions[i]}");
            }

            positions.Dispose();
            velocities.Dispose();
            finalPositions.Dispose();
        }
    }

    public struct SomeParalelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> Positions;
        [ReadOnly] public NativeArray<Vector3> Velocities;
        [WriteOnly] public NativeArray<Vector3> FinalPositions;

        public void Execute(int index)
        {
            FinalPositions[index] = Positions[index] + Velocities[index];
        }
    }
}