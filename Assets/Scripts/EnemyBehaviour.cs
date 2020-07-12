using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject movingPositionsHolder;
    Animator anim;
    public GameObject enemyBullet;
    public GameObject bulletHolder;
    public float shootSpeed = 5f;
    public float delayShoot = 3f;
    public bool canAttack = true;

    public List<Transform> movingPositions;
    public float waitTime = 4f;
    public float time = 0;
    public int i = 0;
    GameObject collider;
    public bool shoot = false;
    public Sprite die;

    private float shootTime;
    void Start()
    {
        anim = GetComponent<Animator>();
        collider = transform.GetChild(0).gameObject;
        movingPositions = new List<Transform>(movingPositionsHolder.GetComponentsInChildren<Transform>());
        movingPositions.Remove(movingPositionsHolder.transform);
        anim.SetBool("shoot", false);
    }

    private void Update()
    {
        if (canAttack)
        checkPlayerInSight();
        if (!shoot || !canAttack) moveToNext();
        
    }

    private void LateUpdate()
    {
        if (canAttack)
        shootAtPlayer();
    }
    void moveToNext()
    {
        Transform mp = movingPositions[i];
        Vector2 dir = -movingPositions[i].transform.position + gameObject.transform.position;
        dir.Normalize();
        // collider.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI - 90);
        if (Vector2.Distance(movingPositions[i].transform.position, gameObject.transform.position) < 0.1f)
        {
            if (time > waitTime)
            {
                time = 0;
                if (i < movingPositions.Count - 1) i++;
                else i = 0;
            }
            time += Time.deltaTime;
            return;
        }
        gameObject.GetComponent<BotController>().target = new Vector3(mp.transform.position.x, mp.transform.position.y, 0);
    }

    void checkPlayerInSight()
    {
        if (collider.GetComponent<PolygonCollider2D>().bounds.Contains(GameObject.Find("Player").transform.position))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, GameObject.Find("Player").transform.position);
            if (hit.collider == null || (hit.collider.tag != "Object" && hit.collider.gameObject.name != "circle"))
            {
                shoot = true;
                //gameObject.GetComponent<Animator>().SetTrigger("shoot");
                shootAtPlayer();
                //gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else shoot = false;
    }

    public void shootAtPlayer()
    {
        if (shoot && shootTime >= delayShoot)
        {
            Transform dest = GameObject.Find("Player").transform;
            // face the player
            
            if (dest.position.x < transform.position.x)
            {
                transform.localScale = new Vector2(-1, 1);
            } else transform.localScale = new Vector2(1, 1);

            StartCoroutine(waitForShoot());
            Rigidbody2D go = Instantiate(enemyBullet, bulletHolder.transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
            
            Vector3 dir = (dest.position - transform.position).normalized;
            go.gameObject.transform.rotation = Quaternion.FromToRotation(transform.position, dir);
            go.velocity = new Vector2(dir.x, dir.y) * shootSpeed;
            shootTime = 0;
        }
        else if (!shoot)
        {
            anim.SetBool("shoot", false);
        }
        else
        {
            shootTime += Time.deltaTime;
        }
    }

    IEnumerator waitForShoot()
    {
        anim.SetBool("shoot", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("shoot", false);
        Debug.Log("Stopped shooting");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet_Friendly"))
        {
            StartCoroutine(animationBeforeDead());
        }
    }

    IEnumerator animationBeforeDead()
    {
        GetComponent<Animator>().SetTrigger("dead");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
