using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameFeedbackManager : MonoBehaviour
{
    [Header("Efeitos Sonoros")]
    [Tooltip("Som tocado quando o jogador vence.")]
    public AudioClip winSound;
    [Tooltip("Som tocado quando o jogador perde.")]
    public AudioClip loseSound;

    [Header("Efeitos Visuais")]
    [Tooltip("Sistema de partículas de confete para a vitória.")]
    public ParticleSystem confettiParticles;

    private AudioSource audioSource;

    private void Awake()
    {
        // Pega o AudioSource do próprio objeto
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // Inscreve-se nos eventos do GameManager
        GameManager.OnGameWon += PlayVictoryFeedback;
        GameManager.OnGameLost += PlayDefeatFeedback;
    }

    private void OnDisable()
    {
        // Desinscreve-se para evitar erros se o objeto for destruído
        GameManager.OnGameWon -= PlayVictoryFeedback;
        GameManager.OnGameLost -= PlayDefeatFeedback;
    }

    private void PlayVictoryFeedback()
    {
        if (winSound != null) 
            audioSource.PlayOneShot(winSound);
            
        if (confettiParticles != null) 
            confettiParticles.Play();
    }

    private void PlayDefeatFeedback()
    {
        if (loseSound != null) 
            audioSource.PlayOneShot(loseSound);
    }
}