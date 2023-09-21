using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private void Start()
    {
        InvokeRepeating("bibdStart", 0, Time.deltaTime * 5);
    }

    void bibdStart()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }

}
