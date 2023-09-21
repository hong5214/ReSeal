using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class csLogo : MonoBehaviour
{
    public void NextLogin()
    {

     
        SceneManager.LoadScene("02.Login");

    }

    public void SoundStart()
    {
        SoundManager.Sound.StagePlay(6);

    }
}
