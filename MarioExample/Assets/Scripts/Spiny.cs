using UnityEngine;

public class Spiny : Enemy
{
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
                LevelManager.Instance.TakeHit();
            }

        }
    }
}
