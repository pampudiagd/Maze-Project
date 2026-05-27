using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Prefab Database")]
public class EnemyPrefabDatabase : ScriptableObject
{
    public List<GameObject> prefabs = new();
}
