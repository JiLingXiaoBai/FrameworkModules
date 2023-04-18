using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestTimer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.T))
        {
            Timer.SetTimeout(1.1f, () => { ShowData(); });
        }
    }

    void ShowData()
    {

        Debug.Log("Timer Call");
    }
}
