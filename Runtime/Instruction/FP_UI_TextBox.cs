using FuzzPhyte.Utility;
using UnityEngine;

namespace FuzzPhyte.UI
{
    [CreateAssetMenu(fileName = "TextBox", menuName = "FuzzPhyte/UI/TextBox", order = 0)]
    public class FP_UI_TextBox:ScriptableObject
    {
        public FP_Theme Theme;
        [TextArea(2,5)]
        public string TextBox;
        public int FontSize;
        public int FontMax;
        public int FontMin;
        public TMPro.TextAlignmentOptions TextBoxAlignment;
        public FP_UI_InstructionButtonPosition ButtonPosition;
        public TMPro.FontStyles TextBoxFontStyle;
        [Space]
        [Header("Text Box Properties")]
        [Tooltip("From bottom Left to Top Right in screen % space")]
        public Vector2 BottomLeftPt;
        public Vector2 TopRightPt;
        public float OutlineThickness = 0;
        [Tooltip("Do we want to use text")]
        public bool UseText;
        [Tooltip("Do we want to use an image?")]
        public bool UseImage;
        [Tooltip("Do we want to use an outline?")]
        public bool UseOutline;
        [Tooltip("Do we want to use a scaled font?")]
        public bool UseScaleFont;
    }
}
