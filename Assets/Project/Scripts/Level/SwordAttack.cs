using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private bool isAttacking = false;

    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.gameObject.GetComponent<EnemyController>() != null)
        {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(10);
        }
    }
}
