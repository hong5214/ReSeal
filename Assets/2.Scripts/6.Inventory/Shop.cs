using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Slot[] EquipSlots;//장비 아이템 슬롯들 배열
   


    public void AcquireItem(Item _item, ItemInfo _info)//장비를 습득한다면
    {
       
        EquipSlots=transform.GetComponentsInChildren<Slot>();

        for (int i = 0; i < EquipSlots.Length; i++)//빈 자리가 있는지 체크
        {
            if (EquipSlots[i].item == null)//해당 슬롯이 빈곳이라면
            {                
                EquipSlots[i].AddItem(_item, _info, 1);//장비 슬롯에 아이템을 더해준다
                return;
            }
        }
    }

    public void BuyItem()
    {
        foreach(Slot a in EquipSlots)
        {
            a.Sell();
        }
    }
}

