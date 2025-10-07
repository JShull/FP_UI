
namespace FuzzPhyte.UI
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    [RequireComponent(typeof(RectTransform))]
    public class FPUI_MatchTarget : MonoBehaviour
    {
        [Tooltip("The ID of the match item that should be matched to this target.")]
        public string ExpectedMatchID;
        public bool AllowMultipleMatches = false;
        [Tooltip("If you want to have multiple items that can match this target")]
        public List<string> AcceptedIDs = new List<string>();
        [Tooltip("Set to false if you want multiple matches to this single target")]
        public bool SingleMatch = true;
        //public bool IsMatched = false;
        [Tooltip("Vector3 data for use later as needed for other alignment")]
        public Vector3 LocalOriginMatchPosition;
        public List<FPUI_MatchItem> CurrentMatchItems = new List<FPUI_MatchItem>();
        public List<FPUI_MatchItem> AllItemsHere = new List<FPUI_MatchItem>();
        public UnityEvent OnMatchSuccessLocalEvent;
        public UnityEvent OnRemoveMatchLocalEvent;
        public virtual bool IsMatch(string matchID)
        {
            if (SingleMatch)
            {
                //already have a match
                if (CurrentMatchItems.Count > 0)
                {
                    return false;
                }
            }
            if(AllowMultipleMatches)
            {
                if (AcceptedIDs.Contains(matchID))
                {
                    return true;
                }
            }
            //remove white space trailing?
            var exP = ExpectedMatchID.Trim();
            var passed = matchID.Trim();
            return exP.Equals(passed);

        }
        public void SetMatchedItem(FPUI_MatchItem item)
        {
            
            if (!CurrentMatchItems.Contains(item))
            {
                OnMatchSuccessLocalEvent.Invoke();
                CurrentMatchItems.Add(item);
            }
        }
        public void AddItemHere(FPUI_MatchItem item)
        {
            if (!AllItemsHere.Contains(item))
            {
                AllItemsHere.Add(item);
            }
        }
        public void RemoveItemHere(FPUI_MatchItem item)
        {
            if (AllItemsHere.Contains(item))
            {
                AllItemsHere.Remove(item);
            }
        }
        public void RemoveMatchedItem(FPUI_MatchItem item)
        {
            if (AllItemsHere.Contains(item))
            {
                AllItemsHere.Remove(item);
            }
            if (CurrentMatchItems.Contains(item))
            {
                OnRemoveMatchLocalEvent.Invoke();
                CurrentMatchItems.Remove(item);
            }
        }
    }
}
