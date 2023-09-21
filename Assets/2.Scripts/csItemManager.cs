using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System;//렌덤이 겹쳐서 일단 ItemPickUp으로 옮김

public enum SendType
{
    Inven,
    Equip
}
public enum ItemType
{
    Equipment,
    Used,
    Etc
}
public enum EquipType
{
    Non,
    Weapon,
    Armor,
    Helmet,
    Shoes,
    Portion

}
public enum AtkEquipType
{
    Non,
    AtkDamage, // 무기
    AtkSpeed,//헬멧
    Critical,
    CriticalMax
}
public enum DefEquipType
{
    Non,
    Def,//갑옷
    MoveSpeed,//신발
    MaxHp,
    Drain
}

public class csItemManager : csSceneManager<csItemManager>
{
    public Transform my;
    public Inventory inven;
    // 나의 능력치 정보
    public Text myHp;
    public Text myDef;
    public Text myAtk;
    public Text myAtkSpeed;
    public Text myMoveSpeed;
    public Text myCritical;
    public Text myCriticalMax;
    public Text myDrain;
    public Text myName;
    [System.NonSerialized]
    public Text myLevel;
    private Text myGold;
    public Shop myShop; // 장비 상점
    public PortionShop portionShop;//물약상점
    

    public Transform Battle_Item;
    public Image ImgSlot1, ImgSlot2, ImgSlot3, ImgSlot4;
    public Color colorSlotBG, colorSlotImage, colorSlotText, ChatColor;
    public Text BulletText;
    public Text LogText;
    public bool chatCheck;
    public InputField ChatText;
    public Scrollbar LogScroll;

    public Image expBar;//경험치

