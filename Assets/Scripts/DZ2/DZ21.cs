using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace NikolayTrofimovDZ2
{
    public class DZ21 : MonoBehaviour
    {
        private void Start()
        {
            var array = new NativeArray<int>(20, Allocator.TempJob);
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = Random.Range(0, 20);
                Debug.Log(array[i]);
            }
            
            SomeJob job = new SomeJob()
            {
                SomeArray = array
            };

            JobHandle handle = job.Schedule();
            handle.Complete();

            Debug.Log("--------");
            foreach(var value in array)
            {
                Debug.Log(value);
            }

            array.Dispose();
        }
    }

    public struct SomeJob : IJob
    {
        public NativeArray<int> SomeArray;

        public void Execute()
        {
            for (int i = 0; i < SomeArray.Length; i++)
            {
                if (SomeArray[i] > 10)
                {
                    SomeArray[i] = 0;
                }
            }
        }
    }
}