﻿using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour
{

    private static GUIManager instance;
    public static GUIManager Instance
    {
        get { return instance; }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.getProgress() > 0)
        {

        }
    }
}