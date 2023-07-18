using UnityEngine;

public enum EnemyType
{
    Monster, BossMonster
}
public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private Vector3 playerPosition;
    [SerializeField] private GameObject[] patrolPoints;
    [SerializeField] private int patrolDestination;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Patrol()
    {
        if (patrolDestination == 0)
        {
            Vector3 targetPosition = patrolPoints[0].transform.position;
            Vector3 moveDirection = targetPosition - transform.position;
            moveDirection.y = 0f; // Ensure the enemy doesn't tilt upwards or downwards while moving.

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if ((transform.position - patrolPoints[0].transform.position).sqrMagnitude < 0.2f * 0.2f)
            {
                patrolDestination = 1;
            }
        }

        if (patrolDestination == 1)
        {
            Vector3 targetPosition = patrolPoints[1].transform.position;
            Vector3 moveDirection = targetPosition - transform.position;
            moveDirection.y = 0f; // Ensure the enemy doesn't tilt upwards or downwards while moving.

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if ((transform.position - patrolPoints[1].transform.position).sqrMagnitude < 0.2f * 0.2f)
            {
                patrolDestination = 0;
            }
        }
    }

    private void Update()
    {
        Patrol();
    }

}
