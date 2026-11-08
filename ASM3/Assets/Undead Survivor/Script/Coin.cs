using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum DropType { Exp, Magnet, Heal }
    public DropType type;

    public float coinDistForPull = 2f;
    public float defaultCoinDistForPull = 2f;
    public float coinForceToPull = 20f;
    public float healAmount = 30f;
    public bool isPulled;
    public float magnetTimer;

    Rigidbody2D rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        isPulled = false;
        rigid.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }    
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        switch (type)
        {
            case DropType.Exp:
                GameManager.instance.GetExp();
                break;

            case DropType.Magnet:
                coinsAreAllMine();
                break;

            case DropType.Heal:
                GameManager.instance.health = Mathf.Min(
                GameManager.instance.health + healAmount,
                GameManager.instance.maxHealth);
                break;
        }

        gameObject.SetActive(false);

    }

  

    private void FixedUpdate()
    {
        Vector2 directionToPlayer = (Vector2) GameManager.instance.player.transform.position - rigid.position;
        float distaneToPlayer = directionToPlayer.magnitude;
        
        if (distaneToPlayer > 30f)
        {
            gameObject.SetActive(false);
        }

        if (distaneToPlayer < coinDistForPull)
        {
            isPulled = true;
        }
            

        if (isPulled)
        {
            rigid.AddForce(directionToPlayer.normalized * coinForceToPull);
        }
            
    }

    void coinsAreAllMine()
    {
        Coin[] allCoin= Object.FindObjectsByType<Coin>(FindObjectsSortMode.None);

        foreach (var coin in allCoin)
        {
            coin.isPulled = true;
        }

    }
}
