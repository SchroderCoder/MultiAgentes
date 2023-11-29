using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    // Variables de configuración del agente
    public int id; // Identificador del agente
    public bool has_food; // Indicate whether the agent has food

    // Método para mover el agente a una nueva posición y rotar
    public void Move(Vector2 newPosition)
    {
        // Calculate the direction vector
        Vector2 direction = newPosition - new Vector2(transform.position.x, transform.position.z);
        direction.Normalize(); // Ensure the direction vector has a length of 1

        // Calculate the rotation angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the agent to face the movement direction
        transform.rotation = Quaternion.Euler(0, -angle, 0);

        // Move the agent to the new position
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);

        // Check for collision with food and delete it if the agent has no food
        if (!has_food)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Food"))
                {
                    // Food found, make it invisible
                    collider.gameObject.GetComponent<Renderer>().enabled = false;
                    // Set has_food to true (agent picks up the food)
                    SetHasFood(true);
                }
            }
        }
    }

    // Set the has_food property
    public void SetHasFood(bool hasFood)
    {
        has_food = hasFood;

        // Implement your logic here to update the agent's appearance or perform other actions based on whether the agent has food or not.
        // For example, change the color of the agent or play a different animation.
    }
}
