using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class MainController : MonoBehaviour
{
    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;
    public float cameraOffset = 2.5f;
    public float dashSpeed = 2f;
    public Camera mainCamera;
    public GameObject bullet;
    public float shootSpeed;
    public float shootDelay;

    public bool isJumping;
    public bool isMovingLeft;
    public bool isMovingRight;
    public bool isShooting;
    public bool isGliding;
    public bool isProtecting;

    bool facingRight = true;
    float timePassed = 0;
    float moveDirection = 0;
    bool isGrounded = false;
    float ampSpeed;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    // Check every collider except Player and Ignore Raycast
    LayerMask layerMask = ~(1 << 2 | 1 << 8);
    Transform t;

    // Use this for initialization
    void Start()
    {
        t = transform;
        ampSpeed = 1;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;
        gameObject.layer = 8;

        if (mainCamera)
            cameraPos = mainCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement controls
        if (isMovingLeft || isMovingRight)
        {
            moveDirection = isMovingLeft ? -1 : 1;
        }
        else
        {
            if (isGrounded || r2d.velocity.magnitude < 0.01f)
            {
                moveDirection = 0;
            }
        }

        // Change facing direction
        if (moveDirection != 0)
        {
            if (moveDirection > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        }

        // Jumping
        if (isJumping && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
        }

        //Gliding
        if (isGliding && isGrounded)
        {
            ampSpeed = dashSpeed;
            r2d.gravityScale = 0;
        }
        else if (!isGliding)
        {
            ampSpeed = 1;
            r2d.gravityScale = 1;
        }

        if (isShooting && timePassed >= shootDelay)
        {
            GameObject b = Instantiate(bullet, t.position, t.rotation);
            b.transform.Translate(Vector2.right * moveDirection * shootSpeed);
            timePassed = 0;
        }
        timePassed += Time.deltaTime;
        // Camera follow
        if (mainCamera)
            mainCamera.transform.position = new Vector3(t.position.x + cameraOffset, t.position.y, cameraPos.z);
    }


    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0.1f, 0);
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, 0.23f, layerMask);

        // Apply movement velocity
        r2d.velocity = new Vector2((moveDirection) * maxSpeed * ampSpeed, r2d.velocity.y);

        // Simple debug
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, 0.23f, 0), isGrounded ? Color.green : Color.red);
    }
}
