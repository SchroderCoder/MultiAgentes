    using UnityEngine;

public class AgentController : MonoBehaviour
{
    public Vector2 Position;
    public bool HasFood;

    public void UpdatePosition(Vector2 newPosition)
    {
        Position = newPosition;
        // Update the GameObject's position in Unity
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);
    }

    public void UpdateHasFood(bool hasFood)
    {
        HasFood = hasFood;
        // Adjust the visual representation or perform other actions based on hasFood
    }
}
