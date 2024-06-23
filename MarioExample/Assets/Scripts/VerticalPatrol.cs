using System.Collections;
using UnityEngine;

public class VerticalPatrol : MonoBehaviour
{
    public float upStopY;
    public float downStopY;
    public float speedModifier = 1;

    public float upStopTime = 1;
    public float downStopTime = 2;

    private float targetUp;
    private float targetDown;

    private float target;

    private void Start()
    {
        targetUp = transform.position.y + upStopY;
        targetDown = transform.position.y + downStopY;

        target = targetUp;

        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        Vector3 destination = new Vector3(transform.position.x, target, transform.position.z);
        while (Vector3.Distance(transform.position, destination) > 0.125f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speedModifier);
            yield return null;
        }

        transform.position = destination;
        yield return new WaitForSeconds(target == targetUp ? upStopTime : downStopTime);

        target = target == targetUp ? targetDown : targetUp;

        StartCoroutine(Move());
    }
}
