using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace Experiment
{
    public class OtherObject : MonoBehaviour
    {
        private NativeArray<Vector3> _array;
        private JobHandle _handle;
        private int _counter;

        private void Start()
        {
            _array = new NativeArray<Vector3>(1000, Allocator.TempJob);
            AdvancedJob job = new AdvancedJob();
            job.Array = _array;

            Debug.Log("Перед хэндлером");
            _handle = job.Schedule(100, 5);
            _handle.Complete();

            Debug.Log("после хэндлера");

            _counter = 0;
            StartCoroutine(JobCoroutine());
        }

        private IEnumerator JobCoroutine()
        {
            while(_handle.IsCompleted == false)
            {
                _counter++;
                Debug.Log($"<color=green>жыдём кадр №{_counter}</color>");
                yield return new WaitForEndOfFrame();
            }

            foreach(Vector3 vector in _array)
            {
                print(vector);
            }

            _array.Dispose();
        }
    }

    public struct AdvancedJob : IJobParallelFor
    {
        public NativeArray<Vector3> Array;
        
        public void Execute(int index)
        {
            Array[index] = Array[index].normalized;
            Debug.Log($"<color=blue>выполняется джоба {index}</color>");
            for(int i = 0; i < 1000; i++)
            {
                Debug.Log("гы");
            }
        }
    }
}