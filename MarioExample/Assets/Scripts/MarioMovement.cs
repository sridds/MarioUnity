using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioMovement : MonoBehaviour
{
    [Header("References - DO NOT CHANGE!")]
    public SpriteRenderer marioSprite;
    public Fireball fireBall;

    [Header("Movement Variables")]
    public float movementSpeed = 8.0f;
    public float acceleration = 3.0f;
    public LayerMask groundLayer;

    [Header("Jump Variables")]
    public float jumpForce = 5.0f;
    public float maxJumpTime = 1.0f;

    // Accessible from other scripts, not accessible from Unity Editor.
    public bool isGrounded { get; private set; }
    private bool isCrouching;
    private bool isChangingDirections;
    private bool isBumping;

    // References to Unity components. These are cached privately and located in the Awake() method.
    private Rigidbody2D myRigidbody;
    private BoxCollider2D myCollider;
    private Camera myCamera;
    private Animator animator;

    // Private member variables
    private float inputAxis;
    private float xVelocity;

    private float jumpTimeCounter;
    private bool isJumping;
    private bool jumpCancelled;
    public bool flipX { get; private set; }

    private float fireTime1, fireTime2;
    private float waitBetweenFire = .2f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        myCamera = Camera.main;
    }

    private void OnEnable()
    {
        myRigidbody.isKinematic = false;
        myCollider.enabled = true;
        xVelocity = 0;
        isJumping = false;
    }

    private void OnDisable()
    {
        myRigidbody.isKinematic = true;
        myRigidbody.velocity = Vector2.zero;
        myCollider.enabled = false;
        xVelocity = 0;
        isJumping = false;
        isCrouching = false;
        flipX = false;
        marioSprite.flipX = false;
    }

    private void Update()
    {
        HorizontalMovement();

        // Detect objects above and below by casting a circle and detecting what falls within
        isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 0.5f), 0.2f, groundLayer);
        isBumping = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + 0.5f + (myCollider.offset.y * 2)), 0.2f, groundLayer);

        isCrouching = isGrounded && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow));

        // This checks if there is something above the player which would cause Mario to bump his head and fall back down. Without it, Mario would float as the player holds jump
        if (isBumping && !jumpCancelled) jumpCancelled = true;
        if (isGrounded) jumpCancelled = false;
        if (isCrouching) inputAxis = 0f;

        // Handle size
        if(LevelManager.Instance.marioSize > 0 && !isCrouching) {
            myCollider.size = new Vector2(myCollider.size.x, 1.9f);
            myCollider.offset = new Vector2(myCollider.offset.x, 0.45f);
        }
        else {
            myCollider.size = new Vector2(myCollider.size.x, 0.9f);
            myCollider.offset = new Vector2(myCollider.offset.x, -0.05f);
        }

        // Start jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            ActivateJump();
        }

        if(LevelManager.Instance.marioSize == 2 && Input.GetKeyDown(KeyCode.X)) {
            fireTime2 = Time.time;

            if(fireTime2 - fireTime1 >= waitBetweenFire) {
                animator.SetTrigger("isFire");

                float dir = flipX ? -1 : 1;
                Fireball f = Instantiate(fireBall, new Vector2(transform.position.x + (dir * 0.5f), transform.position.y + 0.5f), Quaternion.identity);

                f.SetDirection(dir);

                LevelManager.Instance.sfxSource.PlayOneShot(LevelManager.Instance.fireThrowSound);
                fireTime1 = Time.time;
            }
        }

        HandleJump();
        
        // Stop jumping
        if (Input.GetKeyUp(KeyCode.Space) || jumpCancelled) {
            isJumping = false;
        }
        
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        animator.SetInteger("marioSize", LevelManager.Instance.marioSize);
        animator.SetFloat("absSpeed", Mathf.Abs(xVelocity));
        isChangingDirections = (xVelocity > 0 && inputAxis < 0 || xVelocity < 0 && inputAxis > 0);

        // Skidding
        if (isChangingDirections) {
            if (Mathf.Abs(xVelocity) >= 2) {
                animator.SetBool("isSkidding", true);
            }
            else {
                animator.SetBool("isSkidding", false);
            }
        } else {
            animator.SetBool("isSkidding", false);
        }

        // Jumping / falling animations
        if (isJumping) {
            animator.SetBool("isJumping", true);
        } else if(!isGrounded && !isJumping) {
            animator.SetBool("isFalling", true);
        }

        if (isCrouching) animator.SetBool("isCrouching", true);
        else animator.SetBool("isCrouching", false);

        // Reset animation bools
        if (isGrounded) {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            marioSprite.flipX = flipX;
        }
        else marioSprite.flipX = xVelocity < 0;
    }
    private void HorizontalMovement()
    {
        if (!isCrouching) {
            inputAxis = Input.GetAxis("Horizontal");

        }
        if(Input.GetAxis("Horizontal") != 0) flipX = inputAxis < 0;
        
        xVelocity = Mathf.MoveTowards(xVelocity, inputAxis * movementSpeed, movementSpeed * acceleration * Time.deltaTime);
    }

    private void ActivateJump() 
    {
        // jump sound
        AudioClip clip = LevelManager.Instance.marioSize > 0 ? LevelManager.Instance.jumpSuperSound : LevelManager.Instance.jumpSmallSound;
        LevelManager.Instance.sfxSource.PlayOneShot(clip);

        isJumping = true;
        jumpTimeCounter = maxJumpTime;
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
    }

    public void BoostMario()
    {
        ActivateJump();
    }

    private void HandleJump()
    {
        if (!isJumping) return;

        if(Input.GetKey(KeyCode.Space))
        {
            if (jumpTimeCounter > 0)
            {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else if (jumpTimeCounter < 0)
            {
                isJumping = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // Changes velocity accordingly
        myRigidbody.velocity = new Vector2(xVelocity, myRigidbody.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") {
            if (transform.DotTest(collision.transform, Vector2.down))
            { 
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
                isJumping = true;
            }
        }
    }
}
