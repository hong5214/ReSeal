using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class csMakeManager : csSceneManager<csMakeManager>
{
    private Transform selectUI;
    private Transform makeUI;
    private Text makeName;
    private int makeAvatar = 0;
    public GameObject SelectSlot;
   
    private void Awake()
    {
        if (!PhotonNetwork.connected)
        {
            // 채널 1에 접속
            PhotonNetwork.ConnectUsingSettings("CH"+ GameManager.GManager.PhotonNum);
            // 로그 레벨 변경
            PhotonNetwork.logLevel = PhotonLogLevel.Full;
            // 플레이어 이름 지정
            PhotonNetwork.playerName = (GameManager.GManager.TestMode) ? "Player" + Random.Range(0, 1000) : "Player" + GameManager.GManager.idx;
        }
        else
        {
            PhotonNetwork.isMessageQueueRunning = true;
        }
        selectUI = GameObject.Find("MakeAcCanvas").transform.Find("BackGround").transform.Find("01.Select").Find("AccountWindow");
        makeUI = GameObject.Find("MakeAcCanvas").transform.Find("BackGround").Find("02.Make");
        if (GameManager.GManager.TestMode)
        {
            Debug.Log("*테스트 모드* 테스트 캐릭터만 사용 가능");
            for (int i = 0; i < 3; i++)
            {
                GameManager.Player.playerName = "Test0" + i;
                GameManager.GManager.num = (i + 1).ToString();
                // 플레이어 데이터 불러오기
                SelectSlot.gameObject.GetComponent<csSelectSlot>().selectNum = int.Parse(GameManager.GManager.num);
                GameObject addPlayer = Instantiate(SelectSlot);
                addPlayer.transform.Find("Name").GetComponent<Text>().text = GameManager.Player.playerName;
                addPlayer.transform.Find("Avatar").GetComponent<RawImage>().texture = Resources.Load<Texture>("myCharacter" + i);
                addPlayer.transform.Find("Level").GetComponent<Text>().text = "Lv.01";
                addPlayer.transform.SetParent(selectUI);
            }
            
            return;
        }
        StartCoroutine(GetPlayerList());
    }

    public void ReFlesh()
    {
        //GameManager.GManager.num = (selectUI.childCount + 1).ToString();
        if (selectUI.childCount == 0)
        {
            selectUI.parent.Find("Slots").Find("Left").GetComponent<Button>().enabled = true;
            selectUI.parent.Find("Slots").Find("Middle").GetComponent<Button>().enabled = false;
            selectUI.parent.Find("Slots").Find("Right").GetComponent<Button>().enabled = false;
        }
        else if (selectUI.childCount == 1)
        {
            selectUI.parent.Find("Slots").Find("Middle").GetComponent<Button>().enabled = true;
            selectUI.parent.Find("Slots").Find("Right").GetComponent<Button>().enabled = false;
        }

        else if (selectUI.childCount == 2)
            selectUI.parent.Find("Slots").Find("Right").GetComponent<Button>().enabled = true;
    }

    // 같은 닉네임이 있는지 확인
    public void CheckName()
    {
        StartCoroutine(GetPlayerList());
    }
    // 로그인 화면으로
    public void ExitMake()
    {
        SceneManager.LoadScene("02.Login");
    }    
    // 캐릭터 생성창 이동
    public void OpenMakeUI()
    {
        // 테스트 모드이면 캐릭터 생성 불가
        if (GameManager.GManager.TestMode)
        {
            Debug.Log("*테스트 모드* 캐릭터 생성 불가");
            return;
        }
        makeName = makeUI.Find("InputNickName").transform.Find("NickName").GetComponent<Text>();
        makeUI.gameObject.SetActive(true);
    }
    // 캐릭터 생성 취소
    public void CloseMakeUI()
    {
        makeUI.Find("LogText").gameObject.SetActive(false);
        ReFlesh();
        makeUI.gameObject.SetActive(false);
    }
    // 캐릭터 생성시 위치에 따라 번호 변경
    public void MakePlayerNum(int count)
    {
        GameManager.GManager.num = count.ToString();
    }
    // 캐릭터 아바타에 따라 번호 변경
    public void MakePlayerAvatar(int count)
    {
        makeAvatar = count;
        makeUI.Find("Avatar").GetComponent<RawImage>().texture = Resources.Load<Texture>("myCharacter" + makeAvatar);
    }
    private IEnumerator GetPlayerList()
    {
        WWWForm form = new WWWForm();
        // json의 로그인 정보 가져오기
        form.AddField("SecretCode", GameManager.GManager.SecretCode); //post방식 전달.

        var www = new WWW(GameManager.GManager.URL + "playerFind.json.php", form);
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

    private void FindPlayer(string strJsonData)
    {
        // 받은 데이터를 json 형식으로 변환
        var jSon = JSON.Parse(strJsonData);
        bool checkName = true;
        if(makeUI.gameObject.activeSelf)
        {
            makeUI.Find("LogText").GetComponent<Text>().text = makeName.text.Length < 3 ? "※ 닉네임을 3자리 이상 입력하세요." : "";
            if(makeUI.Find("LogText").GetComponent<Text>().text.Length > 2)
            {
                makeUI.Find("LogText").gameObject.SetActive(true);
                return;
            }
        }
        else
            makeUI.Find("LogText").gameObject.SetActive(false);

        if (jSon != null)
        {
            for (int i = 0; i < jSon.Count; i++)
            {
                string getIdx = GameManager.JtoS("idx", jSon, i);
                // 접속한 아이디 정보만 확인하기
                if (getIdx == GameManager.GManager.idx)
                {
                    // 플레이어 생성
                    if (makeUI.gameObject.activeSelf)
                    {
                        if (GameManager.JtoS("name", jSon, i) == makeName.text) checkName = false; // 중복            
                    }
                    // 로그인 후 플레이어창 갱신
                    else
                    {
                        // 아이디 불러오기
                        GameManager.Player.playerName = GameManager.JtoS("name", jSon, i);
                        GameManager.GManager.num = GameManager.JtoS("num", jSon, i);
                        // 플레이어 데이터 불러오기
                        SelectSlot.gameObject.GetComponent<csSelectSlot>().selectNum = int.Parse(GameManager.GManager.num);
                        GameObject addPlayer = Instantiate(SelectSlot);
                       
                        addPlayer.transform.Find("Name").GetComponent<Text>().text = GameManager.Player.playerName;
                        addPlayer.transform.Find("Avatar").GetComponent<RawImage>().texture = Resources.Load<Texture>("myCharacter" + GameManager.JtoS("avatar", jSon, i));
                        addPlayer.transform.Find("Level").GetComponent<Text>().text = "Lv." + GameManager.JtoS("level", jSon, i);
                        addPlayer.transform.SetParent(selectUI);
                    }
                }
                
            }
        }
        // 생성창이 닫혀있으면 캐릭터창 초기화 해주고 리턴
        if (!makeUI.gameObject.activeSelf)
        {
            ReFlesh();
            return;
        }

        // 캐릭터 이름 중복체크
        if (checkName)
        {            
            GameManager.Player.playerName = GameManager.JtoS(makeName.text);
            makeUI.Find("LogText").gameObject.SetActive(false);
            StartCoroutine(CreatePlayer());
        }
        else
        {
            makeUI.Find("LogText").GetComponent<Text>().text = "※ 중복된 닉네임입니다.";
            makeUI.Find("LogText").gameObject.SetActive(true);
            Debug.Log("캐릭터 생성 실패");
        }
    }
    private IEnumerator CreatePlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("SecretCode", GameManager.GManager.SecretCode);
        form.AddField("idx", GameManager.GManager.idx);
        form.AddField("num", GameManager.GManager.num);
        form.AddField("name", GameManager.Player.playerName);
        form.AddField("avatar", makeAvatar);

        var www = new WWW(GameManager.GManager.URL + "playerCreate.json.php", form);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            DisplayPlayerResult(www.text);
        }
        else
        {
            makeUI.Find("LogText").GetComponent<Text>().text = "※ 웹서버 에러";
            makeUI.Find("LogText").gameObject.SetActive(true);
            Debug.Log("Error : " + www.error);
        }
    }
    private void DisplayPlayerResult(string strJsonData)
    {
        var jSon = JSON.Parse(strJsonData);

        if(jSon["returnCode"].AsInt == 1) // JSON 성공/실패
        {
            // 캐릭터 생성 성공
            SelectSlot.gameObject.GetComponent<csSelectSlot>().selectNum = int.Parse(GameManager.GManager.num);
            GameObject addPlayer = Instantiate(SelectSlot);
            addPlayer.transform.Find("Name").GetComponent<Text>().text = GameManager.Player.playerName;
            addPlayer.transform.Find("Avatar").GetComponent<RawImage>().texture = Resources.Load<Texture>("myCharacter"+makeAvatar);
            addPlayer.transform.Find("Level").GetComponent<Text>().text = "Lv.1";
            addPlayer.transform.SetParent(selectUI);
            CloseMakeUI();
        }
        else
        {
            makeUI.Find("LogText").GetComponent<Text>().text = "※ 웹서버 에러";
            makeUI.Find("LogText").gameObject.SetActive(true);
        }
        
        Debug.Log("메세지: " + jSon["returnMsg"].ToString()); // 결과 출력
    }
}
