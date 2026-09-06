using UnityEngine;
using TMPro; // Necessário para o TextMeshPro
using System.Collections;

public class GerenciadorDePuzzles : MonoBehaviour
{
    public static GerenciadorDePuzzles Instancia;

    [Header("Estado dos Puzzles")]
    public bool puzzleContabilConcluido = false;
    public bool puzzlePonteConcluido = false;
    public bool jogoDaVelhaConcluido = false;

    [Header("Interface de Usuário (VR Hands / HUD)")]
    [Tooltip("O painel Canvas que fica oculto e aparece com as mensagens.")]
    public GameObject painelPopup;
    [Tooltip("O componente de texto onde a mensagem será injetada.")]
    public TextMeshProUGUI textoPopup;
    [Tooltip("Tempo em segundos que o aviso de transição fica visível.")]
    public float tempoExibicao = 6f;

    private void Awake()
    {
        // Padrão Singleton simples para fácil acesso de outros scripts
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        // Inscreve-se automaticamente no evento de vitória do Jogo da Velha
        GameManager.OnGameWon += CompletarJogoDaVelha;
    }

    private void OnDisable()
    {
        GameManager.OnGameWon -= CompletarJogoDaVelha;
    }

    // --- MÉTODOS DE CONCLUSÃO DE CADA PUZZLE ---

    public void CompletarContabil()
    {
        if (puzzleContabilConcluido) return;
        puzzleContabilConcluido = true;
        AvaliarProgressoGeral("Ativos e Passivos organizados!\nPode ir para o próximo desafio.");
    }

    public void CompletarPonte()
    {
        if (puzzlePonteConcluido) return;
        puzzlePonteConcluido = true;
        AvaliarProgressoGeral("Ponte reconstruída com sucesso!\nPode ir para o próximo desafio.");
    }

    private void CompletarJogoDaVelha()
    {
        if (jogoDaVelhaConcluido) return;
        jogoDaVelhaConcluido = true;
        AvaliarProgressoGeral("Você venceu o Jogo da Velha!\nPode ir para o próximo desafio.");
    }

    // --- LÓGICA DE EXIBIÇÃO ---

    private void AvaliarProgressoGeral(string mensagemDaEtapa)
    {
        // Se todos os 3 estiverem true, exibe o resumo final
        if (puzzleContabilConcluido && puzzlePonteConcluido && jogoDaVelhaConcluido)
        {
            ExibirMensagemFinal();
        }
        else
        {
            // Passa a mensagem e a variável de tempo configurada no Inspector
            ExibirPopupTemporario(mensagemDaEtapa, tempoExibicao);
        }
    }

    private void ExibirPopupTemporario(string mensagem, float tempoRestante)
    {
        painelPopup.SetActive(true);
        StopAllCoroutines();
        // Inicia a rotina com o texto e o tempo que vai decrementar
        StartCoroutine(RotinaPopupComContador(mensagem, tempoRestante));
    }

    private void ExibirMensagemFinal()
    {
        painelPopup.SetActive(true);
        textoPopup.text = "<color=green><b>PARABÉNS!</b></color>\nVocê concluiu todos os desafios:\n- Puzzle Contábil\n- Puzzle da Ponte\n- Jogo da Velha\n<b>O simulador já pode ser encerrado.</b>";
        
        // Interrompe qualquer rotina de ocultar, para que a mensagem final fique na tela para sempre
        StopAllCoroutines(); 
    }

    private IEnumerator RotinaPopupComContador(string mensagemOriginal, float tempoRestante)
    {
        // Loop da contagem regressiva
        while (tempoRestante > 0)
        {
            // Concatena a mensagem com a contagem formatada
            textoPopup.text = $"{mensagemOriginal}\n\n<size=80%>({Mathf.Ceil(tempoRestante)})</size>";
            
            // Espera exatamente 1 segundo antes de decrementar
            yield return new WaitForSeconds(1f);
            tempoRestante -= 1f;
        }

        // Tempo esgotado, oculta o painel flutuante
        painelPopup.SetActive(false);
    }
}