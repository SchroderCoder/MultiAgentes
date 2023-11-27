using UnityEngine;

public class FoodControler : MonoBehaviour
{
    public Vector2 Position;

    public void UpdatePosition(Vector2 newPosition)
    {
        Position = newPosition;
        // Update the GameObject's position in Unity
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);
    }
}