using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//平台生成
public enum PlatformType{
    //定义一个枚举的平台类型  Winter 或 Grass
    Grass,
    Winter
}
public class PlatformSpawner : MonoBehaviour
{
    public Vector3 startSpawnPos;//初始生成位置

    private ManagerVars vars;

    private int platformNum;//平台数量
    private Vector3 platformPos;//平台生成位置
    private bool isLeftSpawn = false;//判断生成位置的方向  初始向右

    private Sprite selectPlatform;//接收随机平台的选择
    private PlatformType platformType;//平台类型

    private bool isSpikePlatformLeft = false;//判断钉子平台生成的方向  默认为右
    private Vector3 spikeDirPlatformPos;//钉子平台生成方向位置  用以创建后续平台
    private int afterSpikePlatformNum;//钉子平台后续平台生成数量
    private bool isSpikeSpawn ;//是否生成钉子平台 

    public int scoreCount = 10;//当分数到达10时开始加速
    public float fallTime;//掉落时间
    public float minfallTime;//最小掉落时间
    public float mutiple;//计算系数

    private void Awake() {
        EventCenter.AddListener(EventDefine.DecidePath,DecidePath);
        vars = ManagerVars.GetManagerVars();
    }

    private void Start() {
        RandomPlatform();
        platformPos = startSpawnPos;
        //生成初始5个平台
        for(int i=0;i<5;i++){
            platformNum = 5;
            DecidePath();
        }

        //生成人物
        GameObject go = Instantiate(vars.playerPre);
        go.transform.position = new Vector3(0,-2f,0);
    }

    private void Update() {
        if(GameManager.Instance.isGameStart && !GameManager.Instance.isGameOver){
            UpdateFallTime();
        }
    }

    //确定移动路径
    private void DecidePath(){
        //当要生成钉子平台时   执行生成两条路径的方法
        if(isSpikeSpawn){
            afterSpawnSpike();
            return;
        }
        if(platformNum > 0){
            platformNum--;
            SpawnPlatform();
        }
        else{
            isLeftSpawn = !isLeftSpawn;//转向
            platformNum = Random.Range(1,4);
            SpawnPlatform();
        }
    }

    //随机平台主题
    private void RandomPlatform(){
        int ran = Random.Range(0,vars.platformList.Count);
        selectPlatform = vars.platformList[ran];
        if(ran == 2){
            platformType = PlatformType.Winter;
        }
        else
        {
            platformType = PlatformType.Grass;
        }
    }

    //生成平台
    private void SpawnPlatform(){
        int ranObstacleDir = Random.Range(0,2);//随机左右方向值  0,1 
        if(platformNum >= 1){
            SpawnNormalPlatform(ranObstacleDir);
        }
        //当平台数量为0时 即最后一个平台 生成组合平台
        else if(platformNum == 0){
            int randomTheme = Random.Range(0,3);
            //生成通用组合平台
            if (randomTheme == 0)
            {
                SpawnCommonPlatforms(ranObstacleDir);
            }
            //生成主题组合平台  (Winter，Grass)
            else if(randomTheme == 1){
                switch(platformType){
                    case PlatformType.Winter :
                        SpawnWinterPlatforms(ranObstacleDir);
                        break;
                    case PlatformType.Grass:
                        SpawnGrassPlatforms(ranObstacleDir);
                        break;
                    default:
                        break; 
                }
            }
            //生成钉子组合平台
            else{
                int dirValue = -1;//方向值  初始默认为-1
                if(isLeftSpawn){
                    dirValue = 0;//向右
                }
                else{
                    dirValue = 1;//向左
                }
                SpawnSpikePlatform(dirValue);

                isSpikeSpawn = true;
                afterSpikePlatformNum = 4;
                //获取钉子平台后续平台的生成位置
                if(isSpikePlatformLeft){
                    spikeDirPlatformPos = new Vector3(platformPos.x - 1.65f,platformPos.y+vars.nextYpos,0);

                }
                else{
                    spikeDirPlatformPos = new Vector3(platformPos.x + 1.65f,platformPos.y+vars.nextYpos,0);
                }
            }
        }

        //生成钻石   当随机到6且玩家在移动时才会生成
        int random = Random.Range(0,10);
        if(random >= 6 && GameManager.Instance.isPlayerMove){
            GameObject go = ObjectPool.Instance.GetDiamond();
            go.transform.position = new Vector3(platformPos.x,platformPos.y+0.55f,platformPos.z);
            go.SetActive(true);
        }

        if(isLeftSpawn){
            platformPos = new Vector3(platformPos.x - vars.nextXpos,platformPos.y + vars.nextYpos,0);
        }
        else{
            platformPos = new Vector3(platformPos.x + vars.nextXpos,platformPos.y + vars.nextYpos,0);
        }
    }

