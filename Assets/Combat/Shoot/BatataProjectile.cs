using UnityEngine;

public class BatataProjectile : MonoBehaviour
{
    public int dano = 20;
    public float velocidade = 10f;
    public float tempoDeVida = 15f;

    private Animator animador;
    private SpriteRenderer renderizador;
    private Transform cameraPrincipal;
    private bool jaCausouDano = false;

    void Start()
    {
        // Garante a destruição automática por tempo de vida
        Destroy(gameObject, tempoDeVida);

        // Busca os componentes visuais no objeto filho do projétil
        animador = GetComponentInChildren<Animator>();
        renderizador = GetComponentInChildren<SpriteRenderer>();

        if (Camera.main != null)
        {
            cameraPrincipal = Camera.main.transform;
        }
    }

    void Update()
    {
        // Movimentação física linear e contínua em 360 graus baseada no forward original
        transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (cameraPrincipal == null || animador == null) return;

        // Pega a direção real para onde a bala está viajando no espaço 3D
        Vector3 direcaoVoo = transform.forward;
        Vector3 frenteCamera = cameraPrincipal.forward;
        Vector3 direitaCamera = cameraPrincipal.right;

        direcaoVoo.y = 0;
        frenteCamera.y = 0;
        direitaCamera.y = 0;

        direcaoVoo.Normalize();
        frenteCamera.Normalize();
        direitaCamera.Normalize();

        // Compara a trajetória da bala com a rotação da câmera
        float dirY = Vector3.Dot(direcaoVoo, frenteCamera);
        float dirX = Vector3.Dot(direcaoVoo, direitaCamera);

        // Snap matemático para alimentar a Blend Tree de 8 direções do projétil
        float snapX = Mathf.Round(dirX);
        float snapY = Mathf.Round(dirY);

        animador.SetFloat("DirX", snapX);
        animador.SetFloat("DirY", snapY);

        // Inversão automática para espelhar os sprites do lado esquerdo
        if (renderizador != null)
        {
            if (snapX < 0f) renderizador.flipX = true;
            else if (snapX > 0f) renderizador.flipX = false;
        }
    }

    private void OnTriggerEnter(Collider outroObjeto)
    {
        if (jaCausouDano) return;

        // 1. Ignora o jogador que realizou o disparo
        if (outroObjeto.CompareTag("Player")) return;

        // 2. Tenta buscar o componente de saúde no objeto atingido (funciona para o alien)
        BatataHealth saudeDoAlvo = outroObjeto.GetComponent<BatataHealth>();

        if (saudeDoAlvo != null)
        {

            // Ativa a trava imediatamente antes de aplicar o dano
            jaCausouDano = true;

            // Aplica o valor do dano diretamente no componente do inimigo
            saudeDoAlvo.ReceberDano(dano);
        }

        // 3. Destrói a instância do projétil ao colidir (seja com o inimigo ou com paredes)
        Destroy(gameObject);
    }
}