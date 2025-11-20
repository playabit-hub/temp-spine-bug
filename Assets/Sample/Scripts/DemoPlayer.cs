using Cysharp.Threading.Tasks;
using UnityEngine;

public class DemoPlayer : MonoBehaviour
{
    public GameObject fakePool;
    public FakePropellerItem fakeItemFromPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Loop().Forget();
    }

    private async UniTask Loop()
    {
        while (true)
        {
            //Simulate get from pool
            fakeItemFromPool.transform.parent = null;
            await fakeItemFromPool.PlayCreationClip(destroyCancellationToken);
            //Mimic static view
            await UniTask.Delay(2000);
            //Mimic removal
            fakeItemFromPool.transform.parent = fakePool.transform;
            fakeItemFromPool.OnBackToPool();
            await UniTask.Delay(1000);
        }
    }
}