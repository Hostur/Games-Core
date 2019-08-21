namespace Development.Logging
{
  public enum LogLevel
  {
    Debug,
    Info,
    Warning,
    Error,
    ServiceNetworkError,
    CheatingError,
    ToDo,
    DevelopmentInfo,
    EditorInfo
  }

  public static class CoreLogger
  {
    public static void Log(this object sender, string message, LogLevel logLevel)
    {
#if UNITY_EDITOR
      if (logLevel == LogLevel.EditorInfo)
        EditorInfo("Editor", message);
#endif
      string name = sender.GetType().Name;
      switch (logLevel)
      {
        case LogLevel.Debug:
          Debug(name, message);
          break;
        case LogLevel.Info:
          Info(name, message);
          break;
        case LogLevel.Warning:
          Warning(name, message);
          break;
        case LogLevel.Error:
          Error(name, message);
          break;
        case LogLevel.ServiceNetworkError:
          ServiceNetworkError(name, message);
          break;
        case LogLevel.CheatingError:
          CheatingError(name, message);
          break;
        case LogLevel.ToDo:
          ToDo(name, message);
          break;
        case LogLevel.DevelopmentInfo:
          DevelopmentInfo(name, message);
          break;
      }
    }

    private static void Debug(string senderName, string message)
    {
//#if UNITY_EDITOR
      UnityEngine.Debug.LogFormat("<color=green>{0} </color>: {1}", senderName, message);
//#endif
    }

    private static void Info(string senderName, string message)
    {
//#if UNITY_EDITOR
      UnityEngine.Debug.LogFormat("<color=green>{0} </color>: {1}", senderName, message);
//#endif
    }

    private static void Warning(string senderName, string message)
    {
//#if UNITY_EDITOR
      UnityEngine.Debug.LogWarningFormat("<color=yellow>{0} </color>: {1}", senderName, message);
//#endif
    }

    private static void Error(string senderName, string message)
    {
//#if UNITY_EDITOR
      UnityEngine.Debug.LogErrorFormat("<color=red>{0} </color>: {1}", senderName, message);
//#endif
    }

    private static void ServiceNetworkError(string senderName, string message)
    {
//#if UNITY_EDITOR
      UnityEngine.Debug.LogErrorFormat("<color=magenta>{0} </color>: {1}", senderName, message);
//#endif
    }

    private static void CheatingError(string senderName, string message)
    {
//#if UNITY_EDITOR
      UnityEngine.Debug.LogFormat("<color=orange>{0} </color>: {1}", senderName, message);
//#endif
    }

    private static void ToDo(string senderName, string message)
    {
//#if UNITY_EDITOR
      UnityEngine.Debug.LogFormat("<color=purple>{0} </color>: {1}", senderName, message);
//#endif
    }

    private static void DevelopmentInfo(string senderName, string message)
    {
//#if UNITY_EDITOR
      UnityEngine.Debug.LogFormat("<color=pink>{0} </color>: {1}", senderName, message);
//#endif
    }

    private static void EditorInfo(string senderName, string message)
    {
      UnityEngine.Debug.LogFormat("<color=cyan>{0} </color>: {1}", senderName, message);
    }
  }
}

