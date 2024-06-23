using UnityEngine;

public class Boo : Enemy
{
    public Sprite scaredSprite;
    public Sprite chaseSprite;

    public float speed;
    public float minDistanceToMove = 14f;
    
    private bool movementActive;
    private bool canMove;
    private MarioMovement marioMovement;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (LevelManager.Instance.isInvincibleStarman) {
                OnHitStarman();
            }
            else {
                LevelManager.Instance.TakeHit();
            }
        }
    }

    private void Awake()
    {
        marioMovement = LevelManager.Instance.Mario.GetComponent<MarioMovement>();
    }

    private void Update()
    {
        if(Vector3.Distance(marioMovement.transform.position, transform.position) < minDistanceToMove) {
            movementActive = true;
        }

        if (!movementActive) return;

        if(transform.position.x < marioMovement.transform.position.x && marioMovement.flipX || transform.position.x > marioMovement.transform.position.x && !marioMovement.flipX) {
            canMove = false;
            sprRenderer.sprite = scaredSprite;
        }
        else {
            canMove = true;
            sprRenderer.sprite = chaseSprite;
        }

        if (canMove && !LevelManager.Instance.isDead && !LevelManager.Instance.levelComplete && !isActive) {
            transform.position = Vector2.MoveTowards(transform.position, marioMovement.transform.position, Time.deltaTime * speed);
        }

        sprRenderer.flipX = transform.position.x > marioMovement.transform.position.x;
    }
}
