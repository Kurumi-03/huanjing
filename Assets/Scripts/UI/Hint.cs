using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;//移动

//UI提示弹窗
public class Hint : MonoBehaviour
{
    private Image img_bg;
    private Text txt_Hint;
    private void Awake() {
        img_bg = GetComponent<Image>();//唤醒时即获取组件
        txt_Hint = GetComponentInChildren<Text>();

        //设置rgba颜色透明度为0  即初始时隐藏
        img_bg.color = new Color(img_bg.color.r,img_bg.color.g,img_bg.color.b,0);
        txt_Hint.color = new Color(txt_Hint.color.r,txt_Hint.color.g,txt_Hint.color.b,0);

        EventCenter.AddListener<string>(EventDefine.Hint,Show);//show方法有String类型参数  需要加上泛型
    }

    private void Show(string text){
        StopCoroutine("Delay");
        transform.localPosition = new Vector3(0,-70,0);
        //在0.3f秒内移动到y为0的位置   
        //使用lambda表达式写匿名方法延时调用  在完成移动动作后
        transform.DOLocalMoveY(0,0.3f).OnComplete(() => {
            StartCoroutine("Delay");
        });
        //重新显示  因为是color类型 全为0~1的值
        img_bg.color = new Color(img_bg.color.r,img_bg.color.g,img_bg.color.b,0.4f);
        txt_Hint.color = new Color(txt_Hint.color.r,txt_Hint.color.g,txt_Hint.color.b,1.0f);
        txt_Hint.text = text;
    }

    //延时调用方法
    private IEnumerator Delay(){
        yield return new WaitForSeconds(1.0f);//等待1秒
        transform.DOLocalMoveY(70,0.3f);
        //再次隐藏
        img_bg.color = new Color(img_bg.color.r,img_bg.color.g,img_bg.color.b,0);
        txt_Hint.color = new Color(txt_Hint.color.r,txt_Hint.color.g,txt_Hint.color.b,0);
    }

    private void OnDestroy() {
        EventCenter.RemoveListener<string>(EventDefine.Hint,Show);
    }
}
