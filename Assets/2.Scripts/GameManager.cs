using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using UnityEngine.SceneManagement;
using SimpleJSON;
using System.Runtime.InteropServices; // 마샬링관련

public class GameManager : MonoBehaviour
{
    private static PlayerCtrl player = null;
    // 플레이어의 정보
    // Player를 통해 PlayerCtrl의 변수, 함수들을 가져올 수 있다.
    public static PlayerCtrl Player
    {
        get
        {
            if (player == null)
            {
                player = Resources.Load<PlayerCtrl>("Player");
            }

            return player;
        }
        set
        {
            player = value;
        }
    }
    private static GameManager gameManager = null;
    // GManager 통해 GameManager 변수, 함수들을 가져올 수 있다.
    public static GameManager GManager
    {
        get
        {
            if (gameManager == null)
            {
                // 해당 스크립트를 가지고 있는 오브젝트가 있는지 확인
                gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
                gameManager.clearProcess = 1;

                // 없으면 새로 만든다.
                if (gameManager == null)
                {
                    gameManager = new GameObject("Singleton_" + typeof(GameManager).ToString(), typeof(GameManager)).GetComponent<GameManager>();                    
                }
            }
            return gameManager;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)] // 한글 됨
    public struct SecretData
    {
        //문자열 256 한글용
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public System.String secretCode; // 보안 코드
        public int photonNum; // 포톤 서버 번호
    }
    public SecretData secretData = new SecretData();

    [DllImport("ResealDLL_CPP")]
    public static extern void GetSecretCode(ref SecretData Data);

    [DllImport("ResealDLL_CPP")]
    public static extern int SetPhotonServer(ref SecretData Data, int num);

    public void Awake()
    {
        // 보안 코드 가져오기
        GetSecretCode(ref secretData);
        SecretCode = secretData.secretCode;
        PhotonNum = SetPhotonServer(ref secretData, 1);

        List<Dictionary<string, object>> data = ExCelParsing.Read("Item");
        itemData = new string[5, data.Count];
        for (var i = 0; i < data.Count; i++)
        {
            itemData[0, i] = data[i]["Name"].ToString();
            itemData[1, i] = data[i]["Index"].ToString();
            itemData[2, i] = data[i]["Level"].ToString();
            itemData[3, i] = data[i]["Option"].ToString();
            itemData[4, i] = data[i]["Gold"].ToString();
        }
        data = ExCelParsing.Read("Monster");
        monsterData = new string[6, data.Count];
        for (var i = 0; i < data.Count; i++)
        {
            monsterData[0, i] = data[i]["Name"].ToString();
            monsterData[1, i] = data[i]["Level"].ToString();
            monsterData[2, i] = data[i]["Hp"].ToString();
            monsterData[3, i] = data[i]["Atk"].ToString();
            monsterData[4, i] = data[i]["Def"].ToString();
            monsterData[5, i] = data[i]["Sp"].ToString();
        }
        data = ExCelParsing.Read("Skill");
        skillData = new string[4, data.Count];
        for (var i = 0; i < data.Count; i++)
        {
            skillData[0, i] = data[i]["Name"].ToString();
            skillData[1, i] = data[i]["CoolTime"].ToString();
            skillData[2, i] = data[i]["Value"].ToString();
            skillData[3, i] = data[i]["DESC"].ToString();
        }
        data = ExCelParsing.Read("Setting");
        settingData = new string[4];
        settingData[0] = data[0]["mouse"].ToString();
        settingData[1] = data[0]["soundBG"].ToString();
        settingData[2] = data[0]["soundEF"].ToString();
        settingData[3] = data[0]["mapSize"].ToString();
    }
    // json, string 등 문자열에 특수문자 제거해주기
    public static string JtoS(string type, JSONNode json = null, int index = -1)
    {
        char[] removeChar = { ' ', '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '\\', '|', '\'', '"', '/', '?', '.', ',', '<', '>', ':', ';', '[', '{', ']', '}' };
        if (index == -1) return string.Join("", type.ToString().Split(removeChar));
        return string.Join("", json[index][type].ToString().Split(removeChar));
    }

