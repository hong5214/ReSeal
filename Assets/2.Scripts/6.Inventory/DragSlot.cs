using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour//드레그시 이미지를 보여주고,정보를 전달해준다.
{
    static public DragSlot instance;//여러곳에서 공유 가능..
    public Slot dragSlot;
    [SerializeField]
    private Image imageItem;
    [SerializeField]
    private Image myImage;

    public ItemType dragType;//슬롯 타입
    public EquipType d_equipType;//아이템 타입

    public ItemInfo slot_info;//아이템의 정보


    // Start is called before the first frame update
    private void Awake()
    {
        imageItem = transform.Find("ItemImage").GetComponent<Image>();//자식의 이미지
        Debug.Assert(imageItem);
        myImage = GetComponent<Image>();//나의 이미지.여기서는 틀.
        Debug.Assert(myImage);
    }
    void Start()
    {
        //if (instance == null)//빼도 ok
        {
            instance = this;
        }
    }

    //드레그 대상(시작)의 이미지를 넣는다
    public void DragSetImage(Image _itemImage)
    {
        imageItem.sprite = _itemImage.sprite;//전달받은 이미지를 넣어준다
        SetColor(1);
    }

    //색상을 바꾸어준다
    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        myImage.color = color;//배경
        imageItem.color = color;//아래

    }

    public void SetInfo(ItemInfo _info)
    {
        slot_info.ItemName=_info.ItemName;//이름코드 . 이걸로 데이터베이스와 파일을 주고받을것임.

        slot_info.Index=_info.Index;//아이템의 효과를 엑셀로 부터 받아오는 인덱스

        slot_info.ItemLevel = _info.ItemLevel; // 아이템 레벨

        slot_info.ItemOption = _info.ItemOption; // 아이템 고정옵션(장비), 개수(소비, 기타)

        slot_info.ItemGold = _info.ItemGold; // 아이템 판매금액

        slot_info.ItemStat = _info.ItemStat; // 장비 착용 추가스탯 {스탯 종류, 추가 수치}

       slot_info.ItemExp = _info.ItemExp;//아이템의 설명
  
    }

}


