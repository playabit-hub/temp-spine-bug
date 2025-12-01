/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated April 5, 2025. Replaces all prior versions.
 *
 * Copyright (c) 2013-2025, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software
 * or otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THE SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using UnityEngine;

namespace Spine.Unity
{
    // To use this example component, add it to your SkeletonAnimation Spine GameObject.
    // This component will disable that SkeletonAnimation component to prevent it from calling its own Update and LateUpdate methods.

    [DisallowMultipleComponent]
    public sealed class ManualSkeletonAnimationPlayer : MonoBehaviour
    {
        #region Inspector

        public SkeletonAnimation skeletonAnimation;

        [HideInInspector] public float frameDeltaTime = 1 / 60f;

        [Header("Advanced")]
        [Tooltip(
            "The maximum number of fixed timesteps. If the game framerate drops below the If the framerate is consistently faster than the limited frames, this does nothing.")]
        public int maxFrameSkip = 4;

        [Tooltip(
            "If enabled, the Skeleton mesh will be updated only on the same frame when the animation and skeleton are updated. Disable this or call SkeletonAnimation.LateUpdate yourself if you are modifying the Skeleton using other components that don't run in the same fixed timestep.")]
        public bool frameskipMeshUpdate = true;

        [HideInInspector] public float timeOffset = 1 / 120f;

        #endregion

        float accumulatedTime = 0;
        bool requiresNewMesh;
		float lastTime = -1;
		float lastAnimTime = 0;
		float lastTrackTime = 0;

		void OnValidate()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            if (frameDeltaTime <= 0) frameDeltaTime = 1 / 60f;
            if (maxFrameSkip < 1) maxFrameSkip = 1;
        }

        void OnEnable()
        {
            requiresNewMesh = true;
            accumulatedTime = 0;

            if (skeletonAnimation.enabled)
                skeletonAnimation.enabled = false;
            skeletonAnimation.Update(timeOffset);
        }

        void Update()
        {
            if (lastTime == Time.time) {
				return;
			}
			lastTime = Time.time;

            accumulatedTime += Time.deltaTime;

            float frames = 0;
            while (accumulatedTime >= frameDeltaTime)
            {
                frames++;
                if (frames > maxFrameSkip) break;
                accumulatedTime -= frameDeltaTime;
            }

            if (frames > 0)
            {
                skeletonAnimation.Update(frames * frameDeltaTime);
                // Try-2 skeletonAnimation.Update(frames * frameDeltaTime + timeOffset);
                requiresNewMesh = true;
            }

            TrackEntry trackEntry = skeletonAnimation.AnimationState.Tracks.Items[0];
			float animationTime = trackEntry.AnimationTime;
			float trackTime = trackEntry.TrackTime;
            float deltaAnimTime = animationTime - lastAnimTime;
			float deltaTrackTime = trackTime - lastTrackTime;

            Debug.Log(string.Format("Unity Time: {0}, delta|FPS: {1}|{2}. Frames: {3}, animT|trackT: {4}|{5}. dAnimT|dTrackT{6}|{7} FrameNo:{8}",
                Time.time, Time.deltaTime, 1f / Time.deltaTime, frames, animationTime, trackTime, deltaAnimTime, deltaTrackTime, Time.frameCount));
		}

		void LateUpdate()
        {
            if (frameskipMeshUpdate && !requiresNewMesh) return;

            skeletonAnimation.Renderer.LateUpdate();
            requiresNewMesh = false;
        }
    }
}