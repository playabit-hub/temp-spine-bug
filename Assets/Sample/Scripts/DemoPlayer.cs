using Cysharp.Threading.Tasks;
using UnityEngine;

public class DemoPlayer : MonoBehaviour
{
    public GameObject fakePool;
    public FakePropellerItem fakeItemFromPool;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

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

            await UniTask.Delay(3000);
            //Mimic removal
            fakeItemFromPool.transform.parent = fakePool.transform;
            fakeItemFromPool.OnBackToPool();
            await UniTask.Delay(3000);
        }
    }
}