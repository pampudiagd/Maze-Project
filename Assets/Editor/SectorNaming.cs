#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ParentRenamePropagator
{
    // Define all child naming rules here
    static readonly (string prefix, string baseName)[] rules =
    {
        ("Grid",    "Grid"),
        ("Load Zone",  "Load Zone"),
        ("Collectible Grid", "Collectible Grid"),
    };

    static ParentRenamePropagator()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    static void OnHierarchyChanged()
    {
        GameObject world = GameObject.Find("Game World");
        if (world == null) return;

        foreach (Transform parent in world.transform)
        {
            if (!parent.name.StartsWith("Sector "))
                continue;

            string suffix = parent.name.Substring("Sector ".Length); // "D3"

            foreach (var rule in rules)
            {
                Transform child = FindChildStartingWith(parent, rule.prefix);
                if (child == null) continue;

                string desired = $"{rule.baseName} {suffix}";

                if (child.name != desired)
                    child.name = desired;
            }
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