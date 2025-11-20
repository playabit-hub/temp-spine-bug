using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Scripts.GServer.Utils.Extensions;
using Spine;
using Spine.Unity;
using UnityEngine;
using Animation = UnityEngine.Animation;

namespace Sample.Scripts
{
    public static class SpineExtensions
    {
        public static void ClearStateSafe(this SkeletonAnimation skeletonAnimation)
        {
            if (skeletonAnimation != null && skeletonAnimation.IsValid)
            {
                skeletonAnimation.ClearState();
            }
        }

        public static UniTask PlayWithSpineAnimation(this Animation animation, AnimationClip animationClip,
            SkeletonAnimation skeletonAnimation, AnimationReferenceAsset animationReference,
            CancellationToken animationToken)
        {
            return animation.PlayWithSpineAnimation(animationClip, skeletonAnimation, animationReference,
                animationToken, animationToken);
        }

        public static async UniTask PlayWithSpineAnimation(this Animation animation, AnimationClip animationClip,
            SkeletonAnimation skeletonAnimation, AnimationReferenceAsset animationReference,
            CancellationToken animationToken, CancellationToken poolToken)
        {
            TrackEntry trackEntry = skeletonAnimation.SetAnimation(animationReference, false);

            try
            {
                await animation.StopThenPlayAndWaitSafe(animationClip, animationToken);
            }
            finally
            {
                if (animationToken.IsCancellationRequested
                    && !poolToken.IsCancellationRequested
                    && skeletonAnimation != null
                    && skeletonAnimation.IsValid)
                {
                    TrackEntry current = skeletonAnimation.AnimationState.GetCurrent(0);
                    if (current == trackEntry && current.Next == null)
                    {
                        skeletonAnimation.AnimationState.ClearTrack(0);
                    }
                }
            }

            await UniTask.WaitUntil(trackEntry, e => e.IsComplete, cancellationToken: animationToken);

        }

        public static async UniTask PlayAnimationAndWait(this SkeletonAnimation skeletonAnimation,
            AnimationReferenceAsset animationReference, bool isLoop, CancellationToken animationToken,
            CancellationToken poolToken = default)
        {
            TrackEntry trackEntry = skeletonAnimation.SetAnimation(animationReference, isLoop);
            try
            {
                await UniTask.WaitUntil(
                    trackEntry,
                    e => e.Animation == null || e.IsComplete,
                    cancellationToken: animationToken
                );
            }
            catch (OperationCanceledException)
            {
                if (animationToken.IsCancellationRequested
                    && !poolToken.IsCancellationRequested
                    && skeletonAnimation != null
                    && skeletonAnimation.IsValid)
                {
                    TrackEntry current = skeletonAnimation.AnimationState.GetCurrent(0);
                    if (current == trackEntry && current.Next == null)
                    {
                        skeletonAnimation.AnimationState.ClearTrack(0);
                    }
                }

                throw;
            }
        }

        public static TrackEntry SetAnimation(this SkeletonAnimation skeletonAnimation, Spine.Animation animation,
            bool isLoop)
        {
            return skeletonAnimation.AnimationState.SetAnimation(0, animation, isLoop);
        }

        public static TrackEntry AddAnimation(this SkeletonAnimation skeletonAnimation, Spine.Animation animation,
            bool isLoop)
        {
            return skeletonAnimation.AnimationState.AddAnimation(0, animation, isLoop, 0f);
        }
    }
}