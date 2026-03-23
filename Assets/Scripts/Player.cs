using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Grid grid; // Swap for a global variable later.
    public Tilemap tilemap; // Swap for a global varaible later.

    [SerializeField] private int speed = 3;

    [SerializeField] private Vector3Int direction = new(1, 0);
    [SerializeField] private Vector3Int storedDirection = new(1, 0);
    [SerializeField] private Vector3Int adjacentTile;

    [SerializeField] private bool isMoving = false;

    private Rigidbody2D rb;

    private Vector3Int MyGridPos => grid.WorldToCell(transform.position);
    private 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputReader();
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        if (isMoving)
            yield break;

        isMoving = true;

        // Check if the latest input direction points to an open tile
        if (tilemap.GetColliderType(MyGridPos + direction) == Tile.ColliderType.None)
        { 
            storedDirection = direction; // Updates the Vector used to determine the next tile for movement

            // Set the player's rotation based on the last recorded directional input
            if (direction.y > 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (direction.y < 0)
                transform.rotation = Quaternion.Euler(0, 0, 180);
            if (direction.x > 0)
                transform.rotation = Quaternion.Euler(0, 0, 270);
            else if (direction.x < 0)
                transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        adjacentTile = MyGridPos + storedDirection; // Updates the tile the player is trying to move into

        if (tilemap.GetColliderType(adjacentTile) == Tile.ColliderType.None)
        {
            while ((transform.position - grid.GetCellCenterWorld(adjacentTile)).sqrMagnitude > 0.001f)
            {
                Vector2 newPos = Vector2.MoveTowards(transform.position, grid.GetCellCenterWorld(adjacentTile), speed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
                yield return new WaitForFixedUpdate();
            }
        }

        isMoving = false;
    }

    void InputReader()
    {
        // Record the player's last directional input
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            direction.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            direction.y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        }
    }
}
