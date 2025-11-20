namespace Sample.Scripts
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    namespace GServer.Utils.Extensions
    {
        public static class AnimationExtensions
        {
            public static async UniTask StopThenPlayAndWaitSafe(this Animation controller, AnimationClip clip,
                CancellationToken cancellationToken, bool shouldSendEndFrameIfCancelled = false)
            {
                if (controller == null)
                {
                    return;
                }

                controller.Stop();
                await PlayAndWaitSafe(controller, clip, cancellationToken, shouldSendEndFrameIfCancelled);
            }


            public static void JumpToFinalFrame(this Animation controller, AnimationClip clip)
            {
                if (controller == null || clip == null)
                {
                    return;
                }

                controller.Stop();
                // Sample the animation at its end time
                clip.SampleAnimation(controller.gameObject, clip.length);
            }

            public static void JumpToFirstFrame(this Animation controller, AnimationClip clip)
            {
                if (controller == null || clip == null)
                {
                    return;
                }

                controller.Stop();
                // Sample the animation at its end time
                clip.SampleAnimation(controller.gameObject, 0);
            }

            public static void JumpToFinalFrameOfResetIfTargetIsPlaying(this Animation controller, AnimationClip target,
                AnimationClip reset)
            {
                if (controller == null || reset == null || target == null)
                {
                    return;
                }

                if (!controller.isPlaying)
                {
                    return;
                }

                if (controller.clip != target)
                {
                    return;
                }

                controller.Stop();
                // Sample the animation at its end time
                reset.SampleAnimation(controller.gameObject, reset.length);
            }

            public static async UniTask PlayAndWaitSafe(this Animation controller, AnimationClip clip,
                CancellationToken cancellationToken, bool shouldSendEndFrameIfCancelled = false)
            {
                if (controller == null)
                {
                    return;
                }

                if (!controller.isActiveAndEnabled)
                {
                    return;
                }

                if (clip == null)
                {
                    return;
                }

                controller.clip = clip;
                if (clip.length == 0)
                {
                    // the animation has no length, so we just sample the first frame and return
                    clip.SampleAnimation(controller.gameObject, 0);
                    return;
                }

                controller.Play();
                await controller.WaitAnimation(clip, cancellationToken, shouldSendEndFrameIfCancelled);
            }

            public static async UniTask PlayBackwardCurrentAnimation(this Animation controller,
                CancellationToken cancellationToken, bool shouldSendFirstFrameIfCancelled = false)
            {
                string clipName = controller.clip.name;
                AnimationState state = controller[clipName];
                float oldSpeed = state.speed;

                try
                {
                    state.speed *= -1;
                    await UniTask.WaitWhile(state, s => s.enabled, cancellationToken: cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    if (shouldSendFirstFrameIfCancelled && controller != null && controller.clip != null)
                    {
                        controller.Stop();
                        // Sample the animation at its first frame
                        controller.clip.SampleAnimation(controller.gameObject, 0);
                    }

                    throw;
                }
                catch (Exception e)
                {
                    //Life-cycle error
                    Debug.LogWarning("Animation error:" + e.Message);
                }
                finally
                {
                    if (controller != null)
                    {
                        //it is possible that object is destroyed before animation is finished
                        state.speed = oldSpeed;
                    }
                }
            }

            public static async UniTask WaitCurrentAnimation(this Animation controller,
                CancellationToken cancellationToken,
                bool shouldSendEndFrameIfCancelled = false)
            {
                if (!controller.isPlaying)
                {
                    return;
                }

                if (controller.clip == null)
                {
                    return;
                }

                await WaitAnimation(controller, controller.clip, cancellationToken, shouldSendEndFrameIfCancelled);
            }

            private static async UniTask WaitAnimation(this Animation controller, AnimationClip clip,
                CancellationToken cancellationToken, bool shouldSendEndFrameIfCancelled = false)
            {
                AnimationState state = controller[clip.name];
                try
                {
                    if (state.enabled)
                    {
                        await UniTask.WaitWhile(state, s => s.enabled, cancellationToken: cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (shouldSendEndFrameIfCancelled)
                    {
                        controller.JumpToFinalFrame(clip);
                    }

                    throw;
                }
                catch (Exception e)
                {
                    //Life-cycle error
                    Debug.LogWarning("Animation error 2:" + e.Message);
                }
                finally
                {
                    if (controller != null && state.enabled)
                    {
                        controller.Stop(state.name);
                    }
                }
            }
        }
    }
}