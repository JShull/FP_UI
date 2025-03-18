using UnityEngine;
using UnityEngine.EventSystems;

namespace FuzzPhyte.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class FPUI_HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool HoverEnabled = true;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!HoverEnabled) return;
            FPUI_DragDropManager.Instance.OnHoverEnter.Invoke(GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(!HoverEnabled) return;
            FPUI_DragDropManager.Instance.OnHoverExit.Invoke(GetComponent<RectTransform>());
        }
    }
}
