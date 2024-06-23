using UnityEngine;

public class PiranhaPlant : Enemy
{
    private void OnTriggerEnter2D(Collider2D collision)
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

    public override void OnHitBlock()
    {
        
    }
    public override void OnFireballHit()
    {
        Destroy(gameObject);
    }
    public override void OnHitStarman()
    {
        Destroy(gameObject);
    }
}
