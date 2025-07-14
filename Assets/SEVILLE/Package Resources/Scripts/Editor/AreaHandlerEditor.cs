using UnityEngine;
using UnityEditor;
using Seville.Utilites;
using Seville;
using UnityEditor.SceneManagement;

namespace Seville
{
    [CustomEditor(typeof(AreaHandler))]
    public class AreaHandlerEditor : Editor
    {
        private Vector2 scrollPosition;

        public override void OnInspectorGUI()
        {
            // [Watermark Added] Memanggil fungsi DisplayWatermarkImage dari WatermarkComponentEditor
            WatermarkUtility.DisplayWatermarkImage();

            AreaHandler handler = (AreaHandler)target;

            GUIContent areaTextureContent = new GUIContent("Area Texture", "Texture Area is a 360 photo that will be displayed as a background in the area.");
            handler.areaTexture = (Texture2D)EditorGUILayout.ObjectField(areaTextureContent, handler.areaTexture, typeof(Texture2D), false);

            // Show a warning if areaTexture is null
            if (handler.areaTexture == null)
            {
                EditorGUILayout.HelpBox("Area Texture must be assigned.", MessageType.Warning);
            }

            string toggleBtnText = handler.IsHideArea() ? "Hide Area Texture" : "Preview Area Texture";
            GUIContent toggleOpacityContent = new GUIContent(toggleBtnText, "Toggle button to show and hide the texture area.");
            if (GUILayout.Button(toggleOpacityContent))
            {
                handler.ToggleAlpha();
            }

            EditorGUILayout.Space(10);

            GUIContent backsoundContent = new GUIContent("Backsound", "Select an audio clip to be played in the background when entering the area. If you do not include an audio clip, the audio asset that will be played is the background music from the audio manager.");
            handler.backsound = (AudioClip)EditorGUILayout.ObjectField(backsoundContent, handler.backsound, typeof(AudioClip), false);

            GUIContent playerRotationContent = new GUIContent("Player Rotation", "Set the character player's rotation value when they enter the area.");
            handler.playerRotation = EditorGUILayout.Slider(playerRotationContent, handler.playerRotation, 0, 360);

            GUIContent restartOnExitContent = new GUIContent("On Exit Reset Settings", "If enabled, the gameobject transform will reset when the player exits and re-enters.");
            handler.onExitResetSettings = EditorGUILayout.Toggle(restartOnExitContent, handler.onExitResetSettings);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUIContent areaObjsListContent = new GUIContent("List of Features in Area", "you have to enter all the features into the list to run the show and hide feature mechanism in each area.");
            EditorGUILayout.LabelField(areaObjsListContent);

            GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box);
            //? backgroundStyle.normal.background = Texture2D.whiteTexture; // Set background texture (default white)

            EditorGUILayout.BeginVertical(backgroundStyle);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150)); // Scrollable list
            for (int i = 0; i < handler.featureList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                handler.featureList[i] = (GameObject)EditorGUILayout.ObjectField("Object Feature " + (i + 1), handler.featureList[i], typeof(GameObject), true);

                GUIContent deleteContent = new GUIContent("X", "Delete this gameobject from the scene.");
                if (GUILayout.Button(deleteContent, GUILayout.Width(20)))
                {
                    if (handler.featureList[i] != null)
                    {
                        DestroyImmediate(handler.featureList[i]);
                    }

                    handler.featureList.RemoveAt(i);
                    EditorSceneManager.MarkSceneDirty(handler.gameObject.scene);

                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            GUIContent addObjectContent = new GUIContent("+ Add Object", "Click to add a new object to the area.");
            GUIContent addFeaturesContent = new GUIContent("+ Add Features", "Open the features window to add more functionality in area.");

            int childrenCount = UtilityFunctions.GetChildren(handler.gameObject).Length;
            // Show a warning if areaTexture is null
            if (childrenCount != handler.featureList.Count)
            {
                string msg = handler.featureList.Count > childrenCount ? "The number of features added does not match the number of gameobject features in the area, adjust it to the features in the list." : "If you have features outside seville plugin, please add the gameobject parent to the list.";
                EditorGUILayout.HelpBox(msg, MessageType.Warning);
            }

            if (handler.featureList.Count == childrenCount && handler.IsFeatureListHasNullItem())
            {
                string msg2 = $"Object Feature {handler.featureList.FindIndex(item => item == null) + 1} is an object with null value, please add object feature into List of Features or delete item.";
                EditorGUILayout.HelpBox(msg2, MessageType.Warning);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(addObjectContent, GUILayout.Width(150), GUILayout.Height(30))) // Adjust button size and width
            {
                handler.featureList.Add(null);
            }

            if (GUILayout.Button(addFeaturesContent, SevilleStyleEditor.BlueButton, GUILayout.Height(30), GUILayout.ExpandWidth(true))) // Stretch button to fill space
            {
                AreaHandlerWindow.ShowWindow(handler);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}