namespace FuzzPhyte.UI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "FPUITStyleData", menuName = "FuzzPhyte/UI/Toolkit/FPUITStyleData")]
    public class FPUITStyleData : ScriptableObject
    {
       [Header("Colors")]
        public Color BackgroundColor;
        public Color FillColor;
        public Color TextColor;
        public Color BorderColor;

        [Header("Shape & Layout")]
        [Tooltip("Used for elements as well as radius for circles")]
        public float CornerRadius;
        public float BorderThickness;
        public float Padding;

        [Header("Font Settings")]
        public Font Font; // UI Toolkit compatible
        public int FontSize;
        public int MinFontSize; // Optional - for auto sizing logic
        public int MaxFontSize;
        public bool UseAutoSizing;
        public FontStyle FontWeight; // Unity's FontStyle enum: Normal, Bold, Italic, BoldAndItalic
        public TextAnchor FontAlignment; // For UI Toolkit alignment approximation

        [Header("Optional Tagging")]
        public string StyleTag;
    }
}
