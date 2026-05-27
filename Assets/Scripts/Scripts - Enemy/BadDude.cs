using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BadDude : MonoBehaviour
{
    public EnemyManager.EnemyType myType = EnemyManager.EnemyType.None;

    public Pathfinding myPathfindingType;

    public Spawner mySpawner;
    public int speed = 5;

    public bool isPursuing = false;

    public Vector3Int MyGridPos => MapManager.currentGrid.WorldToCell(transform.position);
    public Vector3Int myPriorPos;

    protected virtual void OnEnable()
    {
        Activate();
    }

    protected virtual void OnDisable()
    {
        if (!isPursuing)
        {
            EventManager.OnPursuingNewSector -= PursueSector;
            Destroy(gameObject);
        }
    }

    public virtual void Initialize(Spawner newSpawner)
    {
        mySpawner = newSpawner;
    }

    protected virtual void Activate()
    {
        StartCoroutine(Move());
    }

    protected abstract IEnumerator Move();

    protected Vector3 RotateToVector3(Vector3Int targetTile)
    {
        Vector3Int difference = targetTile - MyGridPos;
        if (difference.x > 0)
            return Vector3.right;
        else if (difference.x < 0)
            return Vector3.left;
        else if (difference.y > 0)
            return Vector3.up;
        else if (difference.y < 0)
            return Vector3.down;
        else
            return new Vector3(1, 1, 0); // This should NEVER get here.
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet") || collision.CompareTag("BankNuke"))
            Death();
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Pursuit"))
        {
            print("ENTERED PURSUIT RANGE");
            isPursuing = true;
            EventManager.OnPursuingNewSector += PursueSector;
        }
    }

    private void Death()
    {
        EnemyManager.Instance.StartCoroutine(EnemyManager.TriggerSingleSpawner(mySpawner, startTimer: 4.0f)); // Calls for mySpawner to spawn
        Destroy(gameObject);
    }    

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pursuit"))
        {
            print("LEFT THE PURSUIT RANGE");
            isPursuing = false;
            EventManager.OnPursuingNewSector -= PursueSector;
        }
    }

    // Sets parent to the EnemyList of the newSector and assigns to an empty spawner. If none are empty, this object destroys itself
    protected virtual void PursueSector(GameObject newSector)
    {
        EventManager.PursueLogicStart.Invoke();
        Transform newParentList = EnemyManager.currentEnemyList.transform;

        mySpawner = EnemyManager.AssignSpawner(this, myType);

        if (mySpawner != null)
        {
            transform.SetParent(newParentList);
            EventManager.PursueLogicEnd.Invoke();
        }
        else
        {
            EventManager.PursueLogicEnd.Invoke();
            Destroy(gameObject);
        }
    }
}
