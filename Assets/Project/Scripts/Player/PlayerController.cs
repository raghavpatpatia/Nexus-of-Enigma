using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] float turnSmoothTime = 0.1f;
    [SerializeField] Transform cam;
    private float turnSmoothVelocity;
    private float horizontal;
    private float vertical;
    private Vector3 movementVector;

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
            movementVector = moveDirection.normalized * speed;
        }
        else
        {
            movementVector = Vector3.zero;
        }
        movementVector.y += gravity;
        characterController.Move(movementVector * Time.deltaTime);
    }

    private void Update()
    {
        PlayerMovement();
    }
}
