using System.Collections;
using UnityEngine;

public class Tiger : MonoBehaviour
{
    [SerializeField] private float erodeRate = 0.03f;
    [SerializeField] private float erodeRefreshRate = 0.01f;
    [SerializeField] private float erodeDelay = 3f;
    [SerializeField] private SkinnedMeshRenderer erodeObject;
    [SerializeField] private float destroyTime = 3.5f;
    private IEnumerator summon;

    private void Start()
    {
        TigerCoroutine();
        Destroy(gameObject, destroyTime);
    }

    private void TigerCoroutine()
    {
        if (summon != null)
        {
            StopCoroutine(summon);
        }
        summon = ErodeObject();
        StartCoroutine(summon);
    }

    private IEnumerator ErodeObject()
    {
        yield return new WaitForSeconds(erodeDelay);
        float t = 0;
        while(t < 1)
        {
            t += erodeRate;
            erodeObject.material.SetFloat("_Erode", t);
            yield return new WaitForSeconds(erodeRefreshRate);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<EnemyController>() != null)
        {
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(20);
        }
    }
}
