using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TaallamGame.Dialogue
{
    public class PortraitController : MonoBehaviour
    {
        [SerializeField] private Image portraitImage;          // assign your PortraitFrame Image
        [SerializeField] private PortraitLibrary library;      // assign the PortraitLibrary asset
        [SerializeField] private float crossfadeDuration = 0.15f;
        [SerializeField] private Sprite fallbackSprite;

        private Coroutine _fadeCo;

        public void SetPortrait(string id)
        {
            if (portraitImage == null || library == null) return;

            if (!library.TryGet(id, out var sprite))
                sprite = fallbackSprite;

            if (crossfadeDuration <= 0f)
            {
                portraitImage.sprite = sprite;
                portraitImage.color = new Color(1, 1, 1, 1);
                return;
            }

            if (_fadeCo != null) StopCoroutine(_fadeCo);
            _fadeCo = StartCoroutine(FadeSwap(sprite, crossfadeDuration));
        }

        private IEnumerator FadeSwap(Sprite next, float duration)
        {
            var img = portraitImage;
            var c = img.color;

            // fade out
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Lerp(1f, 0f, t / duration);
                img.color = new Color(c.r, c.g, c.b, a);
                yield return null;
            }

            img.sprite = next;

            // fade in
            t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Lerp(0f, 1f, t / duration);
                img.color = new Color(c.r, c.g, c.b, a);
                yield return null;
            }

            _fadeCo = null;
        }
    }
}