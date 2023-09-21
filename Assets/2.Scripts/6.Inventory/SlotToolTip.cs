using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour
{
    [SerializeField]
    private GameObject tip_Base;//껐다 켰다 할 UI의 Base

    [SerializeField]
    private Image itemImage;      //아이템의 이미지//
    [SerializeField]
    private Text itemName;      //아이템의 이름//
    [SerializeField]
    private Text itemType;      //아이템의 타입//
    [SerializeField]
    private Text itemGrade;     //등급

    [SerializeField]
    private Text[] TipItemStat;     //스텟들
  
    [SerializeField]
    private Text itemExplain;   //아이템의 설명//
    [SerializeField]
    private Text itemMoney;     //금액//



    public void ShowToolTip(Item _item,ItemInfo _info,Vector3 _pos)
    {
        tip_Base.SetActive(true);//창을 켜준다
        //_pos += new Vector3(tip_Base.GetComponent<RectTransform>().rect.width * 0.5f,   //위치를 전달받은 슬롯의 중앙으로 옮겨준다
        //    - tip_Base.GetComponent<RectTransform>().rect.height * 0.5f, 0);  
        
        tip_Base.transform.position = _pos;         //설정한 위치로 변경
        itemImage.sprite = _item.itemImage;             //이미지
        itemName.text = _info.ItemName;                 //이름
        itemExplain.text = _info.ItemExp;               //설명
        itemMoney.text = _info.ItemGold.ToString();     //골드

        if (_item.itemType == ItemType.Equipment)//장비라면
        {
            itemType.text ="장비";      //타입

            //등급의 결정

            if (_info.Index % 4 == 0)        //일반등급은 흰색  
            {
                itemGrade.text = "일반";
                itemGrade.color = itemName.color = Color.white;
                GradeSet(_info);
                      
            }
            else if (_info.Index % 4 == 1)        //레어등급은 노란색  
            {
                itemGrade.text = "레어";
                itemGrade.color = itemName.color = Color.yellow;
                GradeSet( _info);

            }
            else if (_info.Index % 4 == 2)        //에픽등급은 보라색  
            {
                itemGrade.text = "에픽";
                Color col;
                ColorUtility.TryParseHtmlString("#8b00ff",out col);//hex 코드를 RGB값으로 쓸때
                itemGrade.color = itemName.color = col;
                GradeSet(_info);

            }
            else if (_info.Index % 4 == 3)        //에픽등급은 연두색  
            {
                itemGrade.text = "전설";
                Color col;
                ColorUtility.TryParseHtmlString("#80FF00", out col);
                itemGrade.color = itemName.color = col;
                GradeSet( _info);

            }

        }


        else if(_item.itemType == ItemType.Used)//물약이라면
        {
            itemType.text = "물약";      //타입
        }


        else if (_item.itemType == ItemType.Etc)//재료라면
        {
            itemType.text = "재료";      //타입
        }



      

        





    }

    public void HideToolTip()
    {
        itemGrade.color = itemType.color = Color.white;

        itemImage.sprite = null;
        itemName.text = null;
        itemType.text = null;
        
        itemGrade.text = null;

        TipItemStat[0].text = null;
        TipItemStat[1].text =null;
        TipItemStat[2].text = null;
        TipItemStat[3].text = null;

        itemExplain.text =null;
        itemMoney.text = null;
        tip_Base.SetActive(false);
    }

    private void GradeSet(ItemInfo _info)
    {

        if(_info.Index<24)//공격장비라면
        {
           
            for (int i = 0; i < 4; i++)//등급에 따른 속성부여
            {
                if (_info.Index % 4 >= i)
                {
                    if ((_info.ItemStat[i, 0]+1 ) == ((int)AtkEquipType.AtkDamage))//해당 스텟이 공격이라면
                    {
                        TipItemStat[i].text = "ATK                  " + _info.ItemStat[i, 1].ToString();
                    }
                    else if ((_info.ItemStat[i, 0]+1 ) == ((int)AtkEquipType.AtkSpeed))//해당 스텟이 공격속도라면
                    {
                        TipItemStat[i].text = "ATK SPEED         " + _info.ItemStat[i, 1].ToString();
                    }
                    else if ((_info.ItemStat[i, 0]+1) == ((int)AtkEquipType.Critical))//해당 스텟이 크리티컬라면
                    {
                        TipItemStat[i].text = "CRITICAL           " + _info.ItemStat[i, 1].ToString();
                    }
                    else if ((_info.ItemStat[i, 0]+1 ) == ((int)AtkEquipType.CriticalMax))//해당 스텟이 크리티컬맥스라면
                    {
                        TipItemStat[i].text = "CRITICAL MAX      " + _info.ItemStat[i, 1].ToString();
                    }

                }

               
            }
        }
        else
        {
           

            for (int i =0; i < 4; i++)//등급에 따른 속성부여
            {
               
                if (_info.Index % 4 >= i)
                {
                    if ((_info.ItemStat[i, 0]+1 ) == ((int)DefEquipType.Def))//해당 스텟이 방어라면
                    {
                        TipItemStat[i].text = "Def                  " + _info.ItemStat[i, 1].ToString();
                    }
                    else if ((_info.ItemStat[i, 0]+1 ) == ((int)DefEquipType.MoveSpeed))//해당 스텟이 이동속도라면
                    {
                        TipItemStat[i].text = "MOVE SPEED       " + _info.ItemStat[i, 1].ToString();
                    }
                    else if ((_info.ItemStat[i, 0] +1) == ((int)DefEquipType.MaxHp))//해당 스텟이 최대체력이라면
                    {
                        TipItemStat[i].text = "MAX HP             " + _info.ItemStat[i, 1].ToString();
                    }
                    else if ((_info.ItemStat[i, 0] +1) == ((int)DefEquipType.Drain))//해당 스텟이 흡혈이라면
                    {
                        TipItemStat[i].text = "DRAIN               " + _info.ItemStat[i, 1].ToString();
                    }
                }


            }


        }

    }

   
}
