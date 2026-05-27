using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Player : MonoBehaviour
{
    public Grid grid; // The current coordinate system
    public Tilemap tilemap; // The current tiles that are interactable

    public int coinCount = 0;
    public int coinCapacity = 50;

    public int ammoCount = 0;
    public int ammoCapacity = 10;
    [SerializeField] private float gunCooldown = 1f;
    [SerializeField] private float gunClock;

    //Speed should be public since it changes dynamically with equip load
    //Light load (default, 0-24%) = 7
    //Med light load (25-49%) = 6
    //Med heavy load (50-74%) = 5
    //Heavy load (75-99%) = 4
    //Overburdened (100%) = 2
    public int speed = 7;
    [SerializeField] private int dashSpeedMultiplier = 4; //Changed this slightly to work with dynamic speed 
    [SerializeField] private int dashDistance = 2;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashClock;

    public static Vector3Int direction = new(0, 1);
    [SerializeField] private Vector3Int storedDirection = new(0, 1);
    [SerializeField] private Vector3Int adjacentTile;
    private Vector3Int dashTarget;

    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool wantToDash = false;
    private bool updatedMap = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public GameObject myBullet;

    public bool isDead = false;

    public Vector3Int MyGridPos => grid.WorldToCell(transform.position);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventManager.OnCollectCoin += TryAddCoin;
        EventManager.OnCollectPowerup += GivePowerup;
    }

    private void OnDisable()
    {
        EventManager.OnCollectCoin -= TryAddCoin;
        EventManager.OnCollectPowerup -= GivePowerup;
    }

    private void Update()
    {
        if (dashClock > 0)
            dashClock = Timer(dashClock);
        if (gunClock > 0)
            gunClock = Timer(gunClock);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDead)
            return;

        InputReader();
        if (wantToDash && dashClock <= 0 && speed >= 4) //speed>=4 makes sure dashing is impossible when overburdened
                                                        //this should be changed to directly check the equip load variable once that exists
            StartCoroutine(Dash());
        else
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
                if (updatedMap)
                {
                    updatedMap = false;
                    break;
                }
                if (isDead)
                    break;
            }
        }

        isMoving = false;
    }

    private IEnumerator Dash()
    {
        if (isMoving || coinCapacity - coinCount == 0)
            yield break;

        isMoving = true;

        adjacentTile = MyGridPos;

        for (int i = 0; i < dashDistance; i++)
        {
            dashTarget = adjacentTile;
            adjacentTile += storedDirection;

            if (tilemap.GetColliderType(adjacentTile) != Tile.ColliderType.None)
                break;
        }

        while ((transform.position - grid.GetCellCenterWorld(dashTarget)).sqrMagnitude > 0.001f)
        {
            sr.color = Color.red;
            Vector2 newPos = Vector2.MoveTowards(transform.position, grid.GetCellCenterWorld(dashTarget), dashSpeedMultiplier * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        sr.color = Color.white;
        dashClock = dashCooldown;
        wantToDash = false;
        isMoving = false;
    }

    private void FireBullet()
    {
        Instantiate(myBullet, transform.position + transform.up, transform.rotation);

        ammoCount--;
        gunClock = gunCooldown;
    }

    private float Timer(float clock)
    {
        return clock -= Time.deltaTime;
    }

    private bool TryAddCoin()
    {
        if (coinCount >= coinCapacity)
            return false;
        else
        {
            coinCount++;
            CalculateWeight();
            return true;
        }
    }

    private void GivePowerup()
    {
        ammoCount = ammoCapacity;
    }

    //Light load (default, 0-24%) = 7
    //Med light load (25-49%) = 6
    //Med heavy load (50-74%) = 5
    //Heavy load (75-99%) = 4
    //Overburdened (100%) = 2
    public void CalculateWeight()
    {
        float heldPercent = (float)coinCount / coinCapacity;
        print(heldPercent);
        speed = heldPercent switch
        {
            < 0.24f => 7,
            < 0.49f => 6,
            < 0.74f => 5,
            < 0.99f => 4,
            _ => 2
        };
    }

    private void InputReader()
    {
        // Record the player's last directional input
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            direction.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            direction.y = 0;
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            direction.y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
            direction.x = 0;
        }
        if (Input.GetKey(KeyCode.LeftShift) && dashClock <= 0)
            wantToDash = true;
        else if (Input.GetKey(KeyCode.Space) && ammoCount > 0 && gunClock <= 0)
            FireBullet();
    }

    public void Death()
    {
        // Death animation, UI change
    }

    public void UpdateMapInfo(Transform newGrid)
    {
        grid = newGrid.GetComponent<Grid>();
        tilemap = newGrid.GetComponentInChildren<Tilemap>();
        updatedMap = true;
    }

}
