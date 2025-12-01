using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Scripts.GServer.Utils.Extensions;
using Spine.Unity;
using UnityEngine;

namespace Sample.Scripts
{
    public class FakePropellerItem : MonoBehaviour
    {
        [SerializeField] protected Animation animator;

        [SerializeField] internal AnimationClip creationAnimationClip;
        [SerializeField] internal AnimationClip resetAnimationClip;

        [SerializeField] internal SkeletonAnimation skeletonAnimation;
        [SerializeField] internal AnimationReferenceAsset creationSpineReference;


        public async UniTask PlayCreationClip(CancellationToken token)
        {
            await animator.PlayWithSpineAnimation(
                creationAnimationClip, skeletonAnimation, creationSpineReference, token, destroyCancellationToken
            );
        }

        public void OnBackToPool()
        {
            ResetAnimation();
        }

        private void ResetAnimation()
        {
            animator.Stop();
            animator.JumpToFinalFrame(resetAnimationClip);
            skeletonAnimation.ClearStateSafe();
        }


    }
}