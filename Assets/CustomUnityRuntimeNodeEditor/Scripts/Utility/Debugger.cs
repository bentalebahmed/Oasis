using UnityEngine;

public static class Debugger
{
    private static bool _enabled = true;
    private const string Prefix = "";

    /// <summary>
    /// Enable debug printing globally.
    /// </summary>
    public static void Enable() => _enabled = true;

    /// <summary>
    /// Disable all debug printing globally.
    /// </summary>
    public static void Disable() => _enabled = false;

    /// <summary>
    /// Log a normal message if enabled.
    /// </summary>
    public static void Log(string message, string category = "")
    {
#if UNITY_EDITOR || DEBUG
        if (!_enabled) return;
        if (!string.IsNullOrEmpty(category))
            Debug.Log($"{Prefix} [{category}] {message}");
        else
            Debug.Log($"{Prefix} {message}");
#endif
    }

    /// <summary>
    /// Log a warning message if enabled.
    /// </summary>
    public static void Warning(string message, string category = "")
    {
#if UNITY_EDITOR || DEBUG
        if (!_enabled) return;
        if (!string.IsNullOrEmpty(category))
            Debug.LogWarning($"{Prefix} [{category}] {message}");
        else
            Debug.LogWarning($"{Prefix} {message}");
#endif
    }

    /// <summary>
    /// Log an error message if enabled.
    /// </summary>
    public static void Error(string message, string category = "")
    {
#if UNITY_EDITOR || DEBUG
        if (!_enabled) return;
        if (!string.IsNullOrEmpty(category))
            Debug.LogError($"{Prefix} [{category}] {message}");
        else
            Debug.LogError($"{Prefix} {message}");
#endif
    }
}
