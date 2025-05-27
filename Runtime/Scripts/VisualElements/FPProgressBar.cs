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

            if (styleData.BackgroundColor.HasValue)
                background.style.backgroundColor = styleData.BackgroundColor.Value;

            if (styleData.FillColor.HasValue)
                fill.style.backgroundColor = styleData.FillColor.Value;

            if (styleData.CornerRadius.HasValue)
            {
                background.style.borderTopLeftRadius = styleData.CornerRadius.Value;
                background.style.borderTopRightRadius = styleData.CornerRadius.Value;
                background.style.borderBottomLeftRadius = styleData.CornerRadius.Value;
                background.style.borderBottomRightRadius = styleData.CornerRadius.Value;

                fill.style.borderTopLeftRadius = styleData.CornerRadius.Value;
                fill.style.borderTopRightRadius = styleData.CornerRadius.Value;
                fill.style.borderBottomLeftRadius = styleData.CornerRadius.Value;
                fill.style.borderBottomRightRadius = styleData.CornerRadius.Value;
            }

            if (styleData.BorderThickness.HasValue)
            {
                background.style.borderTopWidth = styleData.BorderThickness.Value;
                background.style.borderRightWidth = styleData.BorderThickness.Value;
                background.style.borderBottomWidth = styleData.BorderThickness.Value;
                background.style.borderLeftWidth = styleData.BorderThickness.Value;
            }

            if (styleData.BorderColor.HasValue)
            {
                background.style.borderTopColor = styleData.BorderColor.Value;
                background.style.borderRightColor = styleData.BorderColor.Value;
                background.style.borderBottomColor = styleData.BorderColor.Value;
                background.style.borderLeftColor = styleData.BorderColor.Value;
            }

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
