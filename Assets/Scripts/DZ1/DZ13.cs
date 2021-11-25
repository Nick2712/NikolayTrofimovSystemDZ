using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace NikolayTrofimovDZ1
{
    public class DZ13 : MonoBehaviour
    {
        private CancellationTokenSource _cancellationSource;

        private async void Start()
        {
            _cancellationSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationSource.Token;

            Task<bool> task1 = Unit1Async(cancellationToken);
            Task<bool> task2 = Unit2Async(cancellationToken);

            var result = await WhatTaskFasterAsync(cancellationToken, task1, task2);
            Debug.Log($"результат выполнения {nameof(WhatTaskFasterAsync)}: {result}");
        }

        private async Task<bool> Unit1Async(CancellationToken cancellationToken)
        {
            Debug.Log("первая задача запустилась");
            for (int i = 0; i < 50; i++)
            {
                await Task.Delay(20);
                if(cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("первая задача прервана токеном");
                    return false;
                }
            }
            Debug.Log("первая задача завершилась!");
            return true;
        }

        private async Task<bool> Unit2Async(CancellationToken cancellationToken)
        {
            Debug.Log("вторая задача запустилась");
            for (int i = 0; i < 60; i++)
            {
                await Task.Yield();
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("вторая задача прервана токеном");
                    return false;
                }
            }
            Debug.Log("вторая задача завершилась!");
            return false;
        }

        public async Task<bool> WhatTaskFasterAsync(CancellationToken ct, Task<bool> task1, Task<bool> task2)
        {
            Task<bool> fasterTask = await Task.WhenAny(task1, task2);
            _cancellationSource.Cancel();
            return fasterTask.Result;
        }
    }
}