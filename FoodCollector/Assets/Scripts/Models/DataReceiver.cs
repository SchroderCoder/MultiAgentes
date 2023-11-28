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
    public List<FoodData> food_positions;
    public int[] deposit_position;
}

public class DataReceiver : MonoBehaviour
{
    public string serverUrl = "http://127.0.0.1:8585"; 
    public GameObject agentPrefab;   
    public GameObject foodPrefab;
    public GameObject depositPrefab;

    private bool depositCreated = false;

    // Dictionary to keep track of food instances
    public Dictionary<Vector2Int, GameObject> foodInstances;

    // Singleton of APIRequest
    public static DataReceiver Instance { get; private set; }

    private List<GameObject> agentObjects = new List<GameObject>(); // List to store spawned agent objects
    private List<GameObject> foodObjects; // List to store spawned food objects
    private bool shouldFetchData = true; // Flag to control when to fetch new data
    private int fetchCounter = 0; // Counter to limit the number of calls

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            foodObjects = new List<GameObject>(); // Initialize the foodObjects list
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
        foodObjects = new List<GameObject>(); // Initialize the foodObjects list
        StartCoroutine(GetDataFromServer());
    }

    IEnumerator GetDataFromServer()
    {
        while(true)
        {
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

        foreach (var agentData in data.agent_data)
        {
            int agentID = agentData.id;
            Vector2Int newPosition = new Vector2Int(agentData.position[0], agentData.position[1]);
            bool hasFood = agentData.has_food;

            GameObject agentObject = agentObjects.Find(agent => agent.GetComponent<AgentController>().id == agentID);

            if (agentObject != null)
            {
                // Update the agent position and properties
                AgentController agentController = agentObject.GetComponent<AgentController>();
                agentController.id = agentID;
                agentController.Move(newPosition);
            }
            else
            {
                // Instantiate new agent if not found
                GameObject newAgent = Instantiate(agentPrefab, new Vector3(newPosition.x, 0, newPosition.y), Quaternion.identity);
                AgentController newAgentController = newAgent.GetComponent<AgentController>();
                newAgentController.id = agentID;
                agentObjects.Add(newAgent);
            }
        }

        if (!depositCreated)
        {
            InstantiateDeposit(data.deposit_position);
            depositCreated = true;
        }

        foreach (var foodData in data.food_positions)
        {
            
            Vector2Int foodPosition = new Vector2Int(foodData.position[0], foodData.position[1]);
            UpdateFoodPosition(foodPosition);
        }
    }

    void UpdateFoodPosition(Vector2Int position)
    {
        // Check if food exists at the given position
        if (foodInstances.TryGetValue(position, out GameObject foodObject))
        {
            // Food exists, update its position
            foodObject.transform.position = new Vector3(position[0], 0, position[1]);
        }
        else
        {
            // Food doesn't exist, instantiate a new one
            GameObject newFood = Instantiate(foodPrefab, new Vector3(position[0], 0, position[1]), Quaternion.identity);
            foodObjects.Add(newFood);
            foodInstances[position] = newFood;
        }
    }

    void InstantiateDeposit(int[] depositPosition)
    {
        Vector2Int position = new Vector2Int(depositPosition[0], depositPosition[1]);
        Instantiate(depositPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
    }
}
