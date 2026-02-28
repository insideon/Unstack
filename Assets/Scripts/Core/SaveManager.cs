using UnityEngine;

namespace Unstack.Core
{
    public static class SaveManager
    {
        private const string HighScoreKey = "HighScore";
        private const string HighestLevelKey = "HighestLevel";
        private const string SfxVolumeKey = "SfxVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string HasPlayedBeforeKey = "HasPlayedBefore";

        public static int GetHighScore() => PlayerPrefs.GetInt(HighScoreKey, 0);
        public static void SetHighScore(int score)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }

        public static int GetHighestLevel() => PlayerPrefs.GetInt(HighestLevelKey, 0);
        public static void SetHighestLevel(int level)
        {
            if (level > GetHighestLevel())
            {
                PlayerPrefs.SetInt(HighestLevelKey, level);
                PlayerPrefs.Save();
            }
        }

        public static float GetSfxVolume() => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        public static void SetSfxVolume(float volume)
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, volume);
            PlayerPrefs.Save();
        }

        public static float GetMusicVolume() => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        public static void SetMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, volume);
            PlayerPrefs.Save();
        }

        public static bool HasPlayedBefore() => PlayerPrefs.GetInt(HasPlayedBeforeKey, 0) == 1;
        public static void SetHasPlayedBefore()
        {
            PlayerPrefs.SetInt(HasPlayedBeforeKey, 1);
            PlayerPrefs.Save();
        }
    }
}
