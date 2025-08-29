using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrenadeBullet : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private Explosion explosionPrefab;

    [Header("Grenade Timer / Impact")]
    [SerializeField] private float fuseTime = 2.0f;     // "MODIFY EXPLOSION TIMER"
    [SerializeField] private bool explodeOnImpact = true;
    [SerializeField] private float minImpactSpeed = 2f;

    [Header("Visual (tuỳ chọn)")]
    [SerializeField] private GameObject modelToHideOnExplode;

    private bool _exploded;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // PREVENT GO THROUGH
    }

    private void OnEnable()
    {
        if (fuseTime > 0f) StartCoroutine(FuseRoutine());
    }

    private IEnumerator FuseRoutine()
    {
        yield return new WaitForSeconds(fuseTime);
        Detonate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_exploded) return;
        if (explodeOnImpact && collision.relativeVelocity.magnitude >= minImpactSpeed)
            Detonate();
    }

    private void Detonate()
    {
        if (_exploded) return;
        _exploded = true;

        if (modelToHideOnExplode != null) modelToHideOnExplode.SetActive(false);

        if (explosionPrefab != null)
        {
            var exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            exp.Detonate(); // NOW YOUR BULLET SHOULD BE EXPLOSIBLE
        }

        Destroy(gameObject, 0.02f);
    }
}