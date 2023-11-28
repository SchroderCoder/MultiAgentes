using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AgentData
{
    public int id;
    public int[] position;
    public bool has_food;
}

[System.Serializable]
public class FoodData
{
    public int[] position;
}

[System.Serializable]
public class SimulationData
{
    public List<AgentData> agent_data;
    public List<FoodData> food;
    public int[] deposit_position;
}

public class DataReceiver : MonoBehaviour
{
    public string serverUrl = "http://127.0.0.1:8585"; 
    public GameObject agentPrefab;   
    public GameObject foodPrefab;
    public GameObject depositPrefab;

    private bool depositCreated = false;

    // Diccionario para mantener las instancias de comida
    public Dictionary<Vector2Int, GameObject> foodInstances;

    // Singleton de APIRequest
    public static DataReceiver Instance { get; private set; }

    private List<GameObject> agentObjects = new List<GameObject>(); // List to store spawned agent objects
    private bool shouldFetchData = true; // Flag to control when to fetch new data
    private int fetchCounter = 0; // Counter to limit the number of calls


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            foodInstances = new Dictionary<Vector2Int, GameObject>();
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(GetDataFromServer());
    }


    IEnumerator GetDataFromServer()
    {

        while(true) {

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
                    UpdateObjects(json);
                }
            }
            yield return new WaitForSeconds(0.3f);

        }

    }

    void UpdateObjects(string jsonData)
    {
        SimulationData data = JsonUtility.FromJson<SimulationData>(jsonData);
        Debug.Log("Received JSON: " + jsonData);

       /* foreach (var agentData in data.agent_data)
        {   

            int agentID = agentData.id;
            Vector2 newPosition = new Vector2(agentData.position.x, agentData.position.y); // Set z-coordinate to 0
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
        }*/
   

        if (!depositCreated)  {
                InstantiateDeposit(data.deposit_position);
                depositCreated = true;
        }




    }
    void InstantiateDeposit(int[] depositPosition)
    {
        Vector3 position3D = new Vector3(depositPosition[0], 0, depositPosition[1]);
        Instantiate(depositPrefab, position3D, Quaternion.identity);
    }
}