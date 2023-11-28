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
    // Add other data as needed
}

public class DataReceiver : MonoBehaviour
{
    public string serverUrl = "http://127.0.0.1:8585"; // Update this with your server URL
    public GameObject agentPrefab;   // Assign your agent prefab in the Unity Editor

    private List<GameObject> agentObjects = new List<GameObject>(); // List to store spawned agent objects
    private bool shouldFetchData = true; // Flag to control when to fetch new data
    private int fetchCounter = 0; // Counter to limit the number of calls

    void Start()
    {
        // Create 5 agents with unique IDs
        for (int i = 0; i < 5; i++)
        {
            CreateAgent(i);
        }
    }

    void CreateAgent(int id)
    {
        Vector3 position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f); // Random initial position
        GameObject agentObject = Instantiate(agentPrefab, position, Quaternion.identity);
        agentObjects.Add(agentObject);

        // Set unique ID and other properties
        AgentScript agentScript = agentObject.GetComponent<AgentScript>();
        if (agentScript != null)
        {
            agentScript.SetAgentProperties(id, position, false); // Set initial properties
        }
        else
        {
            Debug.LogError("AgentScript component not found on agentPrefab.");
        }
    }

    void Update()
    {
        if (shouldFetchData && fetchCounter < 570)
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
                UpdateAgents(json);
            }
        }

        shouldFetchData = true; // Set the flag back to true after fetching data
        fetchCounter++;
    }

    void UpdateAgents(string jsonData)
    {
        SimulationData data = JsonUtility.FromJson<SimulationData>(jsonData);


        foreach (AgentData agentData in data.agent_data)
        {   
            Debug.Log(agentData.position.x);
            Debug.Log(agentData.position.y);

            int agentID = agentData.id;
            Vector3 newPosition = new Vector3(agentData.position.x, agentData.position.y, 0f); // Set z-coordinate to 0
            bool hasFood = agentData.has_food;

            GameObject agentObject = agentObjects.Find(agent => agent.GetComponent<AgentScript>().AgentID == agentID);
            if (agentObject != null)
            {
                // Update the agent position and properties
                AgentScript agentScript = agentObject.GetComponent<AgentScript>();
                agentScript.SetAgentProperties(agentID, newPosition, hasFood);
                agentScript.MoveTo(newPosition); // Move the agent to the new position
            }
            else
            {
                Debug.LogError("Agent object not found for ID: " + agentID);
            }
        }
    }
}