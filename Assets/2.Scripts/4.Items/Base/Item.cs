using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItem",menuName="NewItem/item")]
public class Item : ScriptableObject
{
    public ItemType itemType;//아이템타입
    public EquipType equipType;//나머지는 Non / 물약-장비라면 설정할것
    public Sprite itemImage;//이미지
    public GameObject itemPrefab;//프리팹
    public int bullet = 0;
    public int bulletMax = 0;
    public int bulletTotal = 0;
}

