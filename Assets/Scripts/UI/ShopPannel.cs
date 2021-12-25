using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//商店界面
public class ShopPannel : MonoBehaviour
{
    private ManagerVars vars;
    private Transform parent;//皮肤的管理器
    private Text txt_Name;//显示皮肤名称的Text
    private Button btn_Back;//获取到返回按钮
    private Button btn_Select;//获取到选择皮肤按钮
    private Button btn_Buy;//获取得到购买皮肤按钮
    private Text txt_diamond;//获取到显示钻石的文本框

    private int index;//选中的皮肤编号

    private void Awake() {
        EventCenter.AddListener(EventDefine.ShowShopPannel,Show);//显示商店界面的方法注册
        vars = ManagerVars.GetManagerVars();
        parent = transform.Find("ScrollRect/Parent");//查找parent物体
        txt_Name = transform.Find("txt_Name").GetComponent<Text>();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Select = transform.Find("btn_Select").GetComponent<Button>();
        btn_Buy =transform.Find("btn_Buy").GetComponent<Button>();
        txt_diamond = transform.Find("Diamond/text").GetComponent<Text>();

        btn_Back.onClick.AddListener(OnClickBack);//注册按钮点击函数
        btn_Buy.onClick.AddListener(OnClickBuy);
        btn_Select.onClick.AddListener(OnClickSelect);
    }

    //在start方法内初始化可以避免GameData也初始化没有数据的情况
    private void Start() {
        Init();
        gameObject.SetActive(false);//初始化隐藏
    }

    private void Show(){
        gameObject.SetActive(true);
    }

    //初始化
    private void Init(){
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.platformList.Count+2)*160,300);
        for(int i = 0 ; i < vars.skinSpriteList.Count ; i++ ){
            GameObject go = Instantiate(vars.chooseItemPre,parent);//创建皮肤并将皮肤设置为parent的子物体
            //未解锁皮肤时
            if(!GameManager.Instance.GetIsUnlockedSkin(i)){
                go.GetComponentInChildren<Image>().color = Color.gray;//将皮肤设置为灰色
            }
            else{
                go.GetComponentInChildren<Image>().color = Color.white;
            }
            go.GetComponentInChildren<Image>().sprite = vars.skinSpriteList[i];//获取皮肤上的子物体的image组件并皮肤精灵赋值
            go.transform.localPosition = new Vector3((i+1)*160,0,0);//设置皮肤位置
        }
        //在进入商店时即显示当前所选的皮肤
        parent.transform.localPosition = new Vector3(GameManager.Instance.GetSelectedSkin()*-160,0,0);
    }

    private void Update() {
        //使用当前位置除以间隔即可得出皮肤序号
        index = (int)Mathf.Round(parent.transform.localPosition.x/-160.0f);
        //防止index超出索引界限
        if(index <= 0){
            index = 0;
        }else if(index >= 3){
            index = 3;
        }
        //当鼠标左键抬起时   使皮肤悬停在某一位置
        if(Input.GetMouseButtonUp(0)){
            //使用DoTween 缓慢播放动画  增加效果 (在0.3秒内移动到index*-160的位置)
            parent.transform.DOLocalMoveX(index*-160,0.5f);
            //parent.transform.localPosition = new Vector2(index*-160,0);
        }
        SetSkin(index);
        SetButton(index);
    }

    //设置皮肤的属性
    private void SetSkin(int index){
        //遍历parent的子物体
        for(int i = 0 ; i < parent.childCount ; i++){
            if(index == i){
                //获取到parent的子物体的子物体再通过RectTranform修改大小
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
            }
            else{
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(80,80);
            }
        }
        //设置皮肤名称
        txt_Name.text = vars.skinNameList[index];
        //设置钻石总数量
        txt_diamond.text = GameManager.Instance.GetDiamondCount().ToString();
    }

    //设置按钮
    private void SetButton(int index){
        //当皮肤为解锁时  显示购买按钮  隐藏选择按钮
        if(!GameManager.Instance.GetIsUnlockedSkin(index)){
            btn_Buy.gameObject.SetActive(true);
            btn_Select.gameObject.SetActive(false);
            //在button的children下查找text组件并设置皮肤价格
            btn_Buy.GetComponentInChildren<Text>().text = vars.skinPrice[index].ToString();
        }
        else{
            btn_Buy.gameObject.SetActive(false);
            btn_Select.gameObject.SetActive(true);
        }
    }



    //返回按钮的方法
    private void OnClickBack(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        gameObject.SetActive(false);
        EventCenter.Broadcast(EventDefine.ShowMainPannel);//广播显示主界面的方法
    }

    //按下购买皮肤的按钮的方法
    private void OnClickBuy(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        int price =int.Parse(btn_Buy.GetComponentInChildren<Text>().text);//获取皮肤的价格
        //判断皮肤价格是否大于总钻石数
        if(price > GameManager.Instance.GetDiamondCount()){
            EventCenter.Broadcast(EventDefine.Hint,"钻石不足，无法购买");//将提示信息传入show方法
            Debug.Log("error");//钻石数量不足的错误提示
            return;
        }
        //总钻石数减去皮肤价格
        GameManager.Instance.SetDiamondCount(-price);
        //从皮肤管理器中查询选中的皮肤再寻找皮肤的子物体来修改颜色
        parent.GetChild(index).GetChild(0).GetComponent<Image>().color = Color.white;
        //将皮肤从未解锁的数组中解锁
        GameManager.Instance.SetIsUnlockedSkin(index);
    }

    //改变皮肤按钮
    private void OnClickSelect(){
        EventCenter.Broadcast(EventDefine.PlayAudio);//音效的播放
        EventCenter.Broadcast(EventDefine.ChangeSkin,index);
        EventCenter.Broadcast(EventDefine.ShowMainPannel);
        GameManager.Instance.SetSelectedSkin(index);//将当前选中的皮肤设置并存储
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.ShowShopPannel,Show);//方法销毁
    }
}
