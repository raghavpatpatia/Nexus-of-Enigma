using UnityEngine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] float turnSmoothTime = 0.1f;
    [SerializeField] Transform cam;
    [SerializeField] private VisualEffect slash;
    private float turnSmoothVelocity;
    private float horizontal;
    private float vertical;
    private Vector3 movementVector;
    private Animator animator;
    private bool isAttacking = false;
    private bool isAnimationCompleted = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        slash.Stop();
    }

    private void PlayerMovement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            float speed = direction.magnitude >= 0.5 ? runSpeed : walkSpeed;
            movementVector = moveDirection.normalized * speed;
        }
        else
        {
            movementVector = Vector3.zero;
        }

        movementVector.y += gravity;
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

    private void Update()
    {
        PlayerMovement();
        Attack();
    }
}
