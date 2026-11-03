// FireEffects — how the sample's fire looks and sounds. Nothing here uses the SDK: replace it with your own game.
// Light() starts the fire; called again while it burns, the fire flares up and grows. PutOut() ends it.
using UnityEngine;

namespace LatentVoiceSamples
{
    public sealed class FireEffects : MonoBehaviour
    {
        [Tooltip("The fire effect to play.")]
        public ParticleSystem fire;

        [Tooltip("Optional: a light that is on while the fire burns.")]
        public Light glow;

        [Tooltip("Optional: the fire's steady sound (a looping AudioSource), louder as the fire grows.")]
        public AudioSource sound;

        [Tooltip("Optional: the sound of the fire flaring up.")]
        public AudioClip flareSound;

        [Range(0f, 1f)] public float flareVolume = 0.7f;

        [Tooltip("How many steps the fire can grow.")]
        [Range(1, 5)] public int maxSize = 3;

        /// <summary>0 when out; 1 when lit; one more each time Light is called while it burns, up to maxSize.</summary>
        public int Size { get; private set; }

        float _baseSize, _baseRate, _baseGlow, _baseVolume, _flash;

        void Awake()
        {
            _baseSize = fire.main.startSizeMultiplier;
            _baseRate = fire.emission.rateOverTimeMultiplier;
            _baseGlow = glow != null ? glow.intensity : 1f;
            _baseVolume = sound != null ? sound.volume : 1f;
            Size = fire.isEmitting ? 1 : 0;
            Apply();
        }

        public void Light()
        {
            if (Size == 0)
            {
                Size = 1;
                Debug.Log("[Fire sample] the fire is lit", this);
                fire.Play(true);
            }
            else
            {
                Size = Mathf.Min(Size + 1, maxSize);
                Debug.Log("[Fire sample] the fire flares up (size " + Size + "/" + maxSize + ")", this);
                if (sound != null && flareSound != null) sound.PlayOneShot(flareSound, flareVolume);
            }
            Apply();
            fire.Emit(40 * Size);   // a burst, so saying the word again always shows
            _flash = 1f;
        }

        public void PutOut()
        {
            Debug.Log("[Fire sample] the fire is out", this);
            Size = 0;
            fire.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Apply();
        }

        // Each size step makes the flames 35 % larger and denser.
        void Apply()
        {
            float scale = 1f + 0.35f * Mathf.Max(0, Size - 1);
            var main = fire.main;
            main.startSizeMultiplier = _baseSize * scale;
            var emission = fire.emission;
            emission.rateOverTimeMultiplier = _baseRate * scale;
            if (glow != null) glow.enabled = Size > 0;
            if (sound != null && Size > 0 && !sound.isPlaying) { sound.volume = 0f; sound.Play(); }
        }

        // The glow and the sound follow the size, with a short flash after each word; the sound fades out with the flames.
        void Update()
        {
            _flash = Mathf.MoveTowards(_flash, 0f, Time.deltaTime * 2f);
            if (glow != null && Size > 0)
                glow.intensity = _baseGlow * (1f + 0.35f * (Size - 1)) * (1f + 1.5f * _flash);
            if (sound != null && sound.isPlaying)
            {
                float target = Size == 0 ? 0f : _baseVolume * (0.6f + 0.2f * (Size - 1)) * (1f + 0.5f * _flash);
                sound.volume = Mathf.MoveTowards(sound.volume, Mathf.Min(1f, target), Time.deltaTime * (Size == 0 ? 0.8f : 3f));
                if (Size == 0 && sound.volume <= 0f) sound.Stop();
            }
        }
    }
}
