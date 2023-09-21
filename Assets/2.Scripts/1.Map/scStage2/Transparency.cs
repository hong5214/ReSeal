using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    public MeshRenderer[] myMesh;
    public Collider myBox;

    [Range(0, 5)]
    public float StartTime;

    [Range(0, 5)]
    public float transTime;

    [Range(0, 5)]
    public float endTime;
    // Start is called before the first frame update
    void Awake()
    {
        myMesh = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        myBox= GetComponent<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnTransparency());
    }

    // Update is called once per frame
   

    IEnumerator OnTransparency()
    {
        
        while (true)
        {
            // 올라감
            yield return new WaitForSeconds(StartTime);//3.5동안 보인다
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < myMesh.Length; j++)
                {
                    myBox.isTrigger = true;
                    Color color = myMesh[j].material.color;
                    color.a -= 0.01f;
                    myMesh[j].material.color = color;
                }
                yield return new WaitForSeconds(transTime*0.005f);
            }
            //내려감
            for (int i = 0; i < 100; i++)
            {
               
                for (int j = 0; j < myMesh.Length; j++)
                {
                    Color color = myMesh[j].material.color;
                    color.a += 0.01f;
                    myMesh[j].material.color = color;
                }
                yield return new WaitForSeconds(transTime*0.005f);
            }
            myBox.isTrigger = false;
            yield return new WaitForSeconds(endTime);
        }
    }
}
