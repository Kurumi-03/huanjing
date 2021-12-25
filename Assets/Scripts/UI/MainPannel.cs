using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//主界面
public class MainPannel : MonoBehaviour
{
    //引入UI管理
    private Button btn_Start;//开始按钮
    private Button btn_Shop;//商店按钮
    private Button btn_Pank;//排行榜按钮
    private Button btn_Voice;//声音控制按钮
    private Button btn_Reset;//重置游戏数据按钮

    private ManagerVars vars;//游戏内数据存放
    private void Awake() {
        vars = ManagerVars.GetManagerVars();//数据文件初始化
        Init();//初始化
        EventCenter.AddListener(EventDefine.ShowMainPannel,Show);//显示主界面的方法注册
        EventCenter.AddListener<int>(EventDefine.ChangeSkin,changeSkin);
    }

    private void Start() {
        //再来一次的事件广播
        if(GameData.isRestartGame){
            gameObject.SetActive(false);
            EventCenter.Broadcast(EventDefine.ShowGamePannnel);
        }
        //在开始时即更换按钮皮肤
        changeSkin(GameManager.Instance.GetSelectedSkin());
        Sound();//初始化时即判断时是哪种显示图片
    }

    //商店按钮显示当前使用的皮肤的方法
    private void changeSkin(int skinIndex){
        btn_Shop.transform.GetChild(0).GetComponent<Image>().sprite = vars.skinSpriteList[skinIndex];
    }

    //显示MainPannel的方法
    private void Show(){
        gameObject.SetActive(true);
    }

    private void Init(){
        btn_Start = transform.Find("Btn_Start").GetComponent<Button>();//查找游戏物体的组件
        btn_Start.onClick.AddListener(OnStartButtonClick);//事件监听

        btn_Shop = transform.Find("Btns/Btn_Shop").GetComponent<Button>();
        btn_Shop.onClick.AddListener(OnShopButtonClick);//事件监听

        btn_Pank = transform.Find("Btns/Btn_Pank").GetComponent<Button>();
        btn_Pank.onClick.AddListener(OnPankButtonClick);//事件监听

        btn_Voice = transform.Find("Btns/Btn_Voice").GetComponent<Button>();
        btn_Voice.onClick.AddListener(OnVoiceButtonClick);//事件监听

        btn_Reset = transform.Find("Btns/Btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(OnResetButtonClick);//事件监听     

    }

    //点击开始按钮
    private void OnStartButtonClick(){
        GameManager.Instance.isGameStart = true;
        EventCenter.Broadcast(EventDefine.ShowGamePannnel);
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        gameObject.SetActive(false);
    }
    
    //点击商店按钮
    private void OnShopButtonClick(){
        EventCenter.Broadcast(EventDefine.ShowShopPannel);
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        gameObject.SetActive(false);
    }

    //点击排行榜按钮
    private void OnPankButtonClick(){
        EventCenter.Broadcast(EventDefine.ShowRankPannel);
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
    }

    //点击声音按钮
    private void OnVoiceButtonClick(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        //将已存入的数据取反  即可达成点击切换
        GameManager.Instance.SetMusicOn(!GameManager.Instance.GetIsMusicOn());
        Sound();
    }

    //音效的显示判断
    private void Sound(){
        //当音效开启时  显示开启时的图标
        if(GameManager.Instance.GetIsMusicOn()){
            btn_Voice.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOn;
        }
        else{
            btn_Voice.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOff;
        }
        //将音效的开启与关闭传入音效控制脚本
        EventCenter.Broadcast(EventDefine.IsMouseOn,GameManager.Instance.GetIsMusicOn());
    }

    //点击重置按钮
    private void OnResetButtonClick(){
        EventCenter.Broadcast(EventDefine.ShowResetPannel);
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
    }

    //销毁时
    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.ShowMainPannel,Show);//方法销毁
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin,changeSkin);
    }
}


















