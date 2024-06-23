using System.Collections;
using System.Linq;
using UnityEngine;

public class BlockContainer : Block
{
    public enum ContainerType
    {
        ContainsPowerup,
        ContainsCoins
    }
    public BoxCollider2D myCollider;

    [Header("Modifiers")]
    public ContainerType containerType;
    public int storageAmount = 1;
    public bool isVisible = true;

    [Header("For Powerup Container Only")]
    public Powerup powerup;

    [Header("For Coin Container Only")]
    public GameObject blockCoin;

    private bool isActive;
    private float WaitBetweenBounce = .25f;
    private float timer1, timer2;

    IEnumerator bounceCoroutine;

    private void Start() {
        if(TryGetComponent<SpriteRenderer>(out SpriteRenderer icon)) {
            Destroy(icon);
        }

        if (!isVisible) {
            defaultRenderer.gameObject.SetActive(false);
            myCollider.enabled = false;
        }

        isActive = true;
        timer1 = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        timer2 = Time.time;
        if(collision.tag == "Player" && timer2 - timer1 >= WaitBetweenBounce) {

            myCollider.enabled = true;

            LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.blockBumpSound);
            // play sfx

            if(isActive) {
                // Hit any enemy on top
                foreach (GameObject obj in objectsOnTop.ToList())
                {
                    if (obj != null)
                    {
                        if (obj.TryGetComponent<Enemy>(out Enemy enemy))
                        {
                            if (!enemy.isActive)
                            {
                                LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.stompSound);

                                obj.GetComponent<Enemy>().OnHitBlock();
                            }
                        }
                    }
                }

                if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);

                bounceCoroutine = Bounce();
                StartCoroutine(bounceCoroutine);

                if(containerType == ContainerType.ContainsPowerup && powerup != null) { 
                    StartCoroutine(EmergePowerup());
                } else if(containerType == ContainerType.ContainsCoins) {
                    StartCoroutine(EmergeCoin());
                }

                storageAmount--;

                if(storageAmount == 0)
                {
                    emptyRenderer.gameObject.SetActive(true);
                    defaultRenderer.gameObject.SetActive(false);
                    isActive = false;
                }

            }

            timer1 = Time.time;
        }
    }

    private IEnumerator EmergePowerup()
    {
        LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.powerUpAppearSound);
        
        Powerup p = Instantiate(powerup, transform.position, Quaternion.identity);
        p.transform.parent = transform;

        // Disable everything
        Rigidbody2D rb = p.transform.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        foreach(Collider2D c in p.transform.GetComponents<Collider2D>()) {
            c.enabled = false;
        }
        SpriteRenderer renderer = p.transform.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = 0;
        renderer.enabled = false;

        Vector3 startPos = p.transform.localPosition;
        Vector3 endPos = p.transform.localPosition + Vector3.up;

        yield return new WaitForSeconds(0.25f);

        renderer.enabled = true;

        float elapsed = 0.0f;
        float duration = 0.5f;

        while(elapsed < duration)
        {
            float t = elapsed / duration;

            p.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        p.transform.parent = null;

        // Reenable everything
        foreach (Collider2D c in p.transform.GetComponents<Collider2D>()) {
            c.enabled = true;
        }
        rb.isKinematic = false;
        renderer.sortingOrder = 50;

        p.OnPowerupEmerged();
    }

    private IEnumerator EmergeCoin()
    {
        LevelManager.Instance.AddCoin();

        GameObject coin = Instantiate(blockCoin, transform.position, Quaternion.identity);
        coin.transform.parent = transform;

        Vector3 startPos = coin.transform.localPosition;
        Vector3 endPos = coin.transform.localPosition + (Vector3.up * 2);

        float elapsed = 0.0f;
        float duration = 0.25f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            coin.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        coin.transform.localPosition = endPos;

        yield return null;
    }
}