    //普通平台的生成(单个)
    private void SpawnNormalPlatform(int ranObstacleDir){
        GameObject go = ObjectPool.Instance.GetNormalPlatform();
        go.transform.position = platformPos;
        go.GetComponent<PlatformScripte>().Init(selectPlatform,ranObstacleDir,fallTime);
        go.SetActive(true);
    }

    //通用组合平台的生成
    private void SpawnCommonPlatforms(int ranObstacleDir){
        GameObject go = ObjectPool.Instance.GetCommonPlatforms();
        go.transform.position = platformPos;
        go.GetComponent<PlatformScripte>().Init(selectPlatform,ranObstacleDir,fallTime);
        go.SetActive(true);
    }

    //主题组合平台的生成(草地)
    private void SpawnGrassPlatforms(int ranObstacleDir){
        GameObject go = ObjectPool.Instance.GetGrassPlatforms();
        go.transform.position = platformPos;
        go.GetComponent<PlatformScripte>().Init(selectPlatform,ranObstacleDir,fallTime);
        go.SetActive(true);
    }

    //主题组合平台的生成(冬天)
    private void SpawnWinterPlatforms(int ranObstacleDir){
        GameObject go = ObjectPool.Instance.GetWinterPlatforms();
        go.transform.position = platformPos;
        go.GetComponent<PlatformScripte>().Init(selectPlatform,ranObstacleDir,fallTime);
        go.SetActive(true);
    }

    //生成钉子组合平台
    private void SpawnSpikePlatform(int dir){
        GameObject temp;
        if(dir == 0){
            isSpikePlatformLeft = false;
            temp = ObjectPool.Instance.GetSpikeRightPlatform();
        }
        else{
            isSpikePlatformLeft = true;
            temp = ObjectPool.Instance.GetSpikeLeftPlatform();
        }
        temp.transform.position = platformPos;
        temp.GetComponent<PlatformScripte>().Init(selectPlatform,dir,fallTime);
        temp.SetActive(true);
    }

    //钉子平台后续平台的生成
    private void afterSpawnSpike(){
        if(afterSpikePlatformNum > 0){
            afterSpikePlatformNum--;
            for(int i = 0;i < 2;i++){
                GameObject temp = ObjectPool.Instance.GetNormalPlatform();
                //生成原来方向的平台
                if(i == 0){
                    temp.transform.position = platformPos;
                    //当钉子平台生成在左边时   原来路径就在右边
                    if(isSpikePlatformLeft){
                        platformPos = new Vector3(platformPos.x + vars.nextXpos,platformPos.y + vars.nextYpos,0);
                    }
                    else{
                        platformPos = new Vector3(platformPos.x - vars.nextXpos,platformPos.y + vars.nextYpos,0);
                    }
                }
                //生成钉子方向的平台
                else{
                    temp.transform.position = spikeDirPlatformPos;
                    if(isSpikePlatformLeft){
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x - vars.nextXpos,spikeDirPlatformPos.y + vars.nextYpos,0);
                    }
                    else{
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x + vars.nextXpos,spikeDirPlatformPos.y + vars.nextYpos,0);
                    }
                }

                temp.GetComponent<PlatformScripte>().Init(selectPlatform,1,fallTime);
                temp.SetActive(true);
            }
        }
        else{
            isSpikeSpawn = false;
            DecidePath();
        }
    }

    //更新平台掉落时间
    private void UpdateFallTime(){
        if(GameManager.Instance.GetScore() > scoreCount){
            scoreCount*=2;//下一次翻倍
            fallTime*=mutiple;
            if(fallTime < minfallTime){
                fallTime = minfallTime;
            }
        }
    }


    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.DecidePath,DecidePath);
    }
}
