using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestModeButton : MonoBehaviour
{
    private bool check = false;

    public Toggle[] Channel = new Toggle[3];


    public void StartTestButton()
    {
        StartCoroutine("TestButtonCor");
    }

    public void SetChannel(int num)
    {
        GameManager.GManager.PhotonNum = GameManager.SetPhotonServer(ref GameManager.GManager.secretData, num);
    }

    void Start()
    {
        GetComponent<Toggle>().isOn = false;
        GameManager.GManager.TestMode = false;
        Channel[0].isOn = true;
    }



    private IEnumerator TestButtonCor()
    {
        if (!check)
        {
            check = true;
            GameManager.GManager.TestMode = GetComponent<Toggle>().isOn;
            //Debug.Log(GameManager.GManager.TestMode);
            yield return new WaitForSeconds(0.3f);
            check = false;
        }
    }
}