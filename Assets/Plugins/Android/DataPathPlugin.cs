using UnityEngine;

public static class DataPathPlugin 
{
    #if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject m_AndroidPlugin;
    #endif

    private static AndroidJavaObject GetDataPathPlugin()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (m_AndroidPlugin != null)
                return m_AndroidPlugin;

            AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            m_AndroidPlugin = new AndroidJavaObject("com.kapistijn.androiddatapathplugin.AndroidDataPathPlugin", currentActivity);

            return m_AndroidPlugin;
        #else
            return null;
        #endif
    }

    //Utility
    public static string DetermineRootPath()
    {
        //No more Application.persistentDataPath as it will never return the SD card.

        #if UNITY_ANDROID && !UNITY_EDITOR  
        int numberOfStorageDevices = DataPathPlugin.GetExternalDataPathCount();

        //Backwards as we prefer an external SD card!
        for (int i = (numberOfStorageDevices - 1); i >= 0; --i)
        {
            if (DataPathPlugin.CanReadExternalDataPath(i) && DataPathPlugin.CanWriteExternalDataPath(i))
            {
                return DataPathPlugin.GetExternalDataPath(i);
            }
        }
        #endif

        return "";
    }

    //External
    public static int GetExternalDataPathCount()
    {
        if (GetDataPathPlugin() == null)
            return 0;

        return GetDataPathPlugin().Call<int>("GetExternalDataPathCount");
    }

    public static string GetExternalDataPath(int index)
    {
        if (GetDataPathPlugin() == null)
            return "";

        return GetDataPathPlugin().Call<string>("GetExternalDataPath", index);
    }

    public static bool CanReadExternalDataPath(int index)
    {
        if (GetDataPathPlugin() == null)
            return false;

        return GetDataPathPlugin().Call<bool>("CanReadExternalDataPath", index);
    }

    public static bool CanWriteExternalDataPath(int index)
    {
        if (GetDataPathPlugin() == null)
            return false;

        return GetDataPathPlugin().Call<bool>("CanWriteExternalDataPath", index);
    }

    //Internal
    public static string GetInternalDataPath()
    {
        if (GetDataPathPlugin() == null)
            return "";

        return GetDataPathPlugin().Call<string>("GetInternalDataPath");
    }

    public static string GetInternalDataPathAbsolute()
    {
        if (GetDataPathPlugin() == null)
            return "";

        return GetDataPathPlugin().Call<string>("GetInternalDataPathAbsolute");
    }

    public static string GetInternalDataPathCanonical()
    {
        if (GetDataPathPlugin() == null)
            return "";

        return GetDataPathPlugin().Call<string>("GetInternalDataPathCanonical");
    }

    public static bool CanReadInternalDataPath()
    {
        if (GetDataPathPlugin() == null)
            return false;

        return GetDataPathPlugin().Call<bool>("CanReadInternalDataPath");
    }

    public static bool CanWriteInternalDataPath()
    {
        if (GetDataPathPlugin() == null)
            return false;

        return GetDataPathPlugin().Call<bool>("CanWriteInternalDataPath");
    }
}
