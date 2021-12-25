using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//游戏界面
public class GamePannel : MonoBehaviour
{
    private Button Btn_pause;//暂停按钮
    private Button Btn_play;//开始按钮
    private Text Txt_diamond;//钻石的显示
    private Text Txt_score;//得分的显示

    private void Awake() {
        //事件的监听   显示游戏内UI的事件监听
        EventCenter.AddListener(EventDefine.ShowGamePannnel,Show);
        //显示分数的事件监听
        EventCenter.AddListener<int>(EventDefine.ShowScore,ShowScore);//因为函数返回值为int   需要设置泛型为int

        EventCenter.AddListener<int>(EventDefine.ShowDiamond,ShowDiamond);
        Init();
    }

    private void Init(){
        //获取组件
        Btn_pause = transform.Find("Btn_pause").GetComponent<Button>();
        Btn_pause.onClick.AddListener(OnPauseButtonClick);

        Btn_play = transform.Find("Btn_play").GetComponent<Button>();
        Btn_play.onClick.AddListener(OnPlayButtonClick);

        Txt_diamond = transform.Find("Img_diamond/Txt_diamond").GetComponent<Text>();
        Txt_score = transform.Find("Txt_score").GetComponent<Text>();
        gameObject.SetActive(false);
        Btn_play.gameObject.SetActive(false);
    }

    private void OnPauseButtonClick(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        Btn_play.gameObject.SetActive(true);
        Btn_pause.gameObject.SetActive(false);
        Time.timeScale = 0;//游戏暂停
        GameManager.Instance.isPause =true;
    }

    private void OnPlayButtonClick(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        Btn_pause.gameObject.SetActive(true);
        Btn_play.gameObject.SetActive(false);
        Time.timeScale = 1;//游戏继续
        GameManager.Instance.isPause = false;
    }

    private void Show(){
        gameObject.SetActive(true);
    }

    private void ShowScore(int score){
        Txt_score.text = score.ToString();
    }

    private void ShowDiamond(int diamond){
        Txt_diamond.text = diamond.ToString();
    }

    //销毁时
    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.ShowGamePannnel,Show);
        EventCenter.RemoveListener<int>(EventDefine.ShowScore,ShowScore);
        EventCenter.RemoveListener<int>(EventDefine.ShowDiamond,ShowDiamond);
    }

}
