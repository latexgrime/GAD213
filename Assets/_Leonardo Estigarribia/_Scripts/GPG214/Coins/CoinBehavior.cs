using System.Collections;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Coins
{
    public class CoinBehavior : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private AudioSource audioSource;
        
        [SerializeField] private float durationBeforeObjectDestroys = 1f;
        [SerializeField] private ParticleSystem coinGrabEffect;
        [SerializeField] private AudioClip coinGrabSfx;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CoinsManager.Instance.AddCoin();
                StartCoroutine(GrabCoinVisuals());
            }
        }

        private IEnumerator GrabCoinVisuals()
        {
            coinGrabEffect.Play();
            audioSource.PlayOneShot(coinGrabSfx);
            meshRenderer.enabled = false;
            yield return new WaitForSeconds(durationBeforeObjectDestroys);
            Destroy(gameObject);
        }
    }
}