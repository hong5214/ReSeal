using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : csSceneManager<StageManager>
{
    public float playTime; // 플레이 시간
    public int monsterMax; // 최대 몬스터 수
    
    public Text StageText;
    public Text TimeText;
    public Text MissonText;
    
    public int itemActive;
    public bool itemCheck;
    public Transform StartingPannel;// 시작할 때 판넬
    public Transform EndingPannel;// 이길 때 판넬
    public Transform DiePannel; // 질 때 판넬

    public Image HpBar;
    public Image MpBar;
    public Image SpBar;

    public Slot slot1;//인벤토리 상의 물약슬롯1
    public Slot slot2;//인벤토리 상의 물약슬롯2
    public Slot slot3;//인벤토리 상의 메인무기슬롯
    public Slot slot4;//인벤토리 상의 서브무기슬롯



    public void Awake()
    {
        //Instantiate(Resources.Load("Player"), new Vector3(0,0,0), Quaternion.identity);
        //GameManager.GManager.PhotonPlayer = PhotonNetwork.Instantiate("Player", new Vector3(0, 20.0f, 0), Quaternion.identity, 0);
        GameManager.GManager.PhotonPlayer.transform.position = GameObject.Find("Stage").transform.Find("playerSpawn").position;
        PhotonNetwork.isMessageQueueRunning = true;
        Characters.PlayerCtrl.Scene.miniMap.gameObject.SetActive(true);
        SkillManager.Scene.CoolTimeBox.gameObject.SetActive(true);        
        StageText = GameObject.FindGameObjectWithTag("UI").transform.Find("UpUI").transform.Find("StageTxt").GetComponent<Text>();
        TimeText = GameObject.FindGameObjectWithTag("UI").transform.Find("UpUI").transform.Find("TimeTxt").GetComponent<Text>();
        MissonText = GameObject.FindGameObjectWithTag("UI").transform.Find("UpUI").transform.Find("MissionTxt").GetComponent<Text>();        
        StartingPannel = transform.Find("Ui_Battle").Find("StartingPannel");
        EndingPannel = transform.Find("Ui_Battle").Find("EndingPannel");
        DiePannel = transform.Find("Ui_Battle").Find("DiePannel");

        HpBar=transform.Find("Ui_Battle").transform.Find("StatusUI").transform.Find("HpBar").GetComponent<Image>();
        MpBar = transform.Find("Ui_Battle").transform.Find("StatusUI").transform.Find("MpBar").GetComponent<Image>();
        SpBar = transform.Find("Ui_Battle").transform.Find("StatusUI").transform.Find("SpBar").GetComponent<Image>();
        /*//물약 슬롯1에 접근
        slot1 = csItemManager.Scene.transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("Portion1").GetComponentInChildren<Slot>();
        //물약 슬롯2에 접근
        slot2 = csItemManager.Scene.transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("Portion2").GetComponentInChildren<Slot>();*/
        //메인무기 슬롯에 접근
        slot3 = csItemManager.Scene.transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("MainWeapon").GetComponentInChildren<Slot>();
        //서브무기 슬롯에 접근
        slot4 = csItemManager.Scene.transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("SubWeapon").GetComponentInChildren<Slot>();
        ResetStage();
    }

    private void Start()
    {
        Characters.PlayerCtrl.Scene.StopMove();
        StartingPannel.gameObject.SetActive(true);
        //물약 슬롯1에 접근
        slot1 = csItemManager.Scene.transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("Portion1").GetComponentInChildren<Slot>();
        //물약 슬롯2에 접근
        slot2 = csItemManager.Scene.transform.Find("PlayerUI").transform.Find("InventoryUI").transform.Find("03.Eqipment").transform.Find("Portion2").GetComponentInChildren<Slot>();
        Invoke("StartGame", 2f);
    }

    void StartGame()
    {
        Characters.PlayerCtrl.Scene.StartMove();
        StartingPannel.gameObject.SetActive(false);
    }

    void Update()
    {        
        if(!StartingPannel.gameObject.activeSelf && !EndingPannel.gameObject.activeSelf) // 시작 UI, 끝나는 화면 비활성화일 때만 시간 조절
        {
            if (playTime > 0)
            {
                playTime -= Time.deltaTime; // 로딩 시간 만큼은 차감
                TimeText.text = ((int)playTime / 60).ToString("00") + ":" + ((int)playTime % 60).ToString("00");
            }
            else TimeText.text = "시간종료";
        }
    }
    void ResetStage()
    {
        playTime = 180.0f; // 제한시간 3분
        // 데이터를 쓸 때 GameManager의 Player(프리팹) 사용
        Characters.PlayerCtrl.Scene.Hp = Characters.PlayerCtrl.Scene.MaxHp;
        Characters.PlayerCtrl.Scene.Life = true;
        Characters.PlayerCtrl.Scene.weaponNum = 0;
        Characters.PlayerCtrl.Scene.bullet = 0;
        Characters.PlayerCtrl.Scene.bulletMax = 5;
        Characters.PlayerCtrl.Scene.bulletTotal = 20;
        Characters.PlayerCtrl.Scene.playerSector = Sector.One;
        StageText.text = "Stage" + SceneManager.GetActiveScene().name.Substring(10);
        TimeText.text = "03:00";
        if(GameManager.GManager.StageNum == 1)
        {
            monsterMax = GameObject.FindGameObjectsWithTag("Enemy").Length;
            MissonText.text = "모든 적 처치!("+ monsterMax + "/"+ monsterMax + ")";
        }
        else if (GameManager.GManager.StageNum == 2)
            MissonText.text = "제한 시간 내에 탈출!";
        else if (GameManager.GManager.StageNum == 3)
            MissonText.text = "보스 퇴치!";
        csItemManager.Scene.BulletText.text = Characters.PlayerCtrl.Scene.bullet + " / " + Characters.PlayerCtrl.Scene.bulletTotal;
        //Battle_Item.gameObject.SetActive(false);
        csItemManager.Scene.ImgSlot1.gameObject.SetActive(false);
        csItemManager.Scene.ImgSlot2.gameObject.SetActive(false);
        itemActive = 0;        
    }

    public void ChangeSlot(int num)
    {
        //StageManager.Scene.Battle_Item.gameObject.SetActive(true);
        csItemManager.Scene.ImgSlot1.gameObject.SetActive(true);
        csItemManager.Scene.ImgSlot2.gameObject.SetActive(true);

        if (num == 1 || num == 2)
        {
            // 장비 장착
            if ((int)Characters.PlayerCtrl.Scene.weaponType == num) return; // 플레이어 무기 그대로면 교체 안함
            Characters.PlayerCtrl.Scene.weaponType = (Characters.PlayerCtrl.WeaponType)num;
            // 1번 일때 메인 무기 슬롯 아이템 종류
            // 2번 일때 서브 무기 슬롯 아이템 종류                        
            if(csItemManager.Scene.inven.SetSlots[num + 1].item == null)
            {
                Characters.PlayerCtrl.Scene.weaponNum = 0;
                Characters.PlayerCtrl.Scene.bullet = 0;
                Characters.PlayerCtrl.Scene.bulletMax = 5;
                Characters.PlayerCtrl.Scene.bulletTotal = 20;
                csItemManager.Scene.BulletText.text = Characters.PlayerCtrl.Scene.bullet + " / " + Characters.PlayerCtrl.Scene.bulletTotal;
            }
            else
            {                                
                Characters.PlayerCtrl.Scene.weaponNum = csItemManager.Scene.inven.SetSlots[num + 1].slot_info.Index / 4 + 1;
                Characters.PlayerCtrl.Scene.bullet = csItemManager.Scene.inven.SetSlots[num + 1].item.bullet;
                Characters.PlayerCtrl.Scene.bulletMax = csItemManager.Scene.inven.SetSlots[num + 1].item.bulletMax;
                Characters.PlayerCtrl.Scene.bulletTotal = csItemManager.Scene.inven.SetSlots[num + 1].item.bulletTotal;
                csItemManager.Scene.BulletText.text = Characters.PlayerCtrl.Scene.bullet + " / " + Characters.PlayerCtrl.Scene.bulletTotal;
            }
            Characters.PlayerCtrl.Scene.weaponChange = true; // 플레이어 무기교체
            csItemManager.Scene.ImgSlot1.enabled = (num == 1);
            csItemManager.Scene.ImgSlot2.enabled = (num == 2);

            csItemManager.Scene.SetItemColor(1.0f);
            if (itemActive == 1) { StartCoroutine(ItemActive()); }
        }
        else
        {
            // 소비 아이템 퀵슬롯 사용
            Slot useSlot = (num == 3) ? slot1 : slot2;

            if (useSlot.item != null)
                useSlot.UsePortion();
        }        
    }
    public IEnumerator ItemActive()
    {
        itemCheck = true;
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 10; i++)
        {
            if (itemActive == 2)
            {
                csItemManager.Scene.SetItemColor(1.0f);
                itemActive = 1;
                StartCoroutine(ItemActive());
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
            csItemManager.Scene.SetItemColor(1.0f - (i + 1) / 10.0f);
        }

        itemActive = 0;
        itemCheck = false;
        //StageManager.Scene.Battle_Item.gameObject.SetActive(false);
        csItemManager.Scene.ImgSlot1.gameObject.SetActive(false);
        csItemManager.Scene.ImgSlot2.gameObject.SetActive(false);
    }

    public IEnumerator Result()
    {
        if (Characters.PlayerCtrl.Scene.pv.isMine)
        {
            // 다음 스테이지가 있으면 이걸로 다음 스테이지 이동
            if (GameManager.GManager.StageNum > 1 && Characters.PlayerCtrl.Scene.Life)
            {
                EndingPannel.gameObject.SetActive(true);
                // 총알 초기화
                for (int i = 0; i < 4; i++)
                {
                    csItemManager.Scene.ItemBase[i].bullet = csItemManager.Scene.ItemBase[i].bulletMax;
                    csItemManager.Scene.ItemBase[i].bulletTotal = csItemManager.Scene.ItemBase[i].bulletMax * 3;
                }
                Invoke("NextStage", 3.0f);
            }
            else
            {
                DiePannel.gameObject.SetActive(true);
                yield return new WaitForSeconds(3.0f);
                DiePannel.gameObject.SetActive(false);
                Characters.PlayerCtrl.Scene.Awake();
                Characters.PlayerCtrl.Scene.Start();
                Characters.PlayerCtrl.Scene.StartMove();
                Characters.PlayerCtrl.Scene.SetUp.gameObject.SetActive(false);
                Characters.PlayerCtrl.Scene.Inventory.gameObject.SetActive(false);
                Characters.PlayerCtrl.Scene.Skill.gameObject.SetActive(false);
                SkillManager.Scene.CoolTimeBox.gameObject.SetActive(false);
                GameManager.GManager.StageNum = 1;
                GameManager.GManager.NextScene(1);
            }
        }
        yield return null;
    }
    void NextStage()
    {
        EndingPannel.gameObject.SetActive(false);
        if (GameManager.GManager.StageNum < 4 && GameManager.GManager.nextScene != 3) // 다음 단계로
            GameManager.GManager.NextScene(GameManager.GManager.nextScene, GameManager.GManager.StageNum);
        else
        {
            if (GameManager.GManager.StageNum == 2)
            {
                Vector3 pos = GameObject.Find("Stage").transform.Find("playerSpawnThree").position;
                pos.z -= 20.0f;
                GameObject.Find("Stage").transform.Find("playerSpawn").position = pos;
                Characters.PlayerCtrl.Scene.transform.position = pos;                
            }
            else if (GameManager.GManager.StageNum == 3)
            {
                Characters.PlayerCtrl.Scene.transform.position = new Vector3(63.1f, 5, 80);
                SoundManager.Sound.StagePlay(5);
            }
            else if (GameManager.GManager.StageNum == 4)
            {
                Characters.PlayerCtrl.Scene.Awake();
                Characters.PlayerCtrl.Scene.Start();
                Characters.PlayerCtrl.Scene.StartMove();
                Characters.PlayerCtrl.Scene.SetUp.gameObject.SetActive(false);
                Characters.PlayerCtrl.Scene.Inventory.gameObject.SetActive(false);
                Characters.PlayerCtrl.Scene.Skill.gameObject.SetActive(false);
                SkillManager.Scene.CoolTimeBox.gameObject.SetActive(false);
                GameManager.GManager.StageNum = 1;
                if (GameManager.GManager.clearProcess < GameManager.GManager.nextScene)
                {
                    GameManager.GManager.clearProcess++;
                }
                GameManager.GManager.NextScene(1);
                return;
            }
            Characters.PlayerCtrl.Scene.StopMove();
            StartingPannel.gameObject.SetActive(true);
            Invoke("StartGame", 2f);
            StageText.text = "Stage2_" + GameManager.GManager.StageNum;
            TimeText.text = "03:00";
            playTime = 180.0f;
        }
    }
}
