using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Characters;

namespace Monster
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : EnemyManager
    {
        //레퍼런스 변수
        Animator anim;
        NavMeshAgent nav;
        Rigidbody rigid;
        Transform bar;
        Transform hitbox;

        [SerializeField] private bool isAlwaysJumpMonster;
        [SerializeField] public bool isAlwaysFlyMonster;
        [SerializeField] private int LongAttackProbablity = 0;
        [SerializeField] private int RushAttackProbablity = 0;

        // 플레이어 추적용 변수
        GameObject[] players;
        float[] distances;
        List<string> targets = new List<string>();
        [HideInInspector] public Transform chaseTarget;
        GameObject roaming;
        GameObject bullet;
        int skip;
        bool hitBox;
        bool hitchase;
        [HideInInspector] public Transform hitPlayer;

        //피격 애니매이션용 변수
        float HitAnimationHp;
        float HitAnimationHpCount = 0;



        private float slow;
        private enum State { Roaming1 = 0, Roaming2, Chase, Attack1, Attack2, Attack3, Hit };
        private State state;
        private int prevRomingPoint = -1;

        // 사운드 추가
        public AudioClip[] audioClip;
        public AudioSource audio;

        void Awake()
        {
            Life = true;
            Damaged = false;
            rigid = GetComponent<Rigidbody>();
            nav = GetComponent<NavMeshAgent>();
            bar = transform.GetChild(0).Find("Hp").Find("Bar");
            anim = GetComponentInChildren<Animator>();
            hitbox = transform.GetChild(0).Find("HitBox");
            roaming = GameObject.Find("roamingPoint");
            audio = transform.GetComponent<AudioSource>();
            audio.volume = SoundManager.Sound.efAudio.volume;
            if (transform.GetChild(0).Find("Bullet") != null)
            {
                bullet = transform.GetChild(0).Find("Bullet").gameObject;
            }
            MonsterType(); // 몬스터 타입 지정
            Reset(); // 몬스터 타입에 맞게 데이터 초기화
            nav.speed = MoveSpeed * 2.2f;
        }
        void Update()
        {
            if (MaxHp != 0)
            {
                bar.localScale = new Vector3(Hp / MaxHp, 1, 1);
            }
            if (transform.position.y < -100.0f)
            {
                transform.position = GameObject.Find("Stage").transform.Find("playerSpawn").position;
                nav.enabled = true;
                nav.Warp(GameObject.Find("Stage").transform.Find("playerSpawn").position);
                StartCoroutine("RoamingRandom");
            }
        }
        void Start()
        {
            StartCoroutine("RoamingRandom");
            StartCoroutine("ChasePatternReset");
            HitAnimationHp = Random.Range(MaxHp * 0.1f, MaxHp * 0.6f);
            if (isAlwaysFlyMonster) { StartCoroutine("Fly"); }
            if (isAlwaysJumpMonster) { StartCoroutine("Jump"); }
        }
        public override void Hit(float d)
        {
            base.Hit(d);

            if (Life)
            {
                HitAnimationHpCount += (Mathf.Sqrt(Characters.PlayerCtrl.Scene.Atk) + 7) * (1 - d);
                if (HitAnimationHpCount >= HitAnimationHp)
                {
                    nav.enabled = false;
                    anim.SetTrigger("Hit");
                    //nav.speed = 0.0f;
                    StopAllCoroutines();
                    Invoke("SpeedRecover", 0.2f);
                    HitAnimationHp = Random.Range(MaxHp * 0.22f, MaxHp * 0.6f);
                    HitAnimationHpCount = 0;
                }

            }
        }

        void SpeedRecover()
        {
            StartCoroutine("Chase");
            /*nav.enabled = true;
            if (nav.isOnNavMesh)
            {
                nav.destination = chaseTarget.position;
                nav.speed = MoveSpeed * Random.Range(3.8f, 6.2f);
            }*/
        }



        protected override void Die()
        {
            base.Die();
            if (!Life)
            {
                anim.SetTrigger("Die");
                if (isBoss)
                {
                    audio.clip = audioClip[1];
                    audio.Play();
                }
                StopAllCoroutines();
            }
            // 네비게이션 종료
            nav.enabled = false;
            // 체력바 삭제
            bar.gameObject.SetActive(false);
        }





        IEnumerator ChasePatternReset()
        {
            while (true)
            {
                if (skip != 0)
                {
                    yield return new WaitForSeconds(Random.Range(3f, 5f));
                    skip = 0;
                }
                if (hitchase)
                {
                    if (Vector3.Distance(chaseTarget.position, transform.position) < nav.stoppingDistance + 15f)
                    {
                        hitchase = false;
                    }
                }

                yield return new WaitForSeconds(5f);
            }
        }

        IEnumerator AlwaysJump()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
                if (nav.enabled == false)
                {
                    anim.SetBool("Run", false);
                    yield return new WaitForSeconds(0.5f);
                    rigid.AddForce(transform.up * Random.Range(1000f, 1500f));
                    yield return new WaitForSeconds(2.0f);
                    anim.SetBool("Run", true);
                }
            }
        }
        IEnumerator RoamingRandom()
        {
            nav.speed = MoveSpeed * 3;
            state = State.Roaming1;
            StartCoroutine("Roaming1");
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(10.0f, 70.0f));
                int rnd = Random.Range(0, 100);
                if (rnd <= 50)
                {
                    rigid.velocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                    if (nav.isActiveAndEnabled) { nav.destination = transform.position; }
                    if (state == State.Roaming1 && roaming != null) { state = State.Roaming2; StopCoroutine("Roaming1"); StartCoroutine("Roaming2"); }
                    else if (state == State.Roaming2) { state = State.Roaming1; StopCoroutine("Roaming2"); StartCoroutine("Roaming1"); }
                }
            }
        }

        // 제자리에서 랜덤한 방향으로 짧게 움직이는 행동
        IEnumerator Roaming1()
        {
            //Debug.Log("Roaming1");
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            nav.enabled = false;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                players = GameObject.FindGameObjectsWithTag("Player");
                if (players.Length > 0)
                {
                    distances = new float[players.Length];
                    targets.Clear();
                    for (int i = 0; i < players.Length; i++)
                    {
                        distances[i] = Vector3.Distance(players[i].transform.position, transform.position);
                        if (distances[i] <= nav.stoppingDistance + 35.0f)
                        {
                            targets.Add(i + "%" + distances[i]);
                        }
                    }
                    if (targets.Count > 0)
                    {
                        int rnd1 = Random.Range(0, targets.Count);
                        chaseTarget = players[int.Parse(targets[rnd1].Split('%')[0])].transform;
                        StartCoroutine("Chase");
                        state = State.Chase;
                        yield break;
                    }
                }
                float h = Random.Range(-2.0f, 2.0f);
                float v = Random.Range(-2.0f, 2.0f);
                int a = Random.Range(30, 500);
                anim.SetBool("Run", true);
                for (int i = 0; i < a; i++)
                {
                    Vector3 dir = new Vector3(h, 0, v).normalized;
                    rigid.position += dir * 2f * MoveSpeed * slow * Time.fixedDeltaTime;
                    transform.forward = Vector3.Lerp(transform.forward, dir, Time.fixedDeltaTime * 6);
                    float angle = Quaternion.FromToRotation(transform.forward, dir).eulerAngles.y;
                    if (angle >= 0 && angle < 10) { slow = 1.0f; }
                    else if (angle >= 350 && angle <= 360) { slow = 1.0f; }
                    else if (angle >= 10 && angle < 350) { slow = 0.5f; }
                    yield return new WaitForFixedUpdate();
                }
                anim.SetBool("Run", false);
                rigid.velocity = Vector3.zero;
                a = Random.Range(5, 40);
                for (int i = 0; i < a; i++)
                {
                    yield return new WaitForFixedUpdate();
                }
                int rnd = Random.Range(0, 1000);
                if (rnd < 200)
                {
                    yield return new WaitForSeconds(Random.Range(0.3f, 1.5f));
                }
                else if (rnd >= 985)
                {
                    if (roaming != null)
                    {
                        StartCoroutine("Roaming2");
                        state = State.Roaming2;
                        yield break;
                    }
                }

            }
        }

        IEnumerator Roaming2()
        {
            nav.speed = MoveSpeed * 3;
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            //Debug.Log("Roaming2");
            nav.enabled = true;
            while (true)
            {
                anim.SetBool("Run", false);
                yield return new WaitForSeconds(1.0f);
                int rnd = Random.Range(0, roaming.transform.childCount);
                while (rnd == prevRomingPoint)
                {
                    rnd = Random.Range(0, roaming.transform.childCount);
                }
                prevRomingPoint = rnd;
                if (nav.isOnNavMesh)
                {
                    nav.destination = roaming.transform.GetChild(prevRomingPoint).position;
                    anim.SetBool("Run", true);
                }
                else
                {
                    state = State.Roaming1;
                    StartCoroutine("Roaming1");
                    yield break;
                }
                while (true)
                {
                    players = GameObject.FindGameObjectsWithTag("Player");
                    if (players.Length > 0)
                    {
                        distances = new float[players.Length];
                        targets.Clear();
                        for (int i = 0; i < players.Length; i++)
                        {
                            distances[i] = Vector3.Distance(players[i].transform.position, transform.position);
                            if (distances[i] <= nav.stoppingDistance + 35.0f)
                            {
                                targets.Add(i + "%" + distances[i]);
                            }
                        }
                        if (targets.Count > 0)
                        {
                            int rnd1 = Random.Range(0, targets.Count);
                            chaseTarget = players[int.Parse(targets[rnd1].Split('%')[0])].transform;
                            StartCoroutine("Chase");
                            state = State.Chase;
                            yield break;
                        }
                    }
                    if (nav.isOnNavMesh)
                    {
                        if (Vector3.Distance(roaming.transform.GetChild(prevRomingPoint).position, transform.position) <= nav.stoppingDistance + Random.Range(1, 3.0f))
                        {
                            rnd = Random.Range(0, 100);
                            if (rnd > 10)
                            {
                                anim.SetBool("Run", false);
                                if (nav.isOnNavMesh)
                                    nav.Stop();
                                rigid.velocity = Vector3.zero;
                                rigid.angularVelocity = Vector3.zero;
                                yield return new WaitForSeconds(Random.Range(0, 3.0f));
                                if (nav.isOnNavMesh)
                                    nav.Resume();
                                anim.SetBool("Run", true);
                                break;
                            }
                            else
                            {
                                state = State.Roaming1;
                                StartCoroutine("Roaming1");
                                yield break;
                            }
                        }
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        public void HitChase(Transform tr)
        {
            chaseTarget = tr;
            hitchase = true;
            StopCoroutine("Chase");
            StopCoroutine("RoamingRandom");
            StopCoroutine("Roaming1");
            StopCoroutine("Roaming2");
            state = State.Chase;
            StartCoroutine("Chase");
        }


        public IEnumerator Chase()
        {
            nav.speed = MoveSpeed * Random.Range(3.8f, 6.2f);
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            //Debug.Log("Chase");
            state = State.Chase;
            StopCoroutine("RoamingRandom");
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            if (isBoss)
            {
                anim.SetTrigger("Scream");
                audio.clip = audioClip[2];
                audio.Play();
                yield return new WaitForSeconds(1.2f);
                var Players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in Players)
                {
                    if (!player.GetComponent<Characters.PlayerCtrl>().avoidSkill && !player.GetComponent<Characters.PlayerCtrl>().holdSkill)
                    {
                        StartCoroutine(player.GetComponent<Characters.PlayerCtrl>().NoiseAnimation());
                    }
                }
                yield return new WaitForSeconds(3.0f);

            }
            if (nav.enabled == false) { nav.enabled = true; }
            skip = 0;
            while (true)
            {
                yield return null;
                if (nav.isOnNavMesh)
                {
                    nav.destination = chaseTarget.position;
                    anim.SetBool("Run", true);
                }
                else continue;


                if (!Life)
                {
                    MoveSpeed = 0;
                    nav.enabled = false;
                    yield break;
                }

                ////////////////////////////// 추적방식 ///////////////////////////////////////

                //추적 방식에 많은 변화주기 (기본추적, 사이드추적, 와리가리추적, 큰곡선추적, 플레이어 예측 추적, 무리일시 흩어지기 추적 등등)

                int rnd = Random.Range(0, 1300);
                if (rnd < 10)
                {
                    //낮은확률로 사이드로 빨리쫒아가기
                    nav.enabled = false;
                    anim.SetBool("Run", true);
                    int a = Random.Range(0, 2);
                    a = 2 * a - 1;
                    float b = Random.Range(-0.2f, 0.2f);
                    anim.speed = 1.4f;
                    for (int i = 0; i < Random.Range(60, 450); i++)
                    {
                        if (Vector3.Distance(chaseTarget.position, transform.position) < nav.stoppingDistance + 0.2f)
                        {
                            break;
                        }
                        Vector3 vecT = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z).normalized;
                        Vector3 vecR = Quaternion.Euler(0, a * 90f, 0) * vecT;
                        Vector3 vec = (vecT * (0.7f + b) + vecR * (0.5f - b)).normalized;
                        if (isBoss)
                        {
                            rigid.position += vec * nav.speed * 1.3f * 0.01f;
                        }
                        else
                        {
                            rigid.position += vec * nav.speed * 1.1f * 0.01f;
                        }
                        transform.forward = Vector3.Slerp(transform.forward, vec, 0.05f);
                        yield return new WaitForSeconds(0.01f);
                    }
                    anim.speed = 1.0f;
                    nav.enabled = true;
                    if (nav.isOnNavMesh)
                    {
                        nav.Resume();
                        anim.SetBool("Run", true);
                        nav.destination = chaseTarget.position;
                    }
                    else
                    {
                        anim.SetBool("Run", false);
                    }
                    rigid.velocity = Vector3.zero;
                    rigid.angularVelocity = Vector3.zero;
                    continue;
                }




                //////////////////////// 중거리 상태 (루프) ////////////////////
                if (Vector3.Distance(chaseTarget.position, transform.position) <= nav.stoppingDistance + Random.Range(2f, 40f)
                    && Vector3.Distance(chaseTarget.position, transform.position) > nav.stoppingDistance + 1
                    && skip == 0)
                {


                    //낮은확률로 원거리공격
                    if (bullet != null)
                    {
                        int rnd1 = Random.Range(0, 5500);
                        if (rnd1 <= LongAttackProbablity)
                        {
                            for (int i = 0; i < 100; i++)
                            {
                                Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                                Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                                float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                                transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.1f);
                                if ((angle >= -5 && angle <= 15) || (angle >= 345 && angle <= 365))
                                {
                                    break;
                                }
                                yield return new WaitForSeconds(0.01f);
                            }
                            anim.SetBool("Run", false);
                            rigid.velocity = Vector3.zero;
                            rigid.angularVelocity = Vector3.zero;
                            if (nav.isOnNavMesh)
                                nav.Stop();
                            yield return new WaitForSeconds(0.5f);
                            anim.SetTrigger("Attack3");
                            audio.clip = audioClip[4];
                            audio.Play();
                            yield return new WaitForSeconds(0.5f);
                            for (int i = 0; i < 50; i++)
                            {
                                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3")) { break; }
                                yield return null;
                            }
                            if (isBoss) { yield return new WaitForSeconds(0.2f); }
                            GameObject go = Instantiate(bullet.gameObject, bullet.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                            go.transform.parent = null;
                            go.SetActive(true);
                            go.GetComponent<Rigidbody>().velocity = ((chaseTarget.position - go.transform.position) * 0.8f + transform.forward * 0.2f).normalized * 12f;
                            go.GetComponent<EnemyBullet>().bulletHitCheck = false;
                            yield return new WaitForSeconds(0.8f);
                            float timeMax = Mathf.Min(anim.GetCurrentAnimatorStateInfo(0).length, 3.0f);
                            for (float time = 0; time < timeMax * Random.Range(0.6f, 0.9f); time += 0.01f)
                            {
                                if (go != null)
                                {
                                    if (go.GetComponent<EnemyBullet>().bulletHitCheck)
                                    {
                                        if (!Life)
                                        {
                                            MoveSpeed = 0;
                                            nav.enabled = false;
                                            yield break;
                                        }
                                        Attack();
                                        //Debug.Log("Attack3");
                                        Destroy(go);
                                        go.GetComponent<EnemyBullet>().bulletHitCheck = false;
                                        yield return new WaitForSeconds(timeMax * Random.Range(0.6f, 0.9f) - time);
                                        break;
                                    }
                                }
                                yield return new WaitForSeconds(0.01f);
                            }
                            rigid.velocity = Vector3.zero;
                            rigid.angularVelocity = Vector3.zero;
                            for (int i = 0; i < 30; i++)
                            {
                                if (go != null)
                                {
                                    if (go.GetComponent<EnemyBullet>().bulletHitCheck)
                                    {
                                        if (!Life)
                                        {
                                            MoveSpeed = 0;
                                            nav.enabled = false;
                                            yield break;
                                        }
                                        Attack();
                                        //Debug.Log("Attack3");
                                        Destroy(go);
                                        go.GetComponent<EnemyBullet>().bulletHitCheck = false;
                                        yield return new WaitForSeconds((30 - i) * 0.01f);
                                    }
                                }
                                Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                                Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                                float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                                transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.03f);
                                yield return new WaitForSeconds(0.01f);
                            }
                            yield return new WaitForSeconds(Random.Range(0.7f, 1.4f));
                            if (isBoss) { yield return new WaitForSeconds(0.5f); }
                            if (go != null) { Destroy(go); }
                            if (nav.isOnNavMesh)
                            {
                                anim.SetBool("Run", true);
                                nav.Resume();
                                nav.destination = chaseTarget.position;
                            }
                            int rnd2 = Random.Range(0, 100);
                            if (rnd2 > LongAttackProbablity)
                            {
                                skip = 1;
                            }
                            rigid.velocity = Vector3.zero;
                            rigid.angularVelocity = Vector3.zero;
                            continue;
                        }
                    }

                    //낮은확률로 돌진공격
                    if (RushAttackProbablity > 0)
                    {
                        int rnd1 = Random.Range(0, 5500);
                        if (rnd1 <= RushAttackProbablity)
                        {


                        }








                    }




                }


                //////////////////// 근거리 상태 (업데이트 루프)/////////////////////////////
                if (Vector3.Distance(chaseTarget.position, transform.position) < nav.stoppingDistance + 0.2f)
                {
                    rnd = Random.Range(0, 200);
                    // 공격1 발동 (66%)
                    if (rnd < 66)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                            Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                            float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                            transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.1f);
                            if ((angle >= -5 && angle <= 15) || (angle >= 345 && angle <= 365))
                            {
                                break;
                            }
                            yield return new WaitForSeconds(0.01f);
                        }
                        rigid.velocity = Vector3.zero;
                        rigid.angularVelocity = Vector3.zero;
                        anim.SetBool("Run", false);
                        if (nav.isOnNavMesh)
                            nav.Stop();
                        anim.SetTrigger("Attack1");
                        if((TYPE)Type != TYPE.Boss2)
                        {
                            audio.clip = isBoss ? audioClip[0] : audioClip[3];
                        }
                        audio.Play();
                        hitbox.GetComponent<EnemyHitBox>().hitBoxCheck = false;
                        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
                        {
                            yield return null;
                        }
                        float timeMax = anim.GetCurrentAnimatorStateInfo(0).length;
                        for (float time = 0; time < timeMax * Random.Range(0.6f, 0.9f); time += 0.01f)
                        {
                            Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                            Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                            float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                            transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.005f);
                            if (hitbox.GetComponent<EnemyHitBox>().hitBoxCheck)
                            {
                                if (!Life)
                                {
                                    MoveSpeed = 0;
                                    nav.enabled = false;
                                    yield break;
                                }
                                if ((TYPE)Type != TYPE.Boss2)
                                {
                                    audio.clip = isBoss ? audioClip[0] : audioClip[3];
                                }
                                Attack();
                                //Debug.Log("Attack1");
                                yield return new WaitForSeconds(timeMax * Random.Range(0.6f, 0.9f) - time);
                                break;
                            }
                            yield return new WaitForSeconds(0.01f);
                        }
                        hitbox.GetComponent<EnemyHitBox>().hitBoxCheck = false;
                        rigid.velocity = Vector3.zero;
                        rigid.angularVelocity = Vector3.zero;
                        for (int i = 0; i < 30; i++)
                        {
                            Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                            Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                            float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                            transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.03f);
                            yield return new WaitForSeconds(0.01f);
                        }
                        if (nav.isOnNavMesh)
                        {
                            nav.Resume();
                            if (Vector3.Distance(chaseTarget.position, transform.position) > nav.stoppingDistance + 0.2f)
                            {
                                anim.SetBool("Run", true);
                                nav.destination = chaseTarget.position;
                            }
                        }
                        else
                        {
                            anim.SetBool("Run", false);
                        }
                        yield return new WaitForSeconds(Random.Range(0.6f, 1.3f));
                        skip = 0;
                        rigid.velocity = Vector3.zero;
                        rigid.angularVelocity = Vector3.zero;
                        continue;
                    }
                    // 공격2 발동 (33%)
                    else if (rnd < 100)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                            Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                            float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                            transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.1f);
                            if ((angle >= -5 && angle <= 15) || (angle >= 345 && angle <= 365))
                            {
                                break;
                            }
                            yield return new WaitForSeconds(0.01f);
                        }
                        yield return new WaitForSeconds(0.2f);
                        rigid.velocity = Vector3.zero;
                        rigid.angularVelocity = Vector3.zero;
                        anim.SetBool("Run", false);
                        if (nav.isOnNavMesh)
                            nav.Stop();
                        anim.SetTrigger("Attack2");
                        audio.clip = isBoss ? audioClip[0] : audioClip[3];
                        audio.Play();
                        hitbox.GetComponent<EnemyHitBox>().hitBoxCheck = false;
                        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
                        {
                            yield return null;
                        }
                        float timeMax = anim.GetCurrentAnimatorStateInfo(0).length;
                        for (float time = 0; time < timeMax * Random.Range(0.6f, 0.9f); time += 0.01f)
                        {
                            Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                            Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                            float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                            transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.005f);
                            if (hitbox.GetComponent<EnemyHitBox>().hitBoxCheck)
                            {
                                if (!Life)
                                {
                                    MoveSpeed = 0;
                                    nav.enabled = false;
                                    yield break;
                                }
                                Attack();
                                //Debug.Log("Attack2");
                                yield return new WaitForSeconds(timeMax * Random.Range(0.6f, 0.9f) - time);
                                break;
                            }
                            yield return new WaitForSeconds(0.01f);
                        }
                        hitbox.GetComponent<EnemyHitBox>().hitBoxCheck = false;
                        rigid.velocity = Vector3.zero;
                        rigid.angularVelocity = Vector3.zero;
                        for (int i = 0; i < 30; i++)
                        {
                            Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                            Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                            float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                            transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.03f);
                            yield return new WaitForSeconds(0.01f);
                        }
                        if (nav.isOnNavMesh)
                        {
                            nav.Resume();
                            if (Vector3.Distance(chaseTarget.position, transform.position) > nav.stoppingDistance + 0.2f)
                            {
                                anim.SetBool("Run", true);
                                nav.destination = chaseTarget.position;
                            }
                        }
                        else
                        {
                            anim.SetBool("Run", false);
                        }
                        yield return new WaitForSeconds(Random.Range(0.9f, 2.5f));
                        skip = 0;
                        rigid.velocity = Vector3.zero;
                        rigid.angularVelocity = Vector3.zero;
                        continue;
                    }

                    rnd = Random.Range(0, 100);
                    if (rnd < LongAttackProbablity * 0.5f)
                    {
                        // 근거리에서 거리 벌리고 원거리공격
                        nav.enabled = false;
                        Vector3 vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                        Vector3 vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z).normalized;
                        if (Vector3.Distance(chaseTarget.position, transform.position) <= nav.stoppingDistance + Random.Range(0.2f, 5.0f))
                        {
                            int a = Random.Range(0, 2);
                            a = 2 * a - 1;
                            Vector3 vec = (new Vector3(transform.right.x, 0, transform.right.z) * 20 * a - vec2).normalized;
                            for (int i = 0; i < Random.Range(50, 170); i++)
                            {
                                rigid.position += vec * MoveSpeed * 4.5f * 0.01f;
                                transform.forward = Vector3.Slerp(transform.forward, vec, 0.05f);
                                yield return new WaitForSeconds(0.01f);
                            }
                            for (int i = 0; i < 100; i++)
                            {
                                vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                                vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                                float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                                transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.1f);
                                if ((angle >= -5 && angle <= 15) || (angle >= 345 && angle <= 365))
                                {
                                    break;
                                }
                                yield return new WaitForSeconds(0.01f);
                            }
                            anim.SetBool("Run", false);
                            rigid.velocity = Vector3.zero;
                            rigid.angularVelocity = Vector3.zero;
                            nav.enabled = true;
                            if (nav.isOnNavMesh)
                                nav.Stop();
                            anim.SetTrigger("Attack3");
                            audio.clip = audioClip[4];
                            audio.Play();
                            for (int i = 0; i < 50; i++)
                            {
                                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3")) { break; }
                                yield return null;
                            }
                            if (isBoss) { yield return new WaitForSeconds(0.2f); }
                            GameObject go = Instantiate(bullet.gameObject, bullet.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                            go.transform.parent = null;
                            go.SetActive(true);
                            go.GetComponent<Rigidbody>().velocity = ((chaseTarget.position - go.transform.position) * 0.8f + transform.forward * 0.2f).normalized * 12f;
                            rigid.velocity = Vector3.zero;
                            rigid.angularVelocity = Vector3.zero;
                            for (int i = 0; i < 30; i++)
                            {
                                if (go != null)
                                {
                                    if (go.GetComponent<EnemyBullet>().bulletHitCheck)
                                    {
                                        if (!Life)
                                        {
                                            MoveSpeed = 0;
                                            nav.enabled = false;
                                            yield break;
                                        }
                                        Attack();
                                        //Debug.Log("Attack3");
                                        Destroy(go);
                                        go.GetComponent<EnemyBullet>().bulletHitCheck = false;
                                        yield return new WaitForSeconds((30 - i) * 0.01f);
                                    }
                                }
                                vec1 = new Vector3(transform.forward.x, 0, transform.forward.z);
                                vec2 = new Vector3(chaseTarget.position.x - transform.position.x, 0, chaseTarget.position.z - transform.position.z);
                                float angle = Quaternion.FromToRotation(vec1, vec2).eulerAngles.y;
                                transform.forward = Vector3.Slerp(transform.forward, new Vector3(vec2.x, transform.forward.y, vec2.z), 0.03f);
                                yield return new WaitForSeconds(0.01f);
                            }
                            yield return new WaitForSeconds(Random.Range(0.7f, 1.4f));
                            if (isBoss) { yield return new WaitForSeconds(0.3f); }
                            if (go != null) { Destroy(go); }
                            if (nav.isOnNavMesh)
                            {
                                nav.Resume();
                                if (Vector3.Distance(chaseTarget.position, transform.position) > nav.stoppingDistance + 0.2f)
                                {
                                    anim.SetBool("Run", true);
                                    nav.destination = chaseTarget.position;
                                }
                            }
                            else
                            {
                                anim.SetBool("Run", false);
                            }
                            rnd = Random.Range(0, 100);
                            if (rnd > LongAttackProbablity)
                            {
                                skip = 1;
                            }
                            rigid.velocity = Vector3.zero;
                            rigid.angularVelocity = Vector3.zero;
                            continue;
                        }

                    }
                    else if (rnd < 10)
                    {
                        // 낮은확률로 사이드잡기 공격

                    }
                    if (isBoss)
                    {
                        rnd = Random.Range(0, 100);
                        if (rnd < 8)
                        {
                            // 낮은확률로 소리지르기 (보스)
                            anim.SetBool("Run", false);
                            anim.speed = 0.8f;
                            if (nav.isOnNavMesh)
                                nav.Stop();
                            rigid.velocity = Vector3.zero;
                            rigid.angularVelocity = Vector3.zero;
                            anim.SetTrigger("Scream");
                            audio.clip = audioClip[2];
                            audio.Play();
                            yield return new WaitForSeconds(1.2f);
                            var Players = GameObject.FindGameObjectsWithTag("Player");
                            foreach (GameObject player in Players)
                            {
                                if (!player.GetComponent<Characters.PlayerCtrl>().avoidSkill && !player.GetComponent<Characters.PlayerCtrl>().holdSkill)
                                {
                                    StartCoroutine(player.GetComponent<Characters.PlayerCtrl>().NoiseAnimation());
                                }
                            }
                            yield return new WaitForSeconds(3.0f);
                            if (nav.isOnNavMesh)
                            {
                                if (Vector3.Distance(chaseTarget.position, transform.position) > nav.stoppingDistance + 0.2f)
                                {
                                    nav.Resume();
                                    anim.SetBool("Run", true);
                                    nav.destination = chaseTarget.position;
                                }
                            }
                            else
                            {
                                anim.SetBool("Run", false);
                            }
                            anim.speed = 1.0f;
                            rigid.velocity = Vector3.zero;
                            rigid.angularVelocity = Vector3.zero;
                            continue;
                        }
                    }

                    // 똑똑한 근접공격 한번 만들어 보기



                    // 도망가기



                }

                // 추적 범위를 벗어났을 때
                if (Vector3.Distance(chaseTarget.position, transform.position) > nav.stoppingDistance + 35f && !hitchase)
                {
                    StartCoroutine("RoamingRandom");
                    yield break;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
