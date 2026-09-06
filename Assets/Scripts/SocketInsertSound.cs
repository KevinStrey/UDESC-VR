using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
[RequireComponent(typeof(AudioSource))]
public class SocketInsertSound : MonoBehaviour
{
    [SerializeField] private AudioClip insertSound;

    private XRSocketInteractor socket;
    private AudioSource audioSource;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        socket.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (insertSound != null)
        {
            audioSource.PlayOneShot(insertSound);
        }
    }
}