using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ManualCollisionHandler : MonoBehaviour
{
    private CharacterController _characterController;

    [Header("Configurações de Camadas")]
    [Tooltip("Selecione as Layers que devem bloquear o jogador (Ex: Default, Door, Frame).")]
    [SerializeField] private LayerMask collisionLayers;

    [Header("Ajuste Fino")]
    [Tooltip("Fator de força do empurrão. 1.01 adiciona uma micro margem para não travar na parede.")]
    [SerializeField] private float pushForceFactor = 1.01f;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        if (_characterController == null) return;

        // 1. Coleta os dados dinâmicos da cápsula atual do seu Character Controller
        Vector3 capsuleCenter = transform.TransformPoint(_characterController.center);
        float radius = _characterController.radius;
        float height = _characterController.height;

        // 2. Calcula matematicamente onde estão o topo e a base da cápsula no mundo 3D
        Vector3 pointTop = capsuleCenter + Vector3.up * (height / 2f - radius);
        Vector3 pointBottom = capsuleCenter - Vector3.up * (height / 2f - radius);

        // 3. Cria uma varredura invisível para detectar colisores sobrepostos nas camadas escolhidas
        Collider[] overlappedObstacles = Physics.OverlapCapsule(
            pointBottom, 
            pointTop, 
            radius, 
            collisionLayers, 
            QueryTriggerInteraction.Ignore
        );

        // 4. Processa a ejeção para cada colisor que você tentar atravessar
        foreach (var obstacle in overlappedObstacles)
        {
            // Ignora colisores de triggers (gatilhos de scripts) e a si mesmo
            if (obstacle == _characterController || obstacle.isTrigger || obstacle.CompareTag("Interactable")) continue;
            Vector3 pushDirection;
            float pushDistance;

            // Função nativa de alto desempenho da NVIDIA PhysX que calcula a direção e a distância 
            // exata necessárias para descolar dois colisores que se interceptaram
            bool isPenetrating = Physics.ComputePenetration(
                _characterController, transform.position, transform.rotation,
                obstacle, obstacle.transform.position, obstacle.transform.rotation,
                out pushDirection, out pushDistance
            );

            // 5. Se houve intersecção física, empurra o jogador para fora imediatamente
            if (isPenetrating && pushDistance > 0f)
            {
                Vector3 displacementVector = pushDirection * (pushDistance * pushForceFactor);
                
                // Aplica o deslocamento usando a função de movimento oficial do Character Controller
                _characterController.Move(displacementVector);
            }
        }
    }
}