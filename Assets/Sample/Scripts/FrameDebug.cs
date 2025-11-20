using TMPro;
using UnityEngine;

namespace Sample.Scripts
{
    public class FrameDebug : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] sprites;
        [SerializeField] private TextMeshPro frameNo;
        private int _length;

        public void Awake()
        {
            _length = sprites.Length;
        }

        void Update()
        {
            var frameCount = Time.frameCount;
            int frame = frameCount % _length;
            frameNo.text = frameCount.ToString();
            for (var index = 0; index < sprites.Length; index++)
            {
                var sprite = sprites[index];
                sprite.enabled = frame == index;
            }
        }
    }
}