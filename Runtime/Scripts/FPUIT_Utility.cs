namespace FuzzPhyte.UI
{
    using UnityEngine;
    using UnityEngine.UIElements;
    /// <summary>
    /// Static class for utility functions related to the UI Toolkit
    /// </summary>
    public static class FPUIT_Utility
    {
        /// <summary>
        /// Draws a filled arc/circle into the MeshGenerationContext.
        /// </summary>
        public static void DrawCircle(Vector2 center, float radius, float startAngle, float endAngle, Color color, MeshGenerationContext context, int segments = 50)
        {
            if (segments < 3) segments = 3; // Minimum valid segments

            var mesh = context.Allocate(segments + 1, (segments - 1) * 3);

            color.a = 1f; // Ensure full alpha

            // Center vertex
            mesh.SetNextVertex(new Vertex
            {
                position = center,
                tint = color
            });

            // Perimeter vertices
            float angle = startAngle;
            float step = (endAngle - startAngle) / (segments - 1);

            for (int i = 0; i < segments; i++)
            {
                var offset = new Vector2(
                    radius * Mathf.Cos(angle),
                    radius * Mathf.Sin(angle)
                );

                mesh.SetNextVertex(new Vertex
                {
                    position = center + offset,
                    tint = color
                });

                angle += step;
            }

            // Triangles: fan-shaped
            for (ushort i = 1; i < segments; i++)
            {
                mesh.SetNextIndex(0);
                mesh.SetNextIndex(i);
                mesh.SetNextIndex((ushort)(i + 1));
            }
        }

        /// <summary>
        /// Overload: Draws a full circle.
        /// </summary>
        public static void DrawCircle(Vector2 center, float radius, Color color, MeshGenerationContext context, int segments = 50)
        {
            DrawCircle(center, radius, 0f, Mathf.PI * 2f, color, context, segments);
        }
        public static int CalculateFontSize(FPUITStyleData styleData, float containerHeight)
        {
            if (styleData.UseAutoSizing)
            {
                int min = styleData.MinFontSize;
                int max = styleData.MaxFontSize;
                return Mathf.Clamp(Mathf.RoundToInt(containerHeight * 0.1f), min, max);
            }
            return styleData.FontSize;
        }
    }
    #region Interfaces for UIToolkit
    public interface IFPStyleReceiver
    {
        void ApplyFPStyle(FPUITStyleData styleData);
        void SetFillAmount(float value);
    }
    #endregion
    #region Data Classes and Structs for UIToolkit needs
    public enum FPRadialStartPosition
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
        TopMiddle,
        BottomMiddle
    }

    public enum FPRadialDirection
    {
        Clockwise,
        CounterClockwise
    }
    #endregion
}
