using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector2 absVelocity = new Vector2(20, 11);

    [Header("References - DO NOT TOUCH")]
    public Rigidbody2D myRigidbody;
    public GameObject explosionEffect;

    public void SetDirection(float dir)
    {
        myRigidbody.velocity = new Vector2(dir * absVelocity.x, -absVelocity.y);
    }

    public void Update()
    {
        if(Mathf.Abs(myRigidbody.velocity.magnitude) < 0.1f) {
            Explode();
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy")) { 
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnFireballHit();
            Explode();
        }

        else {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 leftSide = new Vector2(-1f, 0f);
            Vector2 rightSide = new Vector2(1f, 0f);
            Vector2 bottomSide = new Vector2(0f, 1f);

            if(normal == leftSide || normal == rightSide) {
                Explode();
            } else if(normal == bottomSide) {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, absVelocity.y);
            }
            else {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -absVelocity.y);
            }
        }
    }

    private void Explode()
    {
        LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBumpSound);

        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
