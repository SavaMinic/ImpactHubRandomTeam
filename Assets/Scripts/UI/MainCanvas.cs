using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RandomName.UI
{
    public class MainCanvas : MonoBehaviour
    {
        #region Static

        public static MainCanvas I { get; private set; }

        #endregion
        
        public InteractButton bubblePrefab;

        public Image backImage;
        public RectTransform progressImage;
        public RectTransform progressIncreaseImage;
        public RectTransform progressDecreaseImage;
        
        public GameObject progressImageObject;

        private Camera mainCamera;

        private IEnumerator progressAnimation;
        public AnimationCurve progressAnimationCurve;
        public float progressDuration;

        public CanvasGroup winGroup;
        public CanvasGroup loseGroup;

        public Button skipButton;
        public Button cameraButton;
        public Button danceButton;
        public Button exitButton;

        public Text FPSCounter;

        [Serializable]
        private class ButtonWithEntity
        {
            public Entity Entity;
            public InteractButton Button;
        }

        private List<ButtonWithEntity> buttons = new List<ButtonWithEntity>();

        private float progressMaxWidth;

        private bool isDanceMode;

        #region Mono

        private void Awake()
        {
            I = this;
            
            // just to pre-fetch it
            var button = Instantiate(bubblePrefab);
            Destroy(button.gameObject);

            progressMaxWidth = 790f;
            progressImage.sizeDelta = new Vector2(0f, progressImage.sizeDelta.y);

            var isDemo = GameSettings.I.DemoMode;
            skipButton.GetComponent<CanvasGroup>().alpha = isDemo ? 1f : 0.05f;
            skipButton.onClick.AddListener(() =>
            {
                GameController.I.SkipLevel();
            });
            
            cameraButton.gameObject.SetActive(isDemo);
            cameraButton.onClick.AddListener(() =>
            {
                CameraController.I.ToggleNextCamera();
            });
            
            danceButton.gameObject.SetActive(isDemo);
            danceButton.onClick.AddListener(() =>
            {
                isDanceMode = !isDanceMode;
                GameController.I.SetDanceMode(isDanceMode);
            });
            
            exitButton.gameObject.SetActive(isDemo);
            exitButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenu");
            });
            
            progressImageObject.gameObject.SetActive(!isDemo);
        }

        void Start()
        {
            mainCamera = Camera.main;
        }


        private void Update()
        {
            FPSCounter.text = (1 / Time.deltaTime).ToString("##");
        }

        #endregion

        #region Public

        public void EndGame(bool isWon)
        {
            if (!GameSettings.I.DemoMode)
            {
                winGroup.alpha = isWon ? 1f : 0;
                loseGroup.alpha = isWon ? 0f : 1f;
            }
            ClearAllInteractible(true);
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

        public void RefreshConsecutiveErrorCount(int errorCnt)
        {
            var errors = (float)errorCnt / GameSettings.I.MaxConsecutiveErrors;
            backImage.color = Color.Lerp(Color.white, Color.red, errors);
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