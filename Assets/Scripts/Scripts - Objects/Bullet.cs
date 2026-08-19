using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private int speed = 2;
    private float bombTimer = 3f;

    public BulletType type;

    public enum BulletType
    {
        Default,
        Reverse,
        Bomb,
        Instant
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //type = BulletType.Reverse;
        AssignTypeBehavior();
        //rb.AddRelativeForce(Vector2.up * speed, ForceMode2D.Impulse);
    }

    private void AssignTypeBehavior()
    {
        switch (type)
        {
            case BulletType.Default:
                rb.AddRelativeForce(Vector2.up * speed, ForceMode2D.Impulse);
                break;
            case BulletType.Reverse:
                rb.AddRelativeForce(Vector2.down * speed, ForceMode2D.Impulse);
                break;
            case BulletType.Bomb:
                StartCoroutine(Explode(bombTimer));
                break;
            case BulletType.Instant:
                StartCoroutine(Explode(0f));
                break;
        }
    }

    private IEnumerator Explode(float timer)
    {
        yield return new WaitForSeconds(timer);

        for (int i = 0; i < 10; i++)
        {
            transform.GetChild(0).transform.localScale += new Vector3(1, 1, 0);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") && (type == BulletType.Default || type == BulletType.Reverse))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && (type == BulletType.Default || type == BulletType.Reverse))
            Destroy(gameObject);
    }
}
