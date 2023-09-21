using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : csSceneManager<SkillManager>
{
    public Transform skillBox;
    Transform Model;
    Transform[] Slots = new Transform[10];
    public Transform CoolTimeBox;

    public int[] levelSkill = new int[10]; // 스킬 레벨
    string[] nameSkill = new string[10]; // 스킬 이름
    int skillPoint = 0; // 스킬 포인트
    public Text skillPointNum; // 스킬창의 스킬 포인트
    public GameObject[] CoolTimeGOs = new GameObject[10];    

    void Start()
    {
        SkillPointCheck();
        Model = skillBox.Find("model" + GameManager.GManager.avatar);
        for (int i = 0; i < 10; i++)
        {            
            Slots[i] = Model.GetChild(i);
            Color color = Slots[i].GetComponent<Image>().color;
            if(levelSkill[i] < 1)
            {
                color.a = 0.35f;
                Slots[i].GetComponent<Image>().color = color;

                CoolTimeGOs[i].GetComponent<Image>().sprite = Slots[i].GetComponent<Image>().sprite;
                CoolTimeGOs[i].transform.Find("Image").GetComponent<Image>().sprite = Slots[i].GetComponent<Image>().sprite;
            }
            else
            {
                color.a = 1.0f;
                Slots[i].GetComponent<Image>().color = color;
                CoolTimeGOs[i].GetComponent<Image>().sprite = Slots[i].GetComponent<Image>().sprite;
                CoolTimeGOs[i].transform.Find("Image").GetComponent<Image>().sprite = Slots[i].GetComponent<Image>().sprite;
                CoolTimeGOs[i].transform.Find("Image").GetComponent<Image>().fillAmount = 0f;
                CoolTimeGOs[i].SetActive(true);
            }
        }
    }


    public void SkillPointCheck()
    {
        // 사용 가능 포인트 계산
        skillPoint = Characters.PlayerCtrl.Scene.Level;
        for (int i = 0; i < 10; i++)
            skillPoint -= levelSkill[i];
        // 스킬 포인트 텍스트 변경하기
        skillPointNum.text = skillPoint.ToString();
        if (skillPoint != 0)
        {

        }
        else
        {

        }


    }
    public void SkillUp(int skillNum)
    {
        if (skillPoint < 1) return;
        levelSkill[skillNum]++;
        StartCoroutine(GameManager.GManager.SetSkillData(skillNum));
        // 올린 스킬 레벨업에 따른 정보 변경하기
        SkillPointCheck();
        Color color = Slots[skillNum].GetComponent<Image>().color;
        color.a = 1.0f;
        Slots[skillNum].GetComponent<Image>().color = color;
        CoolTimeGOs[skillNum].GetComponent<Image>().sprite = Slots[skillNum].GetComponent<Image>().sprite;
        CoolTimeGOs[skillNum].transform.Find("Image").GetComponent<Image>().sprite = Slots[skillNum].GetComponent<Image>().sprite;
        CoolTimeGOs[skillNum].transform.Find("Image").GetComponent<Image>().fillAmount = 0f;
        CoolTimeGOs[skillNum].SetActive(true);
        //skillBox.GetComponent<AudioSource>().Stop();
        skillBox.GetComponent<AudioSource>().Play();
    }


    public void LearnSkill(int skillNum, string txt)
    {
        CoolTimeGOs[skillNum].GetComponent<Image>().sprite = Slots[skillNum].GetComponent<Image>().sprite;
        CoolTimeGOs[skillNum].transform.Find("Image").GetComponent<Image>().sprite = Slots[skillNum].GetComponent<Image>().sprite;
        CoolTimeGOs[skillNum].SetActive(true);
        CoolTimeGOs[skillNum].transform.Find("Text").GetComponent<Text>().text = txt;
    }

    public IEnumerator ShowCoolTimeUI(int skillNum, float time)
    {
        string text1 = CoolTimeGOs[skillNum].transform.Find("Text").GetComponent<Text>().text;
        CoolTimeGOs[skillNum].transform.Find("Image").GetComponent<Image>().fillAmount = 1;
        float tt = time * 0.55f;
        for (float i = 0; i < time; i += 0.01f)
        {
            CoolTimeGOs[skillNum].transform.Find("Image").GetComponent<Image>().fillAmount = (time - i) / time;
            CoolTimeGOs[skillNum].transform.Find("Text").GetComponent<Text>().text = (time - i).ToString("F0");
            yield return new WaitForSeconds(0.01f * 0.55f);
        }
        CoolTimeGOs[skillNum].transform.Find("Image").GetComponent<Image>().fillAmount = 0;
        CoolTimeGOs[skillNum].transform.Find("Text").GetComponent<Text>().text = text1;
    }



}
