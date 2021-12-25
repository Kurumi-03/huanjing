using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//主相机的控制 (跟随玩家移动)
public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    private Vector2 velocity;
    private void Update() {
        //当能找到玩家且位置不为空时
        if(target == null && GameObject.FindGameObjectWithTag("Player") != null){
            target = GameObject.FindGameObjectWithTag("Player").transform;//同步玩家的位置
            offset = target.position - transform.position;//玩家与摄像机的偏移量
        }
    }

    private void FixedUpdate() {
        if(target != null){
            float PosX = Mathf.SmoothDamp(transform.position.x,target.position.x-offset.x,ref velocity.x,0.05f);
            float PosY = Mathf.SmoothDamp(transform.position.y,target.position.y-offset.y,ref velocity.y,0.05f);
            //只有当目标位置的y大于当前位置的y时才能移动  即  不会向下移动
            if(PosY > transform.position.y){
                transform.position = new Vector3(PosX,PosY,transform.position.z);
            }
        }
    }
}
