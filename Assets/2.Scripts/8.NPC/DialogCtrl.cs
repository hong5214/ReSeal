using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogCtrl : csSceneManager<DialogCtrl>
{
    //대화창UX의 텍스트
    public Text dialogText;
    //임시로 대화를 전달받아 저장
    public string tempText;
    //예,아니오 버튼
    public GameObject button;
    public static bool txtSkip;
    //대화창
    public Transform dialog_Box;
    // Start is called before the first frame update
    private void Awake()
    {
        //DialogBox를 연결(비활성화 되어 있기에 부모를 통해 접근해야한다)
        dialog_Box = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("Dialog_Box");
        dialogText =dialog_Box.transform.Find("DialogMask").transform.Find("Dialog").GetComponent<Text>();
        Debug.Assert(dialogText);

        //버튼 연결.
        button = dialog_Box.transform.Find("YNButton").gameObject;
        Debug.Assert(button);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space)) txtSkip = true;
    }

    //대화창이 켜질때
    private void OnEnable()
    {
        //글자를 타이핑 해준다
        StartCoroutine(Typing(tempText));
    }

    //대화창이 꺼질때
    private void OnDisable()
    {
        //글자 타이핑을 멈춘다
        StopAllCoroutines();
        //버튼 꺼준다
        button.SetActive(false);
    }

    //타이핑을 차례로 보여준다
    IEnumerator Typing(string text)
    {
        //먼저 초기화 해주고
        dialogText.text = "";
        txtSkip = false;
        //문자열을 임시 텍스트에서 한글자씩 배열로 받는다
        foreach (char letter in text.ToCharArray())
        {
            if (txtSkip)
            {
                dialogText.text = text;
                break;
            }
            dialogText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
        //대화가 마무리 되고 예,아니오 버튼을 활성화 시킨다.
        button.SetActive(true);
        txtSkip = false;
    }
}
