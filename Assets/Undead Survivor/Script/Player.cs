using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    public float dashDist = 3f;
    public float dashCooldown = 2f;
    public float dashCoolTimer;
    Vector2 lastDir = Vector2.right;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
    }

    private void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        if (dashCoolTimer > 0)
        {
            dashCoolTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);

    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
        if (inputVec != Vector2.zero)
        {
            lastDir = inputVec.normalized;
        }
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        anim.SetFloat("Speed", inputVec.magnitude);
        if (inputVec.x != 0)
        {
                spriter.flipX = inputVec.x < 0;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        GameManager.instance.health -= enemy.damage * Time.deltaTime;

        if (GameManager.instance.health < 0)
        {
            for (int i = 2; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }

    public void OnDash()
    {
        if (!GameManager.instance.isLive || dashCoolTimer > 0)
        {
            return;
        }

        rigid.position = rigid.position + lastDir * dashDist;

        dashCoolTimer = dashCooldown;
        
    }
}
