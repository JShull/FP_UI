namespace FuzzPhyte.UI
{
    using UnityEngine.UI;
    using UnityEngine;
    [RequireComponent(typeof(RectTransform))]
    public class FPUI_VisualStateHandler : MonoBehaviour, IUIDragState
    {
        [Tooltip("If we want to override whatever our image starts with")]
        public bool OverwriteStartingColor = false;
        public Color defaultColor = Color.white;
        public Color draggingColor = Color.green;
        public Color hoverColor = Color.yellow;
        
        [SerializeField]
        protected Image image;

        void Awake()
        {
            Setup(defaultColor,draggingColor,hoverColor);
        }
        public void Setup(Color defaultColor, Color DragColor, Color HoverColor)
        {
            this.draggingColor = DragColor;
            this.hoverColor = HoverColor;
            if (image == null)
            {
                image = GetComponent<Image>();
                if (image == null)
                {
                    Debug.LogError("FPUI_VisualStateHandler needs an Image component.");
                }
            }
            if (OverwriteStartingColor)
            {
                this.defaultColor = defaultColor;
            }
            else
            {
                //use original image color as default
                this.defaultColor = image.color;
            }
            image.color = defaultColor;
        }

        void OnEnable()
        {
            if(FPUI_DragDropManager.Instance == null)
            {
                Debug.LogError("FPUI_DragDropManager is not in the scene.");
                return;
            }
            FPUI_DragDropManager.Instance.OnPickUp.AddListener(HandlePickUp);
            FPUI_DragDropManager.Instance.OnDragging.AddListener(HandleDragging);
            FPUI_DragDropManager.Instance.OnRelease.AddListener(HandleRelease);
            FPUI_DragDropManager.Instance.OnHoverEnter.AddListener(HandleHoverEnter);
            FPUI_DragDropManager.Instance.OnHoverExit.AddListener(HandleHoverExit);
        }

        void OnDisable()
        {
            if (FPUI_DragDropManager.Instance == null)
            {
                Debug.LogError("FPUI_DragDropManager is not in the scene.");
                return;
            }
            FPUI_DragDropManager.Instance.OnPickUp.RemoveListener(HandlePickUp);
            FPUI_DragDropManager.Instance.OnDragging.RemoveListener(HandleDragging);
            FPUI_DragDropManager.Instance.OnRelease.RemoveListener(HandleRelease);
            FPUI_DragDropManager.Instance.OnHoverEnter.RemoveListener(HandleHoverEnter);
            FPUI_DragDropManager.Instance.OnHoverExit.RemoveListener(HandleHoverExit);
        }

        // Interface implementations
        public void OnDragStarted() => image.color = draggingColor;
        public void OnDragging() { /* Optional continuous updates */ }
        public void OnDragEnded() => image.color = defaultColor;
        public void OnHoverEnter() => image.color = hoverColor;
        public void OnHoverExit() => image.color = defaultColor;

        // Event handlers
        private void HandlePickUp(RectTransform rt)
        {
            if (rt == transform)
                OnDragStarted();
        }

        private void HandleDragging(RectTransform rt)
        {
            if (rt == transform)
                OnDragging();
        }

        private void HandleRelease(RectTransform rt)
        {
            if (rt == transform)
                OnDragEnded();
        }

        private void HandleHoverEnter(RectTransform rt)
        {
            if (rt == transform)
                OnHoverEnter();
        }

        private void HandleHoverExit(RectTransform rt)
        {
            if (rt == transform)
                OnHoverExit();
        }
    }
}
