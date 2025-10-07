namespace FuzzPhyte.UI
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class FPUI_MatchManager : MonoBehaviour
    {
        /// <summary>
        /// Make sure script execution order is set so this is behind FPUI_DragDropManager
        /// </summary>
        public static FPUI_MatchManager Instance { get; private set; }
        public UnityEvent<FPUI_MatchItem, FPUI_MatchTarget> OnMatchSuccess = new UnityEvent<FPUI_MatchItem, FPUI_MatchTarget>();
        public UnityEvent<FPUI_MatchItem> OnMatchFailure = new UnityEvent<FPUI_MatchItem>();
        public UnityEvent<FPUI_MatchItem,FPUI_MatchTarget> OnMatchRemoved = new UnityEvent<FPUI_MatchItem, FPUI_MatchTarget>();
        // when we want to notify of any match added (success or failure)
        public UnityEvent<FPUI_MatchItem,FPUI_MatchTarget> OnMatchAdded = new UnityEvent<FPUI_MatchItem, FPUI_MatchTarget>();
        [SerializeField]
        [Tooltip("Canvas Raycaster?")]
        protected GraphicRaycaster graphicRaycaster;
        [Tooltip("Event System?")]
        protected EventSystem eventSystem;
        private PointerEventData pointerEventData;


        
        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //Debug.LogWarning($"FPUI_DragDropManager instance set to {this.name}");
            }
            else
            {
                Destroy(this.gameObject);
            }
            if (eventSystem == null)
            {
                eventSystem = EventSystem.current;
            }
            if (graphicRaycaster == null)
            {
                Debug.LogError($"Need the Canvas / Graphic-Raycaster!");
            }
            
        }
        public virtual void OnEnable()
        {
            FPUI_DragDropManager.Instance.OnRelease.AddListener(HandleDrop);
            FPUI_DragDropManager.Instance.OnPickUp.AddListener(HandlePickUp);
        }
        public virtual void OnDisable()
        {
            FPUI_DragDropManager.Instance.OnRelease.RemoveListener(HandleDrop);
            FPUI_DragDropManager.Instance.OnPickUp.RemoveListener(HandlePickUp);
        }
        public virtual void HandlePickUp(RectTransform draggedItemRT)
        {
            var matchItem = draggedItemRT.GetComponent<FPUI_MatchItem>();
            if (matchItem == null) return;

            // Check if item was already matched
            var previousTargets = FindObjectsByType<FPUI_MatchTarget>(FindObjectsSortMode.InstanceID);
            for (int i = 0; i < previousTargets.Length; i++)
            { 
                var target = previousTargets[i];
                target.RemoveItemHere(matchItem);
                if (target.CurrentMatchItems.Contains(matchItem))
                {
                    target.RemoveMatchedItem(matchItem);
                    OnMatchRemoved.Invoke(matchItem, target);
                    break;
                }
            }
        }
        public virtual void HandleDrop(RectTransform draggedItemRT)
        {
            var matchItem = draggedItemRT.GetComponent<FPUI_MatchItem>();
            if (matchItem == null)
            {
                Debug.Log("Dropped item has no FPUI_MatchItem.");
                return;
            }
            pointerEventData = new PointerEventData(eventSystem)
            {
                selectedObject = draggedItemRT.gameObject
            };
            if (FPUI_DragDropManager.Instance.UseMouse)
            {
                pointerEventData.position = Input.mousePosition;
            }
            if(FPUI_DragDropManager.Instance.UseVRCursor)
            {
                pointerEventData.position = FPUI_DragDropManager.Instance.GetCursorPos;
            }
            var results = new System.Collections.Generic.List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);
            //Debug.LogWarning($"Match Manager HandleDrop results: {results.Count}");
            for (int i = 0; i < results.Count; i++) 
            {
                var matchTarget = results[i].gameObject.GetComponent<FPUI_MatchTarget>();
                if (matchTarget != null)
                {
                    //Debug.LogWarning($"Match? : {matchItem.MatchID} = {matchTarget.ExpectedMatchID} ?");
                    matchTarget.AddItemHere(matchItem);
                    OnMatchAdded?.Invoke(matchItem, matchTarget);
                    
                    if (matchTarget.IsMatch(matchItem.MatchID))
                    {
                        matchTarget.SetMatchedItem(matchItem);
                        OnMatchSuccess?.Invoke(matchItem, matchTarget);

                        return;
                    }
                }
            }
            

            // If no match found:
            OnMatchFailure?.Invoke(matchItem);
        }
    }
}