    public Item[] ItemBase = new Item[4];
    private void Awake()
    {
        inven = transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("04.Inventory").GetComponent<Inventory>();
        my = transform.Find("PlayerUI").transform.Find("InventoryUI");

        //능력치 연결
        myHp=my.transform.Find("02.Status").transform.Find("HP").GetComponent<Text>();
        myDef= my.transform.Find("02.Status").transform.Find("Def").GetComponent<Text>();
        myAtk=my.transform.Find("02.Status").transform.Find("Atk").GetComponent<Text>();
        myAtkSpeed=my.transform.Find("02.Status").transform.Find("AtkSpeed").GetComponent<Text>();
        myMoveSpeed=my.transform.Find("02.Status").transform.Find("MoveSpeed").GetComponent<Text>();
        myCritical=my.transform.Find("02.Status").transform.Find("Critical").GetComponent<Text>();
        myCriticalMax=my.transform.Find("02.Status").transform.Find("CriticalMax").GetComponent<Text>();
        myDrain=my.transform.Find("02.Status").transform.Find("Drain").GetComponent<Text>();
        expBar= my.transform.Find("02.Status").transform.Find("ExpBase").transform.Find("Exp").GetComponent<Image>();
        //이름,레벨,골드 연결
        myName =my.transform.Find("01.Name_Level").transform.Find("Name").GetComponent<Text>();
        myLevel=my.transform.Find("01.Name_Level").transform.Find("Level").GetComponent<Text>();
        myGold=inven.transform.Find("Gold").GetComponent<Text>();
        // 퀵슬롯, 총알, 채팅
        Battle_Item = transform.Find("PlayerUI").Find("ItemBox");
        ImgSlot1 = Battle_Item.Find("Slot1").GetComponent<Image>();
        ImgSlot2 = Battle_Item.Find("Slot2").GetComponent<Image>();
        ImgSlot3 = Battle_Item.Find("Slot3").GetComponent<Image>();
        ImgSlot4 = Battle_Item.Find("Slot4").GetComponent<Image>();
        colorSlotBG = Battle_Item.Find("Slot1").GetComponent<Image>().color;
        colorSlotImage = Battle_Item.Find("Slot1").GetComponent<Image>().transform.Find("Slot").GetComponent<Image>().color;
        colorSlotText = Battle_Item.Find("Slot1").GetComponent<Image>().transform.Find("Slot").Find("Text").GetComponent<Text>().color;
        ChatColor = transform.Find("PlayerUI").transform.Find("ChatBox").Find("MessagePanel").GetComponent<Image>().color;
        BulletText = transform.Find("PlayerUI").transform.Find("BulletTxt").GetComponent<Text>();
        LogText = transform.Find("PlayerUI").transform.Find("ChatBox").Find("MessagePanel").GetComponentInChildren<Text>();
        LogScroll = transform.Find("PlayerUI").transform.Find("ChatBox").Find("MessagePanel").GetComponentInChildren<Scrollbar>();
        ChatText = transform.Find("PlayerUI").transform.Find("ChatBox").GetComponentInChildren<InputField>();
    }
    void Start()
    {
        UpdateData();//정보를 초기화 해준다
        inven.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateData()//플레이어로부터 정보를 받아 업데이트
    {
        myHp.text= Characters.PlayerCtrl.Scene.MaxHp.ToString();
        myDef.text=Characters.PlayerCtrl.Scene.Def.ToString();
        myAtk.text= Characters.PlayerCtrl.Scene.Atk.ToString();
        myAtkSpeed.text =Characters.PlayerCtrl.Scene.AtkSpeed.ToString();
        myMoveSpeed.text = Characters.PlayerCtrl.Scene.MoveSpeed.ToString();
        myCritical.text = Characters.PlayerCtrl.Scene.Critical.ToString();
        myCriticalMax.text = Characters.PlayerCtrl.Scene.CriticalMax.ToString();
        myDrain.text = Characters.PlayerCtrl.Scene.Drain.ToString();
        myName.text = Characters.PlayerCtrl.Scene.playerName.ToString();
        myLevel.text = "Level"+Characters.PlayerCtrl.Scene.Level.ToString();
        myGold.text = Characters.PlayerCtrl.Scene.Gold.ToString();
        expBar.fillAmount = Characters.PlayerCtrl.Scene.Exp / (Characters.PlayerCtrl.Scene.Level * 150.0f);
    }

    public IEnumerator GetItem(int level, int type, Vector3 pos)
    {
       
        int randomEquItem = Random.Range(1, 1000);
        int randomUseItem = Random.Range(1, 1000);
        int randomEtcItem = Random.Range(1, 1000);

        int equipPercent = (GameManager.GManager.TestMode) ? 2 : 10; // 50% or 10%
        int usePercent = (GameManager.GManager.TestMode) ? 4 : 5; // 25% or 20%
        int etcPercent = 4; // 25%

        pos.y += 0.5f;

        // 장비 아이템 확률
        if (randomEquItem % equipPercent == 0) // 10%
        {
            int equIndex = Random.Range(1, 1000) * ((GameManager.GManager.TestMode) ? 2 : 1);
            // 몬스터 레벨이 높을 수록 확률이 증가
            if (equIndex < (870 - level))
            {
                equIndex = equIndex % 10 * 4; //0일때
                // 일반 아이템
          
            }
            else if (equIndex < (985 - level))
            {
                equIndex = equIndex % 10 * 4 + 1;
                // 레어 아이템
              
            }
            else if (equIndex < (995 - level))
            {
                equIndex = equIndex % 10 * 4 + 2;
                // 에픽 아이템
                
            }
            else
            {
                equIndex = equIndex % 10 * 4 + 3;
                // 전설 아이템
                
            }

            LogText.text += "\n아이템을 획득하였습니다.(" + GameManager.GManager.itemData[0, equIndex] + ")";
            // 해당 프리팹 추가(일단 임의로 추가)
            GameObject addItem = (GameObject)Instantiate(Resources.Load("Item/Equipment/Item" + ((equIndex / 4) * 4).ToString()), pos, Quaternion.identity);//인덱스에 맞는 기본 아이템 프리팹 형성
            //아이템에 정보를 담아준다
            addItem.GetComponent<ItemPickUp>().SetItem(GameManager.GManager.itemData[0, equIndex], int.Parse(GameManager.GManager.itemData[1, equIndex]), int.Parse(GameManager.GManager.itemData[2, equIndex]), int.Parse(GameManager.GManager.itemData[3, equIndex]), int.Parse(GameManager.GManager.itemData[4, equIndex]));
            // 이펙트 추가
            if (equIndex % 4 > 1) //기본 아이템은 추가 x
            {
                GameObject particle = (GameObject)Instantiate(Resources.Load("Item/Particle/Ptc" + (equIndex % 4).ToString()), pos, Quaternion.identity);
                particle.transform.parent = addItem.transform;//부모로 만들어준다.
            }
        }

        // 소비 아이템 확률
        if (randomUseItem % usePercent == 0) // 20%
        {
            int useindex = Random.Range(40, 44);
            int useCount = Random.Range(1, int.Parse(GameManager.GManager.itemData[3, useindex]) + 1);

            LogText.text += "\n아이템을 획득하였습니다.(" + GameManager.GManager.itemData[0, useindex] + "x" + useCount + ")";
            GameObject addItem=null;

            //string itemNasme = "UseItem"+((useindex<42)?"Hp":"Mp")+ GameManager.GManager.itemData[2, useindex];//수정..
            string itemName = "Item" + (useindex % 4 + 1).ToString();

            addItem = (GameObject)Instantiate(Resources.Load("Item/Used/"+itemName), pos, Quaternion.identity);
            addItem.GetComponent<ItemPickUp>().SetItem(GameManager.GManager.itemData[0, useindex], int.Parse(GameManager.GManager.itemData[1, useindex]), int.Parse(GameManager.GManager.itemData[2, useindex]), useCount, int.Parse(GameManager.GManager.itemData[4, useindex]));
        }

        // 기타 아이템 확률
        if (randomEtcItem % etcPercent == 0) // 25%
        {
            int etcindex = (type % 3 == 0) ? 0 : 1;
            switch (GameManager.GManager.nextScene-1)
            {
                case 1: etcindex += 48; break; // 48, 49
                case 2: etcindex += 51; break; // 51, 52
                case 3: etcindex += 54; break; // 54, 55
            }
            int etcCount = Random.Range(1, int.Parse(GameManager.GManager.itemData[3, etcindex]) + 1);
            LogText.text += "\n아이템을 획득하였습니다.(" + GameManager.GManager.itemData[0, etcindex] + "x" + etcCount + ")";
            GameObject addItem = (GameObject)Instantiate(Resources.Load("Item/Etc/Item"+(etcindex-47)), pos, Quaternion.identity);
            addItem.GetComponent<ItemPickUp>().SetItem(GameManager.GManager.itemData[0, etcindex], int.Parse(GameManager.GManager.itemData[1, etcindex]), int.Parse(GameManager.GManager.itemData[2, etcindex]), etcCount, int.Parse(GameManager.GManager.itemData[4, etcindex]));
        }
        yield return null;
    }

    public void MakeShopItem()
    {
        for (int i = 0; i < 12; i++)
        {
            int equIndex = Random.Range(1, 1000);
            // 확률적인 아이템 등록
            if (equIndex < 870)
            {
                equIndex = equIndex % 10 * 4; //0일때
                                              // 일반 아이템
            }
            else if (equIndex < 985)
            {
                equIndex = equIndex % 10 * 4 + 1;
                // 레어 아이템

            }
            else if (equIndex < 995)
            {
                equIndex = equIndex % 10 * 4 + 2;
                // 에픽 아이템

            }
            else if (equIndex < 1000)
            {
                equIndex = equIndex % 10 * 4 + 3;
                // 전설 아이템

            }
            //아이템을 리소시스에서 불러온다
            ItemPickUp item = Resources.Load<GameObject>("Item/Equipment/Item" + ((equIndex / 4) * 4).ToString()).GetComponent<ItemPickUp>();            
            string tempString = item.info.ItemExp;
            item.info = new ItemInfo();
            item.info.ItemStat = new int[4, 2];

            item.info.ItemExp = tempString;

            //아이템에 정보를 넣어준다
            item.SetItem(GameManager.GManager.itemData[0, equIndex], int.Parse(GameManager.GManager.itemData[1, equIndex]), int.Parse(GameManager.GManager.itemData[2, equIndex]), int.Parse(GameManager.GManager.itemData[3, equIndex]), 4 * int.Parse(GameManager.GManager.itemData[4, equIndex]));
            //상점에 업데이트
            myShop.AcquireItem(item.item, item.info);
        }
    }
    public void MakePortionItem()
    {
        for (int i = 0; i < 4; i++)
        {
            int portionIndex = 40 + i;
            // 확률적인 아이템 등록

            //아이템을 리소시스에서 불러온다
            ItemPickUp item = Resources.Load<GameObject>("Item/Used/Item" + (portionIndex % 4 + 1).ToString()).GetComponent<ItemPickUp>();
            string tempString = item.info.ItemExp;
            item.info = new ItemInfo();
            item.info.ItemStat = new int[4, 2];

            item.info.ItemExp = tempString;

            //아이템에 정보를 넣어준다
            item.SetItem(GameManager.GManager.itemData[0, portionIndex], int.Parse(GameManager.GManager.itemData[1, portionIndex]), int.Parse(GameManager.GManager.itemData[2, portionIndex]), int.Parse(GameManager.GManager.itemData[3, portionIndex]), 4 * int.Parse(GameManager.GManager.itemData[4, portionIndex]));
            //상점에 업데이portionIndex
            portionShop.AcquireItem(item.item, item.info);
        }
    }


    public void SetItemColor(float val)
    {
        colorSlotBG.a = colorSlotImage.a = colorSlotText.a = val;
        ImgSlot1.color = ImgSlot2.color = colorSlotBG;
        ImgSlot1.transform.Find("Slot").GetComponent<Image>().color =
        ImgSlot2.transform.Find("Slot").GetComponent<Image>().color = colorSlotImage;
        ImgSlot1.transform.Find("Slot").Find("Text").GetComponent<Text>().color =
        ImgSlot2.transform.Find("Slot").Find("Text").GetComponent<Text>().color = colorSlotText;
    }

    public void ScrollbarValue()
    {
        var changeScroll = LogText.rectTransform.position;
        changeScroll.y = 410 - (LogScroll.value * 400);
        LogText.rectTransform.position = changeScroll;
    }
}
