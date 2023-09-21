using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingPoint : MonoBehaviour
{
    private bool Once=false;
  
 
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player"&&Once==false)
        {
           
            Vector3 pos = transform.position;
            pos.x -= 2;
            pos.y += 3;
            pos.z -= 5;
            GameObject.Find("Stage").transform.Find("playerSpawn"+ Characters.PlayerCtrl.Scene.playerSector.ToString()).position= pos;//진행도에 따라 다른 세이빙 포인트
            Once = true;
        }        
    }
}
