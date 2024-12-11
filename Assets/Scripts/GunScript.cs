using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField] public KeyCode shootKey = KeyCode.Mouse0;
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;

    public float impactForce = 30f;
    public float impactRange = 2f;


    public Camera fpsCam;
    public GameObject impactEffect;
    public ParticleSystem muzzleFlash;

    public float nextTimeToFire = 0f;


    void Awake()
    {
        return;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(shootKey) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(hit.normal * impactForce / impactRange);
            }
        }
    }
}
