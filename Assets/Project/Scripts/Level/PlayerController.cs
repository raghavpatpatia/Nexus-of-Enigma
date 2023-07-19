using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] float turnSmoothTime = 0.1f;
    [SerializeField] Transform cam;
    [SerializeField] private VisualEffect slash;
    [SerializeField] private VisualEffect cometShower;
    [SerializeField] float flySpeed = 3f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float health = 100;
    private float currentHealth;
    private float turnSmoothVelocity;
    private float horizontal;
    private float vertical;
    private Vector3 movementVector;
    private Animator animator;
    private bool isAttacking = false;
    private bool isAnimationCompleted = true;
    private bool isFlying = false;
    private bool isCometShowerOnCooldown = false;
    private bool isCometShowerActive = false;
    private float cometShowerDuration = 5f;
    private float cometShowerCooldown = 10f;
    private float cometShowerStartTime;
    private bool canMove = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        slash.Stop();
        cometShower.Stop();
        currentHealth = health;
    }

    private void PlayerMovement()
    {
        if (!canMove) return;
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            targetAngle = Mathf.Repeat(targetAngle, 360f);

            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            Quaternion targetRotation = Quaternion.Euler(0, smoothedAngle, 0);

            transform.rotation = Quaternion.LookRotation(targetRotation * Vector3.forward);

            Vector3 moveDirection = targetRotation * Vector3.forward;
            movementVector = moveDirection.normalized * speed;
        }
        else
        {
            movementVector = Vector3.zero;
        }

        if (!isFlying)
        {
            movementVector.y += gravity;
        }

        characterController.Move(movementVector * Time.deltaTime);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }

    public void PlaySlashEffect()
    {
        slash.Play();
    }

    public void StopSlashEffect()
    {
        slash.Stop();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && isAnimationCompleted)
        {
            isAnimationCompleted = false;
            isAttacking = true;
            animator.SetTrigger("isAttacking");
        }

        if (isAttacking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            isAnimationCompleted = true;
            isAttacking = false;
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(currentHealth);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("isDead");
        Debug.Log("Player died");
    }

    private void PlayerFly()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFlying = true;
            StartCoroutine(FlyRoutine());
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCoroutine(FlyRoutine());
            StartCoroutine(ResetGravity());
        }
    }

    private IEnumerator FlyRoutine()
    {
        while (this.transform.position.y < 25f)
        {
            gravity = 1 * flySpeed;
            yield return null;
        }
        gravity = 0f;
    }

    private IEnumerator ResetGravity()
    {
        yield return new WaitForSeconds(1f);
        gravity = -9.81f;
        isFlying = false;
    }

    private void CometPowerup()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (!isCometShowerOnCooldown && !isCometShowerActive)
            {
                PerformAttack(25, 10f, enemyLayer, 20, 45);
                StartCometShowerEffect();
            }
        }
    }

    private void StartCometShowerEffect()
    {
        canMove = false;
        isCometShowerActive = true;
        cometShower.Play();
        cometShowerStartTime = Time.time;
        StartCoroutine(CometShowerEffect());
    }

    private IEnumerator CometShowerEffect()
    {
        while (Time.time - cometShowerStartTime < cometShowerDuration)
        {
            canMove = false;
            yield return null;
        }

        cometShower.Stop();
        isCometShowerActive = false;
        StartCoroutine(CometCooldown());
    }

    private IEnumerator CometCooldown()
    {
        canMove = true;
        isCometShowerOnCooldown = true;
        yield return new WaitForSeconds(cometShowerCooldown);
        isCometShowerOnCooldown = false;
    }

    private void PerformAttack(int damage, float attackDistance, LayerMask enemyLayer, int rayCount, float spreadAngle)
    {
        for (int i = 0; i < rayCount; i++)
        {
            Vector3 rayDirection = Quaternion.Euler(0, Random.Range(-spreadAngle, spreadAngle), 0) * transform.forward;
            Vector3 extendedEndPoint = transform.position + (transform.forward * 4f);
            RaycastHit hit;
            if (Physics.Raycast(extendedEndPoint, rayDirection, out hit, attackDistance, enemyLayer))
            {
                EnemyController enemyController = hit.collider.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(damage);
                }
            }
        }
    }


    private void Update()
    {
        PlayerMovement();
        Attack();
        PlayerFly();
        CometPowerup();
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAttacking)
            other.gameObject.GetComponent<EnemyController>().TakeDamage(15);
    }
}
