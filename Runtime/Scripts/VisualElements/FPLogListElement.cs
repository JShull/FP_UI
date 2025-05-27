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
        
            style.backgroundColor = styleData.BackgroundColor;

            style.borderTopLeftRadius = styleData.CornerRadius;
            style.borderTopRightRadius = styleData.CornerRadius;
            style.borderBottomLeftRadius = styleData.CornerRadius;
            style.borderBottomRightRadius = styleData.CornerRadius;

            style.paddingLeft = styleData.Padding;
            style.paddingRight = styleData.Padding;
            style.paddingTop = styleData.Padding;
            style.paddingBottom = styleData.Padding;

            style.borderTopWidth = styleData.BorderThickness;
            style.borderRightWidth = styleData.BorderThickness;
            style.borderBottomWidth = styleData.BorderThickness;
            style.borderLeftWidth = styleData.BorderThickness;
        
            style.borderTopColor = styleData.BorderColor;
            style.borderRightColor = styleData.BorderColor;
            style.borderBottomColor = styleData.BorderColor;
            style.borderLeftColor = styleData.BorderColor;
            

            // Update text styles for log entries
            foreach (var child in scrollView.Children())
            {
                if (child is Label label)
                {
                   
                    label.style.color = styleData.TextColor;

                    if (styleData.Font != null)
                    {
                        label.style.unityFont = styleData.Font;
                    }
                        
                    label.style.fontSize = styleData.FontSize;
                    label.style.unityFontStyleAndWeight = styleData.FontWeight;
                    label.style.unityTextAlign = styleData.FontAlignment;
                }
            }
            MarkDirtyRepaint();
        }
    }
}
