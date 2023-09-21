using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonNPC : NpcCtrl
{

    // Start is called before the first frame update
    //미니맵
    public Transform miniMap;//잠시 꺼주기 위해서
    public Animator myAnim;//애니메이션 연결
    public Transform[] Enemys;//상대방을 체크
    public Sector mySector;
    public GameObject monsterBoss; // 2스테이지 보스

    private void Awake()
    {
        //에니메이터 연결
        myAnim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        //미니맵
        miniMap = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("MiniMap_Box");
        //나의 섹터는 1단계로 초기화       
    }
    
       

    public override void myFunction()
    {
        #region 사자상에게 말을 걸때
        if (Characters.PlayerCtrl.Scene.playerSector == Sector.One&&mySector==Sector.One)//사자상에게 말을 건다면
        {
            //대화창 꺼준다.
            dialog_Box.gameObject.SetActive(false);
            //플레이어를 움직이게 해준다
            Characters.PlayerCtrl.Scene.StartMove();
            //플레이어의 스테이지 넘버를 1로 변경
            Characters.PlayerCtrl.Scene.playerSector = Sector.Two;
            //수정 필요
            StageManager.Scene.monsterMax = GameObject.FindGameObjectsWithTag("Enemy").Length;
            StageManager.Scene.MissonText.text = "모든 적 처치!(" + StageManager.Scene.monsterMax + "/" + StageManager.Scene.monsterMax + ")";
        }
        #endregion

        else if (Characters.PlayerCtrl.Scene.playerSector == Sector.Two&& mySector == Sector.Two)// 세개의 머리에 말 걸을때
        {
            if (GameManager.GManager.StageNum == 2)//몬스터 다 잡으면(보스 빼고)
            {
                //대화창 꺼준다
                dialog_Box.gameObject.SetActive(false);
                //플레이어 움직인다
                Characters.PlayerCtrl.Scene.StartMove();
                //플레이어의 스테이지 넘버를 1로 변경
                Characters.PlayerCtrl.Scene.playerSector = Sector.Three;
                //텍스트를 변경
                StageManager.Scene.MissonText.text = "인내의 늪을 통과하세요";
                //문이 열린다.
                myAnim.SetBool("Open", true);
            }
            else //다 못잡으면
            {
                Characters.PlayerCtrl.Scene.StartMove();
                dialog_Box.gameObject.SetActive(false);//대화창 꺼준다.
            }
        }
         else if (Characters.PlayerCtrl.Scene.playerSector == Sector.Three && mySector == Sector.Three)//고뇌의 석상에 말을 걸을때
        {
            //대화창 꺼준다
            dialog_Box.gameObject.SetActive(false);
            //플레이어 움직인다
            Characters.PlayerCtrl.Scene.StartMove();
            //플레이어의 스테이지 넘버를 1로 변경
            Characters.PlayerCtrl.Scene.playerSector = Sector.Four;
            //텍스트를 변경
            StageManager.Scene.MissonText.text = "고뇌의 늪을 통과하세요";
            //문이 열린다.
            myAnim.SetBool("Open", true);
        }
        else if (Characters.PlayerCtrl.Scene.playerSector == Sector.Four && mySector == Sector.Four)//도착지 npc에 말을 걸때
        {
            //대화창 꺼준다
            dialog_Box.gameObject.SetActive(false);
            //플레이어 움직인다
            Characters.PlayerCtrl.Scene.StartMove();

            //텍스트를 변경
            StageManager.Scene.MissonText.text = "말라붙은 거북이를 처치하세요";
            if(GameManager.GManager.StageNum == 2)
            {
                GameManager.GManager.StageNum++;
                StartCoroutine(StageManager.Scene.Result());
                monsterBoss.SetActive(true);
                Vector3 pos = GameObject.Find("roamingPoint").transform.position;
                pos.z -= 140.0f;
                GameObject.Find("roamingPoint").transform.position = pos;
            }
        }
        else
        {
            Characters.PlayerCtrl.Scene.StartMove();
            dialog_Box.gameObject.SetActive(false);//대화창 꺼준다.
           
        }



    }

    public override void SetText()
    {
        if (mySector == Sector.One)//사자상에게 말을 건다면
        {
            //전달하고자 하는 텍스트를 설정
            tempDialog.tempText = "던전의 모든 몬스터를 잡으세요";

            //초기화 해주고(필요없을거같음 추후 삭제)
            npcName.text = "";
            //UI의 이름을 바꾸어 준다.

            npcName.text = "부식된 사자상";
        }
        else if (mySector == Sector.Two)//세개 석상에게 말을 건다면
        {

            //전달하고자 하는 텍스트를 설정
            tempDialog.tempText = "인내를 시험하시겠나요?";

            //초기화 해주고(필요없을거같음 추후 삭제)
            npcName.text = "";
            //UI의 이름을 바꾸어 준다.

            npcName.text = "세개의 석상";
        }
       
        else if (mySector == Sector.Three)//세개의에게 말을 건다면
        {

            //전달하고자 하는 텍스트를 설정
            tempDialog.tempText = "고뇌를 시험하시겠습니까?";

            //초기화 해주고(필요없을거같음 추후 삭제)
            npcName.text = "";
            //UI의 이름을 바꾸어 준다.

            npcName.text = "고뇌의 돌조각";
        }
        else if (Characters.PlayerCtrl.Scene.playerSector == Sector.Four)//인내를 모두 통과했다면
        {

            //전달하고자 하는 텍스트를 설정
            tempDialog.tempText = "숲을 말라붙게 만든 용을 잡아주세요";

            //초기화 해주고(필요없을거같음 추후 삭제)
            npcName.text = "";
            //UI의 이름을 바꾸어 준다.

            npcName.text = "청동";
        }


    }

   

  

}


