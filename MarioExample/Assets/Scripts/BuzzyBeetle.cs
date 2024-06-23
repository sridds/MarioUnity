using UnityEngine;

public class BuzzyBeetle : Enemy
{
    public Shell shellPrefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
                    EnterShell();
                    LevelManager.Instance.AddPoints(stompBonus);
                }
                else
                {
                    LevelManager.Instance.TakeHit();
                }
            }

        }
    }

    public void EnterShell()
    {
        LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.stompSound);

        Instantiate(shellPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public override void OnFireballHit()
    {
        LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBumpSound);
    }
}
