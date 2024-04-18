using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
    private List<int> timesHit = new List<int>();
    private int dictNum = 0;
    private float timer = 0f;
    private List<float> timerTimes = new List<float>();
    private List<int> numberOfEnemyAttack = new List<int>();
    private List<int> totalPlayerMoves = new List<int>();
    private List<string> policyUsed = new List<string>();
    private string currentPolicy;

    // Start is called before the first frame update
    private void Start()
    {
        currentPolicy = "Base";
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

    private void Update()
    {
        timerUpdates();
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
        timerTimes.Add(timer);
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
        timesHit.Add(0);
        numberOfEnemyAttack.Add(0);
        totalPlayerMoves.Add(0);
        createEnemyBehavior();
        timerReset();
        policyUsed.Add(currentPolicy);

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
        if (arrayOfBehaviors[dictNum].ContainsKey(lastAttack))
        {
            Dictionary<String, int> currentDict = arrayOfBehaviors[dictNum];
            currentDict[lastAttack]++;
        }
        else
        {
            arrayOfBehaviors[dictNum].Add(lastAttack, 1);
        }
        
    }

    private void createEnemyBehavior()
    {
        
        List<string> holdBehaviors = enemyBehavior;
        enemyBehavior.Clear();
        int swCount = 0;
        int spCount = 0;

        if (currentSceneIndex == 0)
        {
            Debug.Log("awww shit");
            for (int i = 0; i < 5; i++)
            {
                enemyBehavior.Add("Sw");
                enemyBehavior.Add("Sp");
            }
        }
        else
        {
            
            foreach (string key in arrayOfBehaviors[dictNum].Keys)
            {
                Debug.Log("wtf is going on");
                //TODO: Make based on count I STILL NEED TO DO THIS?????
                if (key == "Sw")
                {
                    int count = arrayOfBehaviors[dictNum][key];
                    for (int i = 0; i < count; i++)
                    {
                        //Counter to Sword
                        enemyBehavior.Add("Sp");
                        spCount++;
                    }
                    
                }
                else if (key == "Sp")
                {
                    //Counter to Spear (Still in work)
                    int count = arrayOfBehaviors[dictNum][key];
                    for (int i = 0; i < count; i++)
                    {
                        enemyBehavior.Add("Sw");
                        swCount++;
                    }
                    
                }
            }
            //TODO: Currently just returing mixed

            Debug.Log(swCount);
            Debug.Log(spCount);
            if (spCount > (swCount))
            {
                currentPolicy = "Anti-Sword";
                policyUsed.Add(currentPolicy);
            }
            else if (swCount > (spCount))
            {
                currentPolicy = "Anti-Spear";
                policyUsed.Add(currentPolicy);
            }
            else
            {
                currentPolicy = "Mixed";
                policyUsed.Add(currentPolicy);
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

    //Basic Increment and timer helpers
    public void increaseHit()
    {
        timesHit[dictNum]++;
    }

    private void timerUpdates()
    {
        timer += Time.deltaTime;
    }

    private void timerReset()
    {
        timer = 0;
    }

    public void recordEnemyAttack()
    {
        numberOfEnemyAttack[dictNum]++;
    }

    public void recordPlayerAttack()
    {
        totalPlayerMoves[dictNum]++;    
    }

    //**********************************************************************
    // Section: CSV recording & quiting
    // Description:


    //Fix on quit
    

    private void OnApplicationQuit()
    {
        
        if (timerTimes.Count != timesHit.Count)
        {
            timerTimes.Add(-1);
        }
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
            // Write header row
            writer.WriteLine($"dictNum,BehaviorName,Frequency, Times hit, Time to complete, Enemy Total Attacks, Player Total Attacks, Policy Used");

            for (int i = 0; i < arrayOfBehaviors.Length; i++)
            {
                

                // Write data rows for the dictionary
                foreach (var kvp in arrayOfBehaviors[i])
                {
                    writer.WriteLine($"{i}, {kvp.Key}, {kvp.Value}, {timesHit[i]}, {timerTimes[i]}, {numberOfEnemyAttack[i]}, {totalPlayerMoves[i]}, {policyUsed[i]}");
                }
            }
        }

        Debug.Log("CSV Exported Successfully!");
    }
}
