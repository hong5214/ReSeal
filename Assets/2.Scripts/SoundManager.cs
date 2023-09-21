using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager :MonoBehaviour
{
    #region 싱글톤
    // SCManager 통해 scManager 다른 씬의 변수, 함수들을 가져올 수 있다.
    private static SoundManager _sound=null;

    public static SoundManager Sound
    {
        get
        {
            if (_sound == null)
            {
                // 해당 스크립트를 가지고 있는 오브젝트가 있는지 확인
                _sound = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
                // 없으면 새로 만든다.
                if (_sound == null)
                {
                    _sound = new GameObject("Singleton_" + typeof(SoundManager).ToString(), typeof(SoundManager)).GetComponent<SoundManager>();
                }
            }
            return _sound;
        }
        set
        {
            _sound = value;
        }
    }
    #endregion


    public AudioSource bgAudio;//배경 오디오 소스
    public AudioSource efAudio;//효과 오디오 소스


    public AudioClip[] BgSound;//배경음들
    public AudioClip[] EfSound;//효과음들

    #region 배경음악 재생

    public void StagePlay(int num)
    {

        bgAudio.clip = null;
        bgAudio.clip = BgSound[num];//전달받은 값으로 오디오 소스에 사운드 연결
        bgAudio.Play();//사운드를 재생한다.
    }

    public void EffectPlay(int num)
    {
        efAudio.clip = null;
        efAudio.clip = EfSound[num];//전달받은 값으로 오디오 소스에 사운드 연결
        efAudio.Play();//사운드를 재생한다.
    }



    #endregion
}
