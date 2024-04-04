using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Stage Tracking")]
    private int enemyCount;
    public string prefabTag = "PrefabTag";
    private TopDownCharacterController player;

    [Header("Scene Management")]
    private int currentSceneIndex;

    [Header("DDA Elements")]
    //Each dictionary then has a list of possible moves and from there ++ count
    Dictionary<string, int>[] arrayOfBehaviors = new Dictionary<string, int>[2];
    private string lastAttack = "";
    private bool dictReady = false;
    List<string> enemyBehavior = new List<string>();
    private int dictNum = 0;

    // Start is called before the first frame update
    private void Start()
    {
        if (!dictReady)
        {
            for (int i = 0; i < arrayOfBehaviors.Length; i++)
            {
                arrayOfBehaviors[i] = new Dictionary<string, int>();

            }
            dictReady = true;
        }
        OnNewFloor();
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
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnNewFloor()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(currentSceneIndex);
        UpdateEnemyCount();
        player = GetComponent<TopDownCharacterController>();
        createEnemyBehavior();

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentSceneIndex >= SceneManager.GetActiveScene().buildIndex)
        {
            OnNewFloor();
        }
    }

    //******************************************************************************
    //Section: DDA
    //In charge of keeping a record of the moves that the player has performed
    public void receiveData(string attack)
    {
        lastAttack = attack;
        playerAIData();
    }

    //Section Dealing with the Dyanamic AI
    private void playerAIData()
    {
        Debug.Log("hit that tack");
        if (arrayOfBehaviors[dictNum].ContainsKey(lastAttack))
        {
            Debug.Log("Twice nice");
            Dictionary<String, int> currentDict = arrayOfBehaviors[dictNum];
            currentDict[lastAttack]++;
        }
        else
        {
            Debug.Log("hope to see this first");
            arrayOfBehaviors[dictNum].Add(lastAttack, 1);
        }
        
    }

    private void createEnemyBehavior()
    {
        enemyBehavior.Clear();
        if (currentSceneIndex == 0)
        {  
            for (int i = 0; i < 5; i++)
            {
                enemyBehavior.Add("Sw");
                enemyBehavior.Add("Sp");
            }
        }
        else
        {
            Debug.Log("Dict Num :" + dictNum);
            Debug.Log(arrayOfBehaviors[dictNum].Count);
            foreach (string key in arrayOfBehaviors[dictNum].Keys)
            {
                Debug.Log("Key : " + key);
                //TODO: Make based on count
                if (key == "Sw")
                {
                    //Counter to Sword
                    enemyBehavior.Add("Sp");
                }
                else if (key == "Sp")
                {
                    //Counter to Spear (Still in work)
                    enemyBehavior.Add("Sw");
                }
            }
            dictNum++;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(prefabTag);
        foreach (GameObject enemy in enemies)
        {
            Enemy script = enemy.GetComponent<Enemy>();
            script.getBehavior(enemyBehavior);
            
        }

    }

    //**********************************************************************
    // Section: CSV recording & quiting
    // Description:


    //Fix on quit
    

    private void OnApplicationQuit()
    {
        ExportArrayOfDictionariesToCSV();
    }


    // Method to export array of dictionaries to CSV file
    public void ExportArrayOfDictionariesToCSV()
    {
        // File path relative to the Assets directory
        string filePath = "Assets/CSVs/Behaviors.csv";

        // Create or overwrite the CSV file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < arrayOfBehaviors.Length; i++)
            {
                // Write header row
                writer.WriteLine($"dictNum,BehaviorName,Frequency");

                // Write data rows for the dictionary
                foreach (var kvp in arrayOfBehaviors[i])
                {
                    writer.WriteLine($"{i},{kvp.Key},{kvp.Value}");
                }

                // Write empty line as a separator between dictionaries
                writer.WriteLine();
            }
        }

        Debug.Log("CSV Exported Successfully!");
    }
}
