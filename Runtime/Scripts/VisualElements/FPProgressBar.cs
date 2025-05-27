namespace FuzzPhyte.UI.Toolkit
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class FPProgressBarElement : VisualElement, IFPStyleReceiver
    {
        protected VisualElement background;
        protected VisualElement fill;
        [Range(0f, 1f)]
        protected float progress = 0f; // 0-1
        public FPProgressBarElement()
        {
            background = new VisualElement();
            fill = new VisualElement();
            hierarchy.Add(background);
            background.Add(fill);
        }

        public void ApplyFPStyle(FPUITStyleData styleData)
        {
            if (styleData == null) return;


            background.style.backgroundColor = styleData.BackgroundColor;


            fill.style.backgroundColor = styleData.FillColor;


            background.style.borderTopLeftRadius = styleData.CornerRadius;
            background.style.borderTopRightRadius = styleData.CornerRadius;
            background.style.borderBottomLeftRadius = styleData.CornerRadius;
            background.style.borderBottomRightRadius = styleData.CornerRadius;

            fill.style.borderTopLeftRadius = styleData.CornerRadius;
            fill.style.borderTopRightRadius = styleData.CornerRadius;
            fill.style.borderBottomLeftRadius = styleData.CornerRadius;
            fill.style.borderBottomRightRadius = styleData.CornerRadius;



            background.style.borderTopWidth = styleData.BorderThickness;
            background.style.borderRightWidth = styleData.BorderThickness;
            background.style.borderBottomWidth = styleData.BorderThickness;
            background.style.borderLeftWidth = styleData.BorderThickness;



            background.style.borderTopColor = styleData.BorderColor;
            background.style.borderRightColor = styleData.BorderColor;
            background.style.borderBottomColor = styleData.BorderColor;
            background.style.borderLeftColor = styleData.BorderColor;


            // Font settings for optional text overlay (if you add one)
            // You could extend this class with a `Label` overlay if needed

            MarkDirtyRepaint();

        }
        public void SetFillAmount(float value)
        {
            progress = Mathf.Clamp01(value);
            fill.style.width = Length.Percent(progress * 100f);
            MarkDirtyRepaint();
        }
    }
}
