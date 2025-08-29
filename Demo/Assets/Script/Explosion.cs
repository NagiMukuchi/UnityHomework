using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private float radius = 6f;
    [SerializeField] private float force = 800f;

    [Header("FX (tuỳ chọn)")]
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float vfxLifetime = 3f;
    [SerializeField] private AudioClip sfx;
    [SerializeField] private float sfxVolume = 1f;

    [SerializeField] private float selfDestructDelay = 0.02f;

    public void Detonate()
    {
        Vector3 center = transform.position;

        // FX
        if (vfxPrefab != null)
        {
            var vfx = Instantiate(vfxPrefab, center, Quaternion.identity);
            if (vfxLifetime > 0f) Destroy(vfx, vfxLifetime);
        }
        if (sfx != null) AudioSource.PlayClipAtPoint(sfx, center, sfxVolume);

        // Thổi văng rigidbody trong bán kính
        var cols = Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Ignore);
        foreach (var c in cols)
        {
            if (c.attachedRigidbody != null)
            {
                c.attachedRigidbody.AddExplosionForce(force, center, radius, 0f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject, selfDestructDelay);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0.1f, 0.35f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif

}