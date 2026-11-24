using UnityEngine;

public static class SaveData
{
    const string UnlockedKey = "unlocked";
    const string MusicKey = "MusicVolume";
    static string BestKey(int l) => "best" + l;

    public static int Unlocked => PlayerPrefs.GetInt(UnlockedKey, 1);
    public static bool IsUnlocked(int level) => level <= Unlocked;
    public static void Unlock(int level)
    {
        level = Mathf.Clamp(level, 1, 5);
        if (level > Unlocked) { PlayerPrefs.SetInt(UnlockedKey, level); PlayerPrefs.Save(); }
    }
    public static bool HasBest(int level) => PlayerPrefs.HasKey(BestKey(level));
    public static float GetBest(int level) => PlayerPrefs.GetFloat(BestKey(level), 0f);
    public static bool Record(int level, float time)
    {
        if (HasBest(level) && time >= GetBest(level)) return false;
        PlayerPrefs.SetFloat(BestKey(level), time); PlayerPrefs.Save();
        return true;
    }
    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MusicKey, 1f);
        set { PlayerPrefs.SetFloat(MusicKey, value); PlayerPrefs.Save(); }
    }
}
