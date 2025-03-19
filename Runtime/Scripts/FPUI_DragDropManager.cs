namespace FuzzPhyte.UI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class FPUI_DragDropManager : MonoBehaviour
    {
        public static FPUI_DragDropManager Instance { get; private set; }
        [SerializeField]
        protected RectTransform currentDragItem;
        [SerializeField]
        [Tooltip("Main Canvas of interest")]
        protected Canvas parentCanvas;
        [Space]
        [Header("Input Type")]
        [Tooltip("Using the Mouse")]
        public bool UseMouse=true;
        [Tooltip("If we are using a VR OVR Cursor")]
        public bool UseVRCursor = false;
        public GameObject VRCursorRef;
        [Space]
        [Header("Bounds Checking")]
        public bool KeepInBounds;
        [Tooltip("if we want to to manage it at the canvas level")]
        public bool CanvasBounds;
        [Tooltip("if we want to manage it at the screen level")]
        public bool ScreenBounds;
        [SerializeField]
        protected Vector3 cursorPos;
        // UI Events for listeners
        public UnityEvent<RectTransform> OnPickUp = new UnityEvent<RectTransform>();
        public UnityEvent<RectTransform> OnDragging = new UnityEvent<RectTransform>();
        public UnityEvent<RectTransform> OnRelease = new UnityEvent<RectTransform>();
        public UnityEvent<RectTransform> OnHoverEnter = new UnityEvent<RectTransform>();
        public UnityEvent<RectTransform> OnHoverExit = new UnityEvent<RectTransform>();

        /// <summary>
        /// holds the offset from the mouse pointer to the item
        /// </summary>
        protected Vector2 offset;
        /// <summary>
        /// holds the pixel radius to keep the item within the bounds
        /// </summary>
        protected float pixelRadius = 10;

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //Debug.LogWarning($"FPUI_DragDropManager instance set to {this.name}");
                if (UseMouse && UseVRCursor)
                {
                    Debug.LogError($"Can't use both, make a choice!");
                    UseVRCursor = false;
                }
                if (parentCanvas == null)
                {
                    parentCanvas = GetComponentInParent<Canvas>();
                    if (parentCanvas == null)
                    {
                        Debug.LogError("FPUI_DragDropManager needs to be under a Canvas.");
                    }
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
            cursorPos = Vector3.zero;
        }

        public virtual void UpdateCanvas(Canvas canvas)
        {
            parentCanvas = canvas;
        }
        public virtual void Update()
        {
            if (currentDragItem != null)
            {
                Vector2 movePos;
                
                if (UseMouse)
                {
                    cursorPos = Input.mousePosition;
                }
                else
                {
                    if (UseVRCursor)
                    {
                        cursorPos=ConvertWorldObjectToCanvasPixelLocation(VRCursorRef);
                    }
                }
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform,
                    cursorPos,
                    parentCanvas.worldCamera,
                    out movePos);
                currentDragItem.position = parentCanvas.transform.TransformPoint(movePos - offset);
                
                OnDragging?.Invoke(currentDragItem);
                //check if we are out of the screen bounds
                if (KeepInBounds)
                {
                    BoundsCheck();
                }
            }
        }
        /// <summary>
        /// Designed to take a 3D cursor that's basically hovering right over the canvas and do a quick canvas conversion to get the pixel location
        /// needs to know the normal to the canvas to do this
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 ConvertWorldObjectToCanvasPixelLocation(GameObject cursorWorld)
        {
            return parentCanvas.worldCamera.WorldToScreenPoint(cursorWorld.transform.position, Camera.MonoOrStereoscopicEye.Right);
        }
        protected virtual void BoundsCheck()
        {
            ///establish corners given current canvas size - it might be live adusting so we need to keep getting this
            Vector2 lowerBounds=Vector2.zero;
            Vector2 upperBounds=Vector2.zero;
            Vector2 areaCenter=Vector2.zero;
            Vector2 currentPosition = currentDragItem.position;

            if (CanvasBounds)
            {
                RectTransform canvasRectTransform = parentCanvas.transform as RectTransform;
                Vector3[] canvasCorners = new Vector3[4];
                canvasRectTransform.GetWorldCorners(canvasCorners);
                lowerBounds = canvasCorners[0];
                upperBounds = canvasCorners[2];
                areaCenter = (lowerBounds + upperBounds) / 2f;
            }
            if (ScreenBounds)
            {
                lowerBounds = new Vector2(0, 0);
                upperBounds = new Vector2(Screen.width, Screen.height);
                areaCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            }
            

            if (currentDragItem.position.x < lowerBounds.x || currentDragItem.position.y < lowerBounds.y ||
                    currentDragItem.position.x > upperBounds.x || currentDragItem.position.y > upperBounds.y)
            {

                //bump it back out towards the center of the canvas by some amount
                //2D vector back to center to calculate direction to move along 
                Vector2 direction = (currentPosition - areaCenter).normalized;

                // Move the item back towards the center by a fixed amount of pixels
                Vector2 newPosition = currentPosition - direction * pixelRadius;

                // Ensure the new position is within screen bounds
                newPosition.x = Mathf.Clamp(newPosition.x, lowerBounds.x, upperBounds.x);
                newPosition.y = Mathf.Clamp(newPosition.y, lowerBounds.x, upperBounds.y);
                currentDragItem.position = newPosition;
                
                EndDrag();
            }
        }
        public void BeginDrag(RectTransform item, Vector2 pointerOffset, float pixelSize=50)
        {
            currentDragItem = item;
            pixelRadius = pixelSize;
            offset = pointerOffset;
            OnPickUp?.Invoke(item);
        }

        public void EndDrag()
        {
            if (currentDragItem != null)
            {
                OnRelease?.Invoke(currentDragItem);
                currentDragItem = null;
            }
        }
    }
}
