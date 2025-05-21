using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class GolemController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2f;

    [Header("Detección de entorno")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public LayerMask wallLayer;

    [Header("Vida del enemigo")]
    public int vida = 3;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    private bool movingRight = true;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // Verificación de referencias
        if (!groundCheck || !wallCheck)
            Debug.LogWarning($"{name}: Falta asignar groundCheck o wallCheck en el Inspector");
    }

    void Update()
    {
        if (isDead) return;

        Patrol();

        animator.SetBool("running", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

    void Patrol()
    {
        rb.velocity = new Vector2((movingRight ? 1 : -1) * speed, rb.velocity.y);

        Vector3 groundOffset = GetGroundOffset();

        bool noGround = !Physics2D.Raycast(groundCheck.position + groundOffset, Vector2.down, 0.4f, groundLayer);
        bool hitWall = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, 0.3f, wallLayer);

        if (noGround || hitWall)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        sr.flipX = !sr.flipX;
    }

    Vector3 GetGroundOffset()
    {
        return new Vector3(movingRight ? 0.3f : -0.3f, 0f, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("PlayerHit"))
        {
            vida--;
            StartCoroutine(FlashRed());

            Debug.Log($"{name} recibió daño. Vida restante: {vida}");

            if (vida <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    IEnumerator Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

            animator.SetTrigger("die");

        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position + GetGroundOffset(), groundCheck.position + GetGroundOffset() + Vector3.down * 0.4f);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector3 dir = movingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * 0.3f);
        }
    }
}
