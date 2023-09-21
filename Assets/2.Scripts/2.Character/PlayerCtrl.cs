using Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Characters
{
    public class PlayerCtrl : csSceneManager<PlayerCtrl>
    {

        public GameObject Model0, Model1, Model2;
        private GameObject playerModel;
        public string playerName;

        //참조 변수
        private Animator anim;
        private Rigidbody rigid;
        private Transform tr;
        private Transform camPos, camRot;
        // 위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정
        private Vector3 currPos = Vector3.zero;
        private Quaternion currRot = Quaternion.identity;
        private Transform weapon;

        private float mp;
        public float Mp { get { return mp; } set { mp = value; } }

        private float sp;
        public float Sp { get { return sp; } set { sp = value; } }


        [SerializeField] private float m_MoveSpeed = 5.2f; // 이동 스피드 조절
        [SerializeField] private float m_JumpForce = 500; // 점프 높이 조절
        [SerializeField] public bool m_MouseLook = false; // 마우스 GameView에 고정 및 마우스 이동 시 카메라 회전        

        //0:실행가능 1:실행중 2:쿨타임 3:실행불가
        private int m_Jump; //점프상태
        private int m_Shoot; //공격상태
        private int m_Reload; //장전상태
        private int m_Move; //이동 상태
        //이동관련 변수
        private bool canMove = true;
        public bool checkDash; // 대쉬 상태

        //이동 관련변수
        private Vector3 moveDirection;
        private float slow = 1;
        private bool shootCantMove;
        private bool reloadSlowMove;

        //마우스 고정 및 카메라 회전 관련 변수
        public bool tab;

        [SerializeField] public float mouseSensitivity = 300;

        private float angleY = 0;
        private float angleX = 0;

        //점프 관련변수
        private bool goaled;
        private bool falled;
        private bool grounded;
        private Ray groundRay;
        private bool space;

        //총쏘기 관련변수
        [SerializeField] public int bullet;
        [SerializeField] public int bulletTotal;
        Vector3 screenVector;
        private Ray shootRay;
        private Ray[] shootGunRay = new Ray[20];
        private RaycastHit hitInfo;
        [SerializeField] public int bulletMax; //한 탄창 총알수 (무기마다 다름)
        //private int recoilStep = 0;
        private float recoil = 0.8f; //반동량 (무기마다 다름)
        private bool click;
        [SerializeField] private GameObject bulletObj;


        //장전 관련변수
        private bool rkey;
        //저격 줌 관련변수
        private bool zkey;

        // 스킬 체크용 변수
        private bool skill_Alt_Key;
        private int skill_Alt_Is;
        private bool skill_Z_Key;
        private int skill_Z_Is;
        private bool skill_X_Key;
        private int skill_X_Is;

        // 스킬 관련 변수
        [HideInInspector] public bool canMoveSkill; // 스킬 중 이동 불가
        bool canTackle; // Mail 캐릭터 태클 콤보 체크
        [HideInInspector] public bool successTackle;
        [HideInInspector] public bool avoidSkill; // 무적 체크용 변수
        [HideInInspector] public bool holdSkill; // 데미지 감소 변수
        private bool isHit;
        private bool isNoise;
        [HideInInspector] public bool isHitDown;

        //무기교체 관련변수
        public bool weaponChange = false;
        public int weaponNum = 1;
        bool isActivated;//UI들이 켜져있는가?
        bool sellActivated;//파는 버튼

        //인벤토리
        public Transform Inventory;
        //인벤토리
        public Transform Skill;
        //셋업
        public Transform SetUp;
        //미니맵
        public Transform miniMap;

        //퀘스트
        public Sector playerSector;

        // 무기 타입
        public WeaponType weaponType;
        public enum WeaponType
        {
            Non, // 기본 권총
            Main, // 메인 무기
            Sub // 서브 무기
        }

        //무기상점을 위해 추가
        public Transform sellButton;
        public Transform exitButton;

        // 사운드 추가
        public AudioClip[] audioClip;
        public AudioSource audio;
        public AudioSource runAudio;

        // 포톤 추가
        public PhotonView pv = null;


        public void Awake()
        {
            rigid = transform.GetComponent<Rigidbody>();
            camPos = transform.Find("camPos");            
            audio = transform.GetComponent<AudioSource>();
            runAudio = transform.Find("Player").transform.GetComponent<AudioSource>();
            tr = transform.GetComponentInChildren<Transform>();
            if (GameManager.GManager.avatar == "0")
            {
                playerModel = Model0;
            }
            else if (GameManager.GManager.avatar == "1")
            {
                playerModel = Model1;
            }
            else if (GameManager.GManager.avatar == "2")
            {
                playerModel = Model2;
            }
            weaponType = WeaponType.Non;
            bullet = 5;
            bulletMax = 5;
            bulletTotal = 20;
            PhotonNetwork.isMessageQueueRunning = true;

            if (pv == null)
            {
                // 포톤 추가
                pv = GetComponent<PhotonView>();
                //PhotonView Observed Components 속성에 PlayerCtrl(현재) 스크립트 Component를 연결
                pv.ObservedComponents[0] = this;
                //데이타 전송 타입을 설정
                pv.synchronization = ViewSynchronization.UnreliableOnChange;
            }
            /*if (PhotonNetwork.isMasterClient)
            {
                clearProcess = 1;
            }*/
        }

        public void Start()
        {            
            if (pv.isMine)
            {
                currPos = transform.position;
                currRot = transform.rotation;
                Vector3 pos = playerModel.transform.position;
                pos.y += 3.0f;
                if (transform.Find("Player").childCount == 0)
                {
                    PhotonNetwork.Instantiate(playerModel.name, pos, Quaternion.identity, 0).transform.parent = transform.Find("Player");
                }
                camPos.gameObject.SetActive(true);
                camRot = camPos.Find("camRot");
                canMoveSkill = false;
                m_MouseLook = true;
                anim = transform.Find("Player").GetChild(0).GetComponent<Animator>();
                Inventory = transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("InventoryUI");
                SetUp = transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("SetUp_Box");
                Skill = transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("Skill_Box");
                Skill.Find("model" + GameManager.GManager.avatar).gameObject.SetActive(true);
                weapon = GameObject.FindGameObjectWithTag("Weapon").transform;
                miniMap = transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("MiniMap_Box");
                miniMap.gameObject.SetActive(true);
                sellButton = Inventory.Find("04.Inventory").Find("SellBtn");
                exitButton = Inventory.Find("06.Exit");
                LoadData();
                Cursor.lockState = CursorLockMode.Locked;
                DontDestroyOnLoad(this);
            }
            else
            {
                var players = GameObject.FindGameObjectsWithTag("Model");
                foreach(GameObject go in players)
                {
                    if(go.GetComponent<PhotonView>().viewID == pv.viewID + 1)
                    {
                        Vector3 pos = go.transform.position;
                        pos.y += 3.0f;
                        go.transform.position = pos;
                        go.transform.parent = transform.Find("Player");
                        transform.Find("MiniMapCamera").gameObject.SetActive(false);
                    }
                }
                camPos.gameObject.SetActive(false);
                rigid.isKinematic = true;
            }
        }



        public void LoadData()
        {
            Reset();
            MaxHp = GameManager.Player.MaxHp;
            Hp = GameManager.Player.Hp;
            Mp = 100.0f;
            Sp = 1.0f;
            Atk = GameManager.Player.Atk;
            Level = GameManager.Player.Level;
            Exp = GameManager.Player.Exp;
            Gold = GameManager.Player.Gold;
            AtkSpeed = GameManager.Player.AtkSpeed;
            MoveSpeed = GameManager.Player.MoveSpeed;
            Critical = GameManager.Player.Critical;
            CriticalMax = GameManager.Player.CriticalMax;
            Drain = GameManager.Player.Drain;
            mouseSensitivity = float.Parse(GameManager.GManager.settingData[0]);
            holdSkill = false;
            avoidSkill = false;
            canMove = true;
            canMoveSkill = false;
            shootCantMove = false;
            skill_Alt_Is = 0;
            skill_X_Is = 0;
            skill_Z_Is = 0;
            slow = 1.0f;
        }

        public void StartMove()
        {
            if (pv.isMine)
            {
                if (!canMove)
                {
                    canMove = true;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                if (canMoveSkill) canMoveSkill = false;
            }
        }

        public void StopMove()
        {
            if (pv.isMine)
            {
                if (canMove)
                {
                    canMove = false;
                    Cursor.lockState = CursorLockMode.None;
                }
                if(!canMoveSkill) canMoveSkill = true;
                anim.SetInteger("move", 0);
            }
        }

        void CameraRotate()
        {            
            tr.rotation = Quaternion.Euler(0, angleY, 0);
            float x = 0.0f;
            float y = 0.0f;
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {

                x = Input.GetAxis("Mouse X");
                y = -Input.GetAxis("Mouse Y");
                if (m_MouseLook)
                {
                    angleY += x * mouseSensitivity * Time.fixedDeltaTime * ((Camera.main.fieldOfView == 15.0f) ? 0.25f : 1.0f);
                    angleX += y * mouseSensitivity * Time.fixedDeltaTime * ((Camera.main.fieldOfView == 15.0f) ? 0.25f : 1.0f);
                    angleX = Mathf.Clamp(angleX, -70, 70);
                    camRot.rotation = Quaternion.Euler(angleX, angleY, 0);
                    if (angleX > 0 && angleX < 90.1f)
                    {
                        float temp = (90 - angleX) / 90f;
                        Camera.main.transform.localPosition = new Vector3(0, 0.2f * (1 - temp), -4 * temp);
                    }
                    else
                    {
                        float temp = -angleX / 90f;
                        Camera.main.transform.localPosition = new Vector3(0, 0, (-2.0f * temp) + -4 * (1 - temp));
                    }

                }
            }
        }

        private void Update()
        {
            if (pv.isMine)
            {
                if (weaponChange)
                {
                    weaponChange = false;
                    StartCoroutine(WeaponChange());
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 3.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 3.0f);
            }


            if (skill_Alt_Is != 1 && grounded && skill_X_Is != 1 && skill_Z_Is != 1)
            {

            }


        }

        void FixedUpdate()
        {
            if (pv.isMine && Life)
            {
                if (SceneManager.GetActiveScene().name != "05.scTown" && SceneManager.GetActiveScene().name != "04.scLoading" && !checkDash)
                {
                    if (Sp < 1.0f)
                    {
                        Sp += 0.1f * Time.fixedDeltaTime;
                        StageManager.Scene.SpBar.fillAmount = Sp;
                    }
                    else Sp = 1.0f;
                }

                


                if (canMove)
                {
                    CameraRotate();
                    //이동 입력


                    if (!canMoveSkill && !shootCantMove)
                    {
                        float h = 0.0f;
                        float v = 0.0f;
                        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                        {
                            h = Input.GetAxis("Horizontal");
                            v = Input.GetAxis("Vertical");
                            moveDirection = new Vector3(tr.forward.x * v + tr.right.x * h, 0, tr.forward.z * v + tr.right.z * h).normalized;
                            float angle = Quaternion.FromToRotation(new Vector3(h, 0, v), new Vector3(transform.forward.x, 0, transform.forward.z)).eulerAngles.y - angleY;
                            float b = Mathf.Abs(Mathf.Abs(angle % 360) - 180) / 360 + 0.3f;
                            if(!runAudio.isPlaying && grounded)
                            {
                                runAudio.Play();
                            }
                            if(grounded)
                            {
                                runAudio.volume = 1;
                            }
                            else
                            {
                                runAudio.volume = 0;
                            }
                            if (checkDash && Sp >= 0.1f)
                            {                                
                                rigid.position += moveDirection * slow * b * m_MoveSpeed * Time.fixedDeltaTime * (MoveSpeed * 0.004f + 1.83f);
                                if (anim.GetInteger("move") != 2) { anim.SetInteger("move", 2); }
                                if (SceneManager.GetActiveScene().name != "05.scTown")
                                {
                                    Sp -= 0.13f * Time.fixedDeltaTime;
                                    StageManager.Scene.SpBar.fillAmount = Sp;
                                }
                            }
                            else
                            {
                                rigid.position += moveDirection * slow * b * m_MoveSpeed * Time.fixedDeltaTime * (MoveSpeed * 0.004f + 1.0f);
                                if (anim.GetInteger("move") != 1) { anim.SetInteger("move", 1); }
                            }

                        }
                        else
                        {
                            if (runAudio.isPlaying)
                                runAudio.Stop();
                            if (anim.GetInteger("move") == 1) { anim.SetInteger("move", 0); }
                        }
                        //점프 입력
                        groundRay.origin = transform.position + new Vector3(0, 1, 0);
                        groundRay.direction = -transform.up;
                        grounded = Physics.Raycast(groundRay, 1.1f, 1 << LayerMask.NameToLayer("Ground"));
                        if (Input.GetAxis("Jump") > 0 && grounded && !space)
                        {
                            space = true;
                            m_Jump = 1;
                            anim.SetTrigger("jump");
                            runAudio.volume = 0;
                            runAudio.Stop();
                            rigid.AddForce(transform.up * m_JumpForce);
                            //StartCoroutine("Jump");
                        }
                        else if (Input.GetAxisRaw("Jump") == 0 && space) { space = false; }
                        else if (grounded && m_Jump == 1) { m_Jump = 0; }
                    }
                    else
                    {
                        if (runAudio.isPlaying)
                            runAudio.Stop();
                    }
                    if (SceneManager.GetActiveScene().name != "05.scTown")
                    {
                        falled = Physics.Raycast(groundRay, 1.1f, 1 << LayerMask.NameToLayer("Fall"));
                        goaled = Physics.Raycast(groundRay, 1.1f, 1 << LayerMask.NameToLayer("Goal"));
                        if(canMove)
                        {
                            if (falled)
                            {
                                if (GameManager.GManager.nextScene != 3 && GameManager.GManager.StageNum == 1)
                                {
                                    GameManager.GManager.StageNum++;
                                    StartCoroutine(StageManager.Scene.Result());
                                }
                                else if(!grounded)
                                {
                                    Sp = 1;
                                    StageManager.Scene.SpBar.fillAmount = Sp;
                                    transform.position = GameObject.Find("Stage").transform.Find("playerSpawn" + playerSector.ToString()).position;
                                }                    
                            }
                            if(goaled)
                            {
                                if (GameManager.GManager.nextScene == 3)
                                {
                                    if(GameManager.GManager.StageNum == 1)
                                    {
                                        GameManager.GManager.StageNum++;
                                        StartCoroutine(StageManager.Scene.Result());
                                        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                                            Destroy(enemy);
                                        StageManager.Scene.MissonText.text = "모든 적 처치!(0/" + StageManager.Scene.monsterMax + ")";
                                    }
                                    else if (GameManager.GManager.StageNum == 2 && playerSector == Sector.Four)
                                        transform.position = GameObject.Find("Stage").transform.Find("playerSpawnFive").position;
                                }
                                else if (GameManager.GManager.StageNum == 2)
                                {
                                    GameManager.GManager.StageNum++;
                                    StartCoroutine(StageManager.Scene.Result());
                                }
                            }             
                        }
                    
                        //총쏘기 입력
                        if (Input.GetAxis("Fire1") > 0 && !click && m_Shoot == 0 && !canMoveSkill && !shootCantMove && canMove
                            && skill_Alt_Is != 1 && skill_X_Is != 1 && skill_Z_Is != 1 && !canMoveSkill)
                        {
                            click = true;
                            StartCoroutine("Shoot");
                        }
                        else if (Input.GetAxis("Fire1") == 0 && click) { click = false; }
                        //장전 입력
                        if (Input.GetKey(KeyCode.R) && !rkey && m_Reload == 0 && !canMoveSkill && !shootCantMove && canMove
                            && skill_Alt_Is != 1 && skill_X_Is != 1 && skill_Z_Is != 1 && !canMoveSkill)
                        {
                            rkey = true;
                            StartCoroutine("Reload");
                        }
                        else if (!Input.GetKey(KeyCode.R) && rkey) { rkey = false; }
                        if (Input.GetAxis("Fire2") > 0 && !zkey && weaponNum == 4) // 저격일 때 줌 가능
                        {
                            zkey = true;
                            if (Camera.main.fieldOfView == 15)                        
                                Camera.main.fieldOfView = 60; // 카메라 플레이어 위치로 원상복구
                            else                        
                                Camera.main.fieldOfView = 15; // 카메라 전환
                        }
                        else if (Input.GetAxis("Fire2") == 0 && zkey) // 저격일 때 줌 해제
                        {
                            zkey = false;
                        }
                        if (Camera.main.fieldOfView == 15 && weaponNum != 4) // 무기 바뀌면 줌 해제
                        {
                            Camera.main.fieldOfView = 60; // 카메라 플레이어 위치로 원상복구
                        }

                        ////////////////////////////////////////// 스킬 제작 ////////////////////////////////////////////////////////

                        // Alt 스킬
                        if (Input.GetKey(KeyCode.LeftAlt) && !skill_Alt_Key && grounded && !shootCantMove && canMove
                            && skill_Alt_Is == 0 && skill_X_Is != 1 && skill_Z_Is != 1 && !canMoveSkill)
                        {
                            skill_Alt_Key = true;
                            StartCoroutine("Skill_Alt");
                        }
                        else if (!Input.GetKey(KeyCode.LeftAlt) && skill_Alt_Key)
                        {
                            skill_Alt_Key = false;
                        }

                        // Z 스킬
                        if (Input.GetKey(KeyCode.Z) && !skill_Z_Key && grounded && !shootCantMove && canMove
                            && skill_Alt_Is != 1 && skill_X_Is != 1 && skill_Z_Is == 0 && !canMoveSkill)
                        {
                            skill_Z_Key = true;
                            StartCoroutine("Skill_Z");
                        }
                        else if (!Input.GetKey(KeyCode.Z) && skill_Z_Key)
                        {
                            skill_Z_Key = false;
                        }

                        // X 스킬
                        if (Input.GetKey(KeyCode.X) && !skill_X_Key && grounded && !shootCantMove && canMove
                            && skill_Alt_Is != 1 && skill_X_Is == 0 && skill_Z_Is != 1 && !canMoveSkill)
                        {
                            skill_X_Key = true;
                            StartCoroutine("Skill_X");
                        }
                        else if (!Input.GetKey(KeyCode.X) && skill_X_Key)
                        {
                            skill_X_Key = false;
                        }
                    }
                }
                else
                {
                    if (runAudio.isPlaying)
                        runAudio.Stop();
                }
            }
        }

        public void DieAnimation()
        {
            if (pv.isMine)
            {
                canMoveSkill = true;
                StopAllCoroutines();
                anim.SetTrigger("end");
                anim.SetTrigger("die");
            }
        }


        IEnumerator Shoot()
        {
            if (pv.isMine)
            {
                GameObject smoke = null, shotgunGO = null;
                float sniperDamage = 0;
                // 리로드 중이면 코루틴 실행 x
                if (m_Reload == 1) yield break;
                if (!m_MouseLook) yield break;
                /*
                if (m_Reload == 1)
                {
                    StopCoroutine("Reload");
                    m_Reload = 0;
                }
                */
                m_Shoot = 1;
                if (bullet > 0)
                {
                    slow = 0.8f;
                    Vector3 v, w;
                    Vector3[] shotGunBulletDistance = new Vector3[shootGunRay.Length];
                    GameObject[] shotGunBullet = new GameObject[shootGunRay.Length];
                    v = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
                    if (weaponNum == 4) // 저격
                    {
                        sniperDamage = -0.4f;
                        shootCantMove = true;
                        smoke = (GameObject)Instantiate(Resources.Load("Skill/BulletSmoke"), transform.position, transform.rotation * Quaternion.Euler(angleX - 1.5f, 0, 0));
                        audio.clip = audioClip[2];
                        audio.Play();
                    }
                    else if(weaponNum == 2) // 샷건
                    {
                        audio.clip = audioClip[1];
                        audio.Play();
                        shotgunGO = Instantiate(Resources.Load<GameObject>("Skill/ShotGunAtk"),transform.position + transform.forward * 0.5f  + transform.up, transform.rotation * Quaternion.Euler(angleX, 0, 0));
                        for (int i = 0; i < shootGunRay.Length; i++)
                        {
                            w = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + Random.Range(-70, 70), Input.mousePosition.y + Random.Range(-30, 30), Camera.main.nearClipPlane));
                            shootGunRay[i].origin = Camera.main.transform.position;
                            shootGunRay[i].direction = (w - Camera.main.transform.position).normalized; ;
                            shotGunBulletDistance[i] = shootGunRay[i].direction * 70f - (transform.Find("firePos").position - shootGunRay[i].origin);
                            shotGunBullet[i] = Instantiate(bulletObj, transform.Find("firePos").position, Quaternion.identity);
                            shotGunBullet[i].GetComponent<Rigidbody>().velocity = shotGunBulletDistance[i].normalized * 200f;
                            if (Physics.Raycast(shootGunRay[i], out hitInfo, Random.Range(30f, 40f)))
                            {
                                shotGunBulletDistance[i] = hitInfo.point - transform.Find("firePos").position;
                                float d = 0;
                                Debug.Log(hitInfo.collider.tag);
                                if (hitInfo.collider.tag == "EnemyBody")
                                {
                                    if (hitInfo.transform.root.GetComponent<Enemy>().Life)
                                    {
                                        hitInfo.transform.root.GetComponent<Enemy>().Damaged = true;
                                        if (float.TryParse(hitInfo.collider.name, out d))
                                        {
                                            //Debug.Log(1 - d);
                                            if(hitInfo.transform.GetComponent<Enemy>().isAlwaysFlyMonster)
                                            {
                                                hitInfo.transform.root.GetComponent<Enemy>().Hit(d - 0.2f);
                                            }
                                            else
                                            {
                                                hitInfo.transform.root.GetComponent<Enemy>().Hit(d + 0.2f);
                                            }
                                            if (hitInfo.transform.root.GetComponent<Enemy>().chaseTarget == null)
                                            {
                                                hitInfo.transform.root.GetComponent<Enemy>().HitChase(transform);
                                            }
                                        }
                                        else
                                        {
                                            hitInfo.transform.root.GetComponent<Enemy>().Hit(0);
                                        }
                                    }
                                }
                                else if (hitInfo.collider.tag == "Enemy" || hitInfo.collider.tag == "Item")
                                {
                                    var array = Physics.RaycastAll(shootRay, 40f);
                                    for (int j = 0; j < array.Length; j++)
                                    {
                                        if (array[j].collider.tag == "EnemyBody")
                                        {
                                            if (array[j].transform.root.GetComponent<Enemy>().Life)
                                            {
                                                array[j].transform.root.GetComponent<Enemy>().Damaged = true;
                                                if (float.TryParse(array[j].collider.name, out d))
                                                {
                                                    //Debug.Log(1 - d);
                                                    if (hitInfo.transform.GetComponent<Enemy>().isAlwaysFlyMonster)
                                                    {
                                                        hitInfo.transform.root.GetComponent<Enemy>().Hit(d - 0.2f);
                                                    }
                                                    else
                                                    {
                                                        hitInfo.transform.root.GetComponent<Enemy>().Hit(d + 0.2f);
                                                    }
                                                    if (array[j].transform.root.GetComponent<Enemy>().chaseTarget == null)
                                                    {
                                                        array[j].transform.root.GetComponent<Enemy>().HitChase(transform);
                                                    }
                                                }
                                                else
                                                {
                                                    array[j].transform.root.GetComponent<Enemy>().Hit(0);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (weaponNum != 2) // 샷건이 아닌 경우 단발
                    {
                        if (weaponNum != 4) // 저격이 아닐 때
                        {
                            audio.clip = audioClip[3];
                            audio.Play();
                        }
                        shootRay.origin = Camera.main.transform.position;
                        shootRay.direction = (v - Camera.main.transform.position).normalized; ;
                        Vector3 v0 = shootRay.direction * 70f - (transform.Find("firePos").position - shootRay.origin);
                        float atkRange = (Camera.main.fieldOfView == 15) ? 300f : 50f;
                        anim.SetTrigger("shoot1");
                        GameObject go = Instantiate(bulletObj, transform.Find("firePos").position, Quaternion.identity);
                        go.GetComponent<Rigidbody>().velocity = v0.normalized * 400f;


                        if (Physics.Raycast(shootRay, out hitInfo, atkRange))
                        {
                            v0 = hitInfo.point - transform.Find("firePos").position;
                            float d = 0;
                            //Debug.Log(hitInfo.collider.tag);
                            if (hitInfo.collider.tag == "EnemyBody")
                            {
                                if (hitInfo.transform.root.GetComponent<Enemy>().Life)
                                {
                                    hitInfo.transform.root.GetComponent<Enemy>().Damaged = true;
                                    if (float.TryParse(hitInfo.collider.name, out d))
                                    {
                                        //Debug.Log(1 - d);
                                        hitInfo.transform.root.GetComponent<Enemy>().Hit(d + sniperDamage);
                                        if (hitInfo.transform.root.GetComponent<Enemy>().chaseTarget == null)
                                        {
                                            hitInfo.transform.root.GetComponent<Enemy>().HitChase(transform);
                                        }
                                    }
                                    else
                                    {
                                        hitInfo.transform.root.GetComponent<Enemy>().Hit(0);
                                    }
                                }                                
                            }
                            else if (hitInfo.collider.tag == "Enemy" || hitInfo.collider.tag == "Item")
                            {
                                var array = Physics.RaycastAll(shootRay, atkRange);
                                for(int i=0; i < array.Length; i++)
                                {
                                    if(array[i].collider.tag == "EnemyBody")
                                    {
                                        if(array[i].transform.root.GetComponent<Enemy>().Life)
                                        {
                                            array[i].transform.root.GetComponent<Enemy>().Damaged = true;
                                            if (float.TryParse(array[i].collider.name, out d))
                                            {
                                                //Debug.Log(1 - d);
                                                array[i].transform.root.GetComponent<Enemy>().Hit(d + sniperDamage);
                                                if (array[i].transform.root.GetComponent<Enemy>().chaseTarget == null)
                                                {
                                                    array[i].transform.root.GetComponent<Enemy>().HitChase(transform);
                                                }
                                            }
                                            else
                                            {
                                                array[i].transform.root.GetComponent<Enemy>().Hit(0);
                                            }
                                            break;

                                        }
                                    }
                                }
                            }
                        }


                    }
                    else
                    {
                        anim.SetTrigger("shoot1");

                    }

                    bullet--;
                    csItemManager.Scene.BulletText.text = bullet.ToString() + " / " + bulletTotal;
                    if (csItemManager.Scene.inven.SetSlots[(int)weaponType + 1].item != null)
                        csItemManager.Scene.inven.SetSlots[(int)weaponType + 1].item.bullet = bullet;
                    for (int i = 0; i < 4; i++)
                    {
                        if (angleX > -70)
                        {
                            camRot.rotation *= Quaternion.Euler(-recoil, 0, 0);
                            angleX += -recoil;
                        }
                        yield return new WaitForSeconds(0.01f);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        if (angleX > -70)
                        {
                            camRot.rotation *= Quaternion.Euler(recoil, 0, 0);
                            angleX += recoil;
                        }
                        yield return new WaitForSeconds(0.01f);
                    }
                }
                m_Shoot = 2;
                slow = 1.0f;
                float atkspd = 0.15f - (AtkSpeed / 1000.0f);
                if (atkspd <= 0.01f) atkspd = 0.01f; // 최대 속도
                if (weaponNum == 4)
                {
                    atkspd *= 3f; // 스나 공격 속도
                    atkspd += 0.2f;
                }
                else if (weaponNum == 2) atkspd *= 1.5f; // 샷건 공격 속도
                yield return new WaitForSeconds(atkspd);
                if(weaponNum == 4)
                    shootCantMove = false;
                if (weaponNum != 3)
                    yield return new WaitForSeconds(0.4f);
                else click = false; // 라이플이면 연사 느낌
                m_Shoot = 0;
                if (smoke != null)
                    Destroy(smoke);
                if (shotgunGO != null)
                    Destroy(shotgunGO);

            }
        }

        IEnumerator Jump()
        {
            if (pv.isMine)
            {
                for (int i = 0; i < 10; i++)
                {
                    rigid.position += transform.up * m_MoveSpeed * Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
            }
        }


        IEnumerator Reload()
        {
            if (pv.isMine)
            {
                Camera.main.fieldOfView = 60;
                if (bulletTotal != 0)
                {
                    m_Reload = 1;
                    slow = 0.8f;
                    if (anim.GetBool("gun") == false) { anim.SetBool("gun", true); }
                    anim.SetTrigger("reload");
                    audio.clip = audioClip[0];
                    audio.Play();
                    yield return new WaitForSeconds(1.9f);
                    int a = bulletMax - bullet;
                    if (bulletTotal >= a)
                    {
                        bullet += a;
                        bulletTotal -= a;
                        if(csItemManager.Scene.inven.SetSlots[(int)weaponType + 1].item != null)
                        {
                            csItemManager.Scene.inven.SetSlots[(int)weaponType + 1].item.bullet = bullet;
                        }
                        else
                        {
                            
                        }
                        csItemManager.Scene.BulletText.text = bullet.ToString() + " / " + bulletTotal;
                    }
                    else
                    {
                        bullet += bulletTotal;
                        bulletTotal = 0;
                        csItemManager.Scene.BulletText.text = bullet.ToString() + " / " + bulletTotal;
                    }
                    if (csItemManager.Scene.inven.SetSlots[(int)weaponType + 1].item != null)
                        csItemManager.Scene.inven.SetSlots[(int)weaponType + 1].item.bulletTotal = bulletTotal;
                }
                yield return null;
                slow = 1.0f;
                anim.SetTrigger("end");
                m_Reload = 0;
            }
        }
        IEnumerator WeaponChange()
        {
            if (pv.isMine)
            {
                weapon.GetChild(0).gameObject.SetActive(weaponNum == 0); // 기본 권총
                weapon.GetChild(1).gameObject.SetActive(weaponNum == 1); // 아이템 권총
                weapon.GetChild(2).gameObject.SetActive(weaponNum == 2); // 아이템 라이플
                weapon.GetChild(3).gameObject.SetActive(weaponNum == 3); // 아이템 샷건
                weapon.GetChild(4).gameObject.SetActive(weaponNum == 4); // 아이템 저격총
                anim.SetBool("gun", false);
                yield return new WaitForSeconds(0.4f);
                anim.SetBool("gun", true);
            }
        }

        public void Exit()
        {
            if (pv.isMine)
            {
                GameManager.GManager.StageNum = 1;            
                if(GameManager.GManager.nextScene != 1)
                {
                    Awake();
                    StartMove();
                    SetUp.gameObject.SetActive(false);
                    Inventory.gameObject.SetActive(false);
                    Skill.gameObject.SetActive(false);
                    SkillManager.Scene.CoolTimeBox.gameObject.SetActive(false);
                    miniMap.gameObject.SetActive(true);
                }
                GameManager.GManager.nextScene = (GameManager.GManager.nextScene == 1) ? 0 : 1;
                PhotonNetwork.Destroy(pv);
                SceneManager.LoadScene("04.scLoading");
            }
        }

        /***********************************************************************
         *                              Inventory
         ***********************************************************************/
        #region Inventory

        //인벤토리를 키고 끈다.keyManager에서 사용 예정
        public void Inven_OnOff()
        {
            if (pv.isMine)
            {
                isActivated = !isActivated;

                if (isActivated)//켜져있다면
                {
                    csItemManager.Scene.Battle_Item.gameObject.SetActive(false);
                    csItemManager.Scene.BulletText.gameObject.SetActive(false);                    
                    miniMap.gameObject.SetActive(false);
                    Inventory.gameObject.SetActive(true);
                    StopMove();

                }
                else
                {
                    csItemManager.Scene.Battle_Item.gameObject.SetActive(true);
                    csItemManager.Scene.BulletText.gameObject.SetActive(true);
                    miniMap.gameObject.SetActive(true);
                    Inventory.gameObject.SetActive(false);
                    StartMove();
                }
            }
        }
        public void Sell_OnOff()
        {
            sellActivated = !sellActivated;
            sellButton.gameObject.SetActive(sellActivated);
            exitButton.gameObject.SetActive(!sellActivated);//나가는 버튼은 꺼준다


        }
        #endregion

        /***********************************************************************
         *                              SetUp
         ***********************************************************************/

        #region SetUp

        //인벤토리를 키고 끈다.keyManager에서 사용 예정
        public void SetUp_OnOff()
        {
            if (pv.isMine)
            {
                isActivated = !isActivated;

                if (isActivated)//켜져있다면
                {
                
                    SetUp.gameObject.SetActive(true);
                    StopMove();

                }
                else
                {
                
                    SetUp.gameObject.SetActive(false);
                    StartMove();
                }
            }
        }
        #endregion

        /***********************************************************************
        *                              Skill
        ***********************************************************************/

        #region Skill

        //인벤토리를 키고 끈다.keyManager에서 사용 예정
        public void Skill_OnOff()
        {
            isActivated = !isActivated;

            if (isActivated)//켜져있다면
            {

                Skill.gameObject.SetActive(true);
                StopMove();

            }
            else
            {

                Skill.gameObject.SetActive(false);
                StartMove();
            }

        }

        IEnumerator Skill_Alt()
        {
            if (SkillManager.Scene.levelSkill[8] < 1) yield break;
            if (Camera.main.fieldOfView == 15)
                Camera.main.fieldOfView = 60; // 카메라 플레이어 위치로 원상복구
            float v, h;
            Vector3 moveVector;
            anim.SetBool("gun", false);
            canMoveSkill = true;
            if (int.Parse(GameManager.GManager.avatar) == 0)
            {
                if (canTackle)
                {
                    canTackle = false;
                    Debug.Log("태클");
                    audio.clip = audioClip[5];
                    audio.Play();
                    skill_Alt_Is = 1;
                    anim.SetTrigger("tackle");
                    holdSkill = true;
                    yield return new WaitForSeconds(0.15f);
                    rigid.AddForce((transform.up * 0.6f + transform.forward) * 850f);
                    // 몬스터에게 일정량 데미지 입히기
                    for (int i = 0; i < 30; i++)
                    {
                        rigid.position += transform.forward * 0.87f * (MoveSpeed * 0.005f + 5.4f) * 0.01f;
                        yield return new WaitForSeconds(0.01f);
                    }
                    yield return new WaitForSeconds(0.2f);
                    holdSkill = false; // 부동상태 풀기
                    skill_Alt_Is = 2;
                    StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(8, 2.7f));
                    anim.SetInteger("move", 0);
                    anim.SetTrigger("end");
                    canMoveSkill = false;
                    anim.SetBool("gun", true);
                    yield return new WaitForSeconds(2.6f);
                    skill_Alt_Is = 0;
                }
                else
                {
                    Debug.Log("밀치기");
                    audio.clip = audioClip[5];
                    audio.Play();

                    skill_Alt_Is = 1;
                    anim.SetInteger("move", 0);
                    anim.SetTrigger("push");
                    anim.speed = 0.95f;
                    canTackle = true;
                    yield return new WaitForSeconds(0.08f);
                    for (int i = 0; i < 34; i++)
                    {
                        // 몬스터 밀치기
                        yield return new WaitForSeconds(0.01f);
                    }
                    skill_Alt_Is = 2;
                    yield return new WaitForSeconds(0.12f);
                    anim.speed = 1.0f;
                    anim.SetBool("gun", true);
                    canMoveSkill = false;
                    anim.SetTrigger("end");
                    skill_Alt_Is = 0;
                    bool check = false;
                    for (int i = 0; i < 45; i++)
                    {
                        if (skill_Alt_Is == 1)
                        {
                            check = true;
                            break;
                        }
                        yield return new WaitForSeconds(0.01f);
                    }
                    if (!check)
                    {
                        canTackle = false;
                        skill_Alt_Is = 2;
                        StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(8, 1.1f));
                        yield return new WaitForSeconds(1.1f);
                        skill_Alt_Is = 0;
                    }
                }
            }
            else if (int.Parse(GameManager.GManager.avatar) == 1)
            {
                Debug.Log("슬라이드");
                audio.clip = audioClip[16];
                audio.Play();
                skill_Alt_Is = 1;
                anim.SetTrigger("slide");
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
                yield return new WaitForSeconds(0.1f);
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                {
                    moveVector = new Vector3(transform.forward.x * v + transform.right.x * h, 0, transform.forward.z * v + transform.right.z * h).normalized;
                    transform.GetChild(0).forward = moveVector;
                }
                else
                {
                    moveVector = new Vector3(transform.forward.x, 0, transform.forward.z);
                }
                anim.SetInteger("move", 0);
                for (int i = 0; i < 65; i++)
                {
                    if (i < 50)
                    {
                        avoidSkill = true; // 이동안 무적 처리
                    }
                    else
                    {
                        avoidSkill = false; // 무적 풀기
                    }
                    rigid.position += moveVector * 2.71f * (MoveSpeed * 0.005f + 6.5f) * 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }
                anim.SetBool("gun", true);
                canMoveSkill = false;
                for (int i = 0; i < 30; i++)
                {
                    transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.identity, 0.2f);
                    yield return new WaitForSeconds(0.005f);
                }
                transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
                skill_Alt_Is = 2;
                anim.SetTrigger("end");
                StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(8, 2.2f));
                yield return new WaitForSeconds(2.2f);
                skill_Alt_Is = 0;
            }
            else if (int.Parse(GameManager.GManager.avatar) == 2)
            {
                Debug.Log("구르기");
                audio.clip = audioClip[16];
                audio.Play();
                skill_Alt_Is = 1;
                anim.SetTrigger("roll");
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
                yield return new WaitForSeconds(0.02f);
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                {
                    moveVector = new Vector3(transform.forward.x * v + transform.right.x * h, 0, transform.forward.z * v + transform.right.z * h).normalized;
                    transform.GetChild(0).forward = moveVector;
                }
                else
                {
                    moveVector = new Vector3(transform.forward.x, 0, transform.forward.z);
                }
                anim.SetInteger("move", 0);
                for (int i = 0; i < 50; i++)
                {
                    if (i < 40)
                    {
                        avoidSkill = true; // 이동안 무적 처리
                    }
                    else
                    {
                        avoidSkill = false; // 무적 풀기
                    }
                    rigid.position += moveVector * 1.7f * (MoveSpeed * 0.005f + 6) * 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }
                anim.SetBool("gun", true);
                canMoveSkill = false;
                for (int i = 0; i < 30; i++)
                {
                    transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.identity, 0.07f);
                    yield return new WaitForSeconds(0.005f);
                }
                transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
                
                skill_Alt_Is = 2;
                StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(8, 1.1f));
                anim.SetTrigger("end");
                yield return new WaitForSeconds(1.1f);
                skill_Alt_Is = 0;
            }
        }

        IEnumerator Skill_Z()
        {
            if (Mp < 20.0f || SkillManager.Scene.levelSkill[3] < 1) yield break;
            if (Camera.main.fieldOfView == 15)
                Camera.main.fieldOfView = 60; // 카메라 플레이어 위치로 원상복구
            anim.SetBool("gun", false);
            canMoveSkill = true;
            Mp -= 20.0f;
            StageManager.Scene.MpBar.fillAmount = Mp * 0.01f;
            if (int.Parse(GameManager.GManager.avatar) == 0)
            {
                Debug.Log("도발");
                skill_Z_Is = 1;
                anim.SetInteger("move", 0);
                anim.SetTrigger("taunt");
                audio.clip = audioClip[6];
                audio.Play();

                // 스피어캐스트로 50거리 안에 있는 에너미 태그 콜라이더를 전부 찾아서
                // 체이스 타겟을 이 트랜스폼으로 연결

                canTackle = true;
                yield return new WaitForSeconds(2f);
                skill_Z_Is = 2;
                StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(3, 10f));
                anim.SetBool("gun", true);
                canMoveSkill = false;
                anim.SetTrigger("end");
                yield return new WaitForSeconds(1.0f);
                canTackle = false;
                yield return new WaitForSeconds(9.0f);
                skill_Z_Is = 0;
            }
            else if (int.Parse(GameManager.GManager.avatar) == 1)
            {
                Debug.Log("속사포");
                audio.clip = audioClip[11];
                audio.Play();
                skill_Z_Is = 1;
                anim.SetInteger("move", 0);
                anim.SetTrigger("shoot3");
                anim.speed = 1.3f;

                // 여기에 레이캐스트 총 파바박 쏘기 만들기
                // 맞은 몬스터 잠깐 정지 + 히트애니매이션
                yield return new WaitForSeconds(0.4f);
                GameObject smoke1 = (GameObject)Instantiate(Resources.Load("Skill/BulletSmoke"), transform.position + transform.right*0.1f, transform.rotation * Quaternion.Euler(0, 0, 0));
                yield return new WaitForSeconds(0.1f);
                GameObject smoke2 = (GameObject)Instantiate(Resources.Load("Skill/BulletSmoke"), transform.position - transform.right*-0.1f, transform.rotation * Quaternion.Euler(0, 0, 0));

                var hits = Physics.BoxCastAll(transform.position, new Vector3(1.0f,1.0f,0.5f), transform.forward * 50f);
                int count = 0;
                for(int i=0; i<hits.Length; i++)
                {
                    if(hits[i].collider.tag == "EnemyBody")
                    {
                        if(hits[i].transform.root.gameObject.activeSelf)
                        {
                            hits[i].transform.root.GetComponent<Enemy>().Hit(-1.0f);
                            count++;
                        }
                    }
                    if(count > 5) { break; }
                }






                yield return new WaitForSeconds(2.0f);

                skill_Z_Is = 2;
                StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(3, 15f));
                anim.SetBool("gun", true);
                canMoveSkill = false;
                anim.speed = 1.0f;
                anim.SetTrigger("end");
                yield return new WaitForSeconds(3.0f);
                Destroy(smoke1);
                Destroy(smoke2);
                yield return new WaitForSeconds(12.0f);
                skill_Z_Is = 0;
            }
            else if (int.Parse(GameManager.GManager.avatar) == 2)
            {
                Debug.Log("주변탐색");
                skill_Z_Is = 1;
                anim.SetTrigger("workDown");
                audio.clip = audioClip[17];
                audio.Play();
                yield return new WaitForSeconds(1.2f);

                //총알 랜덤으로 획득

                yield return new WaitForSeconds(1.0f);

                //총알 랜덤으로 획득

                yield return new WaitForSeconds(1.0f);

                //총알 랜덤으로 획득

                yield return new WaitForSeconds(1.0f);
                skill_Z_Is = 2;
                StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(3, 8f));
                anim.SetBool("gun", true);
                canMoveSkill = false;
                anim.SetTrigger("end");
                yield return new WaitForSeconds(8.0f);
                skill_Z_Is = 0;
            }
        }


        IEnumerator Skill_X()
        {
            if (Mp < 15.0f || SkillManager.Scene.levelSkill[7] < 1) yield break;
            if (Camera.main.fieldOfView == 15)
                Camera.main.fieldOfView = 60; // 카메라 플레이어 위치로 원상복구
            anim.SetBool("gun", false);
            canMoveSkill = true;
            Mp -= 15.0f;
            StageManager.Scene.MpBar.fillAmount = Mp * 0.01f;

            if (int.Parse(GameManager.GManager.avatar) == 0)
            {
                Debug.Log("고기뜯기");
                skill_X_Is = 1;
                anim.SetInteger("move", 0);
                anim.SetTrigger("drink");
                audio.clip = audioClip[10];
                audio.Play();

                // 내 HP 회복
                for (int i = 0; i < 31; i++)
                {
                    Hp += MaxHp * 0.01f;
                    Hp = Mathf.Clamp(Hp, 0, MaxHp);
                    StageManager.Scene.HpBar.fillAmount = Hp / MaxHp;
                    yield return new WaitForSeconds(0.01f);
                }
                canTackle = true;
                StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(7, 35f));
                yield return new WaitForSeconds(2.0f);
                skill_X_Is = 2;
                canMoveSkill = false;
                anim.SetTrigger("end");
                anim.SetBool("gun", true);
                canTackle = false;
                yield return new WaitForSeconds(34.0f);
                skill_X_Is = 0;
            }
            else if (int.Parse(GameManager.GManager.avatar) == 1)
            {
                Debug.Log("회복탄");
                skill_X_Is = 1;
                anim.SetInteger("move", 0);
                anim.SetTrigger("shoot3");
                // 회복 투사체 발사
                yield return new WaitForSeconds(0.7f);
                audio.clip = audioClip[10];
                audio.Play();
                GameObject go = Instantiate(Resources.Load("Skill/HealBullet"), transform.position, Quaternion.identity) as GameObject;
                go.GetComponent<Rigidbody>().velocity = transform.forward * 40f;
                yield return new WaitForSeconds(0.7f);
                skill_X_Is = 2;
                StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(7, 10f));
                anim.SetBool("gun", true);
                canMoveSkill = false;
                anim.SetTrigger("end");
                yield return new WaitForSeconds(5.0f);
                Destroy(go);
                yield return new WaitForSeconds(5.0f);
                skill_X_Is = 0;
            }
            else if (int.Parse(GameManager.GManager.avatar) == 2)
            {
                Debug.Log("물건던지기");
                
                skill_X_Is = 1;
                anim.SetInteger("move", 0);
                anim.SetTrigger("throw");

                // 던져지는 궤적과 범위 표시
                // 안에 아군이 있으면 총알 일정량 채워짐

                yield return new WaitForSeconds(1.0f);
                canMoveSkill = false;
                anim.SetBool("gun", true);
                audio.clip = audioClip[15];
                audio.Play();
                GameObject go = Instantiate(Resources.Load("Skill/HealBullet"), transform.position, Quaternion.identity) as GameObject;
                go.GetComponent<Rigidbody>().velocity = transform.forward * 40f;
                yield return new WaitForSeconds(1.0f);
                skill_X_Is = 2;
                anim.SetTrigger("end");
                StartCoroutine(SkillManager.Scene.ShowCoolTimeUI(7, 8f));
                yield return new WaitForSeconds(6.0f);
                Destroy(go);
                yield return new WaitForSeconds(2.0f);
                skill_X_Is = 0;
            }
        }

        public IEnumerator HitAnimation()
        {
            if (holdSkill) { yield break; }
            Camera.main.fieldOfView = 60;
            anim.SetTrigger("end");
            StopCoroutine("NoiseAnimation");
            anim.SetInteger("move", 0);
            anim.SetTrigger("hit");
            StopCoroutine("Reload");
            m_Reload = 0;
            canMoveSkill = true;
            yield return new WaitForSeconds(0.5f);
            canMoveSkill = false;
        }


        public IEnumerator HitDownAnimation(Vector3 vector)
        {
            if (holdSkill) { yield break; }
            Camera.main.fieldOfView = 60;
            anim.SetTrigger("end");
            StopCoroutine("NoiseAnimation");
            anim.SetBool("gun", false);
            holdSkill = true;
            anim.SetInteger("move", 0);
            canMoveSkill = true;
            yield return new WaitForSeconds(0.02f);
            anim.SetTrigger("hitDown");
            rigid.AddForce((vector + transform.up) * 1200f);
            StopCoroutine("Reload");
            m_Reload = 0;
            canMoveSkill = true;
            yield return new WaitForSeconds(2.2f);
            anim.SetBool("gun", true);
            holdSkill = false;
            canMoveSkill = false;
        }


        public IEnumerator NoiseAnimation()
        {
            if (holdSkill) { yield break; }
            Camera.main.fieldOfView = 60;
            anim.SetTrigger("end");
            anim.SetBool("gun", false);
            anim.SetInteger("move", 0);
            StopCoroutine("Reload");
            m_Reload = 0;
            yield return null;
            anim.SetTrigger("noise");
            anim.speed = 0.88f;
            canMoveSkill = true;
            yield return new WaitForSeconds(4.0f);
            anim.SetTrigger("end");
            anim.SetBool("gun", true);
            canMoveSkill = false;
        }
        #endregion

        // 포톤 추가
        // 네트워크 객체 생성 완료시 자동 호출되는 함수
        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            //info.sender.TagObject = this.GameObject;
            // 네트워크 플레이어 생성시 전달 인자 확인
            /*object[] data = pv.instantiationData;
            Debug.Log((int)data[0]);*/
        }
        //포톤 추가/////////////////////////////////////////////////////////
        /*
         * 게임을 실행하여 자신의 아바타를 이동시키고 있는 상태에서
         * 빌드한 후 실행한 게임 화면으로 보면 아바타 움직임이 끊기는 현상이
         * 나타남.  이유는 PhotonView 컴포넌트의 데이터 전송주기에 맞춰
         * 짧은 거리이지만 순간 이동하기 때문...
         * 이러한 현상을 보정하기 위해 포톤 클라우드도 유니티 빌트인 네트워크의
         * OnSerializeNetworkView 와 동일한 기능을 하는 OnPhotonSerializeView 콜백 함수를 제공!!!
         * 
         * OnPhotonSerializeView 콜백 함수의 호출 간격은 PhotonNetwork.sendRateOnSerialize 속성으로 설정 및 조회 
         * Sendrate 는 초당 패킷 전송 횟수로 기본값은 초당 10회로 설정돼 있다. 게임의 장르 또는 스피드를 고려해 
         * Sendrate 를 설정해야 하며, 네트워크 대역폭(Network Bandwidh)을 고려해 신중히 결정하자
         * 
         * // Debug.Log( PhotonNetwork.sendRateOnSerialize );
         * 
         */


        /*
         * PhotonView 컴포넌트의 Observe 속성이 스크립트 컴포넌트로 지정되면 PhotonView
         * 컴포넌트는 데이터를 송수신할 때, 해당 스크립트의 OnPhotonSerializeView 콜백 함수를 호출한다. 
         */

        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // 로컬 플레이어의 위치 정보 송신
            if (stream.isWriting)
            {
                // 박싱
                stream.SendNext(tr.position);
                stream.SendNext(tr.rotation);
            }
            // 원격 플레이어의 위치 정보 수신
            else
            {
                // 언박싱
                currPos = (Vector3)stream.ReceiveNext();
                currRot = (Quaternion)stream.ReceiveNext();
            }
        }

        public void OnEndEditChat()
        {
            if(csItemManager.Scene.ChatText.text != "")
            {
                // 로그 메시지에 출력할 문자열 생성
                string msg = "\n\t<color=#ff0000>[" + PhotonNetwork.player.NickName + "] </color>" + csItemManager.Scene.ChatText.text;
                //string msg = "test";
                csItemManager.Scene.ChatText.text = "";

                //RPC 함수 호출
                pv.RPC("ChatMsg", PhotonTargets.AllBuffered, msg);
            }
        }

        // 포톤 추가
        [PunRPC]
        void ChatMsg(string msg)
        {
            //로그 메시지 Text UI에 텍스트를 누적시켜 표시
            csItemManager.Scene.LogText.text = csItemManager.Scene.LogText.text + msg;
        }
    }
}