using UnityEngine;

public class BatataSpriteDirection : MonoBehaviour
{
    //Script para controlar a direção do sprite do personagem baseado na câmera e no corpo do personagem
    //Atualiza o animator para a direção correta e também inverte o sprite quando necessário

    private Animator animador;
    private SpriteRenderer renderizador;
    private Transform cameraPrincipal;
    private Transform objetoPai;

    void Start()
    {
        animador = GetComponent<Animator>();
        renderizador = GetComponent<SpriteRenderer>();
        objetoPai = transform.parent;

        if (Camera.main != null)
        {
            cameraPrincipal = Camera.main.transform;
        }
    }

    // Mudamos para LateUpdate para sincronizar perfeitamente com a câmera
    void LateUpdate()
    {
        if (cameraPrincipal == null || objetoPai == null) return;

        Vector3 direcaoCorpo = objetoPai.forward;
        Vector3 direcaoFrenteCamera = cameraPrincipal.forward;
        Vector3 direcaoDireitaCamera = cameraPrincipal.right;

        direcaoCorpo.y = 0;
        direcaoFrenteCamera.y = 0;
        direcaoDireitaCamera.y = 0;

        direcaoCorpo.Normalize();
        direcaoFrenteCamera.Normalize();
        direcaoDireitaCamera.Normalize();

        float dirY = Vector3.Dot(direcaoCorpo, direcaoFrenteCamera);
        float dirX = Vector3.Dot(direcaoCorpo, direcaoDireitaCamera);

        // --- LÓGICA DE PRIORIDADE ---
        float snapX = 0f;
        float snapY = 0f;

        // Se o valor de Y for maior que 0.5 (ou seja, está mais para frente/costas do que para o lado),
        // nós zeramos o X para garantir que o sprite de frente/costas fique perfeitamente centralizado.
        if (Mathf.Abs(dirY) > 0.5f)
        {
            snapY = dirY > 0 ? 1f : -1f;
            snapX = 0f; // FORÇA O X A ZERO para sprite de Frente/Costas
        }
        else
        {
            snapX = dirX > 0 ? 1f : -1f;
            snapY = 0f; // FORÇA O Y A ZERO para sprite de Lado
        }

        animador.SetFloat("DirX", snapX);
        animador.SetFloat("DirY", snapY);

        // Inversão do Sprite
        if (snapX < 0f) renderizador.flipX = true;
        else if (snapX > 0f) renderizador.flipX = false;
    }
}