using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterManager : MonoBehaviour
{
    // 생존유무
    private bool life;
    public bool Life { get { return life; } set { life = value; } }
    // 타격유무
    private bool attacked;    
    public bool Attacked { get { return attacked; } set { attacked = value; } }
    // 피격유무
    private bool damaged;
    public bool Damaged { get { return damaged; } set { damaged = value; } }
    // 레벨
    private int level;
    public int Level { get { return level; } set { level = value; } }
    // 경험치(플레이어 : 현재 경험치, 몬스터 : 처치시 획득 경험치)
    [SerializeField]
    private int exp;
    public int Exp { get { return exp; } set { exp = value; } }
    // 골드(플레이어 : 현재 골드, 몬스터 : 처치시 획득 골드)
    [SerializeField]
    private int gold;
    public int Gold { get { return gold; } set { gold = value; } }
    // 아이템 확률(플레이어 : 아이템 획득률, 몬스터 : 아이템 드랍률)
    private float item;
    public float Item { get { return item; } set { item = value; } }
    // 체력
    private float hp;
    public float Hp { get { return hp; } set { hp = value; } }
    // 최대 체력
    [SerializeField]
    private float maxHp;
    public float MaxHp { get { return maxHp; } set { maxHp = value; } }
    // 공격력
    [SerializeField]
    private float atk;
    public float Atk { get { return atk; } set { atk = value; } }
    // 방어력
    [SerializeField]
    private float def;
    public float Def { get { return def; } set { def = value; } }
    // 공격 속도
    [SerializeField]
    private float atkSpeed;
    public float AtkSpeed { get { return atkSpeed; } set { atkSpeed = value; } }
    // 이동 속도
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    // 이동 속도
    [SerializeField]
    private float critical;
    public float Critical { get { return critical; } set { critical = value; } }
    // 이동 속도
    [SerializeField]
    private float criticalMax;
    public float CriticalMax { get { return criticalMax; } set { criticalMax = value; } }
    // 이동 속도
    [SerializeField]
    private float drain;
    public float Drain { get { return drain; } set { drain = value; } }
    // 현재 위치
    private Vector3 position;
    public Vector3 Position { get { return position; } set { position = value; } }
    // 이동방향
    private Vector3 force;
    public Vector3 Force { get { return force; } set { force = value; } }    

    // 초기화 함수
    public virtual void Reset()
    {
        life = true;
        damaged = false;
        attacked = false;
    }
}
