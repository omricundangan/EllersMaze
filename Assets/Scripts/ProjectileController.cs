using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ProjectileController: MonoBehaviour {

    public Text countText;
    private int ammo;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletSpeed;

    public GameObject player;

	void Start () {
        ammo = 0;
        setAmmoText();
	}
	
	void Update () {
        // Fire a bullet if 'f' is pressed, player has sufficient ammo, and is not jumping.
        if (Input.GetKeyDown(KeyCode.F) && ammo > 0 && !player.GetComponent<FirstPersonController>().m_Jumping)
        {
            Fire();
        }
    }

    // If the player runs into ammo, increase ammo!
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ammo"))
        {
            Destroy(other.gameObject);
            ammo++;
            setAmmoText();
        }
    }

    void setAmmoText()
    {
        countText.text = "Ammo: " + ammo.ToString();
    }

    void Fire()
    {
        ammo--;
        setAmmoText();

        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        // Destroy the bullet after 5 seconds
        Destroy(bullet, 5.0f);
    }

}
