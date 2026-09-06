using UnityEngine;

public class GerenciadorContabil : MonoBehaviour
{
    public static GerenciadorContabil Instancia;
    private int acertos = 0;

    [Header("Feedback de Vitória")]
    public AudioSource audioSource;
    public AudioClip somVitoria;
    
    [Header("Efeitos Visuais")]
    public ParticleSystem sistemaDeConfetes; // Referência para os confetes

    void Awake() 
    { 
        Instancia = this; 
    }

    public void AdicionarPonto()
    {
        acertos++;
        if (acertos == 6) // Quando bater os 6 objetos corretos
        {
            TocarSomDeVitoria();
            DispararConfetes(); // Aciona os confetes
            
            // ADICIONE ESTA LINHA:
            if (GerenciadorDePuzzles.Instancia != null) GerenciadorDePuzzles.Instancia.CompletarContabil();
        }
    }

    public void RemoverPonto() 
    { 
        acertos--; 
    }

    private void TocarSomDeVitoria()
    {
        if (audioSource != null && somVitoria != null)
        {
            audioSource.PlayOneShot(somVitoria);
            Debug.Log("Puzzle concluído! Som de vitória disparado.");
        }
    }

    private void DispararConfetes()
    {
        if (sistemaDeConfetes != null)
        {
            sistemaDeConfetes.Play();
            Debug.Log("Confetes disparados!");
        }
        else
        {
            Debug.LogWarning("Sistema de confetes não foi atribuído no Inspector!");
        }
    }
}