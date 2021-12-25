using System.Collections;
using System.Collections.Generic;
//管理数据类的命名空间
using System.Runtime.Serialization.Formatters.Binary;
//文件流的命名空间
using System.IO;
using UnityEngine;
using System.Linq;//按大小排列的方法

//游戏的控制和管理
public class GameManager : MonoBehaviour
{
    private ManagerVars vars;
    public static GameManager Instance;//单例

    public bool isGameStart {get; set;}//游戏是否开始

    public bool isGameOver {get; set;}//游戏是否结束

    public bool isPause {get; set;}//游戏是否暂停

    public bool isPlayerMove{get; set;}//玩家是否移动

    private int score = 0;//分数

    private int diamond;//钻石数
    private GameData data;//游戏数据的存储类
    //需要存储的数据
    private bool isFirstGame;//是否是第一次游戏
    private bool isMusicOn;//声音是否打开
    private int[] bestScore;//存储最好成绩的数组
    private int selectedSkin;//当前选中的皮肤序号
    private bool[] unlockedSkin;//未解锁的皮肤
    private int diamondCount;//积累的钻石数量

    private void Awake() {
        vars = ManagerVars.GetManagerVars();
        Instance =this;
        EventCenter.AddListener(EventDefine.AddScore,AddScore);//添加分数增加的事件监听
        EventCenter.AddListener(EventDefine.PlayerMove,PlayerMove);//添加玩家移动方法的事件监听
        EventCenter.AddListener(EventDefine.AddDiamond,AddDiamond);

        //当游戏重新开始时
        if(GameData.isRestartGame){
            isGameStart = true;
        }

        //data = new GameData();//数据的对象的初始化  
        InitData();//初始化游戏数据后即可不建立GameData对象 
    }


    private void AddScore(){
        //当游戏未开始   游戏结束  游戏暂停 时直接返回不计数
        if(!isGameStart || isGameOver || isPause)  return;
        score++;
        EventCenter.Broadcast<int>(EventDefine.ShowScore,score);//ShowScore具有参数需要传递参数
    }

    //更新钻石数量
    private void AddDiamond(){
        diamond++;
        EventCenter.Broadcast(EventDefine.ShowDiamond,diamond);
    }

    private void PlayerMove(){
        isPlayerMove = true;
    }

    //获取游戏进度即分数
    public int GetScore(){
        return score;
    }

    //获取本局钻石数量
    public int GetDiamond(){
        return diamond;
    }

    //获取全部获得的钻石
    public int GetDiamondCount(){
        return diamondCount;
    }

    //修改钻石数量
    public void SetDiamondCount(int value){
        diamondCount += value;
        Save();//涉及到数据更改的方法都需要保存
    }

    //获取当前皮肤是否未解锁
    public bool GetIsUnlockedSkin(int index){
        return unlockedSkin[index];
    }

    //设置当前皮肤是否解锁
    public void SetIsUnlockedSkin(int index){
        unlockedSkin[index] = true;     
        Save();
    }

    //获取当前选中的皮肤下标
    public int GetSelectedSkin(){
        return selectedSkin;
    }
    //设置当前选中的皮肤下标
    public void SetSelectedSkin(int index){
        selectedSkin = index;
        Save();
    }

    //获取最高分
    public int GetBestScore(){
        return bestScore.Max();
    }

    //获取最高分数组
    public int[] GetBestScoreArr(){
        //可能数组未排序   需要先排序
        //使用linq排列大小需要将数组转换为list
        List<int> list = bestScore.ToList();
        //从大到小排序   使用lambda表达式  加上-为降序
        list.Sort((x,y) => (-x.CompareTo(y)));
        //将列表转换回数组
        bestScore = list.ToArray();

        return bestScore;
    }

    //设置声音的开启
    public void SetMusicOn(bool isMouseOn){
        this.isMusicOn = isMouseOn;
        Save();
    }

    //获取声音的开启与否
    public bool GetIsMusicOn(){
        return isMusicOn;
    }

