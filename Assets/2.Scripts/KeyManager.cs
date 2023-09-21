using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeyManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "02.Login")
        {
            // 탭 누르면 커서 이동
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (csLogin.Scene.txtId.transform.parent.gameObject.GetComponent<InputField>().isFocused)
                    csLogin.Scene.txtPw.transform.parent.gameObject.GetComponent<InputField>().Select();
                else
                    csLogin.Scene.txtId.transform.parent.gameObject.GetComponent<InputField>().Select();
            }
            if (Input.GetKeyDown(KeyCode.Return))
                csLogin.Scene.Login();
        }
        if (SceneManager.GetActiveScene().name != "01.Logo" && SceneManager.GetActiveScene().name != "02.Login"
             && SceneManager.GetActiveScene().name != "03.MakeAccount" && SceneManager.GetActiveScene().name != "04.scLoading")
        {            
            if(Characters.PlayerCtrl.Scene.pv.isMine)
            {
                // 카메라 시점 고정/해제
                if (Input.GetKey(KeyCode.T) && !Characters.PlayerCtrl.Scene.tab)
                {
                    Characters.PlayerCtrl.Scene.tab = true;
                    Characters.PlayerCtrl.Scene.m_MouseLook = !Characters.PlayerCtrl.Scene.m_MouseLook;
                    Cursor.lockState = Characters.PlayerCtrl.Scene.m_MouseLook ? CursorLockMode.Locked : CursorLockMode.None;
                }
                if (!Input.GetKey(KeyCode.T) && Characters.PlayerCtrl.Scene.tab) { Characters.PlayerCtrl.Scene.tab = false; }
                if (Input.GetKeyDown(KeyCode.LeftShift)) Characters.PlayerCtrl.Scene.checkDash = true;
                if (Input.GetKeyUp(KeyCode.LeftShift)) Characters.PlayerCtrl.Scene.checkDash = false;
                if (Input.GetKeyDown(KeyCode.I) && !Characters.PlayerCtrl.Scene.sellButton.gameObject.activeSelf) Characters.PlayerCtrl.Scene.Inven_OnOff(); // 인벤토리
                if ((Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Escape)) && !Characters.PlayerCtrl.Scene.sellButton.gameObject.activeSelf) Characters.PlayerCtrl.Scene.SetUp_OnOff(); // 셋업
                if (Input.GetKeyDown(KeyCode.K)) Characters.PlayerCtrl.Scene.Skill_OnOff(); // 스킬
                                                                                            // 채팅창 ON/OFF
                if (Input.GetKeyDown(KeyCode.Return) && !csItemManager.Scene.chatCheck)
                {
                    csItemManager.Scene.chatCheck = true;
                    Characters.PlayerCtrl.Scene.SendMessage("StopMove");
                    csItemManager.Scene.ChatColor.a = 0.4f;
                    csItemManager.Scene.LogScroll.transform.parent.GetComponent<Image>().color = csItemManager.Scene.ChatColor;
                    csItemManager.Scene.LogScroll.GetComponent<Image>().color = csItemManager.Scene.ChatColor;
                    csItemManager.Scene.LogScroll.transform.Find("Sliding Area").GetComponentInChildren<Image>().color = csItemManager.Scene.ChatColor;
                    csItemManager.Scene.ChatText.GetComponent<Image>().color = csItemManager.Scene.ChatColor;
                    if (!csItemManager.Scene.ChatText.isFocused)
                        csItemManager.Scene.ChatText.Select();
                }
                else if (Input.GetKeyDown(KeyCode.Return) && csItemManager.Scene.chatCheck)
                {
                    csItemManager.Scene.chatCheck = false;
                    Characters.PlayerCtrl.Scene.SendMessage("StartMove");
                    csItemManager.Scene.ChatColor.a = 0;
                    csItemManager.Scene.LogScroll.transform.parent.GetComponent<Image>().color = csItemManager.Scene.ChatColor;
                    csItemManager.Scene.LogScroll.GetComponent<Image>().color = csItemManager.Scene.ChatColor;
                    csItemManager.Scene.LogScroll.transform.Find("Sliding Area").GetComponentInChildren<Image>().color = csItemManager.Scene.ChatColor;
                    csItemManager.Scene.ChatText.GetComponent<Image>().color = csItemManager.Scene.ChatColor;
                    if (csItemManager.Scene.ChatText.isFocused)
                        csItemManager.Scene.ChatText.Select();
                }
                if (SceneManager.GetActiveScene().name != "05.scTown")
                {
                    // 무기 교체
                    if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        StageManager.Scene.itemActive = StageManager.Scene.itemCheck ? ((StageManager.Scene.itemActive == 0) ? 1 : 2) : 1;
                    }
                    if (Input.GetKey(KeyCode.Alpha1)) StageManager.Scene.ChangeSlot(1);
                    else if (Input.GetKey(KeyCode.Alpha2)) StageManager.Scene.ChangeSlot(2);
                    else if (Input.GetKeyDown(KeyCode.Alpha3)) StageManager.Scene.ChangeSlot(3);
                    else if (Input.GetKeyDown(KeyCode.Alpha4)) StageManager.Scene.ChangeSlot(4);
                }
                else
                {
                    //if (Input.GetKey(KeyCode.Space)) DialogCtrl.txtSkip = true;
                }
            }            
        }
    }
}
