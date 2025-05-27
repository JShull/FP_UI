using UnityEngine;
using UnityEngine.UIElements;

namespace FuzzPhyte.UI.Toolkit
{
    public class FPSquareRadialElement : VisualElement,IFPStyleReceiver
    {
        [Range(0f, 1f)]
        [SerializeField]protected float progress { get; set; } = 0f; // 0 to 1
        public float OuterSize { get; set; } = 100f;
        public float InnerSize { get; set; } = 80f;

        public FPRadialStartPosition StartPosition { get; set; } = FPRadialStartPosition.TopLeft;
        public FPRadialDirection FillDirection { get; set; } = FPRadialDirection.Clockwise;

        protected Color startColor = Color.red;
        protected Color endColor = Color.green;

        public FPSquareRadialElement(Color StartColor, Color EndColor)
        {
            generateVisualContent += OnGenerateVisualContent;
            startColor = StartColor;
            endColor = EndColor;
            style.width = OuterSize;
            style.height = OuterSize;
            style.flexShrink = 0;
        }

        protected virtual void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            var mesh = ctx.Allocate(64, 192);

            float thickness = (OuterSize - InnerSize) * 0.5f;
            float halfSize = OuterSize * 0.5f;
            Vector2 center = new Vector2(halfSize, halfSize);
            float totalPerimeter = OuterSize * 4f;
            float targetLength = progress * totalPerimeter;

            // Generate ordered corners based on starting point and direction
            Vector2[] baseCorners = new Vector2[]
            {
                new Vector2(-halfSize, halfSize),    // TopLeft
                new Vector2(halfSize, halfSize),     // TopRight
                new Vector2(halfSize, -halfSize),    // BottomRight
                new Vector2(-halfSize, -halfSize)    // BottomLeft
            };

            Vector2[] corners = ReorderCorners(baseCorners, StartPosition, FillDirection);

            Vector2[] midpoints = new Vector2[]
            {
                (corners[0] + corners[1]) * 0.5f,  // TopMiddle
                (corners[1] + corners[2]) * 0.5f,  // RightMiddle
                (corners[2] + corners[3]) * 0.5f,  // BottomMiddle
                (corners[3] + corners[0]) * 0.5f   // LeftMiddle
            };

            // Adjust starting point if TopMiddle or BottomMiddle
            if (StartPosition == FPRadialStartPosition.TopMiddle)
            {
                corners = RotateCornersToStartAt(midpoints[0], corners);
            }
            else if (StartPosition == FPRadialStartPosition.BottomMiddle)
            {
                corners = RotateCornersToStartAt(midpoints[2], corners);
            }

            float accumulated = 0f;

            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 start = corners[i];
                Vector2 end = corners[(i + 1) % corners.Length];
                float segmentLength = Vector2.Distance(start, end);

                if (accumulated + segmentLength > targetLength)
                {
                    float t = (targetLength - accumulated) / segmentLength;
                    end = Vector2.Lerp(start, end, t);
                    segmentLength = targetLength - accumulated;
                }

                DrawSegment(mesh, center, start, end, thickness, accumulated / totalPerimeter, (accumulated + segmentLength) / totalPerimeter);
                accumulated += segmentLength;

                if (accumulated >= targetLength)
                    break;
            }
        }

        protected Vector2[] ReorderCorners(Vector2[] baseCorners, FPRadialStartPosition startPos, FPRadialDirection direction)
        {
            int startIndex = startPos switch
            {
                FPRadialStartPosition.TopLeft => 0,
                FPRadialStartPosition.TopRight => 1,
                FPRadialStartPosition.BottomRight => 2,
                FPRadialStartPosition.BottomLeft => 3,
                _ => 0
            };

            var reordered = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                int idx = direction == FPRadialDirection.Clockwise
                    ? (startIndex + i) % 4
                    : (startIndex - i + 4) % 4;

                reordered[i] = baseCorners[idx];
            }

            return reordered;
        }

        protected Vector2[] RotateCornersToStartAt(Vector2 point, Vector2[] corners)
        {
            // Find the closest edge midpoint and reorder the corners to start there
            int closestIndex = 0;
            float minDist = float.MaxValue;

            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 midpoint = (corners[i] + corners[(i + 1) % corners.Length]) * 0.5f;
                float dist = Vector2.Distance(point, midpoint);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }

            var rotated = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                rotated[i] = corners[(closestIndex + i) % 4];
            }

            return rotated;
        }

        protected virtual void DrawSegment(MeshWriteData mesh, Vector2 center, Vector2 start, Vector2 end, float thickness, float startT, float endT)
        {
            Vector2 dir = (end - start).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * thickness * 0.5f;

            Color startC = Color.Lerp(startColor, endColor, startT);
            Color endC = Color.Lerp(startColor, endColor, endT);

            ushort baseIndex = (ushort)mesh.vertexCount;

            mesh.SetNextVertex(new Vertex() { position = center + start + normal, tint = startC });
            mesh.SetNextVertex(new Vertex() { position = center + start - normal, tint = startC });
            mesh.SetNextVertex(new Vertex() { position = center + end + normal, tint = endC });
            mesh.SetNextVertex(new Vertex() { position = center + end - normal, tint = endC });

            mesh.SetNextIndex(baseIndex);
            mesh.SetNextIndex((ushort)(baseIndex + 1));
            mesh.SetNextIndex((ushort)(baseIndex + 2));

            mesh.SetNextIndex((ushort)(baseIndex + 2));
            mesh.SetNextIndex((ushort)(baseIndex + 1));
            mesh.SetNextIndex((ushort)(baseIndex + 3));
        }

        public virtual void ApplyFPStyle(FPUITStyleData styleData)
        {
            if (styleData == null) return;

            if (styleData.FillColor.HasValue)
                endColor = styleData.FillColor.Value;

            if (styleData.BackgroundColor.HasValue)
                startColor = styleData.BackgroundColor.Value;

            if (styleData.CornerRadius.HasValue)
            {
                OuterSize = styleData.CornerRadius.Value;
                style.width = OuterSize;
                style.height = OuterSize;
            }

            MarkDirtyRepaint();
        }
        public virtual void SetFillAmount(float value)
        {
            progress = Mathf.Clamp01(value);
            MarkDirtyRepaint();
        }
    }
}