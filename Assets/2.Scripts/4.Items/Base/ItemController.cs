using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemController : MonoBehaviour
{
    private bool pickUpActivated = false;//아이템 습득 가능 상태
    private GameObject hitInfo;//충돌체의 정보
    public Inventory myInven;//장비 인벤토리
    public Transform pickUpText;//습득여부를 알려주는 UI
    // Start is called before the first frame update

    private void Awake()
    {
        //더 이쁘게 할 방법 찾아보자 ..
        myInven = transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("04.Inventory").transform.GetComponent<Inventory>();
        Debug.Assert(myInven);
        pickUpText = transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("PickUpText");
        Debug.Assert(pickUpText);
    }
   

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)&&pickUpActivated)//E를 누르고 아이템과 충돌했을때
        { 
           //인벤토리에 아이템 추가해주고
           myInven.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item,hitInfo.transform.GetComponent<ItemPickUp>().info);
            //픽업 UI를 꺼준다
            pickUpText.gameObject.SetActive(false);
            //먹을 수 없는 상태로 만들어주고
            pickUpActivated = false;
            //아이템을 삭제 해준다.
            Destroy(hitInfo);
        }
        
    }
    
    //영역에 들어왔을때
    private void OnTriggerStay(Collider other)
    {
        //아이템과 부딪힌다면
        if(other.tag=="Item")
        {
            //픽업 UI를 켜준다
            pickUpText.gameObject.SetActive(true);
            //먹을 수 있는 상태로 만들어주고
            pickUpActivated = true;
            //충돌체의 정보를 전달
            hitInfo= other.gameObject;
            //먹을래요 ? 창 활성화
        }
        
    }

    //영역에서 나갔을때
    private void OnTriggerExit(Collider other)
    {
        //픽업 UI를 꺼준다
        pickUpText.gameObject.SetActive(false);
        //먹을 수 없는 상태로 만들어주고
        pickUpActivated = false;
        //충돌체의 정보를 전달
        hitInfo = null;
        //먹을래요 ? 창 활성화
    }


}
