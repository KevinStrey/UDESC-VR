using UnityEngine;

public class InterruptorDeLuz : MonoBehaviour
{
    [Header("Configurações de Luz")]
    [Tooltip("Arraste o objeto Corredor_EstadoAceso aqui")]
    public GameObject grupoLuzesAcesas;
    
    [Tooltip("Arraste o objeto Corredor_EstadoApagado aqui")]
    public GameObject grupoLuzesApagadas;

    [Header("Configurações de Áudio")]
    public AudioSource audioSource;
    
    [Tooltip("O som que tocará quando as luzes forem ACESAS")]
    public AudioClip somLigar;
    
    [Tooltip("O som que tocará quando as luzes forem APAGADAS")]
    public AudioClip somDesligar;

    [Header("Configurações de Animação Visual")]
    [Tooltip("Arraste a malha 3D do botão (o objeto filho Switch Button) aqui")]
    public Transform modeloInterruptor;
    
    [Tooltip("Rotação (X, Y, Z) quando a luz estiver LIGADA")]
    public Vector3 rotacaoLigado = new Vector3(-100f, -90f, 90f);
    
    [Tooltip("Rotação (X, Y, Z) quando a luz estiver DESLIGADA")]
    public Vector3 rotacaoDesligado = new Vector3(-80f, -90f, 90f);

    public void AlternarLuzes()
    {
        if (grupoLuzesAcesas != null && grupoLuzesApagadas != null)
        {
            // Guarda o estado atual das luzes antes de inverter
            bool luzesEstavamAcesas = grupoLuzesAcesas.activeSelf;
            
            // O novo estado será o inverso do atual
            bool novoEstadoAceso = !luzesEstavamAcesas;

            // Inverte o estado dos dois grupos de luminárias
            grupoLuzesAcesas.SetActive(novoEstadoAceso);
            grupoLuzesApagadas.SetActive(luzesEstavamAcesas);

            // Gira o botão fisicamente para o ângulo correto
            if (modeloInterruptor != null)
            {
                modeloInterruptor.localEulerAngles = novoEstadoAceso ? rotacaoLigado : rotacaoDesligado;
            }

            // Sistema de decisão do áudio
            if (audioSource != null)
            {
                if (novoEstadoAceso && somLigar != null)
                {
                    audioSource.PlayOneShot(somLigar);
                }
                else if (!novoEstadoAceso && somDesligar != null)
                {
                    audioSource.PlayOneShot(somDesligar);
                }
            }
        }
    }
}