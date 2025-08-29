using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Firing")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Rigidbody projectilePrefab;
    [SerializeField] private float muzzleVelocity = 20f;
    [SerializeField] private float fireCooldown = 0.2f;

    [Header("Ammo (LIMIT GUN AMMO)")]
    [SerializeField] private int magazineSize = 6;
    [SerializeField] private int reserveAmmo = 24;
    [SerializeField] private int currentAmmo = 6;

    [Header("Reload (IMPLEMENT GUN RELOADING)")]
    [SerializeField] private float reloadDuration = 1.6f;
    [SerializeField] private Animator animator;                // ADD RELOAD ANIMATION
    [SerializeField] private string reloadTriggerName = "Reload";
    [SerializeField] private AudioSource reloadAudioSource;    // SETUP RELOAD SOUNDS
    [SerializeField] private AudioClip reloadClip;

    [Header("Shot SFX (tuỳ chọn)")]
    [SerializeField] private AudioSource shotAudioSource;
    [SerializeField] private AudioClip shotClip;

    private float _lastShotTime;
    private bool _reloading;

    public System.Action<int, int> OnAmmoChanged; // UI Hook

    private void Start()
    {
        OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryShoot();
        if (Input.GetKeyDown(KeyCode.R)) TryReload();
    }

    public bool TryShoot()
    {
        if (_reloading) return false;
        if (Time.time - _lastShotTime < fireCooldown) return false;
        if (currentAmmo <= 0) { TryReload(); return false; }

        _lastShotTime = Time.time;
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);

        if (projectilePrefab != null && firePoint != null)
        {
            var rb = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = firePoint.forward * muzzleVelocity;
        }

        if (shotClip != null && shotAudioSource != null)
            shotAudioSource.PlayOneShot(shotClip);

        return true;
    }

    public bool TryReload()
    {
        if (_reloading) return false;
        if (currentAmmo >= magazineSize) return false;
        if (reserveAmmo <= 0) return false;

        _reloading = true;

        if (reloadClip != null && reloadAudioSource != null)
            reloadAudioSource.PlayOneShot(reloadClip); // ATTACH RELOAD SOUNDS TO GUN

        if (animator != null && !string.IsNullOrEmpty(reloadTriggerName))
        {
            animator.SetTrigger(reloadTriggerName);    // GUN SHOULD NOW BE RELOAD-ABLE
            Invoke(nameof(FinishReload), reloadDuration); // fallback nếu không gắn event
        }
        else
        {
            Invoke(nameof(FinishReload), reloadDuration);
        }
        return true;
    }

    private void FinishReload()
    {
        int need = magazineSize - currentAmmo;
        int take = Mathf.Min(need, reserveAmmo);
        currentAmmo += take;
        reserveAmmo -= take;

        OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
        _reloading = false;
    }

    public (int current, int reserve, int mag) GetAmmoInfo()
        => (currentAmmo, reserveAmmo, magazineSize);
}