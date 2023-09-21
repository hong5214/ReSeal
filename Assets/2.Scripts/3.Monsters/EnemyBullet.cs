using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public bool bulletHitCheck = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(!bulletHitCheck && !Characters.PlayerCtrl.Scene.avoidSkill)
            {
                Instantiate(Resources.Load("Skill/BossAttack"), transform.position, Quaternion.identity);
                bulletHitCheck = true;
            }
        }
    }

}
