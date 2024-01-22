using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private int enemyCount;
    public string prefabTag = "PrefabTag";

    private int currentSceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        UpdateEnemyCount();
    }

    //Is called at the start and when Enemy Death is called
    //Will update the amount of enemies left on stage by checking for the "Enemy" Tag
    private void UpdateEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(prefabTag);
        enemyCount = enemies.Length;
        Debug.Log("Amount of enemies: " + enemyCount);
        if (enemyCount <= 0)
        {
            floorCleared();
        }
    }

    public void EnemyDeath()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            floorCleared();
        }
    }

    public void teleportCall()
    {
        if (enemyCount <= 0)
        {
            nextFloor();
        }
    }

    private void floorCleared()
    {
        Debug.Log("Floor Cleared");
    }

    private void nextFloor()
    {
        Debug.Log("Heading to next floor");
        currentSceneIndex++;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
