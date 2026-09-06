using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; 
using UnityEngine.XR.Interaction.Toolkit.Interactors; 

public class ReceptorContabil : MonoBehaviour
{
    public ItemContabil.Categoria categoriaDaEstante;
    
    public AudioSource audioSource;
    public AudioClip somAcerto;
    public AudioClip somErro;

    private XRSocketInteractor socket;
    private bool ejetando = false; 

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(VerificarEncaixe);
        socket.selectExited.AddListener(AoRetirarObjeto);
    }

    private void VerificarEncaixe(SelectEnterEventArgs args)
    {
        if (ejetando) return;

        ItemContabil item = args.interactableObject.transform.GetComponent<ItemContabil>();
        
        if (item != null)
        {
            if (item.categoriaDoItem == categoriaDaEstante)
            {
                item.AplicarFeedback(true);
                if (somAcerto != null) audioSource.PlayOneShot(somAcerto);
                GerenciadorContabil.Instancia.AdicionarPonto();
            }
            else
            {
                item.AplicarFeedback(false);
                if (somErro != null) audioSource.PlayOneShot(somErro);
                
                StartCoroutine(Ejetar(item, args.interactableObject));
            }
        }
    }

    private void AoRetirarObjeto(SelectExitEventArgs args)
    {
        ItemContabil item = args.interactableObject.transform.GetComponent<ItemContabil>();
        if (item != null)
        {
            item.LimparFeedback();
            if (item.categoriaDoItem == categoriaDaEstante && !ejetando)
            {
                GerenciadorContabil.Instancia.RemoverPonto();
            }
        }
    }

    private IEnumerator Ejetar(ItemContabil item, IXRSelectInteractable interactable)
    {
        ejetando = true;
        
        // Tempo para o jogador ver a luz vermelha e ouvir o erro
        yield return new WaitForSeconds(1.0f); 
        
        // Verifica se o objeto ainda está engatado
        if (socket.hasSelection && socket.interactablesSelected.Contains(interactable))
        {
            // ABORDAGEM DEFINITIVA: Desliga o Socket. 
            // Isso força o XR a soltar o objeto e impede que ele puxe de volta.
            socket.socketActive = false;
            
            // Espera 1 frame para o XR Interaction Manager processar o desencaixe físico
            yield return null; 
            
            // Agora que o socket está inativo, mandamos o livro para a mesa
            item.RetornarParaMesa();

            // Espera meio segundo para garantir que o livro se estabilizou na mesa
            yield return new WaitForSeconds(0.5f);

            // Religa o Socket para aceitar novas tentativas
            socket.socketActive = true;
        }

        item.LimparFeedback();
        ejetando = false;
    }
}