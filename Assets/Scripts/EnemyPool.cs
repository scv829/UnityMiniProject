using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] int[] size;                // 마지막 웨이브에 나올 각각의 적의 수
    [SerializeField] Enemy[] enemyPrefabs;      // 적의 프리팹
    [SerializeField] List<Enemy>[] enemyPool;   // 적이 들어 있는 오브젝트 풀
    [SerializeField] Transform spawnPoint;      // 스폰 포인트

    [SerializeField] int[] enemies;             // 풀에 들어있는 몬스터의 수
    [SerializeField] int[] spanwEnemeies;       // 이번 웨이브에 소환할 각 타입의 몬스터의 수
    [SerializeField] int totalEnemiesCount;     // 이번 웨이브에 등장하는 모든 적의 수

    private void Awake()
    {
        enemies = new int[enemyPrefabs.Length];
        spanwEnemeies = new int[enemyPrefabs.Length];
        enemyPool = new List<Enemy>[enemyPrefabs.Length];

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            enemyPool[i] = new List<Enemy>();
        }

        for (int index = 0; index < enemyPrefabs.Length; index++)
        {
            enemies[index] = size[index];
            for (int i = 0; i < size[index]; i++)
            {
                Enemy instance = Instantiate(enemyPrefabs[index]);
                instance.Type = index;
                instance.gameObject.SetActive(false);
                instance.transform.parent = transform;
                enemyPool[index].Add(instance);
            }
        }
    }

    private void Start()
    {
        Setting();
    }

    public void Setting()
    {
        GameManager.instance.GetWave(ref spanwEnemeies);
        totalEnemiesCount = spanwEnemeies.Sum();
    }

    public void StartSpawning()
    {
        StartCoroutine(spawnStart());
    }

    IEnumerator spawnStart()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        for(int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (spanwEnemeies[i] > 0)
            {
                for(int index = 0; index < spanwEnemeies[i]; index++)
                {
                    int count = enemyPool[i].Count;
                    Enemy instance = enemyPool[i][count - 1];
                    instance.transform.position = spawnPoint.position;
                    instance.transform.parent = null;
                    instance.ReturnPoll = this;

                    instance.gameObject.SetActive(true);

                    enemies[i]--;
                    enemyPool[i].RemoveAt(count - 1);

                    yield return new WaitForSeconds(2f);
                }
            }
        }
    }

    public void ReturnPool(int index, Enemy enemy)
    {
        // 죽은 몬스터는 다시 풀로
        enemies[index]++;
        enemy.gameObject.SetActive(false);
        enemy.transform.parent = transform;
        enemyPool[index].Add(enemy);

        // 그리고 몬스터 한마리 감소
        totalEnemiesCount--;

        if(totalEnemiesCount <= 0)
        {
            GameManager.instance.WaveClear();
        }
    }

}
