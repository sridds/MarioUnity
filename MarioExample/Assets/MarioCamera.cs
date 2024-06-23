using UnityEngine;

public class MarioCamera : MonoBehaviour
{
    public bool followXAxis = true;
    public bool followYAxis = false;
    public bool onlyFollowYUp = true;
    public bool lockRightSide = true;

    private float startY;
    private Transform mario;

    // Start is called before the first frame update
    void Start()
    {
        mario = FindObjectOfType<MarioMovement>().gameObject.transform;
        startY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (LevelManager.Instance.isDead) return;

        Vector3 pos = transform.position;

        if (followXAxis)
        {
            if (lockRightSide) {
                pos.x = Mathf.Max(transform.position.x, mario.position.x);
            }
            else {
                pos.x = mario.transform.position.x;
            }
        }

        if (followYAxis)
        {
            if (onlyFollowYUp) {
                pos.y = Mathf.Max(startY, mario.transform.position.y);
            }
            else {
                pos.y = mario.transform.position.y;
            }
        }

        transform.position = pos;
    }
}