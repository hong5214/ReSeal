using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StorageNPC : NpcCtrl
{
    // Start is called before the first frame update
    //미니맵
    public Transform miniMap;
    //창고 창
    public Transform storage_Box;
   
    bool once;



    protected override void Start()
    {

        base.Start();
        //미니맵
        miniMap = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("MiniMap_Box");
        Debug.Assert(miniMap);
        //창고
        storage_Box = GameObject.Find("TownUX").transform.Find("Storage_Box");
        Debug.Assert(storage_Box);


    }
    

    public override void myFunction()
    {
        if (once == false)
        {
            once = true;
            //창고창을 켜준다.
            storage_Box.gameObject.SetActive(true);
            //인벤토리 켜준다.
            Characters.PlayerCtrl.Scene.Inven_OnOff();
            //이동
            Characters.PlayerCtrl.Scene.Inventory.GetComponent<RectTransform>().localPosition = new Vector3(116, 0, 0);
            //미니맵을 꺼준다
            miniMap.gameObject.SetActive(false);
            //대화창 역시 꺼준다.
            dialog_Box.gameObject.SetActive(false);
        }

    }

    public override void SetText()
    {
        //전달하고자 하는 텍스트를 설정
        tempDialog.tempText = "모두 모두 맡겨줄게 .";

        //초기화 해주고(필요없을거같음 추후 삭제)
        npcName.text = "";
        //UI의 이름을 바꾸어 준다.
        npcName.text = "여대현 전당포";
    }

    public void QuitStorage()
    {
        once = false;
        //창고창을 꺼준다.
        storage_Box.gameObject.SetActive(false);
        //인벤토리 꺼준다.
        Characters.PlayerCtrl.Scene.Inven_OnOff();
        //이동
        Characters.PlayerCtrl.Scene.Inventory.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        //움직임을 시작 해준다
        Characters.PlayerCtrl.Scene.StartMove();
        //미니맵을 켜준다
        miniMap.gameObject.SetActive(true);
    }
}

