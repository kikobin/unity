public static class RunResultStore
{
    public static bool HasResult { get; private set; }
    public static bool IsWin { get; private set; }
    public static int Score { get; private set; }
    public static int Wave { get; private set; }
    public static float TimeSeconds { get; private set; }

    public static void SetResult(bool isWin, int score, int wave, float timeSeconds)
    {
        HasResult = true;
        IsWin = isWin;
        Score = score < 0 ? 0 : score;
        Wave = wave < 0 ? 0 : wave;
        TimeSeconds = timeSeconds < 0f ? 0f : timeSeconds;
    }

    public static void Reset()
    {
        HasResult = false;
        IsWin = false;
        Score = 0;
        Wave = 0;
        TimeSeconds = 0f;
    }

    public static bool TryGetResult(out bool isWin, out int score, out int wave, out float timeSeconds)
    {
        if (!HasResult)
        {
            isWin = false;
            score = 0;
            wave = 0;
            timeSeconds = 0f;
            return false;
        }

        isWin = IsWin;
        score = Score < 0 ? 0 : Score;
        wave = Wave < 0 ? 0 : Wave;
        timeSeconds = TimeSeconds < 0f ? 0f : TimeSeconds;
        return true;
    }
}
