using UnityEngine;

namespace BigRookGames.Weapons
{
    public class GunfireController : MonoBehaviour
    {
        // --- Audio ---
        public AudioClip GunShotClip;
        public AudioClip ReloadClip;
        public AudioSource source;
        public AudioSource reloadSource;
        public Vector2 audioPitch = new Vector2(.9f, 1.1f);

        // --- Muzzle ---
        public GameObject muzzlePrefab;
        public GameObject muzzlePosition;

        // --- Config ---
        public bool autoFire;              // Bật true = bắn tự động, false = bắn khi nhấn chuột
        public float shotDelay = .5f;
        public bool rotate = true;
        public float rotationSpeed = .25f;

        // --- Options ---
        public GameObject scope;
        public bool scopeActive = true;
        private bool lastScopeState;

        // --- Projectile ---
        [Tooltip("The projectile gameobject to instantiate each time the weapon is fired.")]
        public GameObject projectilePrefab;
        [Tooltip("Sometimes a mesh will want to be disabled on fire. For example: when a rocket is fired, we instantiate a new rocket, and disable" +
            " the visible rocket attached to the rocket launcher")]
        public GameObject projectileToDisableOnFire;

        // --- Timing ---
        [SerializeField] private float timeLastFired;


        private void Start()
        {
            if (source != null) source.clip = GunShotClip;
            timeLastFired = 0;
            lastScopeState = scopeActive;
        }

        private void Update()
        {
            // --- Nếu rotate được bật thì xoay súng ---
            if (rotate)
            {
                transform.localEulerAngles = new Vector3(
                    transform.localEulerAngles.x,
                    transform.localEulerAngles.y + rotationSpeed,
                    transform.localEulerAngles.z
                );
            }

            // --- Chế độ bắn ---
            if (autoFire)
            {
                // Auto fire (bắn liên tục theo shotDelay)
                if ((timeLastFired + shotDelay) <= Time.time)
                {
                    FireWeapon();
                }
            }
            else
            {
                // Click chuột trái để bắn
                if (Input.GetMouseButton(0) && (timeLastFired + shotDelay <= Time.time))
                {
                    FireWeapon();
                }
            }

            // --- Toggle scope ---
            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }
        }

        /// <summary>
        /// Creates an instance of the muzzle flash.
        /// Also creates an instance of the audioSource so that multiple shots are not overlapped on the same audio source.
        /// Insert projectile code in this function.
        /// </summary>
        public void FireWeapon()
        {
            // --- Keep track of when the weapon is being fired ---
            timeLastFired = Time.time;

            // --- Spawn muzzle flash ---
            var flash = Instantiate(muzzlePrefab, muzzlePosition.transform);

            // --- Shoot Projectile Object ---
            if (projectilePrefab != null)
            {
                GameObject newProjectile = Instantiate(projectilePrefab, muzzlePosition.transform.position, muzzlePosition.transform.rotation, transform);
            }

            // --- Disable any gameobjects, if needed ---
            if (projectileToDisableOnFire != null)
            {
                projectileToDisableOnFire.SetActive(false);
                Invoke("ReEnableDisabledProjectile", 3);
            }

            // --- Handle Audio ---
            if (source != null)
            {
                if (source.transform.IsChildOf(transform))
                {
                    source.Play();
                }
                else
                {
                    AudioSource newAS = Instantiate(source);
                    if ((newAS = Instantiate(source)) != null && newAS.outputAudioMixerGroup != null && newAS.outputAudioMixerGroup.audioMixer != null)
                    {
                        newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(audioPitch.x, audioPitch.y));
                        newAS.pitch = Random.Range(audioPitch.x, audioPitch.y);

                        newAS.PlayOneShot(GunShotClip);
                        Destroy(newAS.gameObject, 4);
                    }
                }
            }
        }

        private void ReEnableDisabledProjectile()
        {
            if (reloadSource != null) reloadSource.Play();
            if (projectileToDisableOnFire != null) projectileToDisableOnFire.SetActive(true);
        }
    }
}
