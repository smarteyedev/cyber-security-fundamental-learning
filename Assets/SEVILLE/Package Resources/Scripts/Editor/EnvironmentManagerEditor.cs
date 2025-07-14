using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace Seville
{
    [CustomEditor(typeof(EnvironmentManager))]
    public class EnvironmentManagerEditor : Editor
    {
        private Vector2 scrollPosition;

        public override void OnInspectorGUI()
        {
            WatermarkUtility.DisplayWatermarkImage();
            EnvironmentManager envManager = (EnvironmentManager)target;

            SerializedProperty _envAreaHandlers = serializedObject.FindProperty("envAreaHandlers");
            SerializedProperty _characterOrigin = serializedObject.FindProperty("characterOrigin");
            SerializedProperty _VR360Settings = serializedObject.FindProperty("VR360Settings");
            SerializedProperty _formatMaterial = serializedObject.FindProperty("formatMaterial");
            SerializedProperty _targetSphereArea = serializedObject.FindProperty("targetSphereArea");

            serializedObject.Update();

            GUIContent areaObjsListContent = new GUIContent("List of Area", "you have to insert all the area handler component into the list.");
            EditorGUILayout.LabelField(areaObjsListContent);

            GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box);
            //? backgroundStyle.normal.background = Texture2D.whiteTexture; // Set background texture (default white)

            EditorGUILayout.BeginVertical(backgroundStyle);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150)); // Scrollable list
            for (int i = 0; i < envManager.envAreaHandlers.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                envManager.envAreaHandlers[i] = (AreaHandler)EditorGUILayout.ObjectField("Area Number " + (i), envManager.envAreaHandlers[i], typeof(AreaHandler), true);

                GUIContent deleteContent = new GUIContent("X", "Delete this gameobject from the scene.");
                if (GUILayout.Button(deleteContent, GUILayout.Width(20)))
                {
                    if (envManager.envAreaHandlers[i] != null)
                    {
                        DestroyImmediate(envManager.envAreaHandlers[i].gameObject);
                    }

                    envManager.envAreaHandlers.RemoveAt(i);
                    EditorSceneManager.MarkSceneDirty(envManager.gameObject.scene);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            GUIContent btnAddArea = new GUIContent("+ Add Area", "Click to add a new area to the scene.");
            if (GUILayout.Button(btnAddArea, SevilleStyleEditor.GreenButton, GUILayout.Height(35), GUILayout.ExpandWidth(true))) // Adjust button size and width
            {
                AddNewArea(envManager);
                EditorSceneManager.MarkSceneDirty(envManager.gameObject.scene);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(5f);

            EditorGUILayout.PropertyField(_characterOrigin);
            EditorGUILayout.PropertyField(_VR360Settings);
            EditorGUILayout.PropertyField(_formatMaterial);
            EditorGUILayout.PropertyField(_targetSphereArea);

            EditorGUILayout.Space(10);

            envManager.envAreaHandlers.RemoveAll(item => item == null);
            serializedObject.ApplyModifiedProperties();
        }

#if UNITY_EDITOR
        public void AddNewArea(EnvironmentManager manager)
        {
            string prefabPath = "Assets/SEVILLE/Package Resources/Prefabs/Environments/ENVIRONMENT AREA VR 360.prefab";

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError("Prefab tidak ditemukan di " + prefabPath);
                return;
            }

            GameObject obj = Instantiate(prefab);
            obj.name = $"---- AREA NUMBER: {manager.envAreaHandlers.Count} ----";

            AreaHandler handler = obj.GetComponent<AreaHandler>();

            if (handler == null)
            {
                Debug.LogError("Prefab tidak memiliki komponen ChildHandler");
                return;
            }

            manager.envAreaHandlers.Add(handler);
        }
#endif
    }
}