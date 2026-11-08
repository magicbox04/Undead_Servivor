using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public float levelTime;
    public float eliteChance = 0.1f;

    int level;
    float timer;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);
        timer += Time.deltaTime;
        
        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
           
        }

    }

    void Spawn()
    {
        int isElite = Random.value < eliteChance ? 6 : 0;
        if (isElite == 6)
        {
            Debug.Log("This is elite spawn. You should see it");
        }
        GameObject enemy = GameManager.instance.pool.Get(isElite);

        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}
