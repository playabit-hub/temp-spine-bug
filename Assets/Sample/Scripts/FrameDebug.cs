using UnityEngine;

namespace Sample.Scripts
{
    public class FrameDebug : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] sprites;
        private int _length;

        public void Awake()
        {
            _length = sprites.Length;
        }

        void Update()
        {
            int frame = Time.frameCount % _length;
            for (var index = 0; index < sprites.Length; index++)
            {
                var sprite = sprites[index];
                sprite.enabled = frame == index;
            }
        }
    }
}