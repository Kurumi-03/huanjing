using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//平台控制(不是所有平台都添加了脚本)
public class PlatformScripte : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public GameObject obstacle;//障碍

    private bool startTimer;//开始计时
    private float fallTime;//掉落时间

    private Rigidbody2D body;//获取平台的rigidbody组件

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    public void Init(Sprite s , int obstacleDir , float fallTime){
        body.bodyType = RigidbodyType2D.Static;
        startTimer = true;
        this.fallTime = fallTime;

        for(int i=0;i<spriteRenderers.Length;i++){
            spriteRenderers[i].sprite = s;
        }
        //y右边
        if (obstacleDir == 0)
        {
            if (obstacle != null)
            {
                obstacle.transform.position = new Vector3(-obstacle.transform.position.x,obstacle.transform.position.y,obstacle.transform.position.z);
                
            }
        }
    }

    private void Update() {
        //当游戏未开始或玩家未移动时  直接返回
        if(!GameManager.Instance.isGameStart || !GameManager.Instance.isPlayerMove) return;
        if(startTimer){
            fallTime-=Time.deltaTime;
            //当计时结束时  平台开始下落
            if(fallTime < 0){
                startTimer = false;
                //判断当前平台是否为静态
                if(body.bodyType != RigidbodyType2D.Dynamic){
                    body.bodyType = RigidbodyType2D.Dynamic;//动态平台即实现掉落
                    StartCoroutine(DelayHide());
                }
            }
        }
        //当平台位置离摄像机太远时  直接调用隐藏方法  
        if(transform.position.y - Camera.main.transform.position.y < -7){
            StartCoroutine(DelayHide());
        }
    }

    private IEnumerator DelayHide(){
        yield return new WaitForSeconds(1f);//线程等待1秒
        gameObject.SetActive(false);
    }
}
