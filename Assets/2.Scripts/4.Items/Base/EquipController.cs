using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipController : MonoBehaviour
{
    public Slot mainWeapon;//메인 무기 슬롯
    public Slot subWeapon;//메인 무기 슬롯

    public Slot helmet;//헬멧 슬롯
    public Slot armor;//아머슬롯
    public Slot shoes;//신발슬롯
    // Start is called before the first frame update

    public SlotToolTip toolTip;//마우스 올리면 나오는 정보창
    private void Awake()
    {
        //장비슬롯에 접근
        mainWeapon = Characters.PlayerCtrl.Scene.gameObject.transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("MainWeapon").GetComponentInChildren<Slot>();//메인무기슬롯
        subWeapon = Characters.PlayerCtrl.Scene.gameObject.transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("SubWeapon").GetComponentInChildren<Slot>();//보조무기슬롯
        helmet = Characters.PlayerCtrl.Scene.gameObject.transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("Helmet").GetComponentInChildren<Slot>();//헬멧 슬롯
        armor = Characters.PlayerCtrl.Scene.gameObject.transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("Armor").GetComponentInChildren<Slot>();//아머슬롯
        shoes = Characters.PlayerCtrl.Scene.gameObject.transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("Shoes").GetComponentInChildren<Slot>();//신발슬롯

        toolTip = Characters.PlayerCtrl.Scene.gameObject.transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("05.ToolTip").GetComponent<SlotToolTip>();
    }
    
    public void UseItem(Slot slot)
    {
        SoundManager.Sound.EffectPlay(0);


        DragSlot.instance.dragSlot = slot;//dragSlot의 Slot에 나를 넣어준다
        DragSlot.instance.DragSetImage(slot.itemImage);//나의 이미지를 드레그 슬롯에 넣어준다
        DragSlot.instance.SetColor(0);//투명하게 다시 바꿔준다
        DragSlot.instance.dragType = slot.slotType;//나의 타입을 드레그 슬롯에 넣어준다
        DragSlot.instance.d_equipType = slot.item.equipType;//장비의 타입을 드레그 슬롯에 넣어준다

        DragSlot.instance.SetInfo(slot.slot_info);//슬롯의 정보를 전달

        if (slot.item.equipType == EquipType.Weapon)
        {
            if (mainWeapon.item == null)//메인무기가 비어있다면
            {
                mainWeapon.EquipSlot();
            }
            else if (subWeapon.item == null && mainWeapon.item != null)//서브무기가 비어있다면
            {
                subWeapon.EquipSlot();
            }
            else if( subWeapon.item != null && mainWeapon.item != null )//다 차 있다면
            {
                mainWeapon.EquipSlot();//메인 무기 슬롯에서 호출
            }
        }
        else if(slot.item.equipType == EquipType.Helmet)
        { 
            helmet.EquipSlot();//헬멧 슬롯에서 호출
        }
        else if (slot.item.equipType == EquipType.Armor)
        {
            armor.EquipSlot();//아머 슬롯에서 호출
        }
        else if (slot.item.equipType == EquipType.Shoes)
        {
            shoes.EquipSlot();//신발 슬롯에서 호출
        }

       // Debug.Log(slot.item.itemName + "을 장착했습니다");
        DragSlot.instance.dragSlot = null;//비워준다
    }

    public void ShowToolTip(Item _item, ItemInfo info, Vector3 _pos)
    {
        toolTip.ShowToolTip(_item, info, _pos);
    }

    public void HideToolTip()
    {
        toolTip.HideToolTip();
    }
}
