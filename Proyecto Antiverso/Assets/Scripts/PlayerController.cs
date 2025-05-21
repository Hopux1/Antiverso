using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int extraJumpCount = 1;
    public GameObject espadaHitbox;

    private int extraJumps;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    private bool isGrounded;
    private bool isAttacking;
    private bool isDead;
    private bool isHurt;

    // Duración real de la animación de ataque (ajusta según tu clip)
    private float attackDuration = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        extraJumps = extraJumpCount;

        if (espadaHitbox != null)
            espadaHitbox.SetActive(false);
    }

    void Update()
    {
        if (isDead || isHurt) return;

        float moveInput = Input.GetAxisRaw("Horizontal");

        // Movimiento horizontal
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Flip horizontal
        if (moveInput != 0)
            sr.flipX = moveInput < 0;

        // Saltar o doble salto
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isGrounded)
            {
                Jump();
                extraJumps = extraJumpCount;
            }
            else if (extraJumps > 0)
            {
                Jump();
                extraJumps--;
            }
        }

        // Atacar
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z)) && !isAttacking)
        {
            isAttacking = true;

            // Reproduce el ataque directamente desde el frame 0
            animator.Play("OstAttack", 0, 0f);

            // Bloquear nuevos ataques hasta que termine
            Invoke(nameof(EndAttack), attackDuration);
        }

        // Animación de correr solo si no está atacando
        if (!isAttacking && isGrounded)
        {
            animator.SetBool("running", moveInput != 0);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetBool("jumping", true);
        isGrounded = false;
    }

    private void EndAttack()
    {
        isAttacking = false;

        if (espadaHitbox != null)
            espadaHitbox.SetActive(false);
    }

    // Llamado desde evento en la animación
    public void ActivarHitbox()
    {
        if (espadaHitbox != null)
        {
            espadaHitbox.SetActive(true);
            Debug.Log("🗡️ Hitbox activada desde animación");
        }
    }

    public void DesactivarHitbox()
    {
        if (espadaHitbox != null)
        {
            espadaHitbox.SetActive(false);
            Debug.Log("❌ Hitbox desactivada desde animación");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            animator.SetBool("jumping", false);
            extraJumps = extraJumpCount;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.contactCount > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = false;
            animator.SetBool("jumping", true);
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;
        isHurt = true;
        animator.Play("Hurt");
        Invoke(nameof(EndHurt), 0.5f);
    }

    private void EndHurt()
    {
        isHurt = false;
    }

    public void Die()
    {
        isDead = true;
        animator.Play("Death");
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }
}
