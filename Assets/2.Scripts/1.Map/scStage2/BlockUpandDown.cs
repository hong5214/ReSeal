using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hor_or_Ver
{
    Horizontal,
    Vertical

}

public class BlockUpandDown : MonoBehaviour
{
    [Range(-4, 4)]
    public int range;
    public Hor_or_Ver dir;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpAndDown());
        if (range == 0) { range = 2; }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator UpAndDown()
    {
        Vector3 movePos = transform.localPosition;
        while (true)
        {
            // 올라감
            yield return new WaitForSeconds(1);
            for (int i = 0; i < 100; i++)
            {
                if (dir == Hor_or_Ver.Vertical)
                {
                    //movePos.y += (range * 0.01f);
                    //transform.Translate(new Vector3(0.01f,0,0));

                    transform.position += new Vector3(0, range * 0.01f, 0);

                    //transform.position += transform.forward *range* 0.01f;
                }
                else if (dir == Hor_or_Ver.Horizontal)
                {
                    //movePos.x += (range * 0.01f);
                    // transform.Translate(movePos*Time.deltaTime);
                    //transform.Translate(new Vector3(0.01f, 0, 0));
                    //transform.position += transform.right *range* 0.01f;

                    transform.position += new Vector3(range * 0.01f, 0, 0);

                }
                //transform.localPosition= movePos;
                yield return new WaitForSeconds(0.01f);
            }

            // 내려감
            yield return new WaitForSeconds(1);
            for (int i = 0; i < 100; i++)
            {
                if (dir == Hor_or_Ver.Vertical)
                {
                    transform.position += new Vector3(0, range * -0.01f, 0);

                    // movePos.y -= (range * 0.01f);
                    //transform.Translate(new Vector3(0,0,-0.01f*range));
                    //transform.position = transform.position - transform.right * 0.01f;
                }
                else if (dir == Hor_or_Ver.Horizontal)
                {
                    transform.position += new Vector3(range * -0.01f, 0, 0);

                    //transform.Translate(new Vector3(-0.01f*range, 0, 0));//로컬일때만 작용....

                    //movePos.x -= (range * 0.01f);
                }
                //transform.localPosition= movePos;
                // transform.Translate(movePos*Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}