using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;//动画系统
using UnityEngine.SceneManagement;//场景管理

//弹窗确认是否重置脚本
public class ResetPannel : MonoBehaviour
{
    private Button btn_Yes;
    private Button btn_No;
    private Image img_bg;
    private GameObject dialog;

    private void Awake() {
        //获取按钮组件 并注册事件码
        btn_Yes = transform.Find("Dialog/btn_Yes").GetComponent<Button>();
        btn_Yes.onClick.AddListener(OnClickYesButton);
        btn_No = transform.Find("Dialog/btn_No").GetComponent<Button>();
        btn_No.onClick.AddListener(OnClickNOButton);
        //获取背景
        img_bg = transform.Find("bg").GetComponent<Image>();
        //获取弹窗本体
        dialog = transform.Find("Dialog").gameObject;

        //背景初始为透明
        img_bg.color = new Color(img_bg.color.r,img_bg.color.g,img_bg.color.b,0);
        //初始化时弹窗设置为scale=0
        dialog.transform.localScale = Vector3.zero; 

        gameObject.SetActive(false);//初始化时隐藏
        EventCenter.AddListener(EventDefine.ShowResetPannel,Show);
    }

    //显示reset界面
    private void Show(){
        gameObject.SetActive(true);
        //在0.3秒内将透明度变为0.4
        img_bg.DOColor(new Color(img_bg.color.r,img_bg.color.g,img_bg.color.b,0.4f),0.3f);
        //在0.3秒内将物体的大小变为原来大小  正常显示
        dialog.transform.DOScale(Vector3.one,0.3f);

    }   

    //点击yes按钮
    private void OnClickYesButton(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        GameManager.Instance.ResetData();
        //重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //点击no按钮
    private void OnClickNOButton(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        //在0.3秒内将透明度变为0
        img_bg.DOColor(new Color(img_bg.color.r,img_bg.color.g,img_bg.color.b,0),0.3f);
        //在0.3秒内将物体的大小变为0  隐藏显示
        //在完成动作后使用Lambda表达式
        dialog.transform.DOScale(Vector3.zero,0.3f).OnComplete(()=>{
            gameObject.SetActive(false);
        });
    }

    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.ShowResetPannel,Show);        
    }
}
