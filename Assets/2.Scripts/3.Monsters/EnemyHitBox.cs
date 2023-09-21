using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public bool hitBoxCheck = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (!hitBoxCheck && !Characters.PlayerCtrl.Scene.avoidSkill)
            {
                Instantiate(Resources.Load("Skill/BossAttack"), other.transform.position, Quaternion.identity);
                hitBoxCheck = true;
            }

        }
    }



}
