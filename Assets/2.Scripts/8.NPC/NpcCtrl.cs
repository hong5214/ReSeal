using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Characters;

public abstract class  NpcCtrl : MonoBehaviour   //public선언을 하지 않으면 상속객체에서 접근이 안됨.
{
    //DialogBox 즉, 대화창UI
    public Transform dialog_Box;
    //상호작용을 위한 UI
    private Transform inter_Box;
    //NPC의 이름
    public Text npcName;
    //영역에 들어왔는지 체크
    private bool inArea;
    //대화창 컴포넌트에 접근(메시지를 전달하기 위해)+public 처리 해야지 상속받는 친구에서 접근 가능 ..
    public DialogCtrl tempDialog;
    //기능을 연결할 버튼
    public Button[] funcButt;
    //플레이어 연결
    private GameObject player;


    protected virtual void Start()
    {
       
        //움직임을 제어하기 위해 플레이어에 접근
        player = GameObject.FindWithTag("Player");
        Debug.Assert(player);
        //InterBox를 연결
        inter_Box = transform.Find("Inter_Box").transform;
        Debug.Assert(inter_Box);

        //DialogBox를 연결(비활성화 되어 있기에 부모를 통해 접근해야한다)  
        dialog_Box = GameObject.FindGameObjectWithTag("Player").transform.Find("ItemManager").transform.Find("PlayerUI").transform.Find("Dialog_Box");
        Debug.Assert(dialog_Box);

        //메시지 전달을 위해 dialogController에 접근
        tempDialog = dialog_Box.GetComponent<DialogCtrl>();
        Debug.Assert(tempDialog);

        //버튼에 기능을 추가하기 위해 townUX의 Yes/NO버튼에 배열로 접근.(더 좋은 방식이 있을거 같음.. 추후 고민)
        funcButt = dialog_Box.transform.Find("YNButton").transform.GetComponentsInChildren<Button>();
        Debug.Assert(funcButt[0]);
        Debug.Assert(funcButt[1]);


        //NPC의 이름
        npcName= dialog_Box.transform.Find("NpcName").transform.GetComponent<Text>();
        Debug.Assert(npcName);

    }

    private void Update()
    {
        //만약 NPCArea에 들어와있고 , E 키를 눌렀다면
        if (Input.GetKeyDown(KeyCode.E) && inArea && dialog_Box.gameObject.activeSelf!= true && inter_Box.gameObject.activeSelf)//한번만 활성화
        {
            //플레이어의 움직임을 막아주고
            Characters.PlayerCtrl.Scene.StopMove();
            //대화창을 켜준다
            dialog_Box.gameObject.SetActive(true);
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            //전달하고자 하는 텍스트를 설정
            SetText();
            //상호작용 창을 켜주고
            inter_Box.gameObject.SetActive(true);
            //영역안에 들어왔다
            inArea = true;
            //기능을 yes 버튼에 추가
            funcButt[0].onClick.AddListener(myFunction);
            //나가는 기능 no버튼에 추가
            funcButt[1].onClick.AddListener(QuitDialog);
        }
    }

    //플레이어가 영역밖으로 나간다면
    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            //텍스트를 초기화
            tempDialog.tempText = "";
            //상호작용 창을 꺼주고
            inter_Box.gameObject.SetActive(false);
            //영역 밖으로 나갔다
            inArea = false;
            //yes 버튼에 추가한 기능을 지워준다
            funcButt[0].onClick.RemoveListener(myFunction);
            //나가는 기능 no버튼에 추가
            funcButt[1].onClick.RemoveListener(QuitDialog);
            //Npc이름초기화
            npcName.text = "";
            QuitDialog();
        }
    }

    //아니오 버튼이 눌린다면
    public void QuitDialog()
    {
        //플레이어가 다시 움직이게 해준다
        Characters.PlayerCtrl.Scene.StartMove();
        //대화창을 꺼준다
        dialog_Box.gameObject.SetActive(false);
    }
   
    //추가하고자 하는 기능
    public abstract void myFunction();

    //텍스트를 설정한다.
    public abstract void SetText();
    


}

public enum Dungeon
{
    Forest = 1,
    Desert,
    Frozen
}
public enum Stage
{
    
    One = 1,
    Two,
    Three
}
public enum Sector
{
    One,
    Two,
    Three,
    Four
}
