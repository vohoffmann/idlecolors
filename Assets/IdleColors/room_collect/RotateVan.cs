using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class RotateVan : MonoBehaviour
{
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * 100);
    }
#if UNITY_EDITOR
    private void OnEnable()
    {
        // Abonniere das Editor-Update
        EditorApplication.update += Update;
    }

    private void OnDisable()
    {
        // Entferne das Editor-Update
        EditorApplication.update -= Update;
    }
#endif
}