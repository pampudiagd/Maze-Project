using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadDude : MonoBehaviour
{
    public int prefabIndex = 0;

    //[SerializeField] private GameObject homeSector; // The sector this enemy spawns from. Should NEVER change in-game
    public Spawner mySpawner;
    //public GameObject mySector; // The sector this enemy is currently within. Should change 
    public Vector2 myDirection;
    public int speed = 5;

    public bool isPursuing = false;

    public Vector3Int MyGridPos => MapManager.currentGrid.WorldToCell(transform.position);
    //public GameObject HomeSector => homeSector;

    // Start is called before the first frame update
    void Start()
    {
        //mySector = HomeSector;
    }

    private void OnEnable()
    {
        Activate();
    }

    private void OnDisable()
    {
        if (!isPursuing)
        {
            EventManager.OnPursuingNewSector -= PursueSector;
            Destroy(gameObject);
        }
    }

    public void Initialize(Spawner newSpawner)
    {
        mySpawner = newSpawner;
    }

    private void Activate()
    {
        StartCoroutine(Move());
    }

    // Moves one tile at a time
    private IEnumerator Move()
    {
        yield return null;
        Vector3Int lastPlayerPos = MapManager.PlayerGridPos;
        List<Vector3Int> path = Pathfinding.FindPath(MyGridPos, MapManager.PlayerGridPos);
        int pathIndex = 0;

        while (true)
        {
            if (path == null || pathIndex >= path.Count || lastPlayerPos != MapManager.PlayerGridPos)
            {
                lastPlayerPos = MapManager.PlayerGridPos;
                path = Pathfinding.FindPath(MyGridPos, lastPlayerPos);
                pathIndex = 0;
            }

            Vector3Int targetTile = path[pathIndex];
            transform.up = RotateToVector3(targetTile);

            Vector3 targetWorld = MapManager.currentGrid.GetCellCenterWorld(targetTile);

            while ((transform.position - targetWorld).sqrMagnitude > 0.001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorld, speed * Time.deltaTime);
                yield return null;
            }

            pathIndex++;
        }
    }

    private Vector3 RotateToVector3(Vector3Int targetTile)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            Destroy(gameObject);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pursuit"))
        {
            print("ENTERED PURSUIT RANGE");
            isPursuing = true;
            EventManager.OnPursuingNewSector += PursueSector;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pursuit"))
        {
            print("LEFT THE PURSUIT RANGE");
            isPursuing = false;
            EventManager.OnPursuingNewSector -= PursueSector;
        }
    }

    // Sets parent to the EnemyList of the newSector and assigns to an empty spawner. If none are empty, this object destroys itself
    private void PursueSector(GameObject newSector)
    {
        EventManager.PursueLogicStart.Invoke();
        Transform newParentList = EnemyManager.currentEnemyList.transform;
        //mySector = newSector;

        mySpawner = EnemyManager.AssignSpawner(this, prefabIndex);

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
