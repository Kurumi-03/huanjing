using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//控制按钮点击音效
public class ClickAudio : MonoBehaviour
{
    private AudioSource voice;
    private ManagerVars vars;
    private void Awake() {
        voice = GetComponent<AudioSource>();
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.PlayAudio,PlayAudio);
        EventCenter.AddListener<bool>(EventDefine.IsMouseOn,IsMouseOn);
    }

    private void PlayAudio(){
        voice.PlayOneShot(vars.buttonClip);
    }

    //检测音效是否开启
    private void IsMouseOn(bool value){
        //AudioSource的mute属性为是否静音
        voice.mute = !value;
    }

    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.PlayAudio,PlayAudio);
        EventCenter.RemoveListener<bool>(EventDefine.IsMouseOn,IsMouseOn);
    }
}
