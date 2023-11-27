using UnityEngine;

public class DepositController : MonoBehaviour
{
    public Vector2 Position;
    public int FoodInDeposit;

    public void UpdatePosition(Vector2 newPosition)
    {
        Position = newPosition;
        // Update the GameObject's position in Unity
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);
    }

    public void UpdateFoodInDeposit(int foodCount)
    {
        FoodInDeposit = foodCount;
        // Adjust the visual representation or perform other actions based on foodCount
    }
}