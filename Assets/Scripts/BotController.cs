using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    Animator anim;
    public Vector3 target, direction;
    public float timeStop;
    public float speed = 0.5f;

    float timeCount;
    void Start()
    {
        anim = GetComponent<Animator>();
        timeStop = Random.Range(3.0f, 8.0f);
        speed = Random.Range(0.5f, 1.5f);
        timeCount = timeStop;
    }
    void setDirection()
    {
        if (Vector2.Distance(target, transform.position) >= 0.2f) 
        transform.localScale = new Vector2(1 * Mathf.Sign(direction.x), 1);
    }
    void Update()
    {
        if (!gameObject.GetComponent<EnemyBehaviour>().shoot)
        {
            timeStop -= Time.deltaTime;

            direction = -transform.position + target;
            setDirection();

            
            if (Vector2.Distance(target, transform.position) > 0.1) anim.SetBool("enemyWalk", true);
            else
            {
                anim.SetBool("enemyWalk", false);
            }
            

            this.gameObject.transform.position += direction.normalized * Time.deltaTime * speed;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.y * 10 + 100);
        }   

    }
}
