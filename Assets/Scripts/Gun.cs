using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto,Single,Burst}
    public FireMode fireMode;

    [Header("Gun Properties")]
    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots=100f;
    public float muzzlevelocity = 20f;
    private float nextTimeBetweenShots = 0f;
    public int magSize;
    public AudioClip shootClip;
    public AudioClip reloadClip;

    [Header("Gun Recoil")]
    public Vector2 recoilAngleMinMax = new Vector2(4f,6f);
    public Vector2 recoilDistMinMax = new Vector2(0.1f,0.25f);
    public float recoilAngleSettleTime = .1f;
    public float recoilDistSettleTime = .1f;

    [Header("Shell and Muzzle Flash")]
    public Transform shell;
    public Transform shellEjectPoint;
    MuzzleFlash muzzleFlash;
    public int burstCount;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    int shotsRemainingInMag;
    public float reloadTime;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;
    


    // Start is called before the first frame update
    void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        shotsRemainingInMag = magSize;
    }

    private void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilDistSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilAngleSettleTime);
        transform.localEulerAngles = Vector3.left * recoilAngle;

        if (!isReloading && shotsRemainingInMag == 0)
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if (!isReloading && Time.time > nextTimeBetweenShots && shotsRemainingInMag > 0)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            
            //Bullet Firing
            nextTimeBetweenShots = Time.time + msBetweenShots/1000;
            Projectile newPorjectile = Instantiate(projectile, muzzle.transform.position, muzzle.transform.rotation) as Projectile;
            newPorjectile.SetSpeed(muzzlevelocity);
            Instantiate(shell, shellEjectPoint.position, shellEjectPoint.rotation);
            shotsRemainingInMag--;
            muzzleFlash.Activate();
            AudioManager.instance.PlaySound(shootClip, transform.position);

            //Recoil Effect
            transform.localPosition -= Vector3.forward * Random.Range(recoilDistMinMax.x,recoilDistMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x,recoilAngleMinMax.y);
            Mathf.Clamp(recoilAngle, 0, 60);
        }
    }
    public void Reload()
    {
        if (!isReloading && shotsRemainingInMag != magSize)
        {
            AudioManager.instance.PlaySound(reloadClip, transform.position);
            StartCoroutine(AnimateAttack());
        }
    }

    IEnumerator AnimateAttack()
    {
        isReloading = true;
        yield return new WaitForSeconds(.15f);
        float percent = 0;
        float reloadSpeed = 1 / reloadTime;
        Vector3 initialRotation = transform.localEulerAngles;
        float maxReloadAngle=30f;
        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4f;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.left * reloadAngle;
            yield return null;
        }

        isReloading = false;
        shotsRemainingInMag = magSize;
    }
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerReleased()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
    public void Aim(Vector3 aim)
    {
        transform.LookAt(aim);
    }
}
