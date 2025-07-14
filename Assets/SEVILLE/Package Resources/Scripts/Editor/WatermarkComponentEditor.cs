using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

namespace Seville
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class WatermarkComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Type componentType = target.GetType();

            if (componentType.Namespace == "Seville")
            {
                WatermarkUtility.DisplayWatermarkImage();
            }

            base.OnInspectorGUI();
        }
    }

    public static class WatermarkUtility
    {
        public static void DisplayWatermarkImage()
        {
            float width = EditorGUIUtility.currentViewWidth;

            Texture2D myImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/SEVILLE/Package Resources/Textures/watermark.png", typeof(Texture2D));

            float imageWidth = myImage.width;
            float imageHeight = myImage.height;

            float maxWidth = width * 0.5f;
            float scaleFactor = (imageWidth > maxWidth) ? maxWidth / imageWidth : 1;

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(myImage, GUILayout.Width(imageWidth * scaleFactor), GUILayout.Height(imageHeight * scaleFactor));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.Space(10);
        }
    }
}
