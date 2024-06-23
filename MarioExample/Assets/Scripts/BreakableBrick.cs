using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakableBrick : Block
{
    public GameObject tempColliderPrefab;
    public GameObject brokenPiecePrefab;

    [Header("Bonuses")]
    public int blockBonus = 50;

    private float WaitBetweenBounce = .25f;
    private float time1, time2;

    void OnTriggerEnter2D(Collider2D other)
    {
        time2 = Time.time;
        if (other.tag == "Player" && time2 - time1 >= WaitBetweenBounce)
        {
            // Hit any enemy on top
            foreach (GameObject obj in objectsOnTop.ToList())
            {
                if (obj != null)
                {
                    if (obj.TryGetComponent<Enemy>(out Enemy enemy))
                    {
                        if (!enemy.isActive) { 
                            LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.stompSound);

                            obj.GetComponent<Enemy>().OnHitBlock();                        
                        }
                    }
                }
            }

            // Bounce or break depending on Mario's size
            if (LevelManager.Instance.marioSize == 0)
            {
                StopAllCoroutines();
                StartCoroutine(Bounce());
                LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBumpSound);
            }
            else
            {
                BreakIntoPieces();
                LevelManager.Instance.AddPoints(blockBonus);
                LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBreakSound);
            }
            time1 = Time.time;
        }
    }

    private void BreakIntoPieces()
    {
        CreateBrickPiece(3f, 12f);
        CreateBrickPiece(-3f, 12f);
        CreateBrickPiece(3f, 8f);
        CreateBrickPiece(-3f, 8f);

        // prevent Player from breaking bricks above this
        Instantiate(tempColliderPrefab, transform.position, Quaternion.identity);
        Destroy(transform.gameObject);
    }

    private void CreateBrickPiece(float velX, float velY)
    {
        GameObject brickPiece = Instantiate(brokenPiecePrefab, transform.position, Quaternion.Euler(new Vector3(45, 0, 0))); // up right
        brickPiece.GetComponent<Rigidbody2D>().velocity = new Vector2(velX, velY);
    }
}
