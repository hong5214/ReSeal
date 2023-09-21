using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csSceneManager<T> : CharacterManager where T : csSceneManager<T>
{
    private static T scManager = default(T);
    // SCManager 통해 scManager 다른 씬의 변수, 함수들을 가져올 수 있다.
    public static T Scene
    {
        get
        {
            if (scManager == null)
            {
                // 해당 스크립트를 가지고 있는 오브젝트가 있는지 확인
                scManager = GameObject.FindObjectOfType(typeof(T)) as T;
                // 없으면 새로 만든다.
                if (scManager == null)
                {
                    scManager = new GameObject("Singleton_" + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                }
            }
            return scManager;
        }
        set
        {
            scManager = value;
        }
    }    
}
