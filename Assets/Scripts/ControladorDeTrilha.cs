using System.Collections;
using UnityEngine;

public class ControladorDeTrilha : MonoBehaviour
{
    [Header("Configurações Principais")]
    [Tooltip("Arraste o GerenciadorDeAudio aqui")]
    public AudioSource fonteDeAudioGlobal;
    
    [Tooltip("A música que deve tocar nesta sala específica")]
    public AudioClip musicaDaSala;

    [Range(0f, 1f)]
    [Tooltip("Volume máximo que a música atingirá nesta sala (ex: 0.05f = 5%)")]
    public float volumeMaximo = 0.05f;

    [Tooltip("Tempo em segundos para a música sumir ou aparecer")]
    public float tempoDeFade = 1.5f;

    [Header("Exceções")]
    [Tooltip("Marque isso se quiser que a música PARE completamente ao entrar aqui (ex: Corredor)")]
    public bool mutarNestaArea = false;

    // Controla centralizadamente a transição ativa para evitar conflitos entre múltiplos scripts
    private static Coroutine coroutineAtiva;
    private static MonoBehaviour executorAtivo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (mutarNestaArea)
            {
                GerenciarTransicao(null, 0f);
            }
            else if (fonteDeAudioGlobal.clip != musicaDaSala)
            {
                GerenciarTransicao(musicaDaSala, volumeMaximo);
            }
        }
    }

    private void GerenciarTransicao(AudioClip novoClip, float volumeAlvo)
    {
        // Se já existe uma transição rodando em qualquer sala, interrompe ela antes
        if (coroutineAtiva != null && executorAtivo != null)
        {
            executorAtivo.StopCoroutine(coroutineAtiva);
        }

        // Inicia a nova transição a partir deste script específico
        executorAtivo = this;
        coroutineAtiva = StartCoroutine(TransicionarAudio(novoClip, volumeAlvo));
    }

    private IEnumerator TransicionarAudio(AudioClip novoClip, float volumeAlvo)
    {
        // 1. FADE OUT: Se já estiver tocando algo, reduz o volume gradualmente
        if (fonteDeAudioGlobal.isPlaying && fonteDeAudioGlobal.volume > 0f)
        {
            float volumeInicial = fonteDeAudioGlobal.volume;
            float cronometro = 0f;

            while (cronometro < tempoDeFade)
            {
                cronometro += Time.deltaTime;
                fonteDeAudioGlobal.volume = Mathf.Lerp(volumeInicial, 0f, cronometro / tempoDeFade);
                yield return null; // Espera o próximo frame
            }

            fonteDeAudioGlobal.Stop();
        }

        // 2. TROCA DE FAIXA: Atualiza o clip com o áudio zerado
        fonteDeAudioGlobal.clip = novoClip;
        fonteDeAudioGlobal.volume = 0f;

        // 3. FADE IN: Se não for uma área muda, inicia a nova trilha subindo o som
        if (novoClip != null)
        {
            fonteDeAudioGlobal.Play();
            float cronometro = 0f;

            while (cronometro < tempoDeFade)
            {
                cronometro += Time.deltaTime;
                fonteDeAudioGlobal.volume = Mathf.Lerp(0f, volumeAlvo, cronometro / tempoDeFade);
                yield return null;
            }

            fonteDeAudioGlobal.volume = volumeAlvo; // Garante o valor exato no final
        }

        // Limpa as referências quando o processo termina com sucesso
        coroutineAtiva = null;
        executorAtivo = null;
    }
}