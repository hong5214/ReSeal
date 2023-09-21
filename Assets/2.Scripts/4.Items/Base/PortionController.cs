using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortionController : MonoBehaviour
{
    public string[,] itemData;//아이템의 csv

    // Start is called before the first frame update
    private void Awake()
    {

        //아이템의 정보를 csv로 부터 가져온다
        List<Dictionary<string, object>> data = ExCelParsing.Read("Item");
        itemData = new string[5, data.Count];
        for (var i = 0; i < data.Count; i++)
        {
            itemData[0, i] = data[i]["Name"].ToString();
            itemData[1, i] = data[i]["Index"].ToString();
            itemData[2, i] = data[i]["Level"].ToString();
            itemData[3, i] = data[i]["Option"].ToString();
            itemData[4, i] = data[i]["Gold"].ToString();
        }
    }
    public  void UseItem(int index)
    {
        SoundManager.Sound.EffectPlay(1);
        if (index < 42)//체력템이라면
        {
            //캐릭터의 체력을 채워준다
            Characters.PlayerCtrl.Scene.Hp += Characters.PlayerCtrl.Scene.MaxHp * (int.Parse(itemData[2,index]) == 1 ? 0.2f : 0.5f);//최대 체력에 비례하여 채워줄것임.

            if (Characters.PlayerCtrl.Scene.Hp > Characters.PlayerCtrl.Scene.MaxHp)//최대체력을 넘어간다면
                Characters.PlayerCtrl.Scene.Hp = Characters.PlayerCtrl.Scene.MaxHp;//최대체력으로 고정
            //UI를 업데이트
            StageManager.Scene.HpBar.fillAmount = Characters.PlayerCtrl.Scene.Hp / Characters.PlayerCtrl.Scene.MaxHp;


            //플레이어 정보가 DB로 StartCoroutine(GameManager.GManager.SetPlayerData());
        }
        else//물약템이라면
        {
            //캐릭터의 마력을 채워준다
            Characters.PlayerCtrl.Scene.Mp += (int.Parse(itemData[2,index]) == 1 ? 20.0f : 50.0f); ;// 고정 MP 회복

            if (Characters.PlayerCtrl.Scene.Mp > 100.0f)//최대체력을 넘어간다면
                Characters.PlayerCtrl.Scene.Mp = 100.0f;//최대체력으로 고정
                                                                                   //UI를 업데이트
            StageManager.Scene.MpBar.fillAmount = Characters.PlayerCtrl.Scene.Mp * 0.01f;

        }
    }
}
