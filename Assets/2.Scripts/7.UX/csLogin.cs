using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class csLogin : csSceneManager<csLogin>
{
    public Text txtId;
    public Text txtPw;
    private Text txtLog;
    private Transform popupUI;
    private string jsonURL = ""; //정보가 저장된 페이지주소
    public void Awake()
    {
        if (GameManager.GManager.idx == "")
        {
            SoundManager.Sound.StagePlay(0);
            SoundManager.Sound.bgAudio.GetComponent<AudioSource>().loop = true;
        }
        popupUI = GameObject.Find("LoginCanvas").transform.Find("PopupUI");
        txtLog = GameObject.Find("LoginCanvas").transform.Find("LogText").GetComponent<Text>();
        // 아이디 입력창으로 포커스 지정
        txtId.transform.parent.gameObject.GetComponent<InputField>().Select();
    }
    // 로그인 버튼 함수
    public void Login()
    {
        // 테스트 모드이면 테스트 모드로 강제 로그인
        if (GameManager.GManager.TestMode)
        {
            Debug.Log("*테스트 모드* 로그인 성공");
            SceneManager.LoadScene("03.MakeAccount");
            return;
        }
        jsonURL = GameManager.GManager.URL + "userLogin.json.php"; // 로그인 기능
        StartCoroutine("GetUserList");
    }
    // 팝업창에서 Yes 버튼 누를 시 회원 정보 변경
    public void SetUser()
    {
        if(!GameManager.GManager.TestMode)
            StartCoroutine("SetBoard");
    }
    // 팝업창에서 X 버튼 누를 시 팝업 창 숨김
    public void ClosePopUp()
    {
        popupUI.gameObject.SetActive(false);
    }

    // 아이디, 비밀번호 길이 체크
    private bool CheckLength()
    {
        txtLog.text = txtId.text.Length < 3 ? "※ 아이디를 3자리 이상 입력하세요." : "";
        if(txtLog.text == "") // 아이디 체크 후 패스워드도 체크
            txtLog.text = txtPw.text.Length < 3 ? "※ 비밀번호를 3자리 이상 입력하세요." : "";
        // 아이디, 패스워드 길이가 3 이상이면 true, 3 미만이면 false
        return txtLog.text == "" ? true : false;
    }
    // 팝업 기능을 회원 탈퇴 기능으로 변경
    public void DeleteUser()
    {
        // 테스트 모드이면 삭제 불가
        if (GameManager.GManager.TestMode)
        {
            Debug.Log("*테스트 모드* 회원 탈퇴 불가");

            return;
        }

        if (CheckLength())
        {
            popupUI.Find("TextPopup").GetComponent<Text>().text = "ID(" + txtId.text + ")정보가 삭제됩니다.\n 삭제 하시겠습니까?";
            // 사용자 정보 불러오기
            jsonURL = GameManager.GManager.URL + "userDelete.json.php"; // 회원탈퇴 기능
            StartCoroutine("GetUserList");
        }        
    }
    // 팝업 기능을 회원 가입 기능으로 변경
    private void SignUp()
    {
        if (CheckLength())
            popupUI.Find("TextPopup").GetComponent<Text>().text = "ID(" + txtId.text + ")정보가 없습니다.\n 가입 하시겠습니까?";
        popupUI.gameObject.SetActive(true);
        jsonURL = GameManager.GManager.URL + "userCreate.json.php"; // 회원가입 기능
    }
    // 회원들의 정보를 불러온다.
    private IEnumerator GetUserList()
    {
        WWWForm form = new WWWForm();
        // json의 로그인 정보 가져오기
        form.AddField("SecretCode", GameManager.GManager.SecretCode); //post방식 전달.

        var www = new WWW(GameManager.GManager.URL + "userFind.json.php", form);        
        yield return www; //웹의 다운로드 완료시까지 대기.
        
        // 에러가 없으면
        if (string.IsNullOrEmpty(www.error))
        {
            FindUser(www.text); //파싱후 출력.
        }
        else
        {
            Debug.Log("Error : " + www.error);
        }
    }

    private void FindUser(string strJsonData)
    {
        // 받은 데이터를 json 형식으로 변환
        var jSon = JSON.Parse(strJsonData);
        if (!CheckLength()) return;
        if (jSon != null)
        {
            for (int i = 0; i < jSon.Count; i++)
            {
                string id = GameManager.JtoS("id", jSon, i);
                if (id == txtId.text)
                {
                    string pw = GameManager.JtoS("pw", jSon, i);
                    if (pw == txtPw.text)
                    {
                        GameManager.GManager.idx = GameManager.JtoS("idx", jSon, i);

                        // 로그인 시
                        if (jsonURL == GameManager.GManager.URL + "userLogin.json.php")
                        {
                            // 로그인 성공
                            // 로그인 정보 불러오기
                            Debug.Log("로그인 성공");
                            SceneManager.LoadScene("03.MakeAccount");
                        }
                        // 삭제 시 팝업창 오픈
                        else
                        {
                            popupUI.gameObject.SetActive(true);
                        }
                        return;
                    }
                    else
                    {
                        // 패스워드가 다름
                        txtLog.text = "※ 비밀번호가 다릅니다!";
                        Debug.Log(txtLog.text);
                        Debug.Log("입력한비밀번호 : " + txtPw.text);
                        Debug.Log("--------------------------");
                        return;
                    }
                }
            }
            // 아이디가 없음
            txtLog.text = "※ 가입된 아이디가 없습니다!";
            Debug.Log(txtLog.text);
            Debug.Log("입력한아이디 : " + txtId.text);
            Debug.Log("--------------------------");
            // 삭제할 정보가 일치하지 않으면
            if (jsonURL == GameManager.GManager.URL + "userDelete.json.php")
            {
                GameManager.GManager.idx = "";
                return;
            }
            // 가입한 정보가 없다면 회원 가입할 지 팝업창을 띄운다.
            GameManager.GManager.idx = (int.Parse(GameManager.JtoS("idx", jSon, jSon.Count - 1)) + 1).ToString(); // 가입 했을 경우의 인덱스
        }
        else
            GameManager.GManager.idx = "1"; // 가입 했을 경우의 인덱스
        SignUp();
        Debug.Log("가입 정보 없음");
        Debug.Log("입력한아이디 : " + txtId.text);
        Debug.Log("입력한패스워드 : " + txtPw.text);
        Debug.Log("--------------------------");
    }

    private IEnumerator SetBoard()
    {
        WWWForm form = new WWWForm();
        form.AddField("SecretCode", GameManager.GManager.SecretCode);
        form.AddField("idx", GameManager.GManager.idx);
        form.AddField("id", txtId.text);
        form.AddField("pw", txtPw.text);

        var www = new WWW(jsonURL, form);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            SaveResult(www.text);
        }
        else
        {
            Debug.Log("Error : " + www.error);
        }
    }
    private void SaveResult(string strJsonData)
    {
        var jSon = JSON.Parse(strJsonData);

        int returnCode = jSon["returnCode"].AsInt; //성공.실패 확인용.
        string returnMsg = jSon["returnMsg"].ToString();
        
        if(returnCode == 1)
        {
            ClosePopUp();
            switch (jsonURL.Substring(32))
            {
                case "userCreate.json.php": // 회원 가입
                    // 로그인 정보 불러오기
                    Debug.Log("로그인 성공");
                    SceneManager.LoadScene("03.MakeAccount");
                    break;
                case "userDelete.json.php": // 회원 탈퇴
                    Debug.Log("탈퇴 완료");
                    break;
                case "userChange.json.php": // 회원 정보 변경
                    // 추가 예정
                    break;
            }
        }
        else
        {
            // 에러 내용 전달
        }
        Debug.Log("메세지: " + returnMsg);
    }
}
