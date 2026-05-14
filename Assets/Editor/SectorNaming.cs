#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ParentRenamePropagator
{
    // Prevent recursive hierarchy updates
    private static bool isProcessing;

    // Define all child naming rules here
    static readonly (string prefix, string baseName)[] rules =
    {
        ("Grid", "Grid"),
        ("Load Zone", "Load Zone"),
        ("Collectible Grid", "Collectible Grid"),
        ("Enemies", "Enemies"),
    };

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
            GameObject world = GameObject.Find("Game World");
            if (world == null)
                return;

            foreach (Transform parent in world.transform)
            {
                if (!parent.name.StartsWith("Sector "))
                    continue;

                string suffix = parent.name.Substring("Sector ".Length);

                foreach (var rule in rules)
                {
                    Transform child = FindChildStartingWith(parent, rule.prefix);

                    if (child == null)
                        continue;

                    string desired = $"{rule.baseName} {suffix}";

                    // Only rename if actually different
                    if (child.name != desired)
                    {
                        child.name = desired;
                        EditorUtility.SetDirty(child.gameObject);
                    }
                }
            }
        }
        finally
        {
            isProcessing = false;
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