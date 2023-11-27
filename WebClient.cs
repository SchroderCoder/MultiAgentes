using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    IEnumerator GetData(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("bundle", "the data");
        string url = "http://localhost:8585";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Error:" + www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
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
    }

    void UpdateFoodObjects(List<FoodData> foodDataList)
    {
        // Update Unity objects representing food based on foodDataList
    }

    void UpdateDepositObject(DepositData depositData)
    {
        // Update Unity object representing the deposit based on depositData
    }

    void Start()
    {
        StartCoroutine(GetData("http://127.0.0.1:5000"));
    }
}