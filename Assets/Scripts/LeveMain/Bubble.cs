using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EasingUtil;

namespace Milan.GrassBubble.Gameloop
{
    public class Bubble : MonoBehaviour
    {
        public Transform modelTransform;
        const float bubbleAnimDuration = 0.75f;
        public AnimationCurve animationCurve;
        public ParticleSystem particlePop;
        public ParticleSystem particleSparkle;

        public AudioClip bubbleAppear;
        public AudioClip bubbleDisappear;
        public AudioSource audioSource;

        void Awake() 
        {
            audioSource = GetComponent<AudioSource>();    
        }
        public void PlayBubbleAppearAnimation()
        {
            PlayClip(bubbleAppear);
            StartCoroutine(BubbleAppearAnim());
        }
        public void PlayParticle()
        {
            PlayClip(bubbleDisappear);
            GameObject.Instantiate(particlePop,this.transform.position,Quaternion.identity);
            GameObject.Instantiate(particleSparkle,this.transform.position,Quaternion.identity);
        }
        void PlayClip(AudioClip clip)
        {
            float originalPitch = audioSource.pitch;
            audioSource.clip = clip;
            audioSource.pitch += Random.Range(-0.25f,0.25f);
            audioSource.Play();
            audioSource.pitch = originalPitch;
        }
        
        IEnumerator BubbleAppearAnim()
        {
            float t = 0;
            while (t < bubbleAnimDuration)
            {
                float finalPercent = animationCurve.Evaluate(t);
                Vector3 newScale = new Vector3(finalPercent,finalPercent,finalPercent);
                modelTransform.localScale = newScale;
                yield return null;
                t += Time.deltaTime;
            }   
            t = 1f;
        }
    }
}