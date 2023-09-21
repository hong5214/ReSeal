using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ItemPickUp : MonoBehaviour
{
   

    public Item item;

    public ItemInfo info;

  

   
   

    

    public void Awake()
    {
        string tempString = info.ItemExp;
        info = new ItemInfo();
        info.ItemStat = new int[4, 2];

        info.ItemExp = tempString;
        tempString = null;//필요한가?
    }

    public void SetItem(string itemName, int itemIndex, int itemLevel, int itemOption, int itemGold)
    {
        info.ItemName = itemName;       //아이템마다 인덱스에 따라 이름 부터
        info.Index = itemIndex;         //아이템마다 인덱스 부여
        info.ItemLevel = itemLevel;     //레벨 부여
        info.ItemOption = itemOption;   //옵션 부여
        info.ItemGold = itemGold;       //골드부여
       
        if (itemIndex < 40) //장비라면 스탯을 부여
        {            
            // 일반 아이템 고정 기본 스탯 설정
            info.ItemStat[0, 0] = ((int)item.equipType - 1) / 2; // 추가 스탯 종류(무기=0,헬멧 = 1 갑옷=0, 신발 = 1)

            info.ItemStat[0, 1] = UnityEngine.Random.Range(itemOption, itemOption + itemLevel); // 추가 수치
            // 장비등급에 따라 랜덤 추가 스탯
            for (int i = 0; i < 3; i++)
            {
                if (itemIndex % 4 > i)//아이템의 등급은 0,1,2,3
                {
                    info.ItemStat[i + 1, 0] = UnityEngine.Random.Range(0, 4); // 스탯 종류
                    info.ItemStat[i + 1, 1] = UnityEngine.Random.Range(info.ItemOption, itemOption + itemLevel); //원래 수치 + 0~10의 추가수치
                }
            }
        }
        /*else
        {
            
            // 소비, 기타 아이템 구분
            item.itemType = (itemIndex < 48) ? ItemType.Used : ItemType.Etc;//지울 예정
        }*/
    }
}

[Serializable]
public class ItemInfo//원래 Struck
{
   

    public string ItemName;//이름코드 . 이걸로 데이터베이스와 파일을 주고받을것임.

    public int Index;//아이템의 효과를 엑셀로 부터 받아오는 인덱스

    public int ItemLevel; // 아이템 레벨

    public int ItemOption; // 아이템 고정옵션(장비), 개수(소비, 기타)

    public int ItemGold; // 아이템 판매금액

    public int[,] ItemStat; // 장비 착용 추가스탯 {스탯 종류, 추가 수치}

    [TextArea]//여러줄 가능
    public string ItemExp;//아이템의 설명       //안넘어감 ..

    public void SetInfo(ItemInfo _info)
    {
        ItemName = _info.ItemName;//이름코드 . 이걸로 데이터베이스와 파일을 주고받을것임.

        Index = _info.Index;//아이템의 효과를 엑셀로 부터 받아오는 인덱스

        ItemLevel = _info.ItemLevel; // 아이템 레벨

        ItemOption = _info.ItemOption; // 아이템 고정옵션(장비), 개수(소비, 기타)

        ItemGold = _info.ItemGold; // 아이템 판매금액

        ItemStat = _info.ItemStat; // 장비 착용 추가스탯 {스탯 종류, 추가 수치}

       ItemExp = _info.ItemExp;//설명
    }
}

