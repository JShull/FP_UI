namespace FuzzPhyte.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class FPUI_DragItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        protected RectTransform rectTransform;
        [SerializeField]
        protected FPUI_Moveable moveable;
        [SerializeField]
        protected bool error=false;

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
            Vector2 localPointerPosition;
            Debug.Log($"On Pointer Down!");
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPosition);
            //get my width of the rect transform in pixels
            var width = rectTransform.rect.width;
            FPUI_DragDropManager.Instance.BeginDrag(rectTransform, localPointerPosition, width);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log($"On pointer up");
            if(error) return;
            FPUI_DragDropManager.Instance.EndDrag();
        }
    }
}
