using System.Collection.Generic;

[System.Serializable]

public class Steps {

    [System.Serializable]
    public class AgentData
    {
        public bool has_food;
        public int id;
        public int[] position;
    }

    [System.Serializable]
    public class StepData
    {
        public AgentData[] agent_data;
        public int[] deposit_position;
        public int[] food_positions;
        public int num_steps;
    }

    [System.Serializable]
    public class RootObject
    {
        public StepData[] all_steps_data;
    }

}

    public class JsonReader : MonoBehaviour
    {
        void Start()
        {
            // Aquí deberías tener tu JSON como un string
            string jsonString = "{...}"; // Reemplaza con tu JSON completo

            // Deserializar el JSON a la clase RootObject
            RootObject root = JsonUtility.FromJson<RootObject>(jsonString);

            // Ahora puedes acceder a los datos
            foreach (StepData stepData in root.all_steps_data)
            {
                Debug.Log("Num Steps: " + stepData.num_steps);

                foreach (AgentData agentData in stepData.agent_data)
                {
                    Debug.Log("Agent ID: " + agentData.id);
                    Debug.Log("Has Food: " + agentData.has_food);
                    Debug.Log("Position: " + agentData.position[0] + ", " + agentData.position[1]);
                }

                Debug.Log("Deposit Position: " + stepData.deposit_position[0] + ", " + stepData.deposit_position[1]);
                Debug.Log("Food Positions: " + string.Join(", ", stepData.food_positions));
            }
        }
    }
