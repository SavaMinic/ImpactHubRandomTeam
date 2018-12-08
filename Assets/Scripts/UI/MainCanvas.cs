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

        private Camera mainCamera;

        #region Mono

        private void Awake()
        {
            I = this;
            
            // just to pre-fetch it
            var button = Instantiate(bubblePrefab);
            Destroy(button.gameObject);
        }

        void Start()
        {
            mainCamera = Camera.main;
        }

        #endregion

        #region Public

        public void ShowInteractButton(Vector3 entityPos, Entity entity)
        {
            var button = Instantiate(bubblePrefab);
            button.transform.SetParent(transform);

            var pos = entityPos + Vector3.up * 1.8f;
            Vector2 viewportPoint = mainCamera.WorldToScreenPoint(pos);
            button.Show(viewportPoint, entity);
        }

        public void ClearAllInteractible()
        {
            var buttons = GetComponentsInChildren<InteractButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Die();
            }
        }

        public void ButtonClicked(Entity entity)
        {
            GameController.I.FanInteracted(entity);
        }

        #endregion
    }
}