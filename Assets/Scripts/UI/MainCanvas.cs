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
        }

        void Start()
        {
            mainCamera = Camera.main;;
            ShowInteractButton(new Vector3(0, 2, 10), -1);
        }

        #endregion

        #region Public

        public void ShowInteractButton(Vector3 pos, int entityId)
        {
            var button = Instantiate(bubblePrefab);
            button.transform.SetParent(transform);
            
            Vector2 viewportPoint = mainCamera.WorldToScreenPoint(pos);
            button.Show(viewportPoint, entityId);
        }

        public void ButtonClicked(int entityId)
        {
            Debug.LogError(entityId);
        }

        #endregion
    }
}