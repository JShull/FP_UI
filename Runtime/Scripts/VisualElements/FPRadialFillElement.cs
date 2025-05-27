using UnityEngine;
using UnityEngine.UIElements;

namespace FuzzPhyte.UI.Toolkit
{
    public class FPRadialFillElement : VisualElement,IFPStyleReceiver
    {
        public float FillAmount { get; set; } = 1f; // 0-1 range
        public float Radius { get; set; } = 50f;

        private Color fillColor = Color.green;
        private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.7f);

        public FPRadialFillElement()
        {
            generateVisualContent += OnGenerateVisualContent;

            style.width = Radius * 2f;
            style.height = Radius * 2f;
            style.flexShrink = 0; // Prevents shrinking in flex containers
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Vector2 center = new Vector2(layout.width / 2f, layout.height / 2f);
            float endAngle = Mathf.Lerp(0, Mathf.PI * 2f, FillAmount);

            // Background circle (optional)
            FPUIT_Utility.DrawCircle(center, Radius, 0f, Mathf.PI * 2f, backgroundColor, context);

            // Filled portion
            FPUIT_Utility.DrawCircle(center, Radius, 0f, endAngle, fillColor, context);
        }

        public virtual void ApplyFPStyle(FPUITStyleData styleData)
        {
            if (styleData == null) return;

            
            backgroundColor = styleData.BackgroundColor;
            fillColor = styleData.FillColor;
            Radius = styleData.CornerRadius;

            style.width = Radius * 2f;
            style.height = Radius * 2f;

            // Border/outline not directly supported in mesh drawing
            // Consider adding a separate stroke element if needed

            MarkDirtyRepaint();
        }

        public virtual void SetFillAmount(float amount)
        {
            FillAmount = Mathf.Clamp01(amount);
            MarkDirtyRepaint();
        }
    }
}
