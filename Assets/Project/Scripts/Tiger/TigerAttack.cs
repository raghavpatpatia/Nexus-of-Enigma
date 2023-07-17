using UnityEngine;

public class TigerAttack : MonoBehaviour
{
    [SerializeField] GameObject tiger;
    [SerializeField] float forwardForce, upwardForce;
    [SerializeField] float timeBetweenSummoning, reloadTime, timeBetweenSummons;
    [SerializeField] int summonStrength, summonPerTap;
    [SerializeField] bool allowButtonHold;
    private int summonsLeft, summonsDone;
    private bool shooting, readyToShoot, reloading;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform attackPoint;
    [SerializeField] bool allowInvoke = true;


    private void Awake()
    {
        summonsLeft = summonStrength;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1));
        else shooting = (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1));
        if (Input.GetKeyDown(KeyCode.R) && summonsLeft < summonStrength && !reloading) Reload();
        if (readyToShoot && shooting && !reloading && summonsLeft <= 0)
        {   
            Reload();
        }
        if (readyToShoot && shooting && !reloading && summonsLeft > 0)
        {
            summonsDone = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 direction = hit.point - attackPoint.position;
            GameObject currentSummon = Instantiate(tiger, attackPoint.position, Quaternion.identity);
            currentSummon.transform.forward = direction.normalized;
            currentSummon.GetComponent<Rigidbody>().AddForce(direction.normalized * forwardForce, ForceMode.Impulse);
            currentSummon.GetComponent<Rigidbody>().AddForce(playerCam.transform.up * upwardForce, ForceMode.Impulse);
        }
        else
        {
            Vector3 targetPoint = ray.GetPoint(75);
            Vector3 direction = targetPoint - attackPoint.position;
            GameObject currentSummon = Instantiate(tiger, attackPoint.position, Quaternion.identity);
            currentSummon.transform.forward = direction.normalized;
            currentSummon.GetComponent<Rigidbody>().AddForce(direction.normalized * forwardForce, ForceMode.Impulse);
            currentSummon.GetComponent<Rigidbody>().AddForce(playerCam.transform.up * upwardForce, ForceMode.Impulse);
        }

        summonsLeft--;
        summonsDone++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenSummoning);
            allowInvoke = false;
        }

        if (summonsDone < summonPerTap && summonsLeft > 0)
        {
            Invoke("Shoot", timeBetweenSummons);
        }
    }


    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        summonsLeft = summonStrength;
        reloading = false;
    }
}