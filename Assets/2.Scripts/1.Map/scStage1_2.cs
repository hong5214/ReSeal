using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scStage1_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpAndDown());        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpAndDown()
    {
        Vector3 movePos = transform.position;
        while(true)
        {
            // 올라감
            yield return new WaitForSeconds(1);
            for (int i = 0; i < 100; i++)
            {
                movePos.y += 0.03f;
                transform.position = movePos;
                yield return new WaitForSeconds(0.01f);
            }

            // 내려감
            yield return new WaitForSeconds(1);
            for (int i = 0; i < 100; i++)
            {
                movePos.y -= 0.03f;
                transform.position = movePos;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
