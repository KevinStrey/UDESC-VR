using UnityEngine;

public class ItemContabil : MonoBehaviour
{
    public enum Categoria { Ativo, Passivo, PatrimonioLiquido }
    public Categoria categoriaDoItem;

    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private Outline meuOutline;

    void Start()
    {
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;
        
        meuOutline = GetComponent<Outline>();
        if (meuOutline != null) meuOutline.enabled = false;
    }

    public void RetornarParaMesa()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // O desencaixe do XR devolve a física ao livro.
            // Garantimos que ele não tenha nenhuma velocidade residual de queda.
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Teleporte absoluto
        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;
        
        // Sincroniza a posição da caixa de colisão com o visual instantaneamente
        Physics.SyncTransforms();
    }

    public void AplicarFeedback(bool acertou)
    {
        if (meuOutline != null)
        {
            meuOutline.enabled = true;
            meuOutline.OutlineColor = acertou ? Color.green : Color.red;
        }
    }

    public void LimparFeedback()
    {
        if (meuOutline != null) meuOutline.enabled = false;
    }
	
	private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto com o qual o livro colidiu se chama exatamente "PisoImbuia1"
        if (collision.gameObject.name == "PisoImbuia1")
        {
            Debug.Log("O livro caiu no chão! Retornando para a mesa...");
            RetornarParaMesa();
        }
    }
	
}