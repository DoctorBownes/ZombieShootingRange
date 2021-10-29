using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject Zombie;
    [SerializeField] private Vector2 radius;
    [SerializeField] private int minChance = 0;
    [SerializeField] private int maxChance = 0;
    [SerializeField] private float TimeSpawn = 0;
    void Start()
    {
        StartCoroutine(ZombieSpawner());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator ZombieSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeSpawn);
            int i = Random.Range(minChance, maxChance);
            if (i == 5)
            {
                SpawnRandomZombie();
            }
            Debug.Log(TimeSpawn);
        }
    }

    void SpawnRandomZombie()
    {
        Vector3 RandomPos = this.transform.position + new Vector3(Random.Range(-radius.x / 2, radius.x / 2), Random.Range(-radius.y / 2, radius.y / 2));
        Instantiate(Zombie, RandomPos, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, radius.x / 2);
    }
}
