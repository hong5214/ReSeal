using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapNPC : NpcCtrl
{
    // Start is called before the first frame update
    //미니맵
    public Transform miniMap;
    //월드맵
    public Transform worldMap;
    //경고 메시지
    public Transform warning;
    //던전
    private Dungeon dungeon;
    //스테이지
    private Stage stage;
   

     protected override void Start()
    {
       
        base.Start();
        //미니맵
        miniMap = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("MiniMap_Box");
        Debug.Assert(miniMap);

        //월드맵
        worldMap = GameObject.Find("TownUX").transform.Find("WorldMap_Box");
        Debug.Assert(worldMap);

        ///경고창
        warning = worldMap.transform.Find("Warning");
        Debug.Assert(warning);

        /////////////지울 예정 테스트용 던전 설정////////
        ///
        dungeon = Dungeon.Frozen;
        stage = Stage.One;
    }
    
       

    public override void myFunction()
    {
        //월드맵을 켜준다.
        worldMap.gameObject.SetActive(true);
        //미니맵을 꺼준다
        miniMap.gameObject.SetActive(false);
        //대화창 역시 꺼준다.
        dialog_Box.gameObject.SetActive(false);
      
    }

    public override void SetText()
    {
        //전달하고자 하는 텍스트를 설정
        tempDialog.tempText = "오늘도 날씨가 좋군.항해하기 딱 좋은 날이야!\n배를 타고 모험을 떠나보겠는가?";

        //초기화 해주고(필요없을거같음 추후 삭제)
        npcName.text = "";
        //UI의 이름을 바꾸어 준다.
        npcName.text = "뱃사람 드루이드";
    }

    public void QuitMap()
    {
        //월드맵을 꺼준다.
        worldMap.gameObject.SetActive(false);
        //미니맵을 켜준다
        miniMap.gameObject.SetActive(true);
        Characters.PlayerCtrl.Scene.StartMove();
    }

    public void GoToForest()
    {
        //현재 나의 진행도가 숲이라면(sql에 저장된 스테이지 정보를 불러옴)
      
        if(stage==Stage.One)
        {
            GameManager.GManager.StageNum = 1;
            GameManager.GManager.NextScene(2);
        }
        else if(stage==Stage.Two)
        {

        }
        else if(stage==Stage.Three)
        {
                
        }
    }
    public void GoToDesert()
    {
        //현재 나의 진행도가 사막 혹은 얼음이라면
        if (dungeon == Dungeon.Desert||dungeon==Dungeon.Frozen)        
        {
            if (stage == Stage.One)
            {
                GameManager.GManager.StageNum = 1;
                GameManager.GManager.NextScene(3);
            }
            else if (stage == Stage.Two)
            {

            }
            else if (stage == Stage.Three)
            {

            }

        }
        else
        {
            //경고창을 띄움
            StartCoroutine(SetWarning());
        }

    }
    public void GoToFrozen()
    {
        if (dungeon == Dungeon.Frozen)
        {
            if (stage == Stage.One)
            {
                GameManager.GManager.StageNum = 1;
                GameManager.GManager.NextScene(4);
            }
            else if (stage == Stage.Two)
            {

            }
            else if (stage == Stage.Three)
            {

            }

        }
        else
        {
            //경고창을 띄움
            StartCoroutine(SetWarning());
        }
    }

    //경고창을 띄워준다
    IEnumerator SetWarning()
    {
        warning.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        warning.gameObject.SetActive(false);
        
    }




}

