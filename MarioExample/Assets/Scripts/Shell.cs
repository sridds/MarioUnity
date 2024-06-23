using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : Enemy
{
    public float shellSpeed;

    private Rigidbody2D myRigidbody;
    private bool isMoving;

    private int direction = -1;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isMoving) {
            myRigidbody.velocity = new Vector2(direction * shellSpeed, myRigidbody.velocity.y);
        }
        else {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            if (LevelManager.Instance.isInvincibleStarman) {
                OnHitStarman();
            }
            else {
                if (other.transform.DotTest(transform, Vector2.down)) {
                    LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.kickSound);
                    ChangeDirection();
                    isMoving = !isMoving;
                }
                else {
                    LevelManager.Instance.TakeHit();
                }
            }
        }
        else if(isMoving && other.gameObject.TryGetComponent<Enemy>(out Enemy enemy)){
            enemy.OnShellHit();
        }
        else { 
            Vector2 normal = other.contacts[0].normal;
            Vector2 leftSide = new Vector2(-1f, 0f);
            Vector2 rightSide = new Vector2(1f, 0f);
            Vector2 bottomSide = new Vector2(0f, 1f);

            bool sideHit = normal == leftSide || normal == rightSide;
            bool bottomHit = normal == bottomSide;
            bool topHit = normal == -bottomSide;

            if (sideHit) {
                ChangeDirection();
            }
        }
    }

    void ChangeDirection() {

        LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBumpSound);

        direction *= -1;
    }
}
