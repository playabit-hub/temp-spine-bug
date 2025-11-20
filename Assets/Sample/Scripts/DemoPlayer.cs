using Cysharp.Threading.Tasks;
using UnityEngine;

public class DemoPlayer : MonoBehaviour
{
    public GameObject fakePool;
    public FakePropellerItem fakeItem1FromPool;
    public FakePropellerItem fakeItem2FromPool;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async UniTask Start()
    {
        Loop(fakeItem1FromPool).Forget();
        await UniTask.Delay(500);
        Loop(fakeItem2FromPool).Forget();
    }


    private async UniTask Loop(FakePropellerItem fakePropellerItem)
    {
        while (true)
        {
            //Simulate get from pool
            fakePropellerItem.transform.parent = null;
            await fakePropellerItem.PlayCreationClip(destroyCancellationToken);

            await UniTask.Delay(1000);
            //Mimic removal
            fakePropellerItem.transform.parent = fakePool.transform;
            fakePropellerItem.OnBackToPool();
            await UniTask.Delay(1000);
        }
    }
}