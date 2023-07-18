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
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
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

    private void Update()
    {
        PlayerMovement();
        Attack();
        PlayerFly();
        CometPowerup();
    }
}
