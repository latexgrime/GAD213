using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Coins
{
    public class CoinBehavior : MonoBehaviour
    {
        private AudioSource audioSource;
        private MeshCollider meshCollider;
        private MeshRenderer meshRenderer;
        
        [SerializeField] private float durationBeforeObjectDestroys = 1f;
        [SerializeField] private ParticleSystem coinGrabEffect;
        [SerializeField] private AudioClip coinGrabSfx;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CoinsManager.CoinsManagerInstance.AddCoin();
                Profiler.BeginSample("GPG214: Coin collection logic.");
                StartCoroutine(GrabCoinVisuals());
                Profiler.EndSample();
            }
        }

        private IEnumerator GrabCoinVisuals()
        {
            Profiler.BeginSample("GPG214: Grabbing coin visual effects");
            coinGrabEffect.Play();
            audioSource.PlayOneShot(coinGrabSfx);
            meshRenderer.enabled = false;
            meshCollider.enabled = false;
            Profiler.EndSample();
            yield return new WaitForSeconds(durationBeforeObjectDestroys);
            Profiler.BeginSample("GPG214: Coin clean up.");
            Destroy(gameObject);
            Profiler.EndSample();
        }
    }
}