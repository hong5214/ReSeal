using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.GManager.InitStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
