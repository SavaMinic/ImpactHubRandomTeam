using UnityEngine;
using UnityEngine.UI;

namespace RandomName.UI
{
    public class InteractButton : MonoBehaviour
    {

        private RectTransform myRectTransform;
        private CanvasGroup myCanvasGroup;
        private Button myButton;

        private int myEntityId;
        
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

        public void Show(Vector3 pos, int entityId)
        {
            myEntityId = entityId;
            
            // TODO: fade in
            myCanvasGroup.alpha = 1f;

            transform.position = pos;
        }

        private void OnButtonClick()
        {
            // TODO: fade out
            // notify system
            MainCanvas.I.ButtonClicked(myEntityId);
            // destroy
            Destroy(gameObject);
        }
    }
}