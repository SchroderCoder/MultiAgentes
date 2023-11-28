using UnityEngine;
using System.Collections;
public class AgentScript : MonoBehaviour
{
    public int AgentID { get; private set; }

    public void SetAgentProperties(int id, Vector3 position, bool hasFood)
    {
        AgentID = id;
        transform.position = position;
        // Set other agent properties as needed
    }

    public void MoveTo(Vector3 newPosition)
    {
        // Implement the logic to smoothly move the agent to the new position
        StartCoroutine(MoveCoroutine(newPosition));
    }

    IEnumerator MoveCoroutine(Vector3 targetPosition)
    {
        float duration = 1.0f; // Adjust the duration of the movement
        float elapsed = 0.0f;
        Vector3 initialPosition = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure the final position is set accurately
    }
}