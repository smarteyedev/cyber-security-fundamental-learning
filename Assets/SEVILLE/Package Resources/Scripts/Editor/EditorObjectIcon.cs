using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Seville.Utilites;
using System.Linq;

namespace Seville
{
    internal static class EditorObjectIcon
    {
        #region Properties
        private static readonly Color activeColor = new Color(1f, 1f, 1f, 1f);
        private static readonly Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);

        private static bool enableGameObjectMainIcon = true;
        private static bool iconsLoaded = false;

        private static Texture2D disableImg;
        private static Texture2D enableImg;

        private static Dictionary<GameObject, bool> nextChildState = new Dictionary<GameObject, bool>();
        private static IconData classIconDataList;
        #endregion

        #region Inisialisasi
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            LoadClassIcons();
            LoadCustomIcons();
            SubscribeToEvents();
        }

        private static void SubscribeToEvents()
        {
            if (EditorApplication.hierarchyWindowItemOnGUI == null)
            {
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
                // Debug.Log("Subscribed to hierarchy window item event.");
            }
        }

        private static void LoadCustomIcons()
        {
            if (iconsLoaded) return;

            enableImg = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SEVILLE/Package Resources/Textures/Icons/editor_enable_icon.PNG");
            disableImg = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SEVILLE/Package Resources/Textures/Icons/editor_disable_icon.PNG");

            iconsLoaded = true;
            // Debug.Log("Custom icons loaded.");
        }

        private static void LoadClassIcons()
        {
            string path = "Assets/SEVILLE/Package Resources/Scripts/Editor/IconData.asset";

            classIconDataList = AssetDatabase.LoadAssetAtPath<IconData>(path);

            if (classIconDataList == null)
            {
                Debug.LogError("Failed to load ClassIconDataList from the specified folder!");
            }
        }
        #endregion

        #region Event 
        private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) { return; }

            LoadCustomIcons();

            var components = gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                string className = component.GetType().Name;

                var classIconPair = classIconDataList.classIconPairs.FirstOrDefault(x => x.className == className);

                if (classIconPair != null)
                {
                    DrawGameObjectBackground(selectionRect, gameObject, instanceID);
                    DrawGameObjectMainIcon(gameObject, selectionRect, className);

                    if (classIconPair.isHasChildActiveButton)
                    {
                        DrawVisibilityButton(gameObject, selectionRect);
                    }

                    break; //* Menghentikan loop setelah menemukan kelas yang relevan
                }
            }
        }
        #endregion

        #region Draw Functions
        private static void DrawGameObjectMainIcon(GameObject gameObject, Rect selectionRect, string className)
        {
            Texture2D icon = classIconDataList.classIconPairs
                                .FirstOrDefault(d => d.className == className)?.classIcon;

            if (icon != null)
            {
                float iconSize = selectionRect.height;
                float iconXPosition = selectionRect.xMax - iconSize - 4;
                GUI.color = gameObject.activeInHierarchy ? activeColor : inactiveColor;
                GUI.DrawTexture(new Rect(iconXPosition, selectionRect.y + 1, iconSize, iconSize), icon);
                GUI.color = activeColor;
            }
            else
            {
                Debug.LogWarning("Icon is null, cannot draw.");
            }
        }

        private static void DrawGameObjectBackground(Rect selectionRect, GameObject gameObject, int instanceID)
        {
            Color originalBackgroundColor = GUI.backgroundColor;
            Color originalContentColor = GUI.contentColor;

            GUIStyle style = new GUIStyle(GUI.skin.label);

            if (gameObject.activeInHierarchy)
            {
                style.normal.background = Texture2D.whiteTexture;
                GUI.backgroundColor = new Color(82f / 255f, 90f / 255f, 104f / 255f);
                style.normal.textColor = Color.white;
            }
            else
            {
                style.normal.background = Texture2D.whiteTexture;
                GUI.backgroundColor = new Color(45f / 255f, 48f / 255f, 52f / 255f);
                style.normal.textColor = new Color(125f / 255f, 132f / 255f, 159f / 255f);
            }

            GUI.Box(selectionRect, GUIContent.none, style);
            EditorGUI.LabelField(selectionRect, gameObject.name, style);

            GUI.backgroundColor = originalBackgroundColor;
            GUI.contentColor = originalContentColor;
        }

        public static void DrawVisibilityButton(GameObject gameObject, Rect selectionRect)
        {
            Texture2D buttonImage = UtilityFunctions.HasInactiveChild(gameObject) ? enableImg : disableImg;

            if (buttonImage != null)
            {
                float iconSize = selectionRect.height;
                float spacing = 25f;
                float iconXPosition = selectionRect.xMax - iconSize - spacing;
                float iconYPosition = selectionRect.y;

                Rect buttonRect = new Rect(iconXPosition, iconYPosition, iconSize, iconSize);
                GUI.DrawTexture(buttonRect, buttonImage);

                if (buttonRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    ToggleChildrenActiveStatus(gameObject);
                    Event.current.Use();
                }
            }
        }

        private static void ToggleChildrenActiveStatus(GameObject parent)
        {
            bool shouldDisable = nextChildState.ContainsKey(parent) && nextChildState[parent];

            foreach (Transform child in parent.transform)
            {
                if (child.gameObject.activeSelf != shouldDisable)
                {
                    child.gameObject.SetActive(shouldDisable);
                }
            }

            nextChildState[parent] = !shouldDisable;
            // Debug.Log($"All children of {parent.name} have been {(shouldDisable ? "enabled" : "disabled")}");
        }
        #endregion

        #region Menu untuk Toggle Ikon
        //! [MenuItem("Seville/Toggle Main Icon %&m")]
        private static void ToggleMainIcon()
        {
            enableGameObjectMainIcon = !enableGameObjectMainIcon;
            EditorApplication.RepaintHierarchyWindow();
        }
        #endregion
    }
}