    //保存游戏最高分
    public void SaveScore(int score){
        //使用linq排列大小需要将数组转换为list
        List<int> list = bestScore.ToList();
        //从大到小排序   使用lambda表达式  加上-为降序
        list.Sort((x,y) => (-x.CompareTo(y)));
        //将列表转换回数组
        bestScore = list.ToArray();

        //当传入的得分大于当前的最大值则记录当前最大值的数组下标
        int index = -1;
        for(int i = 0 ; i < bestScore.Length; i++){
            if(score>bestScore[i]){
                index = i;
            }
        }
        //如果未赋值  即当前分数小于最大值  直接返回
        if(index == -1) return;
        
        //从后向前查找
        for(int i=bestScore.Length-1;i>index;i++){
            bestScore[i-1] = bestScore[i];//将后一个的值赋给前一个
        }
        bestScore[index] = score;

        Save();//保存数据
    }

    //保存游戏数据到本地
    private void Save(){
        try
        {
            BinaryFormatter bf = new BinaryFormatter();//序列化需要使用的类
            //创建文件写入流
            //使用uisng可以自动关闭写入流
            //Application.persistentDataPath是默认数据存放地址
            using(FileStream fs = File.Create(Application.persistentDataPath + "/GameData.data") ){
                //初始化数据
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetSelectedSkin(selectedSkin);
                data.SetUnlockedSkin(unlockedSkin);
                data.SetBestScore(bestScore);
                data.SetDiamondCount(diamondCount);
                bf.Serialize(fs,data);//将data类序列化并转化为文件数据流
            }
        }
        catch (System.Exception e)
        {
            Debug.Log (e.Message);//打印出错信息
        }
    }

    //加载数据
    private void Load(){
        try
        {
            BinaryFormatter bf = new BinaryFormatter();//序列化需要使用的类
            //创建文件读取流
            using(FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data",FileMode.Open) ){
                data =(GameData)bf.Deserialize(fs);//将数据反序列化并强转为Gamedata类型
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    //初始化游戏数据
    private void InitData(){
        Load();//先读入一遍本地数据
        //如data为空 证明没有读取到本地的数据文件 即第一次开始游戏
        if(data != null){
            isFirstGame = data.GetIsFirstGame();
        }else{
            isFirstGame = true;
        }
        //当游戏是第一次开始时
        if(isFirstGame){
            isFirstGame = false;//开始游戏后即为第二次开始游戏
            isMusicOn = true;//游戏音乐默认开启
            bestScore = new int[3];//只计入前3名的成绩
            selectedSkin = 0;//默认使用0号皮肤
            unlockedSkin = new bool[vars.skinSpriteList.Count];//默认皮肤数
            unlockedSkin[0] = true;//默认使用第一个皮肤
            diamondCount = 0;//默认钻石数为10
            data = new GameData();//因为 data=null 才能进入判断 所以需要new一个对象
            Save();//保存数据
        }else{
            isMusicOn = data.GetIsMouseOn();
            bestScore = data.GetBestScore();
            selectedSkin = data.GetSelectedSkin();
            unlockedSkin = data.GetUnlockedSkin();
            diamondCount = data.GetDiamondCount();
        }
    }

    //重置游戏数据
    public void ResetData(){
        isFirstGame = false;//开始游戏后即为第二次开始游戏
            isMusicOn = true;//游戏音乐默认开启
            bestScore = new int[3];//只计入前3名的成绩
            selectedSkin = 0;//默认使用0号皮肤
            unlockedSkin = new bool[vars.skinSpriteList.Count];//默认皮肤数
            unlockedSkin[0] = true;//默认使用第一个皮肤
            diamondCount = 0;//默认钻石数为10
            //因为重置时已经有了data实例  所以不需要new
            Save();//保存数据
    }


    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.AddScore,AddScore);//当游戏结束时删除事件监听
        EventCenter.RemoveListener(EventDefine.PlayerMove,PlayerMove);
        EventCenter.RemoveListener(EventDefine.AddDiamond,AddDiamond);
    }
}
