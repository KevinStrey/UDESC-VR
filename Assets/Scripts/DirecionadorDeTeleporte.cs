using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor))]
public class DirecionadorDeTeleporte : MonoBehaviour
{
    [Header("Gerenciamento de Rotas")]
    [Tooltip("Lista de Teleport Anchors (GameObjects) que ficarão VISÍVEIS ao chegar aqui")]
    public List<GameObject> anchorsParaMostrar;

    [Tooltip("Lista de Teleport Anchors (GameObjects) que serão OCULTADOS ao chegar aqui")]
    public List<GameObject> anchorsParaEsconder;

    private UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor meuAnchor;

    private void Awake()
    {
        // Pega automaticamente o componente de Teleport Anchor deste objeto
        meuAnchor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor>();
    }

    private void OnEnable()
    {
        // Inscreve o método no evento nativo de teletransporte da âncora
        if (meuAnchor != null)
        {
            meuAnchor.teleporting.AddListener(AoTeleportar);
        }
    }

    private void OnDisable()
    {
        // Remove a inscrição para evitar vazamentos de memória
        if (meuAnchor != null)
        {
            meuAnchor.teleporting.RemoveListener(AoTeleportar);
        }
    }

    private void AoTeleportar(UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportingEventArgs args)
    {
        // 1. Oculta os caminhos antigos
        foreach (GameObject anchorObj in anchorsParaEsconder)
        {
            if (anchorObj != null)
            {
                anchorObj.SetActive(false);
            }
        }

        // 2. Revela os novos caminhos
        foreach (GameObject anchorObj in anchorsParaMostrar)
        {
            if (anchorObj != null)
            {
                anchorObj.SetActive(true);
            }
        }
    }
}