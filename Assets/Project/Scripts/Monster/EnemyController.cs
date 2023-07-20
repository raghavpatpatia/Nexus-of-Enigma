using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject[] patrolPoints;
    [SerializeField] private int patrolDestination;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 10f;
    [SerializeField] private int maxHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private int killPoints;
    private Transform playerTransform;
    private float attackCooldownTimer = 0f;
    private int currentHealth;
    private bool isDead = false;
    private bool isAttacking = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 moveDirection = targetPosition - transform.position;
        moveDirection.y = 0;
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void Patrol()
    {
        if (patrolDestination == 0)
        {
            MoveTowards(patrolPoints[0].transform.position);
            if ((transform.position - patrolPoints[0].transform.position).sqrMagnitude < 0.2f * 0.2f)
            {
                patrolDestination = 1;
            }
        }

        if (patrolDestination == 1)
        {
            MoveTowards(patrolPoints[1].transform.position);
            if ((transform.position - patrolPoints[1].transform.position).sqrMagnitude < 0.2f * 0.2f)
            {
                patrolDestination = 0;
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (playerTransform != null)
        {
            if ((playerTransform.position - transform.position).sqrMagnitude > attackDistance)
                MoveTowards(playerTransform.position);
        }
    }

    private void Update()
    {
        healthSlider.value = currentHealth;
        if (!isDead)
        {
            if (playerTransform != null)
            {
                if (attackCooldownTimer <= 0f && (playerTransform.position - transform.position).sqrMagnitude <= attackDistance)
                {
                    AttackPlayer();
                }
                else
                {
                    MoveTowardsPlayer();
                    isAttacking = false;
                }

                if (attackCooldownTimer > 0f)
                {
                    attackCooldownTimer -= Time.deltaTime;
                }
            }
            else
            {
                Patrol();
            }
        }
    }

    private void AttackPlayer()
    {
        animator.SetTrigger("isAttacking");
        attackCooldownTimer = attackCooldown;
        isAttacking = true;
    }

    public void TakeDamage(int damage)
    {
        SoundManager.Instance.PlayMusic(Sounds.EnemyHit);
        if (!isDead)
        {
            Debug.Log(currentHealth);
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("isDead");
        GameManager.UpdateScore(killPoints);
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            playerTransform = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAttacking)
            other.gameObject.GetComponent<PlayerController>().TakeDamage(10);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            playerTransform = null;
        }
    }
}
