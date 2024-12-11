using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class GunSounds
{
    public AudioClip shootSound; // Sound of firing
    public AudioClip reloadSound; // Sound of reloading
    public AudioClip dryFireSound; // Sound of dry fire (out of ammo)
}

public enum GunType
{
    Pistol,
    SMG,
    Shotgun,
    // Add more gun types as needed
}

public class GunScript : MonoBehaviour
{
    [SerializeField] public KeyCode shootKey = KeyCode.Mouse0;

    [Header("Gun Paramaters")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public float nextTimeToFire = 0f;
    public float bulletsPerMag = 15f;
    public float bulletsLeft;

    [Header("Audio Parameters")]
    public AudioSource gunAudioSource; // The audio source for the gun sounds
    public GunSounds gunSounds; // Reference to the sound effects for this gun
    public GunType gunType; // Gun type for differentiating sound sets

    private bool isReloading = false;

    public float currentBullets; //Bullets in mag
    float fireTimer;

    [Header("Force Parameters")]
    public float impactForce = 30f;

    [Header("Objects")]
    public Camera fpsCam;
    public GameObject impactEffect;
    public ParticleSystem muzzleFlash;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentBullets = bulletsPerMag;
    }

    void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            Shoot();
        }

        if (fireTimer < fireRate)
            fireTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("Fire")) anim.SetBool("Fire", false);
    }

    private void Shoot()
    {
        if (fireTimer < fireRate) return;

        if (currentBullets <= 0)
        {
            DryFire();
            return;
        }

        muzzleFlash.Play(); // Muzzle flash
        gunAudioSource.PlayOneShot(gunSounds.shootSound); // Play the shoot sound

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(hit.normal * impactForce);
            }
        }
        anim.SetBool("Fire", true);
        currentBullets--;
        Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        fireTimer = 0.0f; // Reset timer
    }

    private void DryFire()
    {
        if (bulletsLeft <= 0)
        {
            
        }
    }

    // Method for reloading (optional)
    private void Reload()
    {
        if (isReloading) return;

        isReloading = true;
        currentBullets = bulletsPerMag;
        isReloading = false;
    }
}
