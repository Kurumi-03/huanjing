using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//排行榜界面
public class RankPannel : MonoBehaviour
{
    private Button btn_Close;//背景上的关闭按钮
    private Text[] txt_Score;//分数文本
    private GameObject scoreList;//管理分数的游戏对象

    private void Awake() {
        EventCenter.AddListener(EventDefine.ShowRankPannel,Show);

        btn_Close = transform.Find("btn_Back").GetComponent<Button>();
        btn_Close.onClick.AddListener(OnClickBackButton);
        txt_Score = gameObject.GetComponentsInChildren<Text>();

        scoreList = transform.Find("Score").gameObject;
        
        //将背景和显示都先隐藏
        btn_Close.GetComponent<Image>().color = new Color(btn_Close.GetComponent<Image>().color.r,btn_Close.GetComponent<Image>().color.g,btn_Close.GetComponent<Image>().color.b,0);
        scoreList.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void Show(){
        //在0.4秒内将透明度改为0.3
        btn_Close.GetComponent<Image>().DOColor(new Color(btn_Close.GetComponent<Image>().color.r,btn_Close.GetComponent<Image>().color.g,btn_Close.GetComponent<Image>().color.b,0.3f),0.4f);
        //在0.4秒内将大小变为原来大小
        scoreList.transform.DOScale(Vector3.one,0.4f);
        gameObject.SetActive(true);

        //获取最高分的数组
        int[] arr = GameManager.Instance.GetBestScoreArr();
        for(int i=0;i<arr.Length;i++){
            txt_Score[i].text = arr[i].ToString();
        }
    }

    private void OnClickBackButton(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        btn_Close.GetComponent<Image>().DOColor(new Color(btn_Close.GetComponent<Image>().color.r,btn_Close.GetComponent<Image>().color.g,btn_Close.GetComponent<Image>().color.b,0),0.4f);
        //在完成隐藏任务后将游戏物体隐藏
        scoreList.transform.DOScale(Vector3.zero,0.4f).OnComplete(() => {
            gameObject.SetActive(false);
        });
        EventCenter.Broadcast(EventDefine.ShowMainPannel);
    }

    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.ShowRankPannel,Show);
    }
}
