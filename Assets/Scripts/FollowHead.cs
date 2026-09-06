using UnityEngine;

public class TutorialFollowVR : MonoBehaviour
{
    [Header("Configurações de Posicionamento")]
    public Transform mainCamera;
    public float distance = 1.5f; // Distância do UI até o rosto do jogador
    
    [Tooltip("Ajuste positivo move o painel para cima, negativo para baixo")]
    public float verticalOffset = 0.5f; // Novo campo para corrigir a altura do pivô
    
    public float smoothTime = 5.0f; // Velocidade com que o UI alcança a visão

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Calcula onde o tutorial deve ficar (distância na frente + deslocamento vertical)
        Vector3 targetPosition = mainCamera.position + (mainCamera.forward * distance) + (Vector3.up * verticalOffset);
        
        // Move suavemente o tutorial para a posição alvo
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothTime);

        // Faz o painel rotacionar para sempre "olhar" para o jogador
        Vector3 directionToFace = transform.position - mainCamera.position;
        if (directionToFace != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToFace);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothTime);
        }
    }
}