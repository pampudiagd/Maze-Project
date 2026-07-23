using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    public Grid grid; // The current coordinate system
    public Tilemap tilemap; // The current tiles that are interactable

    public Slider equipMeterSlider;
    public Image equipMeterFill;

    public int lives = 3;
    [SerializeField] public LivesUI livesUI;

    public int coinCount = 0;
    public int coinCapacity = 100;

    public int ammoCount = 0;
    public int ammoCapacity = 10;
    [SerializeField] public AmmoUI ammoUI;

    [SerializeField] private float gunCooldown = 1f;
    [SerializeField] private float gunClock;

    //Speed is public since it changes dynamically with equip load
    //Light load (default, 0-24%) = 7
    //Med light load (25-49%) = 6
    //Med heavy load (50-74%) = 5
    //Heavy load (75-99%) = 4
    //Overburdened (100%) = 2
    public int speed = 7;
    [SerializeField] private int dashSpeedMultiplier = 4;
    [SerializeField] private int dashDistance = 2;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashClock;
    [Tooltip("Length of dash invulnerability (seconds)")] [SerializeField] private float iDashDuration = 0.5f;

    private float invincibleUntil;
    public float InvincibleUntil => invincibleUntil;
    private bool wasInvincible;
    private bool currentlyInvincible;

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

    //Key binding variables

    public KeyCode dashKey = KeyCode.Space;
    public KeyCode fireKey = KeyCode.LeftShift;

    public TMP_Text dashKeyText;
    public TMP_Text fireKeyText;

    private bool rebindingKey = false;

    private enum RebindAction
    {
        None,
        Dash,
        Fire
    }

    private RebindAction actionToRebind = RebindAction.None;

    // S T A R T
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        livesUI.SetLives(lives);
        ammoUI.SetAmmo(ammoCount);
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
        ICheck();

        //Input Rebinding Stuff
        if (rebindingKey)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    UpdateKey(key);
                    break;
                }
            }
        }
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
            EventManager.OnPlayerMoved.Invoke();
        }

        isMoving = false;
    }

    private IEnumerator Dash()
    {
        if (isMoving || coinCapacity - coinCount == 0)
            yield break;

        isMoving = true;

        invincibleUntil = Time.time + iDashDuration;
        sr.color = Color.red;

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
            Vector2 newPos = Vector2.MoveTowards(transform.position, grid.GetCellCenterWorld(dashTarget), dashSpeedMultiplier * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
        
        dashClock = dashCooldown;
        wantToDash = false;
        isMoving = false;
    }

    private void ICheck()
    { 
        currentlyInvincible = Time.time < invincibleUntil;

        if (wasInvincible && !currentlyInvincible)
            IFramesEnded();

        wasInvincible = currentlyInvincible;
    }

    private void IFramesEnded()
    {
        sr.color = Color.white;
    }

    private void FireBullet()
    {
        Instantiate(myBullet, transform.position + transform.up, transform.rotation);

        LoseAmmo();
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
        GainAmmo();
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
        equipMeterSlider.value = heldPercent;

        speed = heldPercent switch
        {
            < 0.24f => 7,
            < 0.49f => 6,
            < 0.74f => 5,
            < 0.99f => 4,
            _ => 2
        };

        equipMeterFill.color = heldPercent switch
        {
            < 0.24f => Color.green,
            < 0.49f => Color.yellow,
            < 0.74f => new Color32(255,128,0,255), //orange
            < 0.99f => Color.red,
            _ => Color.red //just red for now, may change later
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
        if (Input.GetKey(dashKey) && dashClock <= 0)
            wantToDash = true;
        else if (Input.GetKey(fireKey) && ammoCount > 0 && gunClock <= 0)
            FireBullet();
    }

    //The following code is for changing input bindings.

    //Here's the flow of things:
    //KeyClicked[Action] is called when the button is pressed. It flips rebindingKey to true.
    //Then in Update(), if rebindingKey is true, the system awaits input.
    //Once input is provided, UpdateKey() is called and updates the key.
    //Then it sets rebindingKey to false again, returning to normal.

    private void UpdateKey(KeyCode newKey)
    {
        //Since we only have two keys, this goes through them manually,
        //to keep the code as easy to understand as possible.
        //However, if we add more, this should be redone with a dictionary.

        switch(actionToRebind)
        {
            case RebindAction.Dash:
                dashKey = newKey;
                dashKeyText.text = newKey.ToString();
                break;

            case RebindAction.Fire:
                fireKey = newKey;
                fireKeyText.text = newKey.ToString();
                break;
        }

        rebindingKey = false;
        actionToRebind = RebindAction.None;
    }

    public void KeyClickedDash()
    {
        actionToRebind = RebindAction.Dash;
        rebindingKey = true;
        dashKeyText.text = "Press any key";
    }

    public void KeyClickedFire()
    {
        actionToRebind = RebindAction.Fire;
        rebindingKey = true;
        fireKeyText.text = "Press any key";
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

    public void LoseLife()
    {
        if (lives > 0)
        {
            lives--;
            livesUI.SetLives(lives);
        }
    }

    public void GainLife()
    {
        if (lives < 10)
        {
            lives++;
            livesUI.SetLives(lives);
        }
    }

    public void LoseAmmo()
    {
        if (ammoCount > 0)
        {
            ammoCount--;
            ammoUI.SetAmmo(ammoCount);
        }
    }

    public void GainAmmo()
    {
        ammoCount = ammoCapacity; //Picking up a gun completely fills ammo, every time.
        ammoUI.SetAmmo(ammoCount);
    }

}
