using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AgentData
{
    public int id;
    public PositionData position;
    public bool has_food;
}

[System.Serializable]
public class PositionData
{
    public float x;
    public float y;
}

[System.Serializable]
public class SimulationData
{
    public List<AgentData> agent_data;
    public List<FoodPositionData> food_positions;
    public PositionData deposit_position;
    public int num_steps;
}

[System.Serializable]
public class FoodPositionData
{
    public float x;
    public float y;
}

public class DataReceiver : MonoBehaviour
{
    private string serverUrl = "http://127.0.0.1:8585";  // Update this with your server URL

    public GameObject agentPrefab;   // Assign your agent prefab in the Unity Editor
    public GameObject foodPrefab;    // Assign your food prefab in the Unity Editor

    private List<GameObject> agentObjects = new List<GameObject>(); // List to store spawned agent objects
    private List<GameObject> foodObjects = new List<GameObject>();  // List to store spawned food objects

    private bool shouldFetchData = true; // Flag to control when to fetch new data
    private int stepCounter = 0; // Counter for the number of steps

    void Update()
    {
        if (shouldFetchData && stepCounter <= 570)
        {
            StartCoroutine(GetDataFromServer());
        }
    }

    IEnumerator GetDataFromServer()
    {
        shouldFetchData = false; // Set the flag to false while fetching data

        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                // Parse updated JSON data
                string json = webRequest.downloadHandler.text;
                Debug.Log(json);
                UpdateSimulationData(json);
                stepCounter++;
            }
        }

        shouldFetchData = true; // Set the flag back to true after fetching data
    }

    void UpdateSimulationData(string jsonData)
    {
        SimulationData data = JsonUtility.FromJson<SimulationData>(jsonData);

        // Update or create agents
        foreach (AgentData agentData in data.agent_data)
        {
            int agentID = agentData.id;
            Vector2 position = new Vector2(agentData.position.x, agentData.position.y);
            bool hasFood = agentData.has_food;

            GameObject agentObject = agentObjects.Find(obj => obj != null && obj.GetInstanceID() == agentID);

            if (agentObject == null)
            {
                // Agent not found, create it
                agentObject = Instantiate(agentPrefab, position, Quaternion.identity);
                agentObjects.Add(agentObject);
            }
            else
            {
                // Agent found, move it to the new position
                agentObject.transform.position = position;
            }
        }

        // Update or create food objects
        foreach (FoodPositionData foodPosition in data.food_positions)
        {
            GameObject foodObject = foodObjects.Find(obj => obj != null && obj.transform.position == new Vector3(foodPosition.x, foodPosition.y, 0));

            if (foodObject == null)
            {
                // Food not found, create it
                foodObject = Instantiate(foodPrefab, new Vector3(foodPosition.x, foodPosition.y, 0), Quaternion.identity);
                foodObjects.Add(foodObject);
            }
        }

        // Other updates (e.g., deposit position, num_steps) can be handled similarly
    }
}
