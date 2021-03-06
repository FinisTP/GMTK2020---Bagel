﻿using System.Collections;
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
    public GameObject bulletHori;
    public GameObject bulletVerti;
    public float attackSpeed;
    public float attackDelay;
    public float protectPower;
    private int maxHealth = 3;
    private int currHealth;
    public Direction hitDir;

    public Sprite[] healthStages;

    public Collider2D attackTriggerLeft;
    public Collider2D attackTriggerRight;
    public Collider2D attackTriggerUp;
    public Collider2D attackTriggerDown;

    public Transform shootRight;
    public Transform shootUp;
    public Transform shootDown;
    public Transform shootLeft;

    public bool isJumping;
    public bool isMovingLeft;
    public bool isMovingRight;
    public bool isAttacking;
    public bool isRecovering;
    public bool isProtecting;
    public bool isGliding;
    public bool isGameOver;
    
    bool facingRight = true;
    float timePassed = 0;
    float oldMaxSpeed;
    float moveDirection = 0;
    bool isGrounded = false;
    float ampSpeed;
    float attackPower;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    Collider2D attackCollider;
    Animator anim;
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
        anim = GetComponent<Animator>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;
        gameObject.layer = 8;
        currHealth = maxHealth;
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();

        if (mainCamera)
            cameraPos = mainCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        move();
        attack();
        jump();
        gameOver();
        protect();
        recover();
        glide();
        updateHealthBar();
        if (mainCamera)
            mainCamera.transform.position = new Vector3(t.position.x + cameraOffset, t.position.y, cameraPos.z);
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
        //anim.SetBool("protect", true);
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
        hitDir = dir;
        isAttacking = true;
        attackPower = power;
    }

    public void triggerGlide(float power)
    {
        dashSpeed = power;
        isGliding = true;
    }

    public void updateHealthBar() {
        if (currHealth <= 3 && currHealth > 0)
            healthBar.sprite = healthStages[currHealth-1];
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
                //anim.SetBool("playerWalk", true);
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
                
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
            anim.SetBool("playerWalk", true);
            anim.SetFloat("OldHorizontal", moveDirection);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.y * 10 + 100);
        }
        else
        {
            anim.SetBool("playerWalk", false);
        }
        anim.SetFloat("Horizontal", moveDirection * maxSpeed);
    }

    public void jump()
    {
        if (isJumping && isGrounded)
        {
            anim.SetBool("playerJump", true);
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
        }
        else if (!isJumping && isGrounded)
        {
            anim.SetBool("playerJump", false);
        }
    }

    public void glide()
    {
        if (isGliding)
        {
            r2d.gravityScale = 0;
            oldMaxSpeed = maxSpeed;
            maxSpeed = dashSpeed;
            anim.SetBool("dash", true);
            if (!isMovingLeft && !isMovingRight)
            {
                StartCoroutine(waitForDash());
            }
        }
        else
        {
            r2d.gravityScale = 1;
            anim.SetBool("dash", false);
        }        
    }

    IEnumerator waitForDash()
    {
        if (facingRight) triggerMove(Direction.right, dashSpeed);
        else triggerMove(Direction.left, dashSpeed);
        yield return new WaitForSeconds(0.1f);
        isMovingLeft = isMovingRight = false;
        maxSpeed = oldMaxSpeed;
        //GameObject.Find("GameManager").GetComponent<GameManager>().pauseCards(CardType.move);
    }

    public void protect()
    {
        if (isProtecting)
        {
            anim.SetBool("protect", true);
        }
        else
        {
            anim.SetBool("protect", false);
        }
    }

    public void attack()
    {
        
        if (isAttacking)
        {
            Debug.Log("Aattacking");
            attackCollider.enabled = true;
            Vector2 spawnPos; GameObject go;
            switch (hitDir)
            {
                case Direction.left: // behind
                    StartCoroutine(waitForAttack("punch"));
                    spawnPos = new Vector2(shootLeft.position.x, shootLeft.position.y);
                    go = Instantiate(bulletHori, spawnPos, Quaternion.identity);
                    if (facingRight)
                        go.GetComponent<Rigidbody2D>().velocity = Vector2.left * attackSpeed;
                    else go.GetComponent<Rigidbody2D>().velocity = Vector2.right * attackSpeed;
                    break;
                case Direction.right: // forward
                    StartCoroutine(waitForAttack("punch"));
                    spawnPos = new Vector2(shootRight.position.x, shootRight.position.y);
                    go = Instantiate(bulletHori, spawnPos, Quaternion.identity);
                    if (facingRight)
                    go.GetComponent<Rigidbody2D>().velocity = Vector2.right * attackSpeed;
                    else go.GetComponent<Rigidbody2D>().velocity = Vector2.left * attackSpeed;
                    break;
                case Direction.up:
                    StartCoroutine(waitForAttack("punchUp"));
                    spawnPos = new Vector2(shootUp.position.x, shootUp.position.y);
                    go = Instantiate(bulletVerti, spawnPos, Quaternion.identity);
                    go.GetComponent<Rigidbody2D>().velocity = Vector2.up * attackSpeed;
                    break;
                case Direction.down:
                    StartCoroutine(waitForAttack("punchDown"));
                    spawnPos = new Vector2(shootDown.position.x, shootDown.position.y);
                    go = Instantiate(bulletVerti, spawnPos, Quaternion.identity);
                    go.GetComponent<Rigidbody2D>().velocity = Vector2.down * attackSpeed;
                    break;
            }
            
            //timePassed = 0;
        }
        else
        {
            if (attackCollider)
            attackCollider.enabled = false;
            
        }
        //timePassed += Time.deltaTime;
    }

    IEnumerator waitForAttack(string punchDir)
    {
        anim.SetBool(punchDir, true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool(punchDir, false);
    }

    public void takeDamage(float dmg)
    {
        //Debug.Log("Took damage" + currHealth.ToString());
        if (!isProtecting && !isGliding && dmg >= 0)
        {
            currHealth -= 1;
            if (currHealth >= maxHealth) currHealth = maxHealth;
            StartCoroutine(inflict());
            if (currHealth <= 0) isGameOver = true;
        } else if (dmg < 0)
        {
            currHealth -= (int)dmg;
            if (currHealth >= maxHealth) currHealth = maxHealth;
        }
        //Debug.Log("Took damage" + currHealth.ToString());
    }

    public void recover()
    {
        if (isRecovering)
        {
            anim.SetBool("recover", true);
            currHealth = maxHealth;
        }
        else anim.SetBool("recover", false);
    }

    IEnumerator inflict()
    {
        anim.SetBool("takeDamage", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("takeDamage", false);
        yield return null;
    }

    public void gameOver()
    {
        if (isGameOver)
        GameObject.Find("SceneChanger").GetComponent<SceneChanger>().LoadScene("GameOver");
        //Time.timeScale = 0;
    }

    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0.1f, 0);
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, 0.23f, layerMask);
        if (!isGrounded) anim.SetBool("onGround", false);
        else anim.SetBool("onGround", true);

        // Apply movement velocity
        r2d.velocity = new Vector2((moveDirection) * maxSpeed * ampSpeed, r2d.velocity.y);

        // Simple debug
        // Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, 0.23f, 0), isGrounded ? Color.green : Color.red);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Bullet":
                takeDamage(1);
                Destroy(collision.gameObject);
                break;
            case "Enemy":
                takeDamage(1);
                break;
        }
    }
}
