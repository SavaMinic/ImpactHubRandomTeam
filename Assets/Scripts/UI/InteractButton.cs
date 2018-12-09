using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace RandomName.UI
{
    public class InteractButton : MonoBehaviour
    {

        private RectTransform myRectTransform;
        private CanvasGroup myCanvasGroup;
        private Button myButton;

        private Entity myEntity;

        private IEnumerator fadeAnim;
        private bool isAnimating;
        
        private void Awake()
        {
            myRectTransform = GetComponent<RectTransform>();
            myCanvasGroup = GetComponent<CanvasGroup>();
            myButton = GetComponent<Button>();

            myButton.onClick.AddListener(OnButtonClick);

            // hidden initially
            myCanvasGroup.alpha = 0f;
        }

        private void OnDestroy()
        {
            myButton.onClick.RemoveListener(OnButtonClick);
        }

        public void Show(Vector3 pos, Entity entity)
        {
            myEntity = entity;
            
            // TODO: fade in
            if (fadeAnim != null)
            {
                StopCoroutine(fadeAnim);
            }
            fadeAnim = FadeAnim(true, 0.4f);
            StartCoroutine(fadeAnim);

            transform.position = pos;
        }

        public void Die(bool success = false, bool instantClear = false)
        {
            if (isAnimating)
                return;
            
            if (instantClear)
            {
                Destroy(gameObject);
                return;
            }

            isAnimating = true;
            var colors = myButton.colors;
            colors.disabledColor = success ? Color.green : Color.red;
            myButton.colors = colors;
            myButton.interactable = false;
            if (fadeAnim != null)
            {
                StopCoroutine(fadeAnim);
            }
            fadeAnim = FadeAnim(false, 0.4f, () =>
            {
                Destroy(gameObject);
            });
            StartCoroutine(fadeAnim);
        }

        private void OnButtonClick()
        {
            // notify system
            MainCanvas.I.ButtonClicked(myEntity);
        }

        private IEnumerator FadeAnim(bool isFadeIn, float duration = 0.2f, Action callback = null)
        {
            var endAlpha = isFadeIn ? 1f : 0f;
            float alpha = myCanvasGroup.alpha;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
            {
                myCanvasGroup.alpha = Mathf.Lerp(alpha, endAlpha, t);
                if (!isFadeIn)
                {
                    myRectTransform.localScale = Vector3.one * (3f * t + 1f);
                }
                yield return null;
            }
            callback?.Invoke();
        }
    }
}