using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏数据的存放 控制 获取

//进行数据序列化的标签
[System.Serializable]
public class GameData
{
    public static bool isRestartGame = false;//是否重新开始游戏

    //游戏内需存放的数据 
    private bool isFirstGame;//是否是第一次游戏
    private bool isMusicOn;//声音是否打开
    private int[] bestScore;//存储最好成绩的数组
    private int selectedSkin;//当前选中的皮肤序号
    private bool[] unlockedSkin;//未解锁的皮肤
    private int diamondCount;//积累的钻石数量

    //数据赋值方法
    public void SetIsFirstGame(bool isFirstGame){
        this.isFirstGame = isFirstGame;
    }

    public void SetIsMusicOn(bool isMusicOn){
        this.isMusicOn = isMusicOn;
    }

    public void SetBestScore(int[] bestScore){
        this.bestScore = bestScore;
    }

    public void SetSelectedSkin(int selectedSkin){
        this.selectedSkin = selectedSkin;
    }

    public void SetUnlockedSkin(bool[] unlockedSkin){
        this.unlockedSkin = unlockedSkin;
    }

    public void SetDiamondCount(int diamondCount){
        this.diamondCount += diamondCount;
    }

    //获取方法
    public bool GetIsFirstGame(){
        return isFirstGame;
    }

    public bool GetIsMouseOn(){
        return isMusicOn;
    }

    public int[] GetBestScore(){
        return bestScore;
    }

    public int GetSelectedSkin(){
        return selectedSkin;
    }

    public bool[] GetUnlockedSkin(){
        return unlockedSkin;
    }

    public int GetDiamondCount(){
        return diamondCount;
    }
}
