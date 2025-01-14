using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IdleColors.Editor
{
    public class RemoveNavmeshModifier : IProcessSceneWithReport
    {
        public int callbackOrder { get; }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (report != null && report.summary.options.HasFlag(BuildOptions.Development))
            {
                return;
            }

            var objectsToDelete = Component.FindObjectsByType<NavMeshModifier>(FindObjectsSortMode.None);

            for (var i = objectsToDelete.Length - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(objectsToDelete[i]);
            }
        }
    }
}