using UnityEngine;

namespace Unstack.Audio
{
    public static class ProceduralAudioClipFactory
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateCorrectTap()
        {
            float duration = 0.25f;
            int sampleCount = (int)(SampleRate * duration);
            float[] samples = new float[sampleCount];

            // Ascending arpeggio: C5 -> E5 -> G5
            float[] frequencies = { 523.25f, 659.25f, 783.99f };
            int noteSamples = sampleCount / frequencies.Length;

            for (int n = 0; n < frequencies.Length; n++)
            {
                for (int i = 0; i < noteSamples && (n * noteSamples + i) < sampleCount; i++)
                {
                    int idx = n * noteSamples + i;
                    float t = (float)i / SampleRate;
                    float envelope = 1f - (float)i / noteSamples;
                    envelope *= envelope;
                    samples[idx] = Mathf.Sin(2f * Mathf.PI * frequencies[n] * t) * envelope * 0.4f;
                }
            }

            return CreateClip("CorrectTap", samples);
        }

        public static AudioClip CreateWrongTap()
        {
            float duration = 0.3f;
            int sampleCount = (int)(SampleRate * duration);
            float[] samples = new float[sampleCount];

            // Low buzz with dissonance
            float freq1 = 110f;
            float freq2 = 116.54f; // slightly detuned for buzz

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / SampleRate;
                float envelope = 1f - (float)i / sampleCount;
                float wave = Mathf.Sin(2f * Mathf.PI * freq1 * t) + Mathf.Sin(2f * Mathf.PI * freq2 * t);
                samples[i] = wave * envelope * 0.25f;
            }

            return CreateClip("WrongTap", samples);
        }

        public static AudioClip CreateLevelClear()
        {
            float duration = 0.6f;
            int sampleCount = (int)(SampleRate * duration);
            float[] samples = new float[sampleCount];

            // Fanfare: C5 -> E5 -> G5 -> C6 with overlap
            float[] frequencies = { 523.25f, 659.25f, 783.99f, 1046.5f };
            int noteDuration = sampleCount / 3;

            for (int n = 0; n < frequencies.Length; n++)
            {
                int startSample = n * (sampleCount / 4);
                for (int i = 0; i < noteDuration && (startSample + i) < sampleCount; i++)
                {
                    int idx = startSample + i;
                    float t = (float)i / SampleRate;
                    float envelope = 1f - (float)i / noteDuration;
                    samples[idx] += Mathf.Sin(2f * Mathf.PI * frequencies[n] * t) * envelope * 0.3f;
                }
            }

            ClampSamples(samples);
            return CreateClip("LevelClear", samples);
        }

        public static AudioClip CreateGameOver()
        {
            float duration = 0.5f;
            int sampleCount = (int)(SampleRate * duration);
            float[] samples = new float[sampleCount];

            // Descending tone
            float startFreq = 440f;
            float endFreq = 110f;

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                float freq = Mathf.Lerp(startFreq, endFreq, t);
                float envelope = 1f - t;
                samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * 0.35f;
            }

            return CreateClip("GameOver", samples);
        }

        public static AudioClip CreateButtonClick()
        {
            float duration = 0.08f;
            int sampleCount = (int)(SampleRate * duration);
            float[] samples = new float[sampleCount];

            float freq = 800f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / SampleRate;
                float envelope = 1f - (float)i / sampleCount;
                envelope *= envelope;
                samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * 0.3f;
            }

            return CreateClip("ButtonClick", samples);
        }

        public static AudioClip CreateBGM()
        {
            float duration = 8f;
            int sampleCount = (int)(SampleRate * duration);
            float[] samples = new float[sampleCount];

            // Harmonic layers loop
            float[] baseFreqs = { 130.81f, 164.81f, 196f, 174.61f }; // C3, E3, G3, F3
            float chordDuration = duration / baseFreqs.Length;
            int chordSamples = (int)(SampleRate * chordDuration);

            for (int c = 0; c < baseFreqs.Length; c++)
            {
                float fundamental = baseFreqs[c];
                int startIdx = c * chordSamples;

                for (int i = 0; i < chordSamples && (startIdx + i) < sampleCount; i++)
                {
                    float t = (float)i / SampleRate;
                    float crossfade = 1f;
                    int fadeLen = SampleRate / 10; // 100ms fade
                    if (i < fadeLen) crossfade = (float)i / fadeLen;
                    if (i > chordSamples - fadeLen) crossfade = (float)(chordSamples - i) / fadeLen;

                    float wave = Mathf.Sin(2f * Mathf.PI * fundamental * t) * 0.15f;
                    wave += Mathf.Sin(2f * Mathf.PI * fundamental * 2f * t) * 0.08f; // octave
                    wave += Mathf.Sin(2f * Mathf.PI * fundamental * 1.5f * t) * 0.06f; // fifth

                    samples[startIdx + i] += wave * crossfade;
                }
            }

            ClampSamples(samples);
            return CreateClip("BGM", samples);
        }

        private static AudioClip CreateClip(string name, float[] samples)
        {
            var clip = AudioClip.Create(name, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static void ClampSamples(float[] samples)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Mathf.Clamp(samples[i], -1f, 1f);
            }
        }
    }
}