    public static string[] JtoSItem(string type, JSONNode json = null, int index = -1)
    {
        char[] removeChar = { ' ', '`', '~', '!', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '\\', '|', '\'', '"', '/', '?', '.', ',', '<', '>', ':', ';', '[', '{', ']', '}' };
        return string.Join("", json[index][type].ToString().Split(removeChar)).Split('@');
    }


    // 게임의 정보
    public bool TestMode;
    public int clearProcess;
    public int PhotonNum;
    [System.NonSerialized]
    public string idx = "";
    [System.NonSerialized]
    public string num = "";
    [PunRPC]
    [System.NonSerialized]
    public string avatar = "";
    [System.NonSerialized]
    //public string URL = "http://localhost:8080/ReSeal_";
    public string URL = "http://reseal.ddns.net:8080/ReSeal_";
    [System.NonSerialized]
    public string SecretCode = "";
    [System.NonSerialized]
    public int nextScene = 1;
    [System.NonSerialized]
    public int StageNum = 1;
    [System.NonSerialized]
    public string[] settingData;
    // 0번째 배열 : 마우스 감도
    // 1번째 배열 : 배경음악 볼륨
    // 2번째 배열 : 효과음악 볼륨
    // 3번째 배열 : 미니맵 사이즈
    [System.NonSerialized]
    public string[,] itemData;
    // 0번째 배열 : 아이템 이름
    // 1번째 배열 : 아이템 번호
    // 2번째 배열 : 아이템 등급
    // 3번째 배열 : 아이템 고정옵션, 개수
    // 4번째 배열 : 아이템 판매금액
    [System.NonSerialized]
    public string[,] monsterData;
    // 0번째 배열 : 몬스터 이름
    // 1번째 배열 : 몬스터 레벨
    // 2번째 배열 : 몬스터 체력
    // 3번째 배열 : 몬스터 공격력
    // 4번째 배열 : 몬스터 방어력
    // 5번째 배열 : 몬스터 스피드
    // 선택한 플레이어 데이터 가져오기
    [System.NonSerialized]
    public string[,] skillData;
    // 0번째 배열 : 스킬 이름
    // 1번째 배열 : 쿨타임
    // 2번째 배열 : 스킬 수치
    // 3번째 배열 : 스킬 설명
    
    public StageManager stageManager;

    public GameObject PhotonPlayer;    

    public void LoadPlayerData()
    {        
        Reset();
    }

    public void NextScene(int sceneNum, int stageNum = 1)
    {
        GameManager.GManager.nextScene = sceneNum;
        GameManager.GManager.StageNum = stageNum;
        SceneManager.LoadScene("04.scLoading");
    }

