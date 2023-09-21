using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PortionNPC : NpcCtrl
{
    // Start is called before the first frame update
    //미니맵
    public Transform miniMap;
    //상점
    public Transform shop_Box;
    bool once;



    protected override void Start()
    {

        base.Start();

        miniMap = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("MiniMap_Box");
        Debug.Assert(miniMap);
        shop_Box = GameObject.Find("TownUX").transform.Find("Portion_Box");
        //마을일때만 상점을 연결
        csItemManager.Scene.portionShop = GameObject.Find("TownUX").transform.Find("Portion_Box").GetComponent<PortionShop>();
        csItemManager.Scene.MakePortionItem();
    }


    public override void myFunction()
    {
        if (once == false)
        {
            once = true;
            Debug.Log("기능 추가 성공");
            //무기창을 켜준다.
            shop_Box.gameObject.SetActive(true);
            //인벤토리 켜준다.
            Characters.PlayerCtrl.Scene.Inven_OnOff();
            //버튼 켜준다
            Characters.PlayerCtrl.Scene.Sell_OnOff();
            //이동
            Characters.PlayerCtrl.Scene.Inventory.GetComponent<RectTransform>().localPosition = new Vector3(116, 0, 0);
            Debug.LogWarning("몇번");
            //미니맵을 꺼준다
            //miniMap.gameObject.SetActive(false);
            //대화창 역시 꺼준다.
            dialog_Box.gameObject.SetActive(false);
        }

    }

    public override void SetText()
    {
        //전달하고자 하는 텍스트를 설정
        tempDialog.tempText = "물약 사세요";

        //초기화 해주고(필요없을거같음 추후 삭제)
        npcName.text = "";
        //UI의 이름을 바꾸어 준다.
        npcName.text = "떠돌이 물약상";
    }

    public void QuitPortion()
    {
        once = false;
        //무기창을 꺼준다.
        shop_Box.gameObject.SetActive(false);
        //인벤토리 꺼준다.
        Characters.PlayerCtrl.Scene.Inven_OnOff();
        //버튼 꺼준다
        Characters.PlayerCtrl.Scene.Sell_OnOff();
        //이동
        Characters.PlayerCtrl.Scene.Inventory.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        //움직임을 멈추어 준다
        Characters.PlayerCtrl.Scene.StartMove();
    }
}

