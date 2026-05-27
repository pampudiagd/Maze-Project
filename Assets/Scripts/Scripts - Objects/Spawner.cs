using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public BadDude myEnemy;
    [SerializeField] private EnemyManager.EnemyType myType = EnemyManager.EnemyType.None; // Doesn't change during runtime

    [SerializeField] private EnemyPrefabDatabase database;

    private void Awake()
    {
        enemyPrefab = database.prefabs[(int)myType];
    }

    public void EmptySpawner()
    {
        print($"Emptying {this.name}");
        enemyPrefab = null;
        myEnemy = null;
        EventManager.OnBankFilled -= ResetOriginal;
    }

    public void FillSpawner(EnemyManager.EnemyType prefabIndex, BadDude enemy = null)
    {
        myEnemy = enemy;
        enemyPrefab = database.prefabs[(int)prefabIndex];
        if (enemy != null)
            EventManager.OnBankFilled += ResetOriginal;
    }

    private void ResetOriginal()
    {
        FillSpawner(myType);
        EventManager.OnBankFilled -= ResetOriginal;
    }

    public void SpawnEnemy()
    {
        if (myEnemy == null && enemyPrefab != null)
        {
            myEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation, EnemyManager.currentEnemyList.transform).GetComponent<BadDude>();
            myEnemy.Initialize(this);
        }
        else
            print("--------------Enemy not spawned----------------");
        if (myEnemy == null)
            print($"{this.name} FAILED BECAUSE NULL");
        else if (myEnemy.isPursuing)
            print($"{this.name} FAILED BECAUSE PURSUING");
    }
}
