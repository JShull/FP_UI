namespace FuzzPhyte.UI
{
    using UnityEngine;
    using UnityEngine.UIElements;
    public class FPLogListElement : VisualElement,IFPStyleReceiver
    {
        protected ScrollView scrollView;
        protected int maxEntries = 100;
        public int MaxEntries { get => maxEntries; set => maxEntries = value; }
        public FPLogListElement(int _maxEntries = 100)
        {
            maxEntries = _maxEntries;
            style.flexDirection = FlexDirection.Column;
            style.flexGrow = 1;
            style.height = Length.Percent(100);
            style.width = Length.Percent(100);
            style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.7f);
            style.borderTopLeftRadius = 8;
            style.borderTopRightRadius = 8;
            style.borderBottomLeftRadius = 8;
            style.borderBottomRightRadius = 8;
            style.paddingLeft = 5;
            style.paddingRight = 5;

            scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            scrollView.style.height = Length.Percent(100);
            scrollView.style.width = Length.Percent(100);
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Auto;

            hierarchy.Add(scrollView);
        }
        public void AddLog(string message)
        {
            var label = new Label(message);
            label.style.unityFontStyleAndWeight = FontStyle.Normal;
            label.style.fontSize = 12;
            label.style.color = Color.white;
            label.style.marginBottom = 2;
            scrollView.Add(label);

            // Auto-scroll to latest
            scrollView.scrollOffset = new Vector2(0, scrollView.contentContainer.layout.height);

            // Optional: limit entries
            if (scrollView.childCount > maxEntries)
            {
                scrollView.RemoveAt(0);
            }
        }

        public void ClearLog()
        {
            scrollView.Clear();
        }
        public void SetFillAmount(float value)
        {
            // Not applicable for log list, but required by interface
            // Could be used for a progress indicator if needed
        }

        public virtual void ApplyFPStyle(FPUITStyleData styleData)
        {
            if (styleData == null) return;

            // Container styles
            if (styleData.BackgroundColor.HasValue)
                style.backgroundColor = styleData.BackgroundColor.Value;

            if (styleData.CornerRadius.HasValue)
            {
                style.borderTopLeftRadius = styleData.CornerRadius.Value;
                style.borderTopRightRadius = styleData.CornerRadius.Value;
                style.borderBottomLeftRadius = styleData.CornerRadius.Value;
                style.borderBottomRightRadius = styleData.CornerRadius.Value;
            }

            if (styleData.Padding.HasValue)
            {
                style.paddingLeft = styleData.Padding.Value;
                style.paddingRight = styleData.Padding.Value;
                style.paddingTop = styleData.Padding.Value;
                style.paddingBottom = styleData.Padding.Value;
            }

            if (styleData.BorderThickness.HasValue)
            {
                style.borderTopWidth = styleData.BorderThickness.Value;
                style.borderRightWidth = styleData.BorderThickness.Value;
                style.borderBottomWidth = styleData.BorderThickness.Value;
                style.borderLeftWidth = styleData.BorderThickness.Value;
            }

            if (styleData.BorderColor.HasValue)
            {
                style.borderTopColor = styleData.BorderColor.Value;
                style.borderRightColor = styleData.BorderColor.Value;
                style.borderBottomColor = styleData.BorderColor.Value;
                style.borderLeftColor = styleData.BorderColor.Value;
            }

            // Update text styles for log entries
            foreach (var child in scrollView.Children())
            {
                if (child is Label label)
                {
                    if (styleData.TextColor.HasValue)
                        label.style.color = styleData.TextColor.Value;

                    if (styleData.Font != null)
                        label.style.unityFont = styleData.Font;

                    if (styleData.FontSize.HasValue)
                        label.style.fontSize = styleData.FontSize.Value;

                    if (styleData.FontWeight.HasValue)
                        label.style.unityFontStyleAndWeight = styleData.FontWeight.Value;

                    if (styleData.FontAlignment.HasValue)
                        label.style.unityTextAlign = styleData.FontAlignment.Value;
                }
            }
            MarkDirtyRepaint();
        }
    }
}
