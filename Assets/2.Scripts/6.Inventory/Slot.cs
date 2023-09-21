using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Ipointer 인터페이스 사용하기 위해서



public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,IPointerEnterHandler,IPointerExitHandler
{
    

   #region 변수들

    public Item item;//아이템
    public ItemInfo slot_info;//아이템의 정보

    public Image itemImage;//아이템의 이미지
    public int itemCount;//획득한 아이템의 개수

    [SerializeField]
    private Text text_Count;//아이템의 개수 UI
    [SerializeField]
    private GameObject go_CountImage;//아이템 개수 표시창 아이템 종류에 따라 On/Off

    public PortionController portionUse;//물약을 사용하기 위해 itemManager컴포넌트에 접근
    public EquipController equipUse;

    public ItemType slotType;//슬롯의 타입
    public EquipType equipType;//장비라면 각각의 타입

    public Transform clicked;//클릭되면 하이라이트
    bool canSell;//팔수 있는지
    public bool isMarket;//마켓인지 아닌지

    #endregion



    #region 초기화

    public void Awake()
    {

        //포션을 사용하기 위해
        portionUse = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").GetComponent<PortionController>();
        equipUse = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").GetComponent<EquipController>();
        clicked = transform.Find("Clicked");
    }

    #endregion

    

   #region 아이템을 먹는다면

    //아이템을 더해준다(아이템 먹으면 호출)
    public void AddItem(Item _item,ItemInfo _info, int _count = 1)
    {
        //전달받은 아이템으로 교체
        item = _item;
        itemCount = _count;//개수
        itemImage.sprite = item.itemImage;//이미지

        SetInfo(_info);//정보를 저장


        if (item.itemType!=ItemType.Equipment)//장비타입이 아이템이 아니라면 개수를 표시
        {
           
            go_CountImage.SetActive(true);//아이템 개수 표시창 꺼주고
            text_Count.text = itemCount.ToString();//개수 표시
        }
        else if(item.itemType == ItemType.Equipment) //장비라면 장비만 표시
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }
        SetColor(1);//보이게 해준다
        SendData();//데이터베이스로 보내자
    }


    //아이템 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }


    //해당 슬롯의 아이템 개수 업데이트
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();//개수 업데이트
        SendData();//데이터베이스로 보내자

        //만약 아이템이 없다면 그 슬롯 초기화
        if (itemCount <= 0)
            ClearSlot();
    }

    //초기화
    private void ClearSlot()
    {
        ClearData(equipType,slot_info.Index);//장비의 종류 보내서 지워주자
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetInfoClear();
        SetColor(0);
        text_Count.text = "0";
        go_CountImage.SetActive(false);
        //데이터 베이스에 빈 값 넣어주기
        
    }

    #endregion


   
   #region 아이템 정보 세팅

    //전달받은 정보로 슬롯의 정보 초기화
   public void SetInfo(ItemInfo _info)
    {
        slot_info.ItemName = _info.ItemName;//이름코드 . 이걸로 데이터베이스와 파일을 주고받을것임.

        slot_info.Index = _info.Index;//아이템의 효과를 엑셀로 부터 받아오는 인덱스

        slot_info.ItemLevel = _info.ItemLevel; // 아이템 레벨

        slot_info.ItemOption = _info.ItemOption; // 아이템 고정옵션(장비), 개수(소비, 기타)

        slot_info.ItemGold = _info.ItemGold; // 아이템 판매금액

        slot_info.ItemStat = _info.ItemStat; // 장비 착용 추가스탯 {스탯 종류, 추가 수치}

        slot_info.ItemExp = _info.ItemExp;//설명
    }

    //모두 삭제 (비어있음)
    void SetInfoClear()
    {
        slot_info.ItemName = null;//이름코드 . 이걸로 데이터베이스와 파일을 주고받을것임.

        slot_info.Index = 0;//아이템의 효과를 엑셀로 부터 받아오는 인덱스

        slot_info.ItemLevel = 0; // 아이템 레벨

        slot_info.ItemOption = 0; // 아이템 고정옵션(장비), 개수(소비, 기타)

        slot_info.ItemGold = 0; // 아이템 판매금액

        slot_info.ItemStat = null; // 장비 착용 추가스탯 {스탯 종류, 추가 수치}

        slot_info.ItemExp = null;//설명
    }
    #endregion



    #region  슬롯간 이동

    //마우스 클릭 이벤트
    //이벤트에 대한 Raycast는 Canvas의 Graphic Raycaster 컴포넌트에서 쏴주고
    //어떤 종류의 이벤트인지에 대한 답을 Raycast 로 쏴주는건 EventSystem 오브젝트이다.
    // 이 이벤트에 대한 Raycast를 받기 위해선 UI들은 Raycast Target 가 체크 되어 있어야 하며 일반 오브젝트들은 Collider 가 붙어 있어야 한다.


    //드레그 시작하면 발생하는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && !isMarket)//마켓이 아닐때
        {
            canSell = false;
            clicked.gameObject.SetActive(canSell);//하이라이트 꺼준다


            DragSlot.instance.dragSlot = this;//dragSlot의 Slot에 나를 넣어준다
            DragSlot.instance.DragSetImage(itemImage);//나의 이미지를 드레그 슬롯에 넣어준다
            DragSlot.instance.transform.position = eventData.position;//드레그 시작시의 위치로 변경

            DragSlot.instance.dragType = slotType;//나의 타입을 드레그 슬롯에 넣어준다
            DragSlot.instance.d_equipType = item.equipType;//장비의 타입을 드레그 슬롯에 넣어준다

            DragSlot.instance.SetInfo(slot_info);//슬롯의 정보를 전달

            if (equipType != EquipType.Non)
            {
                //if (transform.name != "Slot (3)")
                SetPlayerData(-1, slot_info);
            }
        }
    }

    //드레그중 호출
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null && !isMarket)//마켓이 아닐때
        {
            DragSlot.instance.transform.position = eventData.position;//위치값을 드레그 위치로 옮겨준다
        }
    }

    //나 자신의 드레그 끝나면 호출(드레그 대상 오브젝트에서 호출)
    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;//비워준다



        if (equipType == DragSlot.instance.d_equipType && item != null)//나 자신을 왔다 갓다 했을ㄷ댸
        {
            //if (transform.name != "Slot (3)")
            SetPlayerData(1, slot_info);
        }
    }

    //슬롯에 무언가가 마우스 드롭이 된다면/드래그 멈춘 위치의 오브젝트에서 호출 + 슬롯의 종류가 일치한다면 + 장비라면 종류까지 일치할때
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null && DragSlot.instance.dragType == slotType)//목적지와 시작지의 장비타입(equip,use ,etc)이 같다면
        {
            if (equipType != EquipType.Non)//만약 나의 equip타입이 있다면(장착창에 드랍될때)
            {
                if (equipType == DragSlot.instance.d_equipType)//서로의 장비타입이 일치할때
                {
                    //if (transform.name != "Slot (3)")
                    SetPlayerData(-1, slot_info);
                    ChangeSlot();
                    //if(transform.name!="Slot (3)")
                    SetPlayerData(1, slot_info);
                }
            }

            else//인벤토리에 드랍될때

            {
                //if (DragSlot.instance.d_equipType==item.equipType || equipType==EquipType.Non)//아 모르ㅔㅆ다
                if (item == null)
                {
                    ChangeSlot();
                }
                else//아이템이 있을때
                {
                    if (DragSlot.instance.d_equipType == item.equipType)
                    {
                        ChangeSlot();
                    }
                }
            }

        }
    }


    private void ChangeSlot()
    {
        //해당 슬롯에 추가
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        //ItemInfo _tempItemInfo = slot_info;//여기가 문제네 ,, 
        ItemInfo _tempItemInfo = new ItemInfo();
        _tempItemInfo.ItemStat = new int[4, 2];

        _tempItemInfo.SetInfo(slot_info);


        //_tempItemInfo.ItemName = slot_info.ItemName;//이름코드 . 이걸로 데이터베이스와 파일을 주고받을것임.

        //_tempItemInfo.Index = slot_info.Index;//아이템의 효과를 엑셀로 부터 받아오는 인덱스

        // _tempItemInfo.ItemLevel =  slot_info.ItemLevel; // 아이템 레벨

        //  _tempItemInfo.ItemOption =  slot_info.ItemOption; // 아이템 고정옵션(장비), 개수(소비, 기타)

        //  _tempItemInfo.ItemGold =  slot_info.ItemGold; // 아이템 판매금액

        //  _tempItemInfo.ItemStat =  slot_info.ItemStat; // 장비 착용 추가스탯 {스탯 종류, 추가 수치}

        //  _tempItemInfo.ItemExp =  slot_info.ItemExp;//설명

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.slot_info, DragSlot.instance.dragSlot.itemCount);


        if (_tempItem != null)//아이템이 있었다면 위치를 바꾸어 주고
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemInfo, _tempItemCount);
        }
        else//아니라면 원래 있던 슬롯을 비워준다
        {
            DragSlot.instance.dragSlot.ClearSlot();

        }
        SoundManager.Sound.EffectPlay(0);
    }

    public void SetPlayerData(int type, ItemInfo _info)//Test
    {
        if (_info.ItemStat == null) return;

        if (_info.Index < 24)//공격장비라면
        {
            for (int i = 0; i < 4; i++)//등급에 따른 속성 플레이어에 더해준다
            {
                if (_info.Index % 4 >= i)
                {
                    if ((_info.ItemStat[i, 0] + 1) == ((int)AtkEquipType.AtkDamage))//해당 스텟이 공격이라면
                    {
                        Characters.PlayerCtrl.Scene.Atk += (type * _info.ItemStat[i, 1]);
                    }
                    else if ((_info.ItemStat[i, 0] + 1) == ((int)AtkEquipType.AtkSpeed))//해당 스텟이 공격속도라면
                    {
                        Characters.PlayerCtrl.Scene.AtkSpeed += (type * _info.ItemStat[i, 1]);
                    }
                    else if ((_info.ItemStat[i, 0] + 1) == ((int)AtkEquipType.Critical))//해당 스텟이 크리티컬라면
                    {
                        Characters.PlayerCtrl.Scene.Critical += (type * _info.ItemStat[i, 1]);
                    }
                    else if ((_info.ItemStat[i, 0] + 1) == ((int)AtkEquipType.CriticalMax))//해당 스텟이 크리티컬맥스라면
                    {
                        Characters.PlayerCtrl.Scene.CriticalMax += (type * _info.ItemStat[i, 1]);
                    }

                }


            }
        }
        else if (_info.Index < 40)//방어장비라면
        {


            for (int i = 0; i < 4; i++)//등급에 따른 속성부여
            {

                if (_info.Index % 4 >= i)
                {
                    if ((_info.ItemStat[i, 0] + 1) == ((int)DefEquipType.Def))//해당 스텟이 방어라면
                    {
                        Characters.PlayerCtrl.Scene.Def += (type * _info.ItemStat[i, 1]);
                    }
                    else if ((_info.ItemStat[i, 0] + 1) == ((int)DefEquipType.MoveSpeed))//해당 스텟이 이동속도라면
                    {
                        Characters.PlayerCtrl.Scene.MoveSpeed += (type * _info.ItemStat[i, 1]);
                    }
                    else if ((_info.ItemStat[i, 0] + 1) == ((int)DefEquipType.MaxHp))//해당 스텟이 최대체력이라면
                    {
                        Characters.PlayerCtrl.Scene.MaxHp += (type * _info.ItemStat[i, 1]);
                    }
                    else if ((_info.ItemStat[i, 0] + 1) == ((int)DefEquipType.Drain))//해당 스텟이 흡혈이라면
                    {
                        Characters.PlayerCtrl.Scene.Drain += (type * _info.ItemStat[i, 1]);
                    }
                }


            }


        }

        ////////////////////////////////여기서 정보를 업데이트
        csItemManager.Scene.UpdateData();

        StartCoroutine(GameManager.GManager.SetPlayerData());
        /////////////////////////////////

    }


    #endregion



    #region 아이템 클릭 장착과 판매

    //아이템 사용
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !isMarket)//마켓이 아닐때)//우클릭이라면
        {
            if (item != null)//아이템이 있다면
            {
                if (item.itemType == ItemType.Equipment && equipType == EquipType.Non)//장비창이라면
                {
                    UseEquip();
                }
                else if (item.itemType == ItemType.Used)//사용이라면
                {
                    UsePortion();
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Left)//좌클릭이라면
        {
            if (item != null && equipType == EquipType.Non)//인벤토리창 일때만
            {
                canSell = !canSell;//팔 수 있다.
                clicked.gameObject.SetActive(canSell);//하이라이트 켜준다
            }
        }
    }


    public void UsePortion()
    {
        //전투앱에서만 작동
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "05.scTown")
        {
            //물약 사용
            portionUse.UseItem(slot_info.Index); //아이템을 사용해준다
            Debug.Log(slot_info.ItemName + "을 사용했습니다");
            SetSlotCount(-1);//슬롯 하나 감소
        }
    }

    public void UseEquip()
    {
        Debug.Log(slot_info.ItemName + "를 장착했습니다");
        equipUse.UseItem(this.GetComponent<Slot>());
    }


    public void EquipSlot()//itemController에서 호출.장비가 장착됨
    {

        if (equipType != EquipType.Non)//만약 나의 equip타입이 있다면(장착창일때는)
        {
            if (equipType.ToString() == DragSlot.instance.d_equipType.ToString())//서로의 장비타입이 일치할때
            {

                // 전에 무기가 있다면 그만큼 스탯 감소 시키기
                SetPlayerData(-1, slot_info);//여기서 능력치의 변화를 주자
                ChangeSlot();
                SetPlayerData(1, slot_info);//여기서 능력치의 변화를 주자

            }
        }

    }
    #endregion



    #region 데이터 베이스

    void SendData()//보내자
    {
        if(!isMarket)
        {
            char[] removeChar = { 'S', 'l', 'o', 't', ' ', '(', ')' };
            int slotNum = int.Parse(string.Join("", transform.name.Split(removeChar)));//숫자만 남긴다

            SendType sendType = (equipType != EquipType.Non) ? SendType.Equip : SendType.Inven;
            GameManager.GManager.StartSaving(sendType, slotNum, slot_info, itemCount);//장비창에 저장을 시작하자
        }
    }

    void ClearData(EquipType clearType, int clearIndex)//지워주자
    {

        char[] removeChar = { 'S', 'l', 'o', 't', ' ', '(', ')' };
        int slotNum = int.Parse(string.Join("", transform.name.Split(removeChar)));//숫자만 남긴다
        Debug.LogWarning(clearIndex);
        Debug.LogWarning(clearType);

        SendType sendType = (clearType != EquipType.Non) ? SendType.Equip : SendType.Inven;

        GameManager.GManager.StartErase(sendType, slotNum, clearIndex);//장비창에 저장을 시작하자

    }
    #endregion



    #region 정보창 ToolTip
    public void OnPointerEnter(PointerEventData eventData)//IPOinterEnterHandler 인터페이스 상속시 사용 가능 마우스 들어오면 발생
    {
        if (item != null)
        {
            equipUse.ShowToolTip(item, slot_info, transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        equipUse.HideToolTip();
    }
    #endregion

    #region 판매관련
    //꺼지면 다시 판매 불가로 변경
    private void OnDisable()
    {
        canSell = false;
        clicked.gameObject.SetActive(canSell);//하이라이트 켜준다
    }
    public void Sell()//판매한다
    {
        if (canSell)//파는게 체크 되어 있다면
        {
            int sellorBuy = (isMarket) ? -1 : 1;//상점에서 파는거라면 빼주는거다.
            if (isMarket)//마켓일때
            {
                if (Characters.PlayerCtrl.Scene.Gold < slot_info.ItemGold)//만약 사려는 물건보다 골드가 없다면,
                {
                    Debug.Log("골드부족");
                    return;
                }
            }


            if (slotType == ItemType.Equipment)
            {

                Characters.PlayerCtrl.Scene.Gold += slot_info.ItemGold * sellorBuy;//골드를 더해준다,마켓이라면 빼준다

                //인벤토리창 업데이트
            }
            else
            {
                Characters.PlayerCtrl.Scene.Gold += slot_info.ItemGold * itemCount * sellorBuy;//개수를 곱하여 골드를 더해준다,마켓이라면 빼준다
            }

            if (!isMarket)//마켓이 아니라면 슬롯을 비워준다.
            {
                ClearSlot();
            }
            else//마켓이라면 인벤토리에 추가
            {
                slot_info.ItemGold = (int)(slot_info.ItemGold * 0.25f);
                csItemManager.Scene.inven.AcquireItem(item, slot_info);
                slot_info.ItemGold = (int)(slot_info.ItemGold * 4.0f);
            }
            SoundManager.Sound.EffectPlay(2);
            canSell = false;
            clicked.gameObject.SetActive(canSell);//하이라이트 꺼준다
            StartCoroutine(GameManager.GManager.SetPlayerData());
        }

    }
    #endregion

}