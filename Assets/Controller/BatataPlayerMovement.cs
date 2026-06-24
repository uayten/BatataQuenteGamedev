using UnityEngine;
using UnityEngine.InputSystem;

public class BatataPlayerMovement : MonoBehaviour
{
    private CharacterController controle;
    private Transform cameraPrincipal;
    private Animator animador;

    [Header("Configuração de Controles")]
    public InputActionReference referenciaMover;
    public float velocidade = 5f;

    [Header("Filtro de Soltura (Dead Zone)")]
    public float tempoFiltro = 0.05f; // 50 milissegundos de tolerância
    private Vector2 entradaFiltrada;
    private Vector2 entradaAntiga;
    private float timerFiltro = 0f;

    void Start()
    {
        controle = GetComponent<CharacterController>();
        animador = GetComponentInChildren<Animator>();
        referenciaMover.action.Enable();

        if (Camera.main != null)
        {
            cameraPrincipal = Camera.main.transform;
        }
    }

    void Update()
    {
        Vector2 entradaReal = referenciaMover.action.ReadValue<Vector2>();

        // Se há qualquer valor de input, ele está andando.
        bool estaMovendo = entradaReal.magnitude > 0.1f;
        animador.SetBool("Andando", estaMovendo);

        // --- Delay para não rotacionar quando soltar W e D ao mesmo tempo ---

        // 1. Se soltou tudo, para na hora (Zero escorregamento)
        if (entradaReal == Vector2.zero)
        {
            entradaFiltrada = Vector2.zero;
            entradaAntiga = Vector2.zero;
            timerFiltro = 0f;
        }
        // 2. Se estava parado e começou a andar, responde na hora (Zero Input Lag)
        else if (entradaAntiga == Vector2.zero)
        {
            entradaFiltrada = entradaReal;
            entradaAntiga = entradaReal;
            timerFiltro = 0f;
        }
        // 3. Se está trocando de direção (ex: Soltou o W, mas ainda segura o D)
        else if (entradaReal != entradaAntiga)
        {
            timerFiltro += Time.deltaTime;

            // Só aceita a nova direção se o D for segurado por mais de 50ms
            if (timerFiltro >= tempoFiltro)
            {
                entradaFiltrada = entradaReal;
                entradaAntiga = entradaReal;
                timerFiltro = 0f;
            }
        }
        // 4. Se não mudou nada, segue a vida
        else
        {
            timerFiltro = 0f;
        }
        // -------------------------------------

        Vector3 direcao;

        if (cameraPrincipal != null)
        {
            Vector3 frenteCamera = cameraPrincipal.forward;
            Vector3 direitaCamera = cameraPrincipal.right;

            frenteCamera.y = 0f;
            direitaCamera.y = 0f;

            frenteCamera.Normalize();
            direitaCamera.Normalize();

            // O corpo agora usa a entrada FILTRADA, imune a solturas falsas
            direcao = (frenteCamera * entradaFiltrada.y) + (direitaCamera * entradaFiltrada.x);
        }
        else
        {
            direcao = new Vector3(entradaFiltrada.x, 0f, entradaFiltrada.y);
        }

        if (direcao.magnitude >= 0.1f)
        {
            Quaternion rotacaoAlvo = Quaternion.LookRotation(direcao);
            transform.rotation = rotacaoAlvo;
        }

        controle.Move(direcao * velocidade * Time.deltaTime);
    }
}