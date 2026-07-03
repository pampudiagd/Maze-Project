#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ParentRenamePropagator
{
    // Prevent recursive hierarchy updates
    private static bool isProcessing;

    static ParentRenamePropagator()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    static void OnHierarchyChanged()
    {
        // Prevent recursive calls caused by renaming
        if (isProcessing)
            return;

        isProcessing = true;

        try
        {
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

            // Only run while editing an isolated prefab.
            if (stage == null)
                return;

            Transform sector = stage.prefabContentsRoot.transform;

            if (!sector.name.StartsWith("Sector "))
                return;

            string suffix = sector.name.Substring("Sector ".Length);

            Transform contents = FindChildStartingWith(sector, "Sector Contents");
            if (contents == null)
                return;

            RenameSuffix(contents, suffix);

            foreach (Transform child in contents)
            {
                RenameSuffix(child, suffix);
            }
        }
        finally
        {
            isProcessing = false;
        }
    }

    static void RenameSuffix(Transform t, string suffix)
    {
        int lastSpace = t.name.LastIndexOf(' ');
        if (lastSpace < 0)
            return;

        string desired = t.name.Substring(0, lastSpace + 1) + suffix;

        if (t.name != desired)
        {
            t.name = desired;
            EditorUtility.SetDirty(t.gameObject);
        }
    }

    static Transform FindChildStartingWith(Transform parent, string prefix)
    {
        foreach (Transform child in parent)
        {
            if (child.name.StartsWith(prefix))
                return child;
        }

        return null;
    }
}
#endif