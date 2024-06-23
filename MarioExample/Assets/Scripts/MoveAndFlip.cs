using UnityEngine;

public class MoveAndFlip : MonoBehaviour
{
    public enum DirectionFacing
    {
        Left = -1,
        Right = 1
    }

    public bool canMove = false;
    public bool canMoveAutomatic = true;
    public Vector2 speed = new(3, 0);

    private Rigidbody2D myRigidbody;

    private float minDistanceToMove = 14f;
    public DirectionFacing directionFacing = DirectionFacing.Right;

    float timeSinceOnScreen;


    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        OrientSprite();
    }

    void Update()
    {
        if (!canMove & Mathf.Abs(LevelManager.Instance.Mario.transform.position.x - transform.position.x) <= minDistanceToMove && canMoveAutomatic) {
            canMove = true;
        }

        if (canMove) {
            timeSinceOnScreen += Time.deltaTime;
        }

        // If moving too slow, automatically change directions
        if(Mathf.Abs(myRigidbody.velocity.magnitude) < 0.001f && canMove && !myRigidbody.isKinematic && timeSinceOnScreen > 1.0f) {
            ChangeDirection();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move() {
        if (canMove) {
            myRigidbody.velocity = new Vector2(speed.x * (int)directionFacing, myRigidbody.velocity.y);
        }
    }

    // Assuming default sprites face right
    void OrientSprite()
    {
        if ((int)directionFacing > 0) {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if ((int)directionFacing < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 normal = other.contacts[0].normal;
        Vector2 leftSide = new Vector2(-1f, 0f);
        Vector2 rightSide = new Vector2(1f, 0f);
        Vector2 bottomSide = new Vector2(0f, 1f);

        bool sideHit = normal == leftSide || normal == rightSide;
        bool bottomHit = normal == bottomSide;

        // reverse direction
        if (other.gameObject.tag != "Player" && sideHit)
        {
            ChangeDirection();
        }

        else if (other.gameObject.tag.Contains("Platform") && bottomHit && canMove) {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, speed.y);
        }
    }

    private void ChangeDirection()
    {
        int dir = (int)directionFacing;
        directionFacing = (DirectionFacing)(-dir);
        OrientSprite();
    }
}
