using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{

    public GameObject agentPrefab;
    public GameObject foodPrefab;
    public GameObject depositPrefab;
        [System.Serializable]
    public class AgentData
    {
        public int agent_id;
        public Vector2 position;
        public bool has_food;
    }

    [System.Serializable]
    public class FoodData
    {
        public Vector2 position;
    }

    [System.Serializable]
    public class DepositData
    {
        public Vector2 position;
    }

    [System.Serializable]
    public class StepData
    {
        public int num_steps;
    }

    [System.Serializable]
    public class SimulationData
    {
        public List<AgentData> agent_positions;
        public List<FoodData> food_positions;
        public DepositData deposit_position;
        public StepData step_data;
    }
    
    IEnumerator GetData()
    {
        string url = "http://localhost:8585";
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                Debug.Log("Received data: " + www.downloadHandler.text);
                ProcessData(www.downloadHandler.text);
            }
        }
    }

    void ProcessData(string jsonData)
    {
        // Deserialize the JSON data into custom classes or structures
        SimulationData simulationData = JsonUtility.FromJson<SimulationData>(jsonData);

        // Access agent, food, deposit, and step data
        List<AgentData> agentDataList = simulationData.agent_positions;
        List<FoodData> foodDataList = simulationData.food_positions;
        DepositData depositData = simulationData.deposit_position;
        StepData stepData = simulationData.step_data;

        // Process and update Unity scene based on the received data
        UpdateAgentObjects(agentDataList);
        UpdateFoodObjects(foodDataList);
        UpdateDepositObject(depositData);

        // Access and use the number of steps
        Debug.Log("Number of Steps: " + stepData.num_steps);

        // Additional processing if needed
    }

    void UpdateAgentObjects(List<AgentData> agentDataList)
    {
        // Update Unity objects representing agents based on agentDataList
        foreach (var agentData in agentDataList)
        {
            Debug.Log($"Agent {agentData.agent_id}: Position = {agentData.position}, Has Food = {agentData.has_food}");
        }
    }

    void UpdateFoodObjects(List<FoodData> foodDataList)
    {
        // Update Unity objects representing food based on foodDataList
        foreach (var foodData in foodDataList)
        {
            Debug.Log($"Food Position: {foodData.position}");
        }
    }

    void UpdateDepositObject(DepositData depositData)
    {
        // Update Unity object representing the deposit based on depositData
        if (depositData != null)
        {
            Debug.Log($"Deposit Position: {depositData.position}");
        }
    }

    void Start()
    {
        StartCoroutine(GetData());
    }
}