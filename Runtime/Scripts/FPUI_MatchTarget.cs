
namespace FuzzPhyte.UI
{
    using UnityEngine;

    using System.Collections.Generic;
    [RequireComponent(typeof(RectTransform))]
    public class FPUI_MatchTarget : MonoBehaviour
    {
        [Tooltip("The ID of the match item that should be matched to this target.")]
        public string ExpectedMatchID;
        [Tooltip("Set to false if you want multiple matches to this single target")]
        public bool SingleMatch = true;
        //public bool IsMatched = false;
        [Tooltip("Vector3 data for use later as needed for other alignment")]
        public Vector3 LocalOriginMatchPosition;
        public List<FPUI_MatchItem> CurrentMatchItems = new List<FPUI_MatchItem>();

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
            //remove white space trailing?
            var exP = ExpectedMatchID.Trim();
            var passed = matchID.Trim();
            return exP.Equals(passed);

        }
        public void SetMatchedItem(FPUI_MatchItem item)
        {
            if(!CurrentMatchItems.Contains(item))
            {
               CurrentMatchItems.Add(item);
            }
        }
        public void RemoveMatchedItem(FPUI_MatchItem item)
        {
            if(CurrentMatchItems.Contains(item))
            {
                CurrentMatchItems.Remove(item);
            }
        }
    }
}