    // 플레이어 데이터 초기화 후 가져오기
    private void Reset()
    {
        Player.Reset();
        StartCoroutine(GetPlayerList());
    }
    // 플레이어 데이터 가져오기
    private IEnumerator GetPlayerList()
    {
        WWWForm form = new WWWForm();
        // json의 로그인 정보 가져오기
        form.AddField("SecretCode", SecretCode); //post방식 전달.

        var www = new WWW(URL + "playerFind.json.php", form);
        yield return www; //웹의 다운로드 완료시까지 대기.

        // 에러가 없으면
        if (string.IsNullOrEmpty(www.error))
        {
            FindPlayer(www.text); //파싱후 출력.            
        }
        else
        {
            Debug.Log("Error : " + www.error);
        }
    }
    // DB에 선택한 플레이어 데이터를 찾아서 PlayerCtrl 각 변수에 저장
    private void FindPlayer(string strJsonData)
    {
        // 받은 데이터를 json 형식으로 변환
        var jSon = JSON.Parse(strJsonData);

        if (jSon != null)
        {
            for (int i = 0; i < jSon.Count; i++)
            {
                string getName = string.Join("", jSon[i]["name"].ToString().Split('"', '/'));

                // 접속한 플레이어 정보만 확인하기
                if (getName == Player.playerName)
                {
                    GameManager.GManager.avatar = JtoS("avatar", jSon, i);
                    Player.Level = int.Parse(JtoS("level", jSon, i));
                    Player.Exp = int.Parse(JtoS("exp", jSon, i));
                    Player.Hp = Player.MaxHp = float.Parse(JtoS("hp", jSon, i));
                    Player.Atk = float.Parse(JtoS("atk", jSon, i));
                    Player.Def = float.Parse(JtoS("def", jSon, i));
                    Player.Gold = int.Parse(JtoS("gold", jSon, i));
                    Player.AtkSpeed = float.Parse(JtoS("atkspd", jSon, i));
                    Player.MoveSpeed = float.Parse(JtoS("movespd", jSon, i));
                    Player.Critical = float.Parse(JtoS("cri", jSon, i));
                    Player.CriticalMax = float.Parse(JtoS("criMax", jSon, i));
                    Player.Drain = float.Parse(JtoS("drain", jSon, i));
                    for (int k = 1; k <= 10; k++)
                        Player.transform.Find("ItemManager").GetComponent<SkillManager>().levelSkill[k-1] = int.Parse(JtoS("skill" + k, jSon, i));
                    num = string.Join("", jSon[i]["num"].ToString().Split('"', '/'));
                    break;
                }
            }
        }
        Debug.LogWarning("서버 : " + PhotonNetwork.connected);
        Debug.LogWarning("방 : " + PhotonNetwork.inRoom);
        PhotonNetwork.JoinLobby(); // 방 들어가기
    }
    void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.playerName + " 플레이어 입장");
        // 플레이어 이름 저장
        PhotonNetwork.player.NickName = GameManager.player.playerName;
        PhotonNetwork.playerName = GameManager.player.playerName;
        // 랜덤으로 방에 입장한다.
        PhotonNetwork.JoinRandomRoom();
    }
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("방 없음");

        // 방이 없으면 새로 만들어준다.
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 50;
        PhotonNetwork.CreateRoom("CH1", roomOptions, TypedLobby.Default);

        Debug.Log("[정보] 게임 방 생성 완료");
    }
    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        //오류 코드( ErrorCode Class )
        Debug.Log(codeAndMsg[0].ToString());
        //오류 메시지
        Debug.Log(codeAndMsg[1].ToString());
        // 여기선 디버그로 표현했지만 릴리즈 버전에선 사용자에게 메시지 전달
        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
    }
    void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");
        PhotonNetwork.isMessageQueueRunning = false;
        //NextScene(nextScene);
        NextScene(1);
    }
    // 플레이어 데이터 저장
    public IEnumerator SetPlayerData()
    {
        // 현재 씬에 있는 플레이어 정보 프리팹으로 저장
        Player.Level = PlayerCtrl.Scene.Level;
        Player.Exp = PlayerCtrl.Scene.Exp;
        Player.Gold = PlayerCtrl.Scene.Gold;
        Player.MaxHp = PlayerCtrl.Scene.MaxHp;
        Player.Atk = PlayerCtrl.Scene.Atk;
        Player.Def = PlayerCtrl.Scene.Def;
        Player.AtkSpeed = PlayerCtrl.Scene.AtkSpeed;
        Player.MoveSpeed = PlayerCtrl.Scene.MoveSpeed;
        Player.Critical = PlayerCtrl.Scene.Critical;
        Player.CriticalMax = PlayerCtrl.Scene.CriticalMax;
        Player.Drain = PlayerCtrl.Scene.Drain;

        if (csItemManager.Scene.myLevel.text != Player.Level.ToString())
        {
            csItemManager.Scene.UpdateData();
            SkillManager.Scene.SkillPointCheck();
        }



        if (GameManager.GManager.TestMode)
        {
            Debug.Log("*테스트 모드* 테스트 캐릭터만 사용 가능");
            yield break;
        }
        WWWForm form = new WWWForm();
        // DB로 저장
        form.AddField("SecretCode", SecretCode); //post방식 전달.
        form.AddField("name", Player.playerName);
        form.AddField("level", Player.Level);
        form.AddField("exp", Player.Exp);
        form.AddField("gold", Player.Gold);
        form.AddField("hp", Player.MaxHp.ToString());
        form.AddField("atk", Player.Atk.ToString());
        form.AddField("def", Player.Def.ToString());
        form.AddField("atkspd", Player.AtkSpeed.ToString());
        form.AddField("movespd", Player.MoveSpeed.ToString());
        form.AddField("cri", Player.Critical.ToString());
        form.AddField("criMax", Player.CriticalMax.ToString());
        form.AddField("drain", Player.Drain.ToString());

        var www = new WWW(URL + "playerSave.json.php", form);
        yield return www; //웹의 다운로드 완료시까지 대기.

        // 에러가 없으면
        if (string.IsNullOrEmpty(www.error))
        {
            //Debug.Log("데이터 저장 완료");
        }
        else
        {
            Debug.Log("Error : " + www.error);
        }
    }

    public IEnumerator SetSkillData(int skillNum)
    {
        // 현재 씬에 있는 플레이어 정보 프리팹으로 저장
        Player.transform.Find("ItemManager").GetComponent<SkillManager>().levelSkill[skillNum] = PlayerCtrl.Scene.transform.Find("ItemManager").GetComponent<SkillManager>().levelSkill[skillNum];
        
        if (GameManager.GManager.TestMode)
        {
            Debug.Log("*테스트 모드* 테스트 캐릭터만 사용 가능");
            yield break;
        }
        WWWForm form = new WWWForm();
        // DB로 저장
        form.AddField("SecretCode", SecretCode); //post방식 전달.
        form.AddField("name", Player.playerName);
        form.AddField("SkillNum", skillNum+1);
        form.AddField("SkillLevel", Player.transform.Find("ItemManager").GetComponent<SkillManager>().levelSkill[skillNum]);

        var www = new WWW(URL + "skillSave.json.php", form);
        yield return www; //웹의 다운로드 완료시까지 대기.

        // 에러가 없으면
        if (string.IsNullOrEmpty(www.error))
        {
            //Debug.Log("데이터 저장 완료");
        }
        else
        {
            Debug.Log("Error : " + www.error);
        }
    }

    // 선택한 플레이어 데이터 삭제
    public IEnumerator DeletePlayerData(GameObject delObj)
    {
        WWWForm form = new WWWForm();
        // json의 로그인 정보 가져오기
        form.AddField("SecretCode", SecretCode); //post방식 전달.
        form.AddField("idx", idx);
        form.AddField("num", num);
        form.AddField("name", Player.playerName);

        var www = new WWW(URL + "playerDelete.json.php", form);
        yield return www; //웹의 다운로드 완료시까지 대기.
        // 에러가 없으면
        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("데이터 삭제 완료");
            Destroy(delObj); // SelectSlot 삭제
            csMakeManager.Scene.ReFlesh();
        }
        else
        {
            Debug.Log("Error : " + www.error);
        }
    }

    public void StartSaving(SendType type, int _num, ItemInfo _info, int _itemCount)//slot으로부터 정보를 받아 데이터 저장을 시작한다.
    {
        StartCoroutine(GameManager.GManager.SetInventoryData(type, _num, _info, _itemCount));//변동된 데이터와 위치한 슬롯의 번호를 보낸다

    }
    public void StartErase(SendType sendType, int _slotNum, int _clearIndex)
    {
        StartCoroutine(GameManager.GManager.ClearData(sendType, _slotNum, _clearIndex));//변동된 데이터와 위치한 슬롯의 번호를 보낸다
    }


    public IEnumerator SetInventoryData(SendType type, int _slotNum, ItemInfo _info, int _itemCount)//인벤토리 혹은 장비창의 데이터를 데이터베이스로 보내준다
    {
        string slotType = "";
        string myItem = "";

        if (_info.Index < 40)//장비라면
        {
            slotType = (type == SendType.Equip) ? "setitem" + _slotNum : "equitem" + _slotNum;//보낼 곳(장비창.인벤창)을 지정
            if (type == SendType.Equip)
            {
                if (_slotNum + 1 == 1) // 소비 슬롯1 변경
                    csItemManager.Scene.ImgSlot3.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = csItemManager.Scene.inven.SetSlots[_slotNum].item.itemImage;
                else if (_slotNum + 1 == 2) // 소비 슬롯2 변경
                    csItemManager.Scene.ImgSlot4.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = csItemManager.Scene.inven.SetSlots[_slotNum].item.itemImage;
                else if (_slotNum + 1 == 3) // 메인 무기 변경
                {
                    csItemManager.Scene.ImgSlot1.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = csItemManager.Scene.inven.SetSlots[_slotNum].item.itemImage;
                    PlayerCtrl.Scene.weaponNum = (csItemManager.Scene.inven.SetSlots[_slotNum].slot_info == null) ? 0 : csItemManager.Scene.inven.SetSlots[_slotNum].slot_info.Index / 4 + 1;
                    if (PlayerCtrl.Scene.weaponType == PlayerCtrl.WeaponType.Main)
                        PlayerCtrl.Scene.weaponChange = true; // 플레이어 무기교체
                }
                else if (_slotNum + 1 == 4) // 서브 무기 변경
                {
                    csItemManager.Scene.ImgSlot2.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = csItemManager.Scene.inven.SetSlots[_slotNum].item.itemImage;
                    PlayerCtrl.Scene.weaponNum = (csItemManager.Scene.inven.SetSlots[_slotNum].slot_info == null) ? 0 : csItemManager.Scene.inven.SetSlots[_slotNum].slot_info.Index / 4 + 1;
                    if (PlayerCtrl.Scene.weaponType == PlayerCtrl.WeaponType.Sub)
                        PlayerCtrl.Scene.weaponChange = true; // 플레이어 무기교체
                }
                
            }
            myItem = _info.Index.ToString() + "@" + "10" +  //저장이름
       "@" + _info.ItemStat[0, 0].ToString() + "#" + _info.ItemStat[0, 1].ToString() +//첫번째 스텟
       "@" + _info.ItemStat[1, 0].ToString() + "#" + _info.ItemStat[1, 1].ToString() +//두번째 스텟
       "@" + _info.ItemStat[2, 0].ToString() + "#" + _info.ItemStat[2, 1].ToString() +//세번째 스텟
       "@" + _info.ItemStat[3, 0].ToString() + "#" + _info.ItemStat[3, 1].ToString();//네번째 스텟
        }

        else if (_info.Index < 44)//소비라면
        {
            slotType = (type == SendType.Equip) ? "setitem" + _slotNum : "useitem" + _slotNum;//보낼 곳(장비창.인벤창)을 지정
            if (type == SendType.Equip)
            {
                if (_slotNum + 1 == 1) // 물약 1 슬롯 변경
                {
                    csItemManager.Scene.ImgSlot3.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = csItemManager.Scene.inven.SetSlots[_slotNum].item.itemImage;
                }
                else if (_slotNum + 1 == 2) // 물약 2 슬롯 변경
                {
                    csItemManager.Scene.ImgSlot4.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = csItemManager.Scene.inven.SetSlots[_slotNum].item.itemImage;
                }
            }
            myItem = _info.Index.ToString() + "@" + _itemCount.ToString();//개수표시
        }
        else//기타라면
        {
            slotType = "matitem" + _slotNum;
            myItem = _info.Index.ToString() + "@" + _itemCount.ToString();//개수표시
        }
        if (!TestMode)
        {

            WWWForm form = new WWWForm();
            form.AddField("SecretCode", SecretCode); //post방식 전달.
            form.AddField("idx", idx);
            form.AddField("num", num);
            form.AddField("index", slotType);
            form.AddField("value", myItem);

            var www = new WWW(URL + "itemSave.json.php", form);
            yield return www; //웹의 다운로드 완료시까지 대기.

            // 에러가 없으면
            if (string.IsNullOrEmpty(www.error))
            {
                //Debug.Log(JSON.Parse(www.text)["returnMsg"].ToString());
            }
            else
            {
                //Debug.Log("Error : " + www.error);
            }
        }
    }


    public IEnumerator ClearData(SendType sendType, int _slotNum, int clearIndex)//테이터를 초기화 해준다 데이터베이스로 보내준다
    {
        string slotType = "";
        string myItem = "";

        if (clearIndex < 40)//장비라면
        {
            slotType = (sendType == SendType.Equip) ? "setitem" + _slotNum : "equitem" + _slotNum;//보낼 곳(장비창.인벤창)을 지정
            if (_slotNum + 1 == 3) // 메인 무기 변경
            {
                csItemManager.Scene.ImgSlot1.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Non");
                PlayerCtrl.Scene.weaponNum = 0;
                if (PlayerCtrl.Scene.weaponType == PlayerCtrl.WeaponType.Main)
                    PlayerCtrl.Scene.weaponChange = true; // 플레이어 무기교체
            }
            else if (_slotNum + 1 == 4) // 서브 무기 변경
            {
                csItemManager.Scene.ImgSlot2.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Non");
                PlayerCtrl.Scene.weaponNum = 0;
                if (PlayerCtrl.Scene.weaponType == PlayerCtrl.WeaponType.Sub)
                    PlayerCtrl.Scene.weaponChange = true; // 플레이어 무기교체
            }

            myItem = "65@0@0";//비었다는 표시
        }

        else if (clearIndex < 44)//소비라면
        {
            slotType = (sendType == SendType.Equip) ? "setitem" + _slotNum : "useitem" + _slotNum;//보낼 곳(장비창.인벤창)을 지정
            if (_slotNum + 1 == 1) // 물약 1 슬롯 변경
            {
                csItemManager.Scene.ImgSlot3.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Non");
            }
            else if (_slotNum + 1 == 2) // 물약 2 슬롯 변경
            {
                csItemManager.Scene.ImgSlot4.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Non");
            }

            myItem = "65@0@0";//비었다는 표시
        }
        else//기타라면
        {
            slotType = "matitem" + _slotNum;
            myItem = "65@0@0";//비었다는 표시


        }
        if (!TestMode)
        {
            WWWForm form = new WWWForm();
            form.AddField("SecretCode", SecretCode); //post방식 전달.
            Debug.Log(SecretCode);
            form.AddField("idx", idx);
            Debug.Log(idx);
            form.AddField("num", num);
            Debug.Log(num);
            form.AddField("index", slotType);
            Debug.Log(slotType);
            form.AddField("value", myItem);
            Debug.Log(myItem);

            var www = new WWW(URL + "itemSave.json.php", form);
            yield return www; //웹의 다운로드 완료시까지 대기.

            // 에러가 없으면
            if (string.IsNullOrEmpty(www.error))
            {
                Debug.Log(JSON.Parse(www.text)["returnMsg"].ToString());
            }
            else
            {
                Debug.Log("Error : " + www.error);
            }
        }        
    }

    public void GetInvenDataStart()
    {
        if(!TestMode)
            StartCoroutine(GetInventoryData());
    }

    //받아올때
    public IEnumerator GetInventoryData()
    {

        WWWForm form = new WWWForm();
        // json의 로그인 정보 가져오기
        form.AddField("SecretCode", SecretCode); //post방식 전달.
        form.AddField("idx", idx); //post방식 전달.
        form.AddField("num", num); //post방식 전달.

        var www = new WWW(URL + "itemFind.json.php", form);
        yield return www; //웹의 다운로드 완료시까지 대기.


        // 에러가 없으면
        if (string.IsNullOrEmpty(www.error))
        {
            FindItem(www.text); //파싱후 출력.            
        }
        else
        {
            Debug.Log("Error : " + www.error);
        }

    }

    private void FindItem(string strJsonData)
    {
        // 받은 데이터를 json 형식으로 변환
        var jSon = JSON.Parse(strJsonData);

        if (jSon != null)
        {
          

            #region 장착창

            for (int i = 0; i <= 6; i++)
            {
                string[] setData = JtoSItem("setitem" + i, jSon, 0);//데이터 베이스로부터 정보를 받아온다.

                getItem(setData, i+1, true);

            }
            #endregion

            //장비창
            for (int i = 1; i <= 40; i++)
            {

                string[] equData = JtoSItem("equitem" + i, jSon, 0);//데이터 베이스로부터 정보를 받아온다.
                string[] useData =  JtoSItem("useitem" + i, jSon, 0);//데이터 베이스로부터 정보를 받아온다.
                string[] matData =  JtoSItem("matitem" + i, jSon, 0);//데이터 베이스로부터 정보를 받아온다.

                getItem(equData, i, false);
                getItem(useData, i, false);
                getItem(matData, i, false);
            }
        }
        //csItemManager.Scene.inven.transform.parent.gameObject.SetActive(false);

    }

    private void getItem(string[] itemData, int slotNum, bool equiped)
    {
        int equIndex = int.Parse(itemData[0]);//인덱스를 받아온다

        if (equIndex != 65)//만약 데이터가 있다면 넘기고 (65가 아니라면)
        {
           
            int index=-1;
            string Type="";

            if(equIndex<40)//장비라면
            {
                index = ((equIndex / 4) * 4);
                Type = "Equipment";                
            }
            else if(equIndex<44)//소비라면
            {
                index = ((equIndex % 4) + 1);
                Type = "Used";
            }
            else//기타라면
            {
                index = equIndex - 47;
                Type = "Etc";

            }
         
            ItemPickUp _itemPickUp = (Resources.Load<Transform>("Item/" + Type + "/Item" + index /*(index.ToString())*/)).GetComponent<ItemPickUp>();//리소시스 폴더에서 가져와준다
         
            Item _item = _itemPickUp.item;
            ItemInfo _info = new ItemInfo();
            _info.ItemStat = new int[4, 2];

            _info.ItemName = GameManager.GManager.itemData[0, equIndex];    //아이템마다 인덱스에 따라 이름 부터
            _info.Index = int.Parse(GameManager.GManager.itemData[1, equIndex]);         //아이템마다 인덱스 부여
            _info.ItemLevel = int.Parse(GameManager.GManager.itemData[2, equIndex]);     //레벨 부여
            _info.ItemOption = int.Parse(GameManager.GManager.itemData[3, equIndex]);   //옵션 여
            _info.ItemGold = int.Parse(GameManager.GManager.itemData[4, equIndex]);       //골드부여
            _info.ItemExp = _itemPickUp.info.ItemExp;//설명부여

            int _count = 1;

            if (equIndex >= 40)
            {
                _count = int.Parse(itemData[1]);
            }
            else
            {
                for (int j = 0; j < 4; j++)//장비라면 스텟부여
                {
                    _info.ItemStat[j, 0] = int.Parse(itemData[j + 2].Substring(0, 1));//스텟의 종류
                    _info.ItemStat[j, 1] = int.Parse(itemData[j + 2].Substring(2));//수치                    
                }                
            }

            if (equiped)//장착창이라면
            {                
                csItemManager.Scene.inven.SetSlots[slotNum - 1].AddItem(_item, _info, _count);//해당 인벤토리에 추가해준다.    
                                                                                              // 마을이 아닐 때(전투상태)
                if (slotNum == 1) // 소비 슬롯1 변경
                    csItemManager.Scene.ImgSlot3.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = _item.itemImage;
                else if (slotNum == 2) // 소비 슬롯2 변경
                    csItemManager.Scene.ImgSlot4.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = _item.itemImage;
                else if (slotNum == 3) // 메인 무기 변경
                    csItemManager.Scene.ImgSlot1.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = _item.itemImage;
                else if (slotNum == 4) // 서브 무기 변경
                    csItemManager.Scene.ImgSlot2.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = _item.itemImage;
            }
            else//인벤토리라면
            {
                if (equIndex < 40) csItemManager.Scene.inven.EquipSlots[slotNum - 1].AddItem(_item, _info, _count);//장비 인벤토리에 추가해준다.    
                else if (equIndex < 44) csItemManager.Scene.inven.UseSlots[slotNum - 1].AddItem(_item, _info, _count);//소비 인벤토리에 추가해준다.    
                else csItemManager.Scene.inven.EtcSlots[slotNum - 1].AddItem(_item, _info, _count);//기타 인벤토리에 추가해준다.    
            }
        }
    }

    public void InitStage()
    {
        Instantiate(stageManager).transform.SetParent(GameObject.Find("Stage").transform.root);
    }
}

