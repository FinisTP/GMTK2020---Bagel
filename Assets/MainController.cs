using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class MainController : MonoBehaviour
{
    public Image healthBar;

    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;
    public float cameraOffset = 2.5f;
    public float dashSpeed = 2f;
    public Camera mainCamera;
    public GameObject bullet;
    public float attackSpeed;
    public float attackDelay;
    public float protectPower;
    public float maxHealth;

    public Collider2D attackTriggerLeft;
    public Collider2D attackTriggerRight;
    public Collider2D attackTriggerUp;
    public Collider2D attackTriggerDown;

    public bool isJumping;
    public bool isMovingLeft;
    public bool isMovingRight;
    public bool isAttacking;
    public bool isRecovering;
    public bool isProtecting;

    bool facingRight = true;
    float timePassed = 0;
    float moveDirection = 0;
    bool isGrounded = false;
    float ampSpeed;
    float currHealth;
    float attackPower;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    Collider2D attackCollider;
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
        currHealth = maxHealth;

        if (mainCamera)
            cameraPos = mainCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        move();
        attack();
        jump();
        if (mainCamera)
            mainCamera.transform.position = new Vector3(t.position.x + cameraOffset, t.position.y, cameraPos.z);
        healthBar.fillAmount = currHealth / maxHealth;
    }

    public void triggerMove(Direction direction, float power)
    {
        if (direction == Direction.right) isMovingRight = true;
        else if (direction == Direction.left) isMovingLeft = true;
        maxSpeed = Mathf.Abs(power);
    }

    public void triggerJump(int power)
    {
        isJumping = true;
        jumpHeight = power;
    }

    public void triggerProtect(float power)
    {
        protectPower = power;
        isProtecting = true;
    }

    public void triggerAttack(Direction dir, float power)
    {
        switch (dir)
        {
            case Direction.right:
                attackCollider = attackTriggerRight;
                break;
            case Direction.left:
                attackCollider = attackTriggerLeft;
                break;
            case Direction.up:
                attackCollider = attackTriggerUp;
                break;
            case Direction.down:
                attackCollider = attackTriggerDown;
                break;
            default: break;
        }
        isAttacking = true;
        attackPower = power;
    }

    public void move()
    {
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
    }

    public void jump()
    {
        if (isJumping && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
        }
    }

    public void attack()
    {
        if (isAttacking && timePassed >= attackDelay)
        {
            attackCollider.enabled = true;
            timePassed = 0;
        }
        else
        {
            if (attackCollider)
            attackCollider.enabled = false;
        }
        timePassed += Time.deltaTime;
    }

    public void takeDamage(float dmg)
    {
        if (!isProtecting && dmg >= 0)
        {
            currHealth -= dmg;
            if (currHealth >= maxHealth) currHealth = maxHealth;
            if (currHealth <= 0) gameOver();
        }
        {
            currHealth -= dmg;
            if (currHealth >= maxHealth) currHealth = maxHealth;
        }
        
    }

    public void gameOver()
    {
        Time.timeScale = 0;
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
