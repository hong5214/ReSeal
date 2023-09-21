using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestNPC : NpcCtrl
{
    // Start is called before the first frame update
    //미니맵
    public Transform miniMap;
    //월드맵
    public Transform quest_Box;
    //1단계
    public Image stageOne;
    //2단계
    public Image stageTwo;
    //3단계
    public Image stageThree;


    protected override void Start()
    {

        base.Start();
        //미니맵
        miniMap = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("MiniMap_Box");
        Debug.Assert(miniMap);

        //월드맵
        quest_Box = GameObject.Find("TownUX").transform.Find("Quest_Box");
        //1단계
        stageOne = quest_Box.transform.Find("stage1").GetComponent<Image>();
        //2단계
        stageTwo = quest_Box.transform.Find("stage2").GetComponent<Image>();
        //3단계
        stageThree = quest_Box.transform.Find("stage3").GetComponent<Image>();
        Debug.Assert(quest_Box);
        if (PhotonNetwork.isMasterClient)
        {
            SetClear();

        }


    }



    public override void myFunction()
    {
        //월드맵을 켜준다.
        quest_Box.gameObject.SetActive(true);
        //미니맵을 꺼준다
        miniMap.gameObject.SetActive(false);
        //대화창 역시 꺼준다.
        dialog_Box.gameObject.SetActive(false);

    }

    public override void SetText()
    {
        //전달하고자 하는 텍스트를 설정
        tempDialog.tempText = "마을을 구해주시오";

        //초기화 해주고(필요없을거같음 추후 삭제)
        npcName.text = "";
        //UI의 이름을 바꾸어 준다.
        npcName.text = "오래된 돌조각";
    }

    public void QuitQuest()
    {
        //월드맵을 꺼준다.
        quest_Box.gameObject.SetActive(false);
        //미니맵을 켜준다
        miniMap.gameObject.SetActive(true);
        Characters.PlayerCtrl.Scene.StartMove();
    }
    public void SetClear()
    {
        stageOne.gameObject.SetActive(GameManager.GManager.clearProcess > 1);
        stageTwo.gameObject.SetActive(GameManager.GManager.clearProcess > 2);
        stageThree.gameObject.SetActive(GameManager.GManager.clearProcess > 3);

    }




}

