using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class XRGravity:MonoBehaviour
{
    [Header("Configurações de Gravidade")]
    [Tooltip("Multiplicador para deixar a queda mais rápida ou mais suave.")]
    public float gravityMultiplier = 1.0f;

    private CharacterController characterController;
    private Vector3 velocity;

    private void Start()
    {
        // Pega a referência do Character Controller automaticamente
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 1. Verifica se o jogador já está no chão
        // Se estiver, resetamos a velocidade vertical para um valor pequeno e constante.
        // O valor de -2f garante que o isGrounded funcione corretamente em rampas ou degraus.
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. Aplica a aceleração da gravidade ao longo do tempo (Física Padrão do Unity)
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // 3. Move o Character Controller com a velocidade calculada
        characterController.Move(velocity * Time.deltaTime);
    }
}