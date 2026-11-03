using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animatorController;
    public Rigidbody2D target;

    bool isAlive;

    Rigidbody2D rigid;
    Collider2D colider;
    SpriteRenderer spriter;
    Animator anim;
    WaitForFixedUpdate wait;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        colider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isAlive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            return;
        }


        Vector2 dirVec = target.position - rigid.position;
        Vector2 nestVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nestVec);
        rigid.linearVelocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        if (!isAlive)
            return;
        spriter.flipX = target.position.x < rigid.position.x;
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isAlive = true;
        colider.enabled = true;
        rigid.simulated = true;

        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animatorController[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isAlive)
        {
            return;
        }
        
        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.Playsfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isAlive = false;
            colider.enabled = false;
            rigid.simulated = false;

            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameObject coin = GameManager.instance.pool.Get(3);
            coin.transform.position = transform.position;

            float random = Random.value;
            if (random < 0.88f)
            {
                GameObject item = GameManager.instance.pool.Get(4);

                item.transform.position = transform.position + Vector3.up * 0.5f;

            }

            if (random < 0.88f)
            {
                GameObject item = GameManager.instance.pool.Get(5);
                item.transform.position = transform.position + Vector3.up * 0.5f;

            }

            if (GameManager.instance.isLive)
            {
                AudioManager.instance.Playsfx(AudioManager.Sfx.Dead);
            }
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;

        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }


    void Dead()
    {
        gameObject.SetActive(false);
    }
    
}
