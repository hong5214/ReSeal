using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public enum Dir
{
    Left,Right,Forward,Back
}
public enum BlockType
{
    Type2_1,
    Type2_2,
    Save2,
    Type3_1,
    Type3_2,
    Save3
}

public class MakeBlocks : MonoBehaviour
{
    
    public Transform startPos;//시작위치
    public GameObject blocks;//발판

    public int MakeCount;//생성 개수
    public Dir nextDir;//다음방향

    public BlockType blockType;

    [Range(1,20)]
    public int distance;//다음 발판까지거리
    [Range(-10, 10)]
    public int height;//다음 발판까지 높이

    public bool isMove;//움직일지
    public bool isTransparent;//투명화 할지

    


    private void Awake()
    {
        if(startPos==null)//시작위치가 없다면
        {
            startPos = this.transform;//처음 시작할때의 위치
        }
        
    }
    public void MakeNext()//다음 블록을 만들어준다.
    {
        for(int i=0;i<MakeCount;i++)//생성 개수만큼
        {
            Vector3 pos = startPos.position;//생성위치
            Quaternion rot = startPos.rotation;//회전

            switch(nextDir)//다음생성 방향에 따라서
            {
                case Dir.Forward://앞에 생성한다면
                    pos.z -=   distance;
                    rot = Quaternion.Euler(0, 0, 0);
                    break;

                case Dir.Back://앞에 생성한다면
                    pos.z += distance;
                    rot = Quaternion.Euler(0, 0, 0);
                    break;

                case Dir.Left://왼쪽에 생성 한다면
                    pos.x += distance;
                    rot = Quaternion.Euler(0, rot.y + 90, 0);
                    break;

                case Dir.Right://오른쪽에 생성 한다면
                    pos.x -= distance;
                    rot = Quaternion.Euler(0, rot.y - 90, 0);
                    break;
            }

            pos.y += height;//블록의 높이

            var newCube = Instantiate(blocks,pos,rot);//큐브를 생성
            newCube.transform.SetParent(gameObject.transform);//부모로 지정

            if(isMove)//움직이는 블록이라면
            {
                newCube.AddComponent<BlockUpandDown>();
               
            }
            if(isTransparent)//투명한 블록이라면
            {
                newCube.AddComponent<Transparency>();
            }
           
            startPos = newCube.transform;//다음 블록을 위해 위치를 업데이트
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(newCube, "MakeNext");
#endif
        }
    }
  
    

   

}
