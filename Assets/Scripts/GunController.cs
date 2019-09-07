using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
     Gun equippedGun;
    public Gun[] allGuns;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void EquipGun(Gun gunToEquip)
    {
        if(equippedGun!=null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    public void EquipGun(int weaponIndex)
    {
        EquipGun(allGuns[weaponIndex]);
    }

    public void OnTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerReleased()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerReleased();
        }
    }
    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }
    public void Aim(Vector3 aimPoint)
    {
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        } 
    }

    public void ReloadGun()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }
}
