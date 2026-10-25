using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WispShooting.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float sensitivity = 1f;    // Accelerate
        [SerializeField] private float gravity = 1f;        // Decelerate
        [SerializeField] private bool jumpable = true;


        [Header("Health")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private float smoothInputX;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDeath;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
        }

        private void Update()
        {
            float rate = Mathf.Abs(moveInput.x) > 0.01f ? sensitivity : gravity;
            
            smoothInputX = Mathf.MoveTowards(smoothInputX, moveInput.x, rate * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (rb.gravityScale == 0f)
            {
                rb.linearVelocity = moveInput * moveSpeed;
            }
            else
            {
                rb.linearVelocity = new Vector2(smoothInputX * moveSpeed, rb.linearVelocity.y);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();

            if (moveInput.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveInput.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Jump();
            }
        }

        public void Jump()
        {
            if (jumpable)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            jumpable = false;
        }

        public void TakeDamage(int amount)
        {
            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Die()
        {
            OnDeath?.Invoke();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                jumpable = true;
            }
        }
    }
}
