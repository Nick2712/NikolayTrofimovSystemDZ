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
            Debug.Log($"��������� ���������� {nameof(WhatTaskFasterAsync)}: {result}");
        }

        private async Task<bool> Unit1Async(CancellationToken cancellationToken)
        {
            Debug.Log("������ ������ �����������");
            for (int i = 0; i < 50; i++)
            {
                await Task.Delay(20);
                if(cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("������ ������ �������� �������");
                    return false;
                }
            }
            Debug.Log("������ ������ �����������!");
            return true;
        }

        private async Task<bool> Unit2Async(CancellationToken cancellationToken)
        {
            Debug.Log("������ ������ �����������");
            for (int i = 0; i < 60; i++)
            {
                await Task.Yield();
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("������ ������ �������� �������");
                    return false;
                }
            }
            Debug.Log("������ ������ �����������!");
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