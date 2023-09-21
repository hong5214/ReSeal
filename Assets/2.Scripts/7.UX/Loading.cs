using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [Header("하단 정보글")]
    public Text explain;
    public Image bar;
    public int ex_count=0;
    private float progress=0;
    private string nextSceneName = "";

    private enum NextScene
    {
        Make,
        Town,
        Stage1,
        Stage2,
        Stage3,
    }

    void Start()
    {
        SetText();
        switch (GameManager.GManager.nextScene)
        {
            case (int)NextScene.Make :
                nextSceneName = "03.MakeAccount";
                break;
            case (int)NextScene.Town:
                nextSceneName = "05.scTown";
                break;
            case (int)NextScene.Stage1:
                nextSceneName = (GameManager.GManager.StageNum + 5).ToString("00") + ".scStage1_" + GameManager.GManager.StageNum;
                break;
            case (int)NextScene.Stage2:
                nextSceneName = (GameManager.GManager.StageNum + 8).ToString("00") + ".scStage2_" + GameManager.GManager.StageNum;
                break;
            case (int)NextScene.Stage3:
                nextSceneName = (GameManager.GManager.StageNum + 11) + ".scStage3_" + GameManager.GManager.StageNum;
                break;
        }        
        
        Debug.Log("씬 이동 : " + nextSceneName);
        StartCoroutine(LoadingProgress());        
    }
    public IEnumerator LoadingProgress()
    {

        if (GameManager.GManager.nextScene == 0)
        {
            /*if(GameManager.GManager.nextScene != 0)
            {
                GameManager.GManager.PhotonPlayer = null;
                PhotonNetwork.Destroy(GameManager.GManager.PhotonPlayer);
                while (GameManager.GManager.PhotonPlayer != null) yield return null;
            }*/
            PhotonNetwork.LeaveRoom(); // 방 나가기
            PhotonNetwork.LeaveLobby(); // 로비 나가기
            //while (PhotonNetwork.inRoom) yield return null; // 방 퇴장 대기
        }
        PhotonNetwork.isMessageQueueRunning = false;

        AsyncOperation async =SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        //
        if (GameManager.GManager.StageNum == 3)//보스전이라면
        {
            SoundManager.Sound.StagePlay(5);
        }
        else if(GameManager.GManager.StageNum == 1)//그 이외의 스테이지라면
        {
            SoundManager.Sound.StagePlay(GameManager.GManager.nextScene);

        }
        while (!async.isDone)
        {
            yield return true;
            float progress=async.progress;
            bar.fillAmount= progress > 0.8f ? 1.0f : progress;
        }
    }
    public void SetText()
   {
        explain.text = "";
        ex_count = Random.Range(0, 3);
        if (ex_count==0)
       {
           explain.text="TIP. 주민들에게 말을 걸어보세요.\n 생각보다 좋은 정보를 가지고 있을지도 모릅니다.";
       }
       else if(ex_count==1)
       {
            explain.text="TIP . 마을의 마법사는 다양한 종류의 물약을 팔고 있습니다.\n 좋은 전투를 위해 좋은 물약은 필수입니다";
       }
       else if(ex_count==2)
       {
           explain.text="TIP . 다양한 총기를 구비해보세요 !\n총들에는 다양한 잠재력이 숨겨져 있습니다.";
       }
   }    
}
