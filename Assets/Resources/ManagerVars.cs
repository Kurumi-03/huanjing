using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//创建菜单
//[CreateAssetMenu(menuName ="CreatManagerVarsContainer")]
public class ManagerVars : ScriptableObject
{
    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("ManagerVarsContainer");
    }
    public List<Sprite> bg = new List<Sprite>();//背景的切换

    public float nextXpos = 0.55f, nextYpos = 0.65f;//平台间隔

    public GameObject normalPlatformPre;//常用平台预制体

    public GameObject deathEffect;//死亡特效

    public List<GameObject> platformsCommon = new List<GameObject>();//通用组合平台
    public List<GameObject> platformsWinter = new List<GameObject>();//冬天主题组合平台
    public List<GameObject> platformsGrass = new List<GameObject>();//草地主题组合平台

    public GameObject platformSpikeLeft;//左边有钉子的平台预制体
    public GameObject platformSpikeRight;//右边有钉子的平台预制体

    public GameObject playerPre;//玩家预制体

    public List<Sprite> platformList = new List<Sprite>();//随机平台主题 

    public GameObject diamondPrefab;//钻石预制体

    public List<Sprite> skinSpriteList = new List<Sprite>();//皮肤的精灵列表
    public GameObject chooseItemPre;//皮肤的预制体
    public List<string> skinNameList = new List<string>();//皮肤名称列表
    public List<int> skinPrice = new List<int>();//皮肤价格列表
    public List<Sprite> skinSpriteBackList = new List<Sprite>();//皮肤的背面精灵列表

    //定义各种声音源  碰到障碍物，跳跃，坠落，点击按钮，吃到钻石
    public AudioClip hitClip,jumpClip,fallClip,buttonClip,diamondClip;

    public Sprite musicOn,musicOff;//音效按钮的图片切换
}
