using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏对象池  控制游戏数据
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;//单例
    public int initSpawnCount = 5;//初始生成数量

    //收容平台的列表
    private List<GameObject> normalPlatformList = new List<GameObject>();
    private List<GameObject> commonPlatformList = new List<GameObject>();
    private List<GameObject> grassPlatformList = new List<GameObject>();
    private List<GameObject> winterPlatformList = new List<GameObject>();
    private List<GameObject> spikePlatformLeftList = new List<GameObject>();
    private List<GameObject> spikePlatformRightList = new List<GameObject>();

    //收容死亡特效的列表
    private List<GameObject> deathEffectList = new List<GameObject>();

    //收容钻石的列表
    private List<GameObject> diamondList =new List<GameObject>();

    private ManagerVars vars;

    private void Awake()
    {
        Instance = this;//单例的初始化
        vars = ManagerVars.GetManagerVars();
        Init();
    }

    //初始化  将初始的5个平台加入列表中管理
    // ref关键字可将传入的参数也进行修改
    private void Init()
    {
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.normalPlatformPre, ref normalPlatformList);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            for (int j = 0; j < vars.platformsCommon.Count; j++)
            {
                InstantiateObject(vars.platformsCommon[j], ref commonPlatformList);
            }
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            for (int j = 0; j < vars.platformsGrass.Count; j++)
            {
                InstantiateObject(vars.platformsGrass[j], ref grassPlatformList);
            }
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            for (int j = 0; j < vars.platformsWinter.Count; j++)
            {
                InstantiateObject(vars.platformsWinter[j], ref winterPlatformList);
            }
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.platformSpikeLeft, ref spikePlatformLeftList);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.platformSpikeRight, ref spikePlatformRightList);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.deathEffect, ref deathEffectList);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.diamondPrefab, ref diamondList);
        }
    }

    private GameObject InstantiateObject(GameObject prefab, ref List<GameObject> addList)
    {
        GameObject go = Instantiate(prefab, transform);
        go.SetActive(false);
        addList.Add(go);
        return go;
    }

    //获取方法
    public GameObject GetNormalPlatform()
    {
        for (int i = 0; i < normalPlatformList.Count; i++)
        {
            if (normalPlatformList[i].activeInHierarchy == false)
            {
                return normalPlatformList[i];
            }
        }
        return InstantiateObject(vars.normalPlatformPre, ref normalPlatformList);
    }

    public GameObject GetCommonPlatforms()
    {
        for (int i = 0; i < commonPlatformList.Count; i++)
        {
            if (commonPlatformList[i].activeInHierarchy == false)
            {
                return commonPlatformList[i];
            }
        }
        int ran = Random.Range(0, commonPlatformList.Count);
        return InstantiateObject(vars.platformsCommon[ran], ref commonPlatformList);
    }

    public GameObject GetGrassPlatforms()
    {
        for (int i = 0; i < grassPlatformList.Count; i++)
        {
            if (grassPlatformList[i].activeInHierarchy == false)
            {
                return grassPlatformList[i];
            }
        }
        int ran = Random.Range(0, grassPlatformList.Count);
        return InstantiateObject(vars.platformsGrass[ran], ref grassPlatformList);
    }

    public GameObject GetWinterPlatforms()
    {
        for (int i = 0; i < winterPlatformList.Count; i++)
        {
            if (winterPlatformList[i].activeInHierarchy == false)
            {
                return winterPlatformList[i];
            }
        }
        int ran = Random.Range(0, winterPlatformList.Count);
        return InstantiateObject(vars.platformsWinter[ran], ref winterPlatformList);
    }

    public GameObject GetSpikeLeftPlatform()
    {
        for (int i = 0; i < spikePlatformLeftList.Count; i++)
        {
            if (spikePlatformLeftList[i].activeInHierarchy == false)
            {
                return spikePlatformLeftList[i];
            }
        }
        return InstantiateObject(vars.platformSpikeLeft, ref spikePlatformLeftList);
    }

    public GameObject GetSpikeRightPlatform()
    {
        for (int i = 0; i < spikePlatformRightList.Count; i++)
        {
            if (spikePlatformRightList[i].activeInHierarchy == false)
            {
                return spikePlatformRightList[i];
            }
        }
        return InstantiateObject(vars.platformSpikeRight, ref spikePlatformRightList);
    }

    public GameObject GetDeathEffect()
    {
        for (int i = 0; i < deathEffectList.Count; i++)
        {
            if (deathEffectList[i].activeInHierarchy == false)
            {
                return deathEffectList[i];
            }
        }
        return InstantiateObject(vars.deathEffect, ref deathEffectList);
    }

    public GameObject GetDiamond()
    {
        for (int i = 0; i < diamondList.Count; i++)
        {
            if (diamondList[i].activeInHierarchy == false)
            {
                return diamondList[i];
            }
        }
        return InstantiateObject(vars.diamondPrefab, ref diamondList);
    }
}
