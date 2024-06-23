using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimation : MonoBehaviour
{
    [Header("References - DO NOT CHANGE")]
    public SpriteRenderer spriteRenderer;
    public Sprite deathSprite;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        UpdateSprite();
        DisablePhysics();
        StartCoroutine(Animate());
    }
    private void UpdateSprite()
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sortingOrder = 100;

        if(deathSprite != null) {
            spriteRenderer.sprite = deathSprite;
        }
    }
    private void DisablePhysics()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach(Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Animator>().enabled = false;
        
        MarioMovement movement = GetComponent<MarioMovement>();
        if (movement != null) movement.enabled = false;
    }
    private IEnumerator Animate()
    {
        float elapsed = 0f;
        float duration = 3.0f;
        float jumpVelocity = 15f;
        float gravity = -36f;

        Vector3 vel = Vector3.up * jumpVelocity;

        while(elapsed < duration) {
            transform.position += vel * Time.deltaTime;
            vel.y += gravity * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
        LevelManager.Instance.RestartLevel();
    }
}
