using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    [SerializeField]
    private string m_RootPath;

    [SerializeField]
    private LocationManager m_LocationManager;

    private void Awake()
    {
        string rootPath = DataPathPlugin.DetermineRootPath();

        if (rootPath != "")
            m_RootPath = rootPath;

        Debug.Log("Registered root path: " + m_RootPath);
    }

    public void Serialize()
    {
        //Create the root object
        JSONClass rootNode = new JSONClass();

        //Serialize the location manager
        m_LocationManager.Serialize(rootNode);

        //Write the JSON data (.ToString in release as it saves a lot of data compard to ToJSON)
        string jsonStr = "";
        #if UNITY_ANDROID && !UNITY_EDITOR
            jsonStr = rootNode.ToString();
        #else
            jsonStr = rootNode.ToJSON(0);
        #endif

        try
        {
            File.WriteAllText(m_RootPath + "/savegame.json", jsonStr);
            Debug.Log("Save game succesfully saved!");
        }
        catch (Exception e)
        {
            Debug.LogError("Save game error: " + e);
        }
    }

    public void Deserialize()
    {

    }
}
