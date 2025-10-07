namespace FuzzPhyte.UI.Samples
{
    using UnityEngine;
    using FuzzPhyte.UI;
    using System.Collections;
    using UnityEngine.Events;
    public class MatchTestBankController : MonoBehaviour
    {
        [Tooltip("Remove Drag Item on Match")]
        public bool RemoveDragItemOnMatch = true;
        [Tooltip("Disable Hover on Match")]
        public bool RemoveHoverOnMatch = true;
        [Tooltip("Will set the item's parent as the Target Bin")]
        public bool SetParentTransform = true;
        [Tooltip("Set Position Center on Drop")]
        public bool SetPositionCenter = true;
        [Space]
        [Header("Running Counts")]
        public int RunningMatchScore = 0;
        public int RunningMatchFailure = 0;
        public void OnEnable()
        {
            FPUI_MatchManager.Instance.OnMatchSuccess.AddListener(HandleMatchSuccess);
            FPUI_MatchManager.Instance.OnMatchFailure.AddListener(HandleMatchFailure);
            FPUI_MatchManager.Instance.OnMatchRemoved.AddListener(HandleMatchRemove);
            FPUI_MatchManager.Instance.OnMatchAdded.AddListener(HandleMatchAdded);
        }
        public void OnDisable()
        {
            FPUI_MatchManager.Instance.OnMatchSuccess.RemoveListener(HandleMatchSuccess);
            FPUI_MatchManager.Instance.OnMatchFailure.RemoveListener(HandleMatchFailure);
            FPUI_MatchManager.Instance.OnMatchAdded.RemoveListener(HandleMatchAdded);
            FPUI_MatchManager.Instance.OnMatchRemoved.RemoveListener(HandleMatchRemove);
        }

        public void HandleMatchSuccess(FPUI_MatchItem matchItem, FPUI_MatchTarget matchTarget)
        {
            Debug.Log($"Matched {matchItem.name} with {matchTarget.name}");
            //we want to remove the drag item the moment we have a match
            if (RemoveDragItemOnMatch)
            {
                if (matchItem.GetComponent<FPUI_DragItem>())
                {
                    matchItem.GetComponent<FPUI_DragItem>().DragEnabled = false;
                }
            }
            if (RemoveHoverOnMatch)
            {
                if (matchItem.GetComponent<FPUI_HoverHandler>())
                {
                    matchItem.GetComponent<FPUI_HoverHandler>().HoverEnabled = false;
                }
            }

            //use the MatchTarget and adjust my parent?
            if (SetParentTransform)
            {
                matchItem.transform.SetParent(matchTarget.transform);
                if (SetPositionCenter)
                {
                    matchItem.transform.localPosition = matchTarget.LocalOriginMatchPosition;
                }

            }
            else
            {
                if (SetPositionCenter)
                {
                    matchItem.transform.position = matchTarget.transform.position;
                }
            }

            RunningMatchScore++;
        }
        public void HandleMatchFailure(FPUI_MatchItem matchItem)
        {
            Debug.Log($"Failed to match {matchItem.name}");
            RunningMatchFailure++;
        }
        public void HandleMatchRemove(FPUI_MatchItem item, FPUI_MatchTarget target)
        {
            Debug.Log($"Match Removed! {item.name} from {target.name}");
            RunningMatchScore--;
        }
        public void HandleMatchAdded(FPUI_MatchItem item, FPUI_MatchTarget target)
        {
            Debug.Log($"Match Added! {item.name} to {target.name}");
            float delayTime = 0.1f;
            //float padding = 2.25f;
            for (int i = 0; i < target.AllItemsHere.Count; i++)
            {
                var stackedItem = target.AllItemsHere[i];
                if (stackedItem.GetComponent<FPUI_DragItem>())
                {
                    StartCoroutine(DelayPlayEvent(delayTime, stackedItem.GetComponent<FPUI_DragItem>().OnMouseDownEvent));
                    delayTime += item.DurationValue;
                }
            }
        }
        IEnumerator DelayPlayEvent(float delay, UnityEvent anEvent)
        {
            yield return new WaitForSecondsRealtime(delay);
            anEvent?.Invoke();
        }
    }
}
