using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyManager : CharacterManager
{
    protected enum TYPE
    {
        Slime = 0,
        Bee,
        Boss1,
        Turtle,
        Dog,
        Boss2,
        Rabbit,
        Bat,
        Boss3
    }
    // 몬스터 타입
    private int type;
    // 보스 유무
    [SerializeField] public bool isBoss;
    public int Type { get { return type; } set { type = value; } }
    public override void Reset()
    {
        base.Reset(); // 기본 값 초기화
        SetStatus(Type);
        //MonsterTypeSet(); // 몬스터 타입에 따라 스탯 지정
    }
    protected void Attack()
    {
        if (!Characters.PlayerCtrl.Scene.avoidSkill)
        {
            float damage;
            if (!Characters.PlayerCtrl.Scene.holdSkill)
            {
                float a = (int) Random.Range(Atk, (Atk + 10)) - (Characters.PlayerCtrl.Scene.Def / 4);
                damage = Mathf.Clamp(a, 0, a);

            }
            else
            {
                //부동중 일경우
                float a = (int)Random.Range(Atk, (Atk + 10)) - (Characters.PlayerCtrl.Scene.Def / 4);
                damage = Mathf.Clamp(a, 0, a) * 0.3f;
            }

            int rnd = Random.Range(0, 100);
            //히트 애니매이션
            //Debug.Log(damage / Characters.PlayerCtrl.Scene.MaxHp);


            //히트다운 애니매이션




            // 타격 시
            if (Characters.PlayerCtrl.Scene.Hp > 0) // 플레이어 살아있으면 데미지
                Characters.PlayerCtrl.Scene.Hp -= (int)damage; // 몬스터 공격력 : atk~atk+10            

            if (Characters.PlayerCtrl.Scene.Hp > 0) // 데미지 받고 살아있으면 체력 변동
            {
                //Debug.Log("Player 체력 : " + Characters.PlayerCtrl.Scene.Hp + "/" + Characters.PlayerCtrl.Scene.MaxHp);
                if (!Characters.PlayerCtrl.Scene.holdSkill && !Characters.PlayerCtrl.Scene.avoidSkill)
                {
                    rnd = Random.Range(0, 100);
                    if (rnd < 19)
                    {
                        StartCoroutine(Characters.PlayerCtrl.Scene.HitDownAnimation(transform.forward));
                    }
                    else
                    {
                        StartCoroutine(Characters.PlayerCtrl.Scene.HitAnimation());
                    }


                }
            }
            else // 죽었으면 체력 0, 사망 처리
            {
                Characters.PlayerCtrl.Scene.Hp = 0;
                Characters.PlayerCtrl.Scene.Life = false;
                Debug.Log("Player 사망");
                Characters.PlayerCtrl.Scene.DieAnimation();
                StartCoroutine(StageManager.Scene.Result());
            }

            StageManager.Scene.HpBar.fillAmount = Characters.PlayerCtrl.Scene.Hp / Characters.PlayerCtrl.Scene.MaxHp;
            Characters.PlayerCtrl.Scene.Damaged = false; // 플레이어 상태변화
        }

    }
    public virtual void Hit(float d)
    {
        if (!Life) return; // 죽어있으면 실행 안함
        float damage = (Mathf.Log(Characters.PlayerCtrl.Scene.Atk - Def) + 7) * (1 - d);
        if (Characters.PlayerCtrl.Scene.weaponNum == 2) damage *= 0.4f;
        else if (Characters.PlayerCtrl.Scene.weaponNum == 3) damage *= 0.8f;
        else if (Characters.PlayerCtrl.Scene.weaponNum == 4) damage *= 2.0f;
        if (Random.Range(0, 100) <= Characters.PlayerCtrl.Scene.Critical / 10)
        {
            Debug.LogWarning("크리티컬!");
            damage += damage * (Characters.PlayerCtrl.Scene.CriticalMax / 100 + 1);
        }

        if (Random.Range(0, 100) <= Characters.PlayerCtrl.Scene.Drain / 10)
        {
            Debug.LogWarning("드레인!");
            Characters.PlayerCtrl.Scene.Hp += damage;
            if (Characters.PlayerCtrl.Scene.Hp > Characters.PlayerCtrl.Scene.MaxHp)
                Characters.PlayerCtrl.Scene.Hp = Characters.PlayerCtrl.Scene.MaxHp;
            StageManager.Scene.HpBar.fillAmount = Characters.PlayerCtrl.Scene.Hp / Characters.PlayerCtrl.Scene.MaxHp;
        }

        if (Characters.PlayerCtrl.Scene.weaponType != Characters.PlayerCtrl.WeaponType.Non)
        {
            if (Characters.PlayerCtrl.Scene.weaponNum == 1) // 권총을 들었으면
            {
                Debug.LogWarning("권총 추가 기능!");
                Characters.PlayerCtrl.Scene.Hp += (Characters.PlayerCtrl.Scene.MaxHp * 0.05f); // 5% 체력 회복
                if (Characters.PlayerCtrl.Scene.Hp > Characters.PlayerCtrl.Scene.MaxHp)
                    Characters.PlayerCtrl.Scene.Hp = Characters.PlayerCtrl.Scene.MaxHp;
                StageManager.Scene.HpBar.fillAmount = Characters.PlayerCtrl.Scene.Hp / Characters.PlayerCtrl.Scene.MaxHp;
            }
        }
        Hp -= damage;
        Characters.PlayerCtrl.Scene.Attacked = false; // 플레이어 상태변화
        Damaged = false; // 몬스터 상태변화
        if (Hp <= 0) Die(); // 몬스터 피없으면 제거
    }
    protected virtual void Die()
    {
        Life = false;
        Characters.PlayerCtrl.Scene.Exp += Exp;
        while (Characters.PlayerCtrl.Scene.Exp > Characters.PlayerCtrl.Scene.Level * 150)
        {
            Characters.PlayerCtrl.Scene.Exp -= Characters.PlayerCtrl.Scene.Level * 150;
            Characters.PlayerCtrl.Scene.Level++;

            GameObject lv=Instantiate(Resources.Load("Skill/LevelUp"), Characters.PlayerCtrl.Scene.transform.position + new Vector3(0,0.05f,0), Quaternion.Euler(-90,0,0))as GameObject;
            lv.transform.parent = Characters.PlayerCtrl.Scene.transform;
            // 스탯 포인트 추가
            // 스킬 포인트 추가
        }
        Characters.PlayerCtrl.Scene.Gold += Gold;
        csItemManager.Scene.LogText.text += "\n경험치를 획득하였습니다.(+" + Exp + "exp)";
        csItemManager.Scene.LogText.text += "\n골드를 획득하였습니다.(+" + Gold + "gold)";
        // 랜덤 드랍(장비, 소비, 기타)
        StartCoroutine(csItemManager.Scene.GetItem(Level, type, transform.position));
        
        //StageManager.Scene.LogText.text += "\n아이템을 획득하였습니다.(HP물약 x2)";

        StartCoroutine(GameManager.GManager.SetPlayerData());
        StageManager.Scene.MissonText.text = "모든 적 처치(" + (GameObject.FindGameObjectsWithTag("Enemy").Length - 1) + "/" + StageManager.Scene.monsterMax + ")";
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 1 && GameManager.GManager.StageNum != 2)
        {
            GameManager.GManager.StageNum++;
            StartCoroutine(StageManager.Scene.Result());
        }
        Invoke("Del", (isBoss) ? 3.0f : 1.2f);
    }
    void Del()
    {
        Destroy(gameObject);
    }
    protected void MonsterType()
    {
        // 게임오브젝트안에 해당 문자열의 오브젝트(프리팹)가 있으면 타입 정해짐
        if (gameObject.transform.Find("Slime") != null)
        {
            Type = (int)TYPE.Slime;
        }
        else if (gameObject.transform.Find("Turtle") != null)
        {
            Type = (int)TYPE.Turtle;
        }
        else if (gameObject.transform.Find("Bee") != null)
        {
            Type = (int)TYPE.Bee;
        }
        else if (gameObject.transform.Find("Dog") != null)
        {
            Type = (int)TYPE.Dog;
        }
        else if (gameObject.transform.Find("Rabbit") != null)
        {
            Type = (int)TYPE.Rabbit;
        }
        else if (gameObject.transform.Find("Bat") != null)
        {
            Type = (int)TYPE.Bat;
        }
        else if (gameObject.transform.Find("Boss1") != null)
        {
            Type = (int)TYPE.Boss1;
        }
        else if (gameObject.transform.Find("Boss2") != null)
        {
            Type = (int)TYPE.Boss2;
        }
        else if (gameObject.transform.Find("Boss3") != null)
        {
            Type = (int)TYPE.Boss3;
        }
    }
    protected void SetStatus(int type)
    {
        int level = int.Parse(GameManager.GManager.monsterData[1, type]);
        float hp = float.Parse(GameManager.GManager.monsterData[2, type]);
        float atk = float.Parse(GameManager.GManager.monsterData[3, type]);
        float def = float.Parse(GameManager.GManager.monsterData[4, type]);
        float speed = float.Parse(GameManager.GManager.monsterData[5, type]);
        // 최소 수치 적용
        if (level < 1) level = 1;
        if (hp < 20.0f) hp = 20.0f;
        if (atk < 10.0f) atk = 10.0f;
        if (def < 5.0f) def = 5.0f;
        if (speed < 1.0f) speed = 1.0f;

        Level = level; // 몬스터 레벨
        Exp = level * 10; // 처치시 획득 경험치
        Gold = Random.Range(((level - 1) * 10 + 1), (level * 10)); // 처치시 획득 골드 : 1~10(level 1기준)
        Item = ((float)level / 10.0f) * 10; // 레벨 10까진 아이템 드랍 X, 10이상부터 드랍율 적용
        MaxHp = Hp = hp; // 최대 체력
        Atk = atk;
        Def = (int)Random.Range(0, def); // 몬스터 방어력 : 0~def
        AtkSpeed = speed; // 공격스피드는 일단 고정
        MoveSpeed = speed; // 이동스피드는 일단 고정
    }
}