using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// This is the class that keeps track of objects protected from being destroyed when a new scene is loaded. Use instead of using DontDestroyOnLoad().
/// </summary>
public class DontDestroyOnLoadManager : MonoBehaviour
{
    /// <summary>
    /// This is the static object that keeps track of other objects protected from being destroyed when a new scene is loaded. Use instead of using DontDestroyOnLoad().
    /// </summary>
    public static DontDestroyOnLoadManager instance;

    private ArrayList objects;

    private void Awake()
    {
        if (instance == null)
        {
            objects = new ArrayList();
            instance = this;
            DontDestroyOnLoad(gameObject);//We don't want to be destroyed ourselves either
        } else
        {
            Destroy(this.gameObject);
            throw new Exception("Only one DontDestoryOnLoadManager allowed!");
        }
    }

    /// <summary>
    /// DANGER this will destroy ALL objects set as protected by DontDestoryOnLoadManager
    /// </summary>
    public void DestroyAllProtectedObjects()
    {
        for(int i = 0; i< objects.Count; i++)
        {
            Destroy((GameObject)objects[i]);
            objects.RemoveAt(i);
        }
    }

    /// <summary>
    /// This will protect the selected object from being destoryed when a new scene is loaded. Use this instead of DontDestroyOnLoad()
    /// </summary>
    /// <param name="obj"></param>
    public void ProtectObject(GameObject obj)
    {
        DontDestroyOnLoad(obj.gameObject);
        objects.Add(obj);
    }


    /// <summary>
    /// This will destroy the provided gameobject.
    /// </summary>
    /// <param name="obj"></param>
    public void DestroyProtectedObject(GameObject obj)
    {
        try
        {
            objects.Remove(obj);
            Destroy(obj);
        } catch
        {
            throw new Exception(" \"" + obj.name + "\" " + "is not protected by DontDestroyOnLoadManager");
        }
    }
}

