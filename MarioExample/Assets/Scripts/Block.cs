using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("References - DO NOT CHANGE")]
    public SpriteRenderer defaultRenderer;
    public SpriteRenderer emptyRenderer;
    protected List<GameObject> objectsOnTop = new List<GameObject>();

    protected IEnumerator Bounce()
    {
        float inLength = 0.1f;
        float outLength = 0.15f;

        float elapsedTime = 0.0f;
        Vector3 currentPos = defaultRenderer.transform.localPosition;

        while (elapsedTime < inLength)
        {
            defaultRenderer.transform.localPosition = Vector3.Lerp(currentPos, new Vector3(0.0f, 0.5f, 0.0f), (elapsedTime / inLength));
            emptyRenderer.transform.localPosition = defaultRenderer.transform.localPosition;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // reset for out
        elapsedTime = 0.0f;
        defaultRenderer.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        emptyRenderer.transform.localPosition = defaultRenderer.transform.localPosition;
        currentPos = defaultRenderer.transform.localPosition;
        yield return null;

        while (elapsedTime < outLength)
        {
            defaultRenderer.transform.localPosition = Vector3.Lerp(currentPos, Vector3.zero, (elapsedTime / outLength));
            emptyRenderer.transform.localPosition = defaultRenderer.transform.localPosition;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        defaultRenderer.transform.localPosition = Vector3.zero;
        emptyRenderer.transform.localPosition = defaultRenderer.transform.localPosition;
        yield return null;
    }

    // Add objects on top
    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 topSide = new Vector2(0, -1);
        bool topHit = normal == topSide;

        if (collision.gameObject.tag.Contains("Enemy") && topHit && !objectsOnTop.Contains(collision.gameObject))
        {
            objectsOnTop.Add(collision.gameObject);
        }
    }

    // Remove objects which are no longer on top
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy"))
        {
            objectsOnTop.Remove(collision.gameObject);
        }
    }
}
