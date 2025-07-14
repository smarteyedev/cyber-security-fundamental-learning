#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Seville
{
    [CustomEditor(typeof(TransformConfig))]
    public class TransformConfigEditor : Editor
    {
        TransformConfig config;
        Transform transform;
        public override void OnInspectorGUI()
        {
            // [Watermark Added] 
            WatermarkUtility.DisplayWatermarkImage();

            config = (TransformConfig)target;
            transform = config.transform;

            DrawDefaultInspector();

            Vector3 position = transform.position;
            Vector3 rotation = transform.eulerAngles;

            float tiltX = (rotation.x > 180) ? rotation.x - 360 : rotation.x;

            if (rotation.y < 0 || rotation.y > 360)
            {
                EditorGUILayout.HelpBox("Rotation (Y) harus berada dalam rentang 0 - 360 derajat.", MessageType.Warning);
            }

            float currentDistance = Vector2.Distance(new Vector2(position.x, position.z), new Vector2(config.centerPos.x, config.centerPos.z));
            bool isOutOfRadius = !Mathf.Approximately(currentDistance, config.distanceFromPlayer);

            if (isOutOfRadius)
            {
                EditorGUILayout.HelpBox("Jarak posisi objek dari pusat harus berada dalam rentang 1.7 - 4.", MessageType.Warning);
                if (GUILayout.Button("Align Position"))
                {
                    config.MoveObject();
                }
            }

            bool isOutOfVertical = (position.y != config.verticalPosition);
            if (isOutOfVertical)
            {
                EditorGUILayout.HelpBox("Vertical Position (Y) harus berada dalam rentang -2 hingga 2.", MessageType.Warning);
                if (GUILayout.Button("Align Position"))
                {
                    config.MoveObject();
                }
            }

            bool isOutOfTilt = (tiltX < -31f || tiltX > 31f);
            if (isOutOfTilt)
            {
                EditorGUILayout.HelpBox("Tilt Angle (Rotation X) harus berada dalam rentang -30 hingga 30 derajat.", MessageType.Warning);
                if (GUILayout.Button("Align Position"))
                {
                    config.MoveObject();
                }
            }
        }

        private void OnDestroy()
        {
            if (transform != null)
            {
                transform.hideFlags = HideFlags.None;
            }
        }
    }
#endif
}