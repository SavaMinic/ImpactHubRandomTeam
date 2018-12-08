using System;
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

        private Camera mainCamera;

        [Serializable]
        private class ButtonWithEntity
        {
            public Entity Entity;
            public InteractButton Button;
        }

        private List<ButtonWithEntity> buttons = new List<ButtonWithEntity>();

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

        public void ClearButton(Entity entity)
        {
            var index = buttons.FindIndex(b => b.Entity == entity);
            if (index == -1)
            {
                Debug.LogError("NO BUTTON WITH INDEX");
                return;
            }
            buttons[index].Button.Die();
            buttons.RemoveAt(index);
        }

        public void ClearAllInteractible()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Button.Die();
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
            buttons[index].Button.Die();
            buttons.RemoveAt(index);
            GameController.I.FanInteracted(entity);
        }

        #endregion
    }
}