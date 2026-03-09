using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 6;
    public int currentWave = 0;
    public TextMeshProUGUI waveText;
    public GameObject boss;

    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        SpawnWave();
    }

    void Update()
    {
        currentEnemies.RemoveAll(e => e == null);

        if (currentEnemies.Count == 0 && !isSpawning)
        {
            isSpawning = true;
            StartCoroutine(NextWave());
        }
    }

    IEnumerator NextWave()
    {
        yield return new WaitForSeconds(3f);
        currentWave++;
        enemiesPerWave += 1;
        SpawnWave();
        isSpawning = false;
    }

    void SpawnWave() //systeme de vague
    {
        Debug.Log("Vague " + (currentWave + 1) + " !");
        StartCoroutine(ShowWaveText());

        for (int i = 0; i < enemiesPerWave; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject en = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            enemy enemyScript = en.GetComponent<enemy>();
            if (enemyScript != null)
            {
                enemyScript.HP += currentWave * 20;
                enemyScript.damage += currentWave * 5;
                enemyScript.moveSpeed += currentWave * 0.3f;
            }

            currentEnemies.Add(en);
            ninja playerNinja = GameObject.FindGameObjectWithTag("Player").GetComponent<ninja>();
            if (playerNinja != null)
            {
                playerNinja.HP = playerNinja.maxHP;
            }
           
        }
        if (currentWave == 3)
        {
            Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            Instantiate(boss, spawnPos, Quaternion.identity);
            Debug.Log("boss spawn");

        }
    }

    IEnumerator ShowWaveText() //UI
    {
        if (waveText != null)
        {
            waveText.text = "Vague " + (currentWave + 1);
            waveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            waveText.gameObject.SetActive(false);
        }
    }
}