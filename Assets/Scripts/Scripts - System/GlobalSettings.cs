using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Global Settings")]
public class GlobalSettings : ScriptableObject
{
    public bool allowEnemySpawns = true;
    public bool allowCoins = true;
}
