using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BridgeCompletionManager : MonoBehaviour
{
    [Header("Sockets")]
    [SerializeField] private XRSocketInteractor[] sockets;

    [Header("Victory")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private ParticleSystem confetti;

    [Header("Settings")]
    [SerializeField] private float completionDelay = 0.5f;

    private bool completed = false;

    private void Update()
    {
        if (completed)
            return;

        foreach (var socket in sockets)
        {
            if (!socket.hasSelection)
                return;
        }

        StartCoroutine(CompleteBridgeRoutine());
    }

    private IEnumerator CompleteBridgeRoutine()
    {
        completed = true;

        // Espera o último objeto terminar de encaixar visualmente
        yield return new WaitForSeconds(completionDelay);

        // Desativa sockets e XRGrabInteractable das peças encaixadas
        foreach (var socket in sockets)
        {
            socket.socketActive = false;

            if (socket.firstInteractableSelected is XRGrabInteractable grab)
            {
                grab.enabled = false;
            }
        }

        // Som de vitória
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        // Reproduz todos os Particle Systems do prefab de confete
        if (confetti != null)
        {
            ParticleSystem[] particleSystems =
                confetti.GetComponentsInChildren<ParticleSystem>(true);

            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }
        }

        Debug.Log("Ponte concluída!");
        // ADICIONE ESTA LINHA NO FINAL DA FUNÇÃO:
        if (GerenciadorDePuzzles.Instancia != null) GerenciadorDePuzzles.Instancia.CompletarPonte();
    }
}