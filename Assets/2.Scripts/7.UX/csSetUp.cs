using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Characters;

public class csSetUp : MonoBehaviour
{
    public Slider slMouse;//마우스 감도
   

    public Slider slMap;//미니맵
    public Camera miniCam;//미니맵 카메라

    public AudioSource bgSound;
    public Slider slBg;//배경음악

    public AudioSource efSound;
    public Slider slEf;//효과음
  

    private void Awake()
    {
        slMouse = transform.Find("Mouse").GetComponent<Slider>();
        
        slMap = transform.Find("MiniMap").GetComponent<Slider>();
        miniCam = transform.root.transform.Find("MiniMapCamera").GetComponent<Camera>();

        slBg = transform.Find("BgSound").GetComponent<Slider>();
        slEf = transform.Find("EffectSound").GetComponent<Slider>();
    }

    private void Start()
    {
        LoadData();//데이터를 불러온다.

    }

    private void OnEnable()//보일때
    {
        LoadData();   
    }
    private void OnDisable()//꺼질때
    {
        SaveData();
    }

    void LoadData()//데이터 베이스로부터 값을 불러온다.
    {
        slMouse.value = (float.Parse(GameManager.GManager.settingData[0]) - 10) / 1990.0f;
        slBg.value = float.Parse(GameManager.GManager.settingData[1]);
        slEf.value = float.Parse(GameManager.GManager.settingData[2]);
        slMap.value = (float.Parse(GameManager.GManager.settingData[3]) - 100) / -50.0f;
        SetData();
    }

    void SaveData()//데이터 베이스로 값을 보내준다.
    {
        GameManager.GManager.settingData[0] = ((slMouse.value * 1990.0f) + 10).ToString();
        GameManager.GManager.settingData[1] = slBg.value.ToString();
        GameManager.GManager.settingData[2] = slEf.value.ToString();
        GameManager.GManager.settingData[3] = ((slMap.value * -50.0f) + 100).ToString();
        ExCelParsing.Write("Setting", GameManager.GManager.settingData);
        SetData();
    }
    void SetData()
    {
        Characters.PlayerCtrl.Scene.mouseSensitivity = ((slMouse.value * 1990.0f) + 10);
        miniCam.orthographicSize = (slMap.value * -50.0f) + 100;//50에서 100사이 설정
        SoundManager.Sound.bgAudio.volume = slBg.value; // 배경음 적용(사운드 매니저 적용)
        SoundManager.Sound.efAudio.volume = slEf.value; // 효과음 적용(사운드 매니저 적용)
        Characters.PlayerCtrl.Scene.audio.volume = slEf.value;
    }
}
