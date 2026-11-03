// FireSampleGuide — on-screen help and Console messages for the "Fire" sample.
//
// Shows what Latent Voice is doing (Voice.State and Voice.Status), what to say, and the last word heard, and logs each
// change once. It is only a guide: delete this object and the sample still works.
using System.Linq;
using UnityEngine;
using Latent;

namespace LatentVoiceSamples
{
    public sealed class FireSampleGuide : MonoBehaviour
    {
        [Tooltip("The FireOnWord component whose words this guide explains.")]
        public FireOnWord fire;

        string _shown, _lastWord;
        float _lastWordTime;
        bool _warnedMissingWord;
        GUIStyle _box, _title, _line;
        float _level, _peak, _peakAt;   // the meter: smoothed level, and a peak that holds briefly

        void OnEnable() { Voice.OnKeywordDetected += OnHeard; }
        void OnDisable() { Voice.OnKeywordDetected -= OnHeard; }

        void OnHeard(string word)
        {
            _lastWord = word;
            _lastWordTime = Time.unscaledTime;
            Debug.Log("[Fire sample] heard 「" + word + "」", this);
        }

        void Update()
        {
            // The meter rises at once and falls over about a third of a second; the peak mark holds for a second.
            float now = Voice.InputLevel;
            _level = now > _level ? now : Mathf.MoveTowards(_level, now, Time.unscaledDeltaTime * 3f);
            if (_level >= _peak || Time.unscaledTime - _peakAt > 1f) { _peak = _level; _peakAt = Time.unscaledTime; }

            VoiceState state = Voice.State;
            string key = state + (Checking ? "/checking" : "");
            if (key == _shown) return;
            _shown = key;
            switch (state)
            {
                case VoiceState.Starting:
                    Debug.Log("[Fire sample] starting Latent Voice (loading the engine, opening the microphone)", this);
                    break;
                case VoiceState.Listening:
                    Debug.Log("[Fire sample] ready. " + Instruction(), this);
                    WarnIfWordMissing();
                    break;
                case VoiceState.Failed:
                case VoiceState.Unlicensed:
                case VoiceState.NotInScene:
                    if (Checking) break;
                    Debug.LogWarning("[Fire sample] not listening: " + Voice.Status + "\n" + Fix(state), this);
                    break;
            }
        }

        // The license check has not answered yet (the first check runs right after Play).
        static bool Checking { get { return Voice.State == VoiceState.Unlicensed && Voice.Status.Contains("not checked"); } }

        void WarnIfWordMissing()
        {
            if (_warnedMissingWord || fire == null) return;
            if (Voice.Keywords.Contains(fire.fireWord)) return;
            _warnedMissingWord = true;
            Debug.LogWarning("[Fire sample] 「" + fire.fireWord + "」 is not in the synced WordSet ("
                             + string.Join(", ", Voice.Keywords) + "). Register it in your portal project, publish, "
                             + "and sync again (Tools > Latent Voice > Sync published WordSet).", this);
        }

        string Instruction()
        {
            if (fire == null) return "Say a registered word.";
            string s = "Say 「" + fire.fireWord + "」 to light the fire (again to make it bigger)";
            if (!string.IsNullOrEmpty(fire.stopWord)) s += ", 「" + fire.stopWord + "」 to put it out";
            return s + ".";
        }

        static string Fix(VoiceState state)
        {
            if (state == VoiceState.NotInScene)
                return "Add GameObject > Latent Voice > Latent Voice to the scene.";
            if (Voice.Status.Contains("no permission"))
                return "Allow Unity under System Settings > Privacy & Security > Microphone, then restart Unity.";
            if (Voice.Status.Contains("no device"))
                return "Connect a microphone and press Play again.";
            return "Open Tools > Latent Voice > Sync published WordSet, press セットアップ (Set up), and save the scene.";
        }

        void OnGUI()
        {
            float scale = Mathf.Max(1f, Screen.height / 720f);
            Matrix4x4 zoom = Matrix4x4.identity;
            zoom.m00 = scale;
            zoom.m11 = scale;
            GUI.matrix = zoom;
            if (_box == null)
            {
                _box = new GUIStyle(GUI.skin.box) { padding = new RectOffset(14, 14, 10, 12), alignment = TextAnchor.UpperLeft };
                _title = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, wordWrap = true };
                _line = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true, richText = true };
            }

            VoiceState state = Voice.State;
            string stateLine;
            switch (state)
            {
                case VoiceState.Listening: stateLine = "<color=#7CFC9A>● Listening</color>"; break;
                case VoiceState.Starting: stateLine = "<color=#FFD166>● Starting…</color>"; break;
                case VoiceState.Stopped: stateLine = "<color=#BBBBBB>● Stopped</color>"; break;
                case VoiceState.Unlicensed:
                    stateLine = Checking
                        ? "<color=#FFD166>● Checking the license…</color>"
                        : "<color=#FF6B6B>● Not listening</color>";
                    break;
                default: stateLine = "<color=#FF6B6B>● Not listening</color>"; break;
            }

            GUILayout.BeginArea(new Rect(16, 16, 440, 400));
            GUILayout.BeginVertical(_box);
            GUILayout.Label("Latent Voice: Fire sample", _title);
            GUILayout.Label(stateLine, _line);
            if (state == VoiceState.Listening)
            {
                GUILayout.Label(Instruction(), _line);
                LevelMeter();
                if (!string.IsNullOrEmpty(_lastWord))
                {
                    bool fresh = Time.unscaledTime - _lastWordTime < 1.5f;
                    GUILayout.Label(fresh ? "<color=#FFD166><b>Heard 「" + _lastWord + "」</b></color>"
                                          : "Last heard: 「" + _lastWord + "」", _line);
                }
            }
            else if (state != VoiceState.Starting && !Checking)
            {
                GUILayout.Label(Voice.Status, _line);
                GUILayout.Label("<i>" + Fix(state) + "</i>", _line);
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        // Voice.InputLevel as a bar: grey when quiet, green at speaking volume, orange near clipping.
        void LevelMeter()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mic", _line, GUILayout.Width(36));
            Rect r = GUILayoutUtility.GetRect(300, 14, GUILayout.ExpandWidth(true));
            r.y += 5;
            Color keep = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.15f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = _level < 0.35f ? new Color(0.6f, 0.6f, 0.6f) : _level < 0.85f ? new Color(0.49f, 0.99f, 0.6f)
                                                                                        : new Color(1f, 0.6f, 0.2f);
            GUI.DrawTexture(new Rect(r.x, r.y, r.width * _level, r.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(r.x + r.width * _peak - 1f, r.y, 2f, r.height), Texture2D.whiteTexture);
            GUI.color = keep;
            GUILayout.EndHorizontal();
            if (_level < 0.2f) GUILayout.Label("<color=#999999>Quiet. If the bar never moves, check the microphone permission.</color>", _line);
        }
    }
}
