using UnityEngine;

public class CeilingFan : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f; // graus por segundo

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }
}