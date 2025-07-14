using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;

namespace Seville
{
    public class AreaHandlerWindow : EditorWindow
    {
        private Vector2 scrollPos;
        private AreaHandler targetManager;
        private string[] prefabPaths;

        public static void ShowWindow(AreaHandler manager)
        {
            AreaHandlerWindow window = GetWindow<AreaHandlerWindow>("Prefab Instantiator");
            window.targetManager = manager;
        }

        private void OnEnable()
        {
            // Ambil dan sort path prefab HANYA SEKALI saat window dibuka
            prefabPaths = new string[]
            {
            "Assets/SEVILLE/Package Resources/Prefabs/Canvas/CAROUSEL CANVAS.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Canvas/VIRTUAL KEYBOARD.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Canvas/INPUT FIELD FORM CANVAS.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Object Interactive/INTERACTABLE OBJECT (Cube-Base).prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Logic/(LOGIC) INTEGERS COUNTERS.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Canvas/NAVIGATION CANVAS.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Canvas/POPUP CANVAS.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Object Interactive/POKE BUTTON.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Canvas/QUEST CANVAS.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Canvas/QUIZ CANVAS.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Object Interactive/SOCKET INTERACTOR.prefab",
            "Assets/SEVILLE/Package Resources/Prefabs/Canvas/VIDEOPLAYER CANVAS.prefab",
            }
            .OrderBy(path => Path.GetFileNameWithoutExtension(path)) // Sort sekali saja
            .ToArray();
        }

        private void OnGUI()
        {
            if (prefabPaths == null || prefabPaths.Length == 0)
            {
                EditorGUILayout.HelpBox("No prefabs found.", MessageType.Warning);
                return;
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 20));

            foreach (string prefabPath in prefabPaths)
            {
                string prefabName = Path.GetFileNameWithoutExtension(prefabPath);

                if (GUILayout.Button($"{prefabName} (+)"))
                {
                    InstantiatePrefab(prefabPath);

                    // Menutup window setelah prefab berhasil diinstansiasi
                    Close();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void InstantiatePrefab(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab)
            {
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                obj.AddComponent<TransformConfig>();

                if (targetManager)
                {
                    obj.transform.SetParent(targetManager.transform);

                    // Menandai scene sebagai perlu disimpan
                    EditorSceneManager.MarkSceneDirty(targetManager.gameObject.scene);

                    // Lakukan proses lain seperti menambahkan ke dalam list dan aktifkan objek
                    AreaHandler handler = obj.GetComponentInParent<AreaHandler>();

                    if (handler)
                    {
                        targetManager.featureList.Add(obj.gameObject);
                        foreach (Transform child in targetManager.transform)
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Objek yang diinstansiasi tidak memiliki komponen EnvAreaHandler.");
                    }
                }
                else
                {
                    Debug.LogWarning("Prefab referensi tidak ditemukan.");
                }
            }
        }
    }
}