using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Bonuses")]
    public int starmanBonus = 100;
    public int rollingShellBonus = 500;
    public int hitByBlockBonus = 100;
    public int fireballBonus = 100;
    public int stompBonus = 100;

    [Header("References - DO NOT CHANGE")]
    public SpriteRenderer sprRenderer;

    public bool isActive { get; private set; }

    protected void FlipAndDie()
    {
        isActive = false;

        LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.kickSound);
        gameObject.layer = LayerMask.NameToLayer("Ignore Enemy Collision");

        if (TryGetComponent<BoxCollider2D>(out BoxCollider2D box)) box.enabled = false;
        if (TryGetComponent<CircleCollider2D>(out CircleCollider2D circle)) circle.enabled = false;
        if (TryGetComponent<MoveAndFlip>(out MoveAndFlip move)) move.enabled = false;

        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Rigidbody2D>().gravityScale = 4f;
        GetComponent<Rigidbody2D>().velocity += new Vector2(0, 3);

        sprRenderer.flipY = true;

        Destroy(gameObject, 4);
    }

    public virtual void OnHitBlock() {
        FlipAndDie();
        LevelManager.Instance.AddPoints(hitByBlockBonus);
    }

    public virtual void OnFireballHit() {
        FlipAndDie();
        LevelManager.Instance.AddPoints(fireballBonus);
    }

    public virtual void OnShellHit() {
        FlipAndDie();
        LevelManager.Instance.AddPoints(rollingShellBonus);
    }

    public virtual void OnHitStarman() {
        FlipAndDie();
        LevelManager.Instance.AddPoints(starmanBonus);
    }
}
