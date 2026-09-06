using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
 // Necessário para acessar o TeleportationAnchor

[RequireComponent(typeof(Button))]
public class BotaoComAtraso : MonoBehaviour
{
    [Header("Configurações de Gatilho")]
    [Tooltip("A âncora de teletransporte que iniciará a contagem regressiva")]
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor ancoraGatilho;

    [Header("Configurações de Tempo")]
    [Tooltip("Tempo em segundos que o botão ficará desativado após o teletransporte")]
    public float tempoDeEspera = 5f;

    [Header("Feedback Visual (Opcional)")]
    [Tooltip("Arraste o Text (TMP) filho do botão aqui para mostrar a contagem regressiva")]
    public TextMeshProUGUI textoDoBotao;

    private Button meuBotao;
    private string textoOriginal;
    private bool contagemIniciada = false;

    void Awake()
    {
        // Pega automaticamente o componente Button onde o script foi colocado
        meuBotao = GetComponent<Button>();

        // Salva o texto original (ex: "Começar") para restaurar depois
        if (textoDoBotao != null)
        {
            textoOriginal = textoDoBotao.text;
        }
    }

    void OnEnable()
    {
        // Garante que o botão inicie desativado e reseta a flag de controle
        meuBotao.interactable = false;
        contagemIniciada = false;

        // Se uma âncora foi configurada, inscreve-se no evento de teletransporte
        if (ancoraGatilho != null)
        {
            ancoraGatilho.teleporting.AddListener(IniciarContagem);
        }
        else
        {
            // Fallback: Se você esquecer de colocar a âncora, ele funciona como antes e já inicia
            StartCoroutine(RotinaDeAtraso());
        }
    }

    void OnDisable()
    {
        // Remove a inscrição do evento para evitar vazamento de memória
        if (ancoraGatilho != null)
        {
            ancoraGatilho.teleporting.RemoveListener(IniciarContagem);
        }
    }

    // Este método é chamado automaticamente quando o teletransporte ocorre
    private void IniciarContagem(UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportingEventArgs args)
    {
        // Garante que a contagem não inicie mais de uma vez se o jogador teleportar na mesma âncora novamente
        if (!contagemIniciada)
        {
            contagemIniciada = true;
            StartCoroutine(RotinaDeAtraso());
        }
    }

    IEnumerator RotinaDeAtraso()
    {
        meuBotao.interactable = false;
        float tempoRestante = tempoDeEspera;

        // Loop da contagem regressiva
        while (tempoRestante > 0)
        {
            if (textoDoBotao != null)
            {
                // Atualiza o texto para algo como "Começar (5)"
                textoDoBotao.text = $"{textoOriginal} ({Mathf.Ceil(tempoRestante)})";
            }
            
            // Espera 1 segundo de tempo real
            yield return new WaitForSeconds(1f);
            tempoRestante -= 1f;
        }

        // Fim do tempo: restaura o texto e reativa o botão
        if (textoDoBotao != null)
        {
            textoDoBotao.text = textoOriginal;
        }
        meuBotao.interactable = true;
    }
}