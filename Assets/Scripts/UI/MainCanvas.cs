using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace RandomName.UI
{
    public class MainCanvas : MonoBehaviour
    {
        #region Static

        public static MainCanvas I { get; private set; }

        #endregion
        
        public InteractButton bubblePrefab;

        public RectTransform progressImage;
        public RectTransform progressIncreaseImage;
        public RectTransform progressDecreaseImage;

        private Camera mainCamera;

        private IEnumerator progressAnimation;
        public AnimationCurve progressAnimationCurve;
        public float progressDuration;

        public CanvasGroup winGroup;
        public CanvasGroup loseGroup;

        [Serializable]
        private class ButtonWithEntity
        {
            public Entity Entity;
            public InteractButton Button;
        }

        private List<ButtonWithEntity> buttons = new List<ButtonWithEntity>();

        private float progressMaxWidth;

        #region Mono

        private void Awake()
        {
            I = this;
            
            // just to pre-fetch it
            var button = Instantiate(bubblePrefab);
            Destroy(button.gameObject);

            progressMaxWidth = 790f;
            progressImage.sizeDelta = new Vector2(0f, progressImage.sizeDelta.y);
        }

        void Start()
        {
            mainCamera = Camera.main;
        }

        #endregion

        #region Public

        public void EndGame(bool isWon)
        {
            winGroup.alpha = isWon ? 1f : 0;
            loseGroup.alpha = isWon ? 0f : 1f;
        }

        public void RefreshProgressBarToEnd()
        {
            if (progressAnimation != null)
            {
                StopCoroutine(progressAnimation);
            }
            progressAnimation = ProgressAnimation(progressMaxWidth, progressDuration, () =>
            {
                progressImage.sizeDelta = new Vector2(0f, progressImage.sizeDelta.y);
            }, 0.4f);
            StartCoroutine(progressAnimation);
        }

        public void RefreshProgressBar(float progress)
        {
            if (progressAnimation != null)
            {
                StopCoroutine(progressAnimation);
            }
            progressAnimation = ProgressAnimation(progress * progressMaxWidth, progressDuration);
            StartCoroutine(progressAnimation);
        }

        public void ShowInteractButton(Vector3 entityPos, Entity entity, int level)
        {
            var button = Instantiate(bubblePrefab);
            button.transform.SetParent(transform);

            var offset = GameSettings.I.GetInteractionOffsetPerLevel(level);
            var pos = entityPos + Vector3.up * offset;
            Vector2 viewportPoint = mainCamera.WorldToScreenPoint(pos);
            button.Show(viewportPoint, entity);
            
            buttons.Add(new ButtonWithEntity()
            {
                Entity = entity,
                Button = button,
            });
        }

        public void ClearButton(Entity entity, bool isSuccess = false)
        {
            var index = buttons.FindIndex(b => b.Entity == entity);
            if (index == -1)
            {
                Debug.LogError("NO BUTTON WITH INDEX");
                return;
            }
            buttons[index].Button.Die(isSuccess);
            buttons.RemoveAt(index);
        }

        public void ClearAllInteractible(bool instantClear = false)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Button.Die(instantClear: instantClear);
            }
            buttons.Clear();
        }

        public void ButtonClicked(Entity entity)
        {
            var index = buttons.FindIndex(b => b.Entity == entity);
            if (index == -1)
            {
                Debug.LogError("NO BUTTON WITH INDEX");
                return;
            }
            GameController.I.FanInteracted(entity);
        }

        #endregion

        #region Private

        private IEnumerator ProgressAnimation(float endWidth, float duration, Action callBack = null, float callbackDelay = 0f)
        {
            var size = progressImage.sizeDelta;
            var startWidth = size.x;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
            {
                float curvePercent = progressAnimationCurve.Evaluate(t);
                size.x = Mathf.LerpUnclamped(startWidth, endWidth, curvePercent);
                progressImage.sizeDelta = size;
                yield return null;
            }

            if (callbackDelay > 0)
            {
                yield return new WaitForSecondsRealtime(callbackDelay);
            }
            callBack?.Invoke();
        }

        #endregion
    }
}