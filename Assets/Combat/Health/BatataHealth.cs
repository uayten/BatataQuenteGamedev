using UnityEngine;

public class BatataHealth : MonoBehaviour
{
    [Header("Atributos")]
    public int vidaMaxima = 100;
    private int vidaAtual;

    [Header("Configuração de Morte")]
    public bool destruirAoMorrer = false;
    public float tempoParaDestruir = 2f;  // O tempo em segundos antes de sumir

    // Referências
    private Animator animador;
    private BatataPlayerMovement scriptMovimento;

    void Start()
    {
        vidaAtual = vidaMaxima;

        // Pega o Animator que está no Objeto Filho
        animador = GetComponentInChildren<Animator>();

        // Pega o script de movimento se ele existir (no caso do Player)
        scriptMovimento = GetComponent<BatataPlayerMovement>();
    }

    public void ReceberDano(int quantidade)
    {
        if (vidaAtual <= 0) return;

        vidaAtual -= quantidade;

        if (vidaAtual > 0)
        {
            if (animador != null)
            {
                animador.SetTrigger("TomouDano");
            }
        }
        else
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        if (animador != null)
        {
            animador.SetBool("Morreu", true);
        }
        // Desativa o movimento se for o jogador
        if (scriptMovimento != null)
        {
            scriptMovimento.enabled = false;
        }

        // Desativa o Character Controller para o corpo não bloquear a passagem ou novos tiros
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // SEGUNDOS DE TOLERÂNCIA: Se a caixa estiver marcada, a Unity limpa o objeto da memória
        if (destruirAoMorrer)
        {
            Destroy(gameObject, tempoParaDestruir);
        }
    }
}