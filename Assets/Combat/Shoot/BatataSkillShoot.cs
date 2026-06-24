using UnityEngine;
using UnityEngine.InputSystem;

public class BatataSkillShoot : MonoBehaviour
{
    [Header("Configuração de Disparo")]
    public InputActionReference acaoAtirar;
    public InputActionReference referenciaMover;
    public GameObject prefabProjetil;
    public Transform pontoDeDisparo;

    [Header("Mecânica de Cast (Preparo)")]
    public float tempoDePreparo = 0.5f;

    private bool estaPreparandoTiro = false;
    private float timerDePreparo = 0f;

    private Animator animador;

    void Start()
    {
        animador = GetComponentInChildren<Animator>();

        if (acaoAtirar != null) acaoAtirar.action.Enable();
        if (referenciaMover != null) referenciaMover.action.Enable();
    }

    void Update()
    {
        Vector2 entradaMovimento = referenciaMover.action.ReadValue<Vector2>();
        bool estaMovendo = entradaMovimento.magnitude > 0.1f;

        // 1. TENTA INICIAR O TIRO (Executa apenas no frame do clique)
        if (acaoAtirar.action.WasPressedThisFrame() && !estaPreparandoTiro && !estaMovendo)
        {
            estaPreparandoTiro = true;
            timerDePreparo = 0f;

            // ADICIONADO AQUI: Rotaciona o corpo uma única vez ao clicar
            RotacionarParaOMouse();

            if (animador != null)
            {
                animador.SetTrigger("Atirou");
            }
        }

        // 2. LÓGICA DE PREPARO (CAST TIME)
        if (estaPreparandoTiro)
        {
            if (estaMovendo)
            {
                CancelarTiro();
                return;
            }

            // REMOVIDO DAQUI: A rotação contínua foi retirada deste bloco

            timerDePreparo += Time.deltaTime;

            if (timerDePreparo >= tempoDePreparo)
            {
                DispararProjetil();
            }
        }
    }

    private void RotacionarParaOMouse()
    {
        Vector2 posicaoMouseTela = Mouse.current.position.ReadValue();
        Ray raioDaCamera = Camera.main.ScreenPointToRay(posicaoMouseTela);
        Plane planoChao = new Plane(Vector3.up, transform.position);

        if (planoChao.Raycast(raioDaCamera, out float distanciaDoRaio))
        {
            Vector3 pontoAlvo = raioDaCamera.GetPoint(distanciaDoRaio);
            Vector3 direcaoOlhar = pontoAlvo - transform.position;
            direcaoOlhar.y = 0f;

            if (direcaoOlhar.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(direcaoOlhar);
            }
        }
    }

    private void DispararProjetil()
    {
        if (prefabProjetil != null && pontoDeDisparo != null)
        {
            // O projétil herda a rotação fixa que foi travada no momento do clique
            Instantiate(prefabProjetil, pontoDeDisparo.position, transform.rotation);
        }

        estaPreparandoTiro = false;
        timerDePreparo = 0f;
    }

    public void CancelarTiro()
    {
        estaPreparandoTiro = false;
        timerDePreparo = 0f;

        if (animador != null)
        {
            animador.ResetTrigger("Atirou");
        }
    }
}