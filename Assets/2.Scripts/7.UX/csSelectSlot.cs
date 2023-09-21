using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class csSelectSlot : MonoBehaviour
{
    public int selectNum;
    // 플레이어 삭제
    public void DeletePlayer()
    {
        // 테스트 모드이면 캐릭터 삭제 불가
        if (GameManager.GManager.TestMode)
        {
            Debug.Log("*테스트 모드* 캐릭터 삭제 불가");
            return;
        }
        var slot = this.transform.parent.GetComponentsInChildren<csSelectSlot>();
        // 번호 초기화
        for (int i = selectNum; i <= slot.Length; i++)
            slot[i-1].selectNum = i;
        GameManager.Player.playerName = transform.Find("Name").GetComponent<UnityEngine.UI.Text>().text;
        GameManager.GManager.num = selectNum.ToString();
        StartCoroutine(GameManager.GManager.DeletePlayerData(this.gameObject));
    }
    // 게임 플레이
    public void PlayGame()
    {        
        GameManager.Player.playerName = transform.Find("Name").GetComponent<UnityEngine.UI.Text>().text;
        // 테스트 모드이면 DB 사용 불가
        if (GameManager.GManager.TestMode)
        {
            transform.Find("Avatar").GetComponent<RawImage>().texture = Resources.Load<Texture>("myCharacter" + (selectNum-1));
            GameManager.Player.Reset();
            Debug.Log("*테스트 모드* 캐릭터 스탯 초기값");
            GameManager.GManager.avatar = (selectNum-1).ToString();
            GameManager.Player.Level = 4;
            GameManager.Player.Exp = 0;
            GameManager.Player.Hp = GameManager.Player.MaxHp = 100;
            GameManager.Player.Mp = 100;
            GameManager.Player.Atk = 20;
            GameManager.Player.Def = 0;
            GameManager.Player.Gold = 10000;
            GameManager.Player.AtkSpeed = 0;
            GameManager.Player.MoveSpeed = 0;
            GameManager.Player.Critical = 0;
            GameManager.Player.CriticalMax = 0;
            GameManager.Player.Drain = 0;
            for (int k = 1; k <= 10; k++)
                GameManager.Player.transform.Find("ItemManager").GetComponent<SkillManager>().levelSkill[k - 1] = 0;
            PhotonNetwork.JoinLobby(); // 방 들어가기
            return;
        }
        GameManager.GManager.LoadPlayerData();
    }
}
