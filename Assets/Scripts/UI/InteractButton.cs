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

        public void Die()
        {
            if (fadeAnim != null)
            {
                StopCoroutine(fadeAnim);
            }
            fadeAnim = FadeAnim(false, 0.2f, () =>
            {
                Destroy(gameObject);
            });
            StartCoroutine(fadeAnim);
        }

        private void OnButtonClick()
        {
            // TODO: fade out
            // notify system
            MainCanvas.I.ButtonClicked(myEntity);

            Die();
        }

        private IEnumerator FadeAnim(bool isFadeIn, float duration = 0.2f, Action callback = null)
        {
            var endAlpha = isFadeIn ? 1f : 0f;
            float alpha = myCanvasGroup.alpha;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
            {
                myCanvasGroup.alpha = Mathf.Lerp(alpha, endAlpha, t);
                yield return null;
            }
            callback?.Invoke();
        }
    }
}