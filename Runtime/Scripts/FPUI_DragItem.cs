namespace FuzzPhyte.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    public class FPUI_DragItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        protected RectTransform rectTransform;
        [SerializeField]
        protected FPUI_Moveable moveable;
        [SerializeField]
        protected bool error=false;
        public bool DragEnabled = true;
        [SerializeField]
        protected bool beingDragged = false;
        public UnityEvent OnMouseDownEvent;
        public UnityEvent OnMouseUpEvent;

        void Awake()
        {
            if (moveable == null)
            {
                moveable = GetComponent<FPUI_Moveable>();
                if (moveable == null)
                {
                    Debug.LogError("FPUI_DragItem needs a FPUI_Moveable component.");
                    error = true;
                    return;
                }
                else
                {
                    error = false;
                }
            } else
            {
                error = false;
            }
            //now check our recttransform
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
                if (rectTransform == null)
                {
                    Debug.LogError("FPUI_DragItem needs a RectTransform.");
                    error = true;
                    return;
                }
                else
                {
                    error = false;
                }
            }
            else
            {
                error = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (error) return;
            if (!DragEnabled) return;
            if (beingDragged) return;
            Vector2 localPointerPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPosition);
            //get my width of the rect transform in pixels
            var width = rectTransform.rect.width;
            FPUI_DragDropManager.Instance.BeginDrag(eventData, rectTransform, localPointerPosition, width);
            beingDragged = true;
            OnMouseDownEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (error) return;
            if (!DragEnabled) return;
            FPUI_DragDropManager.Instance.EndDrag(eventData);
            beingDragged = false;
            OnMouseUpEvent?.Invoke();
        }
    }
}
