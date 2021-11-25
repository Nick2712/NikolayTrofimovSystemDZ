using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace NikolayTrofimovDZ1
{
    public class DZ12 : MonoBehaviour
    {
        private void Start()
        {
            CancellationTokenSource _cancellationSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationSource.Token;

            Task task1 = Task.Run(() => Unit1Async(cancellationToken));
            Task task2 = Task.Run(() => Unit2Async(cancellationToken));
        }

        private async Task Unit1Async(CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            Debug.Log("первая задача завершилась!");
        }

        private async Task Unit2Async(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 60; i++)
            {
                await Task.Yield();
            }
            Debug.Log("вторая задача завершилась!");
        }
    }
}