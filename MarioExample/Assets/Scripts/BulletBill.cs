using UnityEngine;

public class BulletBill : Enemy
{
    [Header("Modifiers")]
    [SerializeField] private float xSpeed;

    [Header("References - DO NOT CHANGE")]
    [SerializeField] private GameObject explosionEffect;

    private Rigidbody2D myRigidbody;
    private BoxCollider2D myCollider;
    private bool isLaunched;
    private int direction = 1;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (!isLaunched) return;

        sprRenderer.flipX = direction == 1 ? false : true;
        myRigidbody.velocity = new Vector2(xSpeed * direction, myRigidbody.velocity.y);

        if (Vector3.Distance(LevelManager.Instance.Mario.transform.position, transform.position) > 30.0f) Destroy(gameObject);
    }

    public void Launch(int direction)
    {
        this.direction = direction;
        isLaunched = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit somehtng;");
        if (collision.gameObject.CompareTag("Player"))
        {

            if (LevelManager.Instance.isInvincibleStarman)
            {
                OnHitStarman();
            }
            else
            {
                if (collision.transform.DotTest(transform, Vector2.down))
                {
                    FlipAndDie();
                    LevelManager.Instance.AddPoints(stompBonus);
                }
                else
                {
                    LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBumpSound);
                    Instantiate(explosionEffect, transform.position, Quaternion.identity);
                    LevelManager.Instance.TakeHit();
                    Destroy(gameObject);
                }
            }

        }
        else {
            LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBumpSound);
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public override void OnHitBlock()
    {
        // nothing lolz
    }

    public override void OnFireballHit()
    {
        // nothing lolz
    }

    public override void OnShellHit()
    {
        FlipAndDie();
        LevelManager.Instance.AddPoints(rollingShellBonus);
    }
}
