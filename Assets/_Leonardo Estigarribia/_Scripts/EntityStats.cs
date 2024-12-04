using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Leonardo_Estigarribia._Scripts
{
    public class EntityStats : MonoBehaviour
    {
        private AudioSource audioSource;

        [Header("- Health points")] [SerializeField]
        private int maxHealth = 10;

        [SerializeField] private int currentHealth;

        [Header("- Particle VFX")] [SerializeField]
        private ParticleSystem[] takingDamageVFX;

        [SerializeField] private ParticleSystem[] healVFX;
        [SerializeField] private GameObject dieVFX;

        [Header("- SFX")] [SerializeField] private AudioClip[] takingDamageSFX;
        [SerializeField] private AudioClip[] healSFX;
        [SerializeField] private AudioClip dieSFX;

        [Header("- UI")] private EntityUIManager entityUIManager;

        private void Start()
        {
            entityUIManager = GetComponentInChildren<EntityUIManager>();
            // Make the player have full health at the beginning of the game.
            currentHealth = maxHealth;
            // Update the UI.
            entityUIManager.UpdateHealthBar(maxHealth, currentHealth);
            audioSource = GetComponent<AudioSource>();
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            // Play some particle effects.
            var randomVFXIndex = Random.Range(0, takingDamageVFX.Length);
            takingDamageVFX[randomVFXIndex].Play();

            //Play some SFX.
            var randomSFXIndex = Random.Range(0, takingDamageSFX.Length);
            audioSource.PlayOneShot(takingDamageSFX[randomSFXIndex]);

            // Avoid negative numbers.
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }

            entityUIManager.UpdateHealthBar(maxHealth, currentHealth);
        }

        public void HealEntity(int healAmount)
        {
            currentHealth += healAmount;
            entityUIManager.UpdateHealthBar(maxHealth, currentHealth);

            // Avoid overhealing.
            if (currentHealth >= maxHealth) currentHealth = maxHealth;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        private void Die()
        {
            // If the current health is the same or less than zero, this entity is dead.
            if (currentHealth <= 0)
            {
                // Play dead animation from animation manager or something idk.
                Debug.Log($"{gameObject.transform.name} died.");

                StartCoroutine(DyingVFX());
            }
        }

        private IEnumerator DyingVFX()
        {
            var dyingVFX = Instantiate(dieVFX, transform.position, Quaternion.identity);
            dyingVFX.GetComponent<ParticleSystem>().Play();
            audioSource.PlayOneShot(dieSFX);
            yield return new WaitForSeconds(dieSFX.length);

            // Later rather than destroying it, deactivate its AI and sent it to a pool of Entities below the map.
            Destroy(gameObject);
        }
    }
}