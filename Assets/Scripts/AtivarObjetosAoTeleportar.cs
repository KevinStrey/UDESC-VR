using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor))]
public class AtivarObjetosAoTeleportar : MonoBehaviour
{
    [Header("Configurações de Visibilidade")]
    [Tooltip("Lista de componentes/objetos que ficarão visíveis após o teletransporte")]
    public List<GameObject> objetosParaMostrar;

    [Tooltip("Tempo em segundos antes dos objetos aparecerem")]
    public float atrasoEmSegundos = 2f;

    [Tooltip("Marque para garantir que estes objetos comecem invisíveis ao iniciar o jogo")]
    public bool esconderNoInicio = true;

    private UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor teleportAnchor;

    private void Awake()
    {
        // Pega automaticamente o componente de Teleport Anchor deste objeto
        teleportAnchor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor>();
    }

    private void Start()
    {
        // Esconde os objetos no início da cena, se a opção estiver marcada
        if (esconderNoInicio)
        {
            foreach (GameObject obj in objetosParaMostrar)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        // Inscreve nosso método no evento nativo de teletransporte do XRI
        if (teleportAnchor != null)
        {
            teleportAnchor.teleporting.AddListener(AoTeleportar);
        }
    }

    private void OnDisable()
    {
        // Remove a inscrição para evitar vazamento de memória caso o anchor seja destruído
        if (teleportAnchor != null)
        {
            teleportAnchor.teleporting.RemoveListener(AoTeleportar);
        }
    }

    private void AoTeleportar(UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportingEventArgs args)
    {
        // Inicia a contagem de tempo antes de mostrar os objetos
        StartCoroutine(MostrarObjetosComAtraso());
    }

    private IEnumerator MostrarObjetosComAtraso()
    {
        // Aguarda o tempo definido no Inspector
        if (atrasoEmSegundos > 0)
        {
            yield return new WaitForSeconds(atrasoEmSegundos);
        }

        // Ativa todos os objetos da lista
        foreach (GameObject obj in objetosParaMostrar)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}