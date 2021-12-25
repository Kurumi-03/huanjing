using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//游戏结束后结算界面
public class GameOverPannel : MonoBehaviour
{
    public Text txt_Score,txt_BestScore,txt_diamond;//得分  最高分  钻石的文本引用
    public Button btn_restart,btn_Rank,btn_Home;//重新开始  排行表   返回主菜单的按钮引用

    public Image img_New;//new图标的获取

    private void Awake() {
        //按钮的时间监听注册
        btn_restart.onClick.AddListener(OnClickRestartButton);
        btn_Rank.onClick.AddListener(OnClickRankButton);
        btn_Home.onClick.AddListener(OnClickHomeButton);
        EventCenter.AddListener(EventDefine.ShowGameOverPannnel,Show);
        gameObject.SetActive(false);
    }

    private void Show(){
        //当前得分与最高分的对比
        if(GameManager.Instance.GetScore() > GameManager.Instance.GetBestScore()){
            txt_BestScore.text = "最高分为："+GameManager.Instance.GetScore();
            img_New.gameObject.SetActive(true);//最高分为当前得分时显示
        }
        else{
            txt_BestScore.text = "最高分为："+GameManager.Instance.GetBestScore();
            img_New.gameObject.SetActive(false);
        }
        //保存当前成绩
        GameManager.Instance.SaveScore(GameManager.Instance.GetScore());
        txt_Score.text = GameManager.Instance.GetScore().ToString();//使用GameManager的实例方法获取分数
        //最高分
        txt_diamond.text ="+" + GameManager.Instance.GetDiamond().ToString();
        //更新总的钻石数
        GameManager.Instance.SetDiamondCount(GameManager.Instance.GetDiamond());

        gameObject.SetActive(true);//显示结束面板
    }

    // 再来一次按钮点击
    private void OnClickRestartButton(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.isRestartGame = true;
    }

    //排行榜按钮点击
    private void OnClickRankButton(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        EventCenter.Broadcast(EventDefine.ShowRankPannel);
    }

    //home按钮点击
    private void OnClickHomeButton(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        //获取到当前正在加载的场景并重新加载
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.isRestartGame = false;
    }


    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.ShowGameOverPannnel,Show);
        btn_restart.onClick.RemoveListener(OnClickRestartButton);
        btn_Rank.onClick.RemoveListener(OnClickRankButton);
        btn_Home.onClick.RemoveListener(OnClickHomeButton);
    }
}
