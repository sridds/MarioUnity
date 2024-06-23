using UnityEngine;

public class Goomba : Enemy
{
    public Sprite flatSprite;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {

            if (LevelManager.Instance.isInvincibleStarman) {
                OnHitStarman();
            }
            else { 
                if (collision.transform.DotTest(transform, Vector2.down)) {
                    Flatten();
                    LevelManager.Instance.AddPoints(stompBonus);
                }
                else {
                    LevelManager.Instance.TakeHit();
                }            
            }

        }
    }

    public void Flatten()
    {
        LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.stompSound);

        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;

        GetComponent<MoveAndFlip>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;

        GetComponentInChildren<SpriteAnimator>().enabled = false;
        
        sprRenderer.sprite = flatSprite;
        Destroy(gameObject, 0.5f);
    }
}
