namespace FuzzPhyte.UI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class FPUI_DragDropManager : MonoBehaviour
    {
        public static FPUI_DragDropManager Instance { get; private set; }

        // UI Events
        public UnityEvent<RectTransform> OnPickUp = new();
        public UnityEvent<RectTransform> OnDragging = new UnityEvent<RectTransform>();
        public UnityEvent<RectTransform> OnDrop = new UnityEvent<RectTransform>();
        public UnityEvent<RectTransform> OnRelease = new UnityEvent<RectTransform>();

        private RectTransform currentDragItem;
        private Canvas parentCanvas;
        private Vector2 offset;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                    Debug.LogError("FPUI_DragDropManager needs to be under a Canvas.");
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Update()
        {
            if (currentDragItem != null)
            {
                Vector2 movePos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform,
                    Input.mousePosition,
                    parentCanvas.worldCamera,
                    out movePos);
                currentDragItem.position = parentCanvas.transform.TransformPoint(movePos - offset);
                OnDragging.Invoke(currentDragItem);
            }
        }

        public void BeginDrag(RectTransform item, Vector2 pointerOffset)
        {
            currentDragItem = item;
            offset = pointerOffset;
            OnPickUp.Invoke(item);
        }

        public void EndDrag()
        {
            if (currentDragItem != null)
            {
                OnRelease.Invoke(currentDragItem);
                currentDragItem = null;
            }
        }
    }
}
