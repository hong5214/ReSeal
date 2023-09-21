using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [SerializeField]
    private GameObject go_InventoryBase;//인벤토리 베이스 이미지.Inventory 연결

    public Slot[] SetSlots;//장착 슬롯의 배열
    public Slot[] EquipSlots;//장비 아이템 슬롯들 배열
    public Slot[] UseSlots;//사용 아이템 슬롯들 배열
    public Slot[] EtcSlots;//기타 아이템 슬롯들 배열

    //창 선택 관련
    public GameObject EquipBox;
    public GameObject UseBox;
    public GameObject EtcBox;

    public static bool inventoryActivated = false;//인벤토리 활성화 여부 이것에 따라 카메라 움직임등의 무빙 막음

    public void Awake()
    {
        //슬롯들을 배열에 담는다
        //SetSlots = transform.parent.Find("03.Eqipment").GetComponentsInChildren<Slot>();
        EquipSlots = EquipBox.GetComponentsInChildren<Slot>();
        UseSlots = UseBox.GetComponentsInChildren<Slot>();
        EtcSlots = EtcBox.GetComponentsInChildren<Slot>();
        //데이터 베이스로부터 받아오자
        GameManager.GManager.GetInvenDataStart();

    }
    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").GetComponent<EquipController>().HideToolTip();
    }

    public void AcquireItem(Item _item, ItemInfo _info)//장비를 습득한다면
    {
        //장비 아이템이라면
        if (_item.itemType == ItemType.Equipment)
        {
            for (int i = 0; i < EquipSlots.Length; i++)//빈 자리가 있는지 체크
            {
                if (EquipSlots[i].item == null)//해당 슬롯이 빈곳이라면
                {
                    EquipSlots[i].AddItem(_item, _info, 1);//장비 슬롯에 아이템을 더해준다
                    return;
                }
            }
        }

        //소비 아이템을 습득한다면
        else if (_item.itemType == ItemType.Used)
        {
            for (int i = 0; i < UseSlots.Length; i++)//모든 곳을 돌며 같은 아이템이 있는지 체크
            {
                if (UseSlots[i].item != null)
                {
                    if (UseSlots[i].item != null)// 해당 슬롯에 아이템이 있고
                    {
                        if (UseSlots[i].slot_info.ItemName == _info.ItemName)//같은 아이템이라면
                        {

                            UseSlots[i].SetSlotCount(_info.ItemOption);//개수를 더해주자
                            return;
                        }
                    }
                }
            }

            //같은 아이템이 없다면
            for (int i = 0; i < UseSlots.Length; i++)//빈 자리가 있는지 체크
            {
                if (UseSlots[i].item == null)//해당 슬롯이 빈곳이라면
                {
                    UseSlots[i].AddItem(_item, _info, _info.ItemOption);//소비 슬롯에 아이템을 더해준다
                    return;
                }
            }

        }

        //기타 아이템을 습득한다면
        else if (_item.itemType == ItemType.Etc)
        {
            for (int i = 0; i < EtcSlots.Length; i++)//모든 곳을 돌며 같은 아이템이 있는지 체크
            {
                if (EtcSlots[i].item != null)
                {
                    if (EtcSlots[i].item != null)// 해당 슬롯에 아이템이 있고
                    {
                        if (EtcSlots[i].slot_info.ItemName == _info.ItemName)//같은 아이템이라면
                        {
                            EtcSlots[i].SetSlotCount(_info.ItemOption);//개수를 더해주자
                            return;
                        }
                    }
                }
            }

            //같은 아이템이 없다면
            for (int i = 0; i < EtcSlots.Length; i++)//빈 자리가 있는지 체크
            {
                if (EtcSlots[i].item == null)//해당 슬롯이 빈곳이라면
                {
                    EtcSlots[i].AddItem(_item, _info, _info.ItemOption);//소비 슬롯에 아이템을 더해준다
                    return;
                }
            }

        }



    }

    //창 선택 버튼 관련(버튼에 직접 연결해줌)
    public void ChangeEquip()//장비창으로 이동
    {
        EquipBox.SetActive(true);
        UseBox.SetActive(false);
        EtcBox.SetActive(false);

    }
    public void ChangeUse()//사용창으로 이동
    {
        EquipBox.SetActive(false);
        UseBox.SetActive(true);
        EtcBox.SetActive(false);
    }
    public void ChangeEtc()//기타창으로 이동
    {
        EquipBox.SetActive(false);
        UseBox.SetActive(false);
        EtcBox.SetActive(true);
    }
    public void SellAll()
    {
        if (EquipBox.activeSelf == true)//장비창이 켜져 있다면
        {
            foreach (Slot a in EquipSlots)
            {
                a.Sell();
            }
        }
        else if (UseBox.activeSelf == true)
        {
            foreach (Slot a in UseSlots)
            {
                a.Sell();
            }
        }
        else if (EtcBox.activeSelf == true)
        {
            foreach (Slot a in EtcSlots)
            {
                a.Sell();
            }
        }
    }
}
