using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableUI : MonoBehaviour,IPointerDownHandler,IDragHandler //버튼을 클릭하는 순간 실행되는 ,드레그 하는 순간 실행되는 인터페이스
{
    [SerializeField]
    private Transform _targetTr;//움직이고자 하는 타켓의 위치
    private Vector2 _beginPoint;//시작점
    private Vector2 _moveBegein;//
    private RectTransform rectTr;

    private void Awake()
    {
        if(_targetTr==null)
        _targetTr = this.transform;//나 자신을 연결
        rectTr = GetComponent<RectTransform>();
    }
    
    //버튼을 클릭했을때 호출 죽 ,드레그 시작 위치를 지정
    public void OnPointerDown(PointerEventData eventData)
    {
        _beginPoint = _targetTr.position;//나의 위치로 초기화
        _moveBegein = eventData.position;//클릭이 된 위치로 초기화
    }

    //드레그를 할때(마우스 커서 위치로 이동)
    public void OnDrag(PointerEventData eventData)
    {
        //목표의 위치는 시작위치에 드레그위치시 마우스 변동 거리를 offset 으로 더해준다(드레그 위치 - 처음 클릭위치)
        _targetTr.position = _beginPoint + (eventData.position- _moveBegein);

    }

    //꺼진다면
   public void OnDisable()
    {
        //원점의 위치로 되돌려놓음 캔버스위 오븍젝트의 위치값 조절은 localposition이 아닌 anchoredposition.조정
        rectTr.anchoredPosition = new Vector2(0, 0);
        
    }


}
