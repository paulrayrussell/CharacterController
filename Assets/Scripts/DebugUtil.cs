using UnityEngine;

namespace SpriteFlick.Util
{
    public class DebugUtil
    {
        public static void DrawMarker(Vector2 position, Color color, float pointSize = 0.02f) {
            float zIndex = -2;
            Debug.DrawLine(position, new Vector3(position.x, position.y + pointSize, zIndex), color);
            Debug.DrawLine(position, new Vector3(position.x, position.y - pointSize, zIndex), color);
            Debug.DrawLine(position, new Vector3(position.x + pointSize, position.y + pointSize, zIndex), color);
            Debug.DrawLine(position, new Vector3(position.x - pointSize, position.y + pointSize, zIndex), color);
            Debug.DrawLine(position, new Vector3(position.x + pointSize, position.y - pointSize, zIndex), color);
            Debug.DrawLine(position, new Vector3(position.x - pointSize, position.y - pointSize, zIndex), color);
            Debug.DrawLine(position, new Vector3(position.x + pointSize, position.y + pointSize, zIndex), color);
        }

        public static void DrawLine(Vector2 start, Vector2 end)
        {
            Debug.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0), Color.red);
        }
    }
}