using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PortionShop : MonoBehaviour
{
    public Slot[] PortionSlots;//장비 아이템 슬롯들 배열

   
    public void AcquireItem(Item _item, ItemInfo _info)//장비를 습득한다면
    {
       
        PortionSlots=transform.GetComponentsInChildren<Slot>();

        for (int i = 0; i < PortionSlots.Length; i++)//빈 자리가 있는지 체크
        {
            if (PortionSlots[i].item == null)//해당 슬롯이 빈곳이라면
            {                
                PortionSlots[i].AddItem(_item, _info, _info.ItemOption);//장비 슬롯에 아이템을 더해준다
                return;
            }
        }
    }

    public void BuyItem()
    {
        foreach(Slot a in PortionSlots)
        {
            a.Sell();
        }
    }
}

