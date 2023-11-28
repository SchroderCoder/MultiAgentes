using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentController : MonoBehaviour
{
    // Variables de configuración del agente
    public int id; // Identificador del agente
    public float speed = 5.0f; // Velocidad de movimiento del agente

    // Método para mover el agente a una nueva posición
    public void Move(Vector2 newPosition)
    {
        // Actualizar la posición del agente en el eje x y z (ignorando y)
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);
    }
}