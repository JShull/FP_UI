using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuzzPhyte.Utility;
namespace FuzzPhyte.UI
{
    [CreateAssetMenu(fileName = "Arrow", menuName = "FuzzPhyte/UI/Arrow", order = 1)]
    public class FP_Arrow:ScriptableObject
    {
        public FP_Theme Theme;
        public string ArrowLabel;
        [Space]
        [Header("Arrow Properties")]
        [Tooltip("From bottom Left to Top Right in screen % space")]
        
        public Vector2 CenterPt;
        [Tooltip("All arrows face up and rotate clockwise from 0 degrees")]
        public float RotationClockwise;
        [Header("Relative Size")]
        public int PixelWidth;
        public int PixelHeight;
    }
}
