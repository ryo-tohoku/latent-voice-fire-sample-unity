// FireOnWord — all the code a game needs for Latent Voice: subscribe to Voice.OnKeywordDetected, unsubscribe when disabled,
// and compare the word that arrives (as registered in the portal, in kana). What the fire looks like is FireEffects.
using UnityEngine;
using Latent;

namespace LatentVoiceSamples
{
    public sealed class FireOnWord : MonoBehaviour
    {
        public string fireWord = "ファイア";
        public string stopWord = "ストップ";
        public FireEffects effects;

        void OnEnable() { Voice.OnKeywordDetected += OnWord; }
        void OnDisable() { Voice.OnKeywordDetected -= OnWord; }

        void OnWord(string word)
        {
            if (word == fireWord) effects.Light();
            else if (word == stopWord) effects.PutOut();
        }
    }
}
