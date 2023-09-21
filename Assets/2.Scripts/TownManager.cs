using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("현재 방 인원 수 : " + PhotonNetwork.countOfPlayersInRooms);
        //Instantiate(Resources.Load("Player"), transform.position, Quaternion.identity);        
        if(GameManager.GManager.PhotonPlayer == null)
        {
            GameManager.GManager.PhotonPlayer = PhotonNetwork.Instantiate("Player", new Vector3(0, 3.0f, 0), Quaternion.identity, 0);
        }
    }

    private void Start()
    {
        if (GameManager.GManager.PhotonPlayer != null)
        {
            //Characters.PlayerCtrl.Scene.Awake();
            GameManager.GManager.PhotonPlayer.transform.position = new Vector3(0, 3.0f, 0);
            // 총알 초기화
            for (int i = 0; i < 4; i++)
            {
                csItemManager.Scene.ItemBase[i].bullet = csItemManager.Scene.ItemBase[i].bulletMax;
                csItemManager.Scene.ItemBase[i].bulletTotal = csItemManager.Scene.ItemBase[i].bulletMax * 3;
            }
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
