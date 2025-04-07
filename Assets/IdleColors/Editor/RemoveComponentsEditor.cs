using UnityEditor;
using UnityEngine;

namespace IdleColors.Editor
{
    public class RemoveComponentsEditor : EditorWindow
    {
        private string componentTypeName = "";

        [MenuItem("Tools/Remove Components of Type")]
        public static void ShowWindow()
        {
            GetWindow<RemoveComponentsEditor>("Remove Components");
        }

        private void OnGUI()
        {
            GUILayout.Label("Remove Components by Type", EditorStyles.boldLabel);

            componentTypeName = EditorGUILayout.TextField("Component Type Name", componentTypeName);

            if (GUILayout.Button("Remove Components"))
            {
                RemoveComponents();
            }
        }

        private void RemoveComponents()
        {
            if (string.IsNullOrEmpty(componentTypeName))
            {
                Debug.LogError("Please enter a valid component type name.");
                return;
            }

            // Suche alle GameObjects in der Szene
            GameObject[] allObjects   = FindObjectsOfType<GameObject>();
            int          removedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                Component component = obj.GetComponent(componentTypeName);

                while (component != null)
                {
                    Undo.DestroyObjectImmediate(component); // Sicherer Entfernen-Befehl mit Undo-Unterstützung
                    removedCount++;
                    component = obj.GetComponent(componentTypeName); // Prüfen, ob weitere vorhanden sind
                }
            }

            Debug.Log($"Removed {removedCount} components of type '{componentTypeName}'.");
        }
    }
}