using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

//游戏角色控制
public class PlayerController : MonoBehaviour
{
    private bool isMoveLeft = false;//默认向右
    private bool isJumping = false;//默认是停止状态
    private bool isMove = false;//玩家是否在移动
    private Vector3 nextPlatformPosLeft;
    private Vector3 nextPlatformPosRight;
    private ManagerVars vars;

    private Rigidbody2D my_body;//获取自身的rigidbody组件
    public Transform rayDown , rayLeft , rayRight;//获取到射线点的transform组件
    public LayerMask platformLayer , obstacleLayer;//获取到平台层级 和 障碍物层级

    private SpriteRenderer spriteRenderer;//获取玩家自身的精灵渲染器

    private GameObject lastHitGo = null;//上一个接触的平台

    private AudioSource voice;//获取控制声音的组件


    private void Awake()
    {
        my_body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        voice = GetComponent<AudioSource>();
        vars = ManagerVars.GetManagerVars();

        EventCenter.AddListener<bool>(EventDefine.IsMouseOn,IsMouseOn);//音效的管理事件
        EventCenter.AddListener<int>(EventDefine.ChangeSkin,changeSkin);//changskin需要int参数 所以需要加上int的泛型
    }

    //因为GameManager在awake时初始化的数据 所以获得数据时需要在start中
    private void Start() {
        changeSkin(GameManager.Instance.GetSelectedSkin());
    }

    private void IsMouseOn(bool value){
        //AudioSource的mute属性为是否静音
        voice.mute = !value;
    }

    //在游戏内更换皮肤的方法
    private void changeSkin(int skinIndex){
        spriteRenderer.sprite = vars.skinSpriteBackList[skinIndex];
    }

    //解决移动端bug
    private bool IsPointerOverGameObject(Vector2 mousePos){
        //创建一个点击事件对象
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePos;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //发射一条射线，判断是否点击到UI
        EventSystem.current.RaycastAll(eventData,raycastResults);
        //返回结果大于0 即点击到UI 返回true
        return raycastResults.Count > 0;
    }

    private void Update()
    {
        // //判断当前使用的系统 是安卓和ios系统时
        // if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer){
        //     //获取当前点击的是第几根手指  和获取手指id
        //     int fingerId = Input.GetTouch(0).fingerId;
        //     //当点击的是UI是 直接返回  不执行后续
        //     if(EventSystem.current.IsPointerOverGameObject(fingerId)) return;
        // }
        // else{
        //     if(EventSystem.current.IsPointerOverGameObject()) return;
        // }

        //当点击是UI是  直接返回  不执行
        if(IsPointerOverGameObject(Input.mousePosition)) return;

        if (GameManager.Instance.isGameStart == false || GameManager.Instance.isGameOver == true || GameManager.Instance.isPause)
            return;

        //按下鼠标左键并且在静止状态 且下一个平台位置存在(避免bug) 才能开始跳跃
        if (Input.GetMouseButtonDown(0) && isJumping == false && nextPlatformPosLeft !=  Vector3.zero)
        {
            if(!isMove){
                isMove = true;
                EventCenter.Broadcast(EventDefine.PlayerMove);
            }
            voice.PlayOneShot(vars.jumpClip);//播放跳跃音效
            EventCenter.Broadcast(EventDefine.DecidePath);
            isJumping = true;//开始跳跃  进入跳跃状态
            Vector3 mousePos = Input.mousePosition;
            //点击屏幕的左边
            if (mousePos.x < Screen.width / 2)
            {
                isMoveLeft = true;
            }
            //点击屏幕的右边
            if (mousePos.x > Screen.width / 2)
            {
                isMoveLeft = false;
            }
            Jump();
        }

        //第一种游戏结束方式
        //如果y方向的速度<0  即处于下落状态  
        if (my_body.velocity.y < 0 && !isRayPlatform() && GameManager.Instance.isGameOver == false)
        {
            voice.PlayOneShot(vars.fallClip);//播放坠落音效
            spriteRenderer.sortingLayerName = "Default";
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.isGameOver = true;
            //调用结束界面
            StartCoroutine(DelayShowGameOverPannel());
        }

        //第二种游戏结束方式
        //当游戏未结束 且玩家在跳跃时接触到障碍物 
        if(isJumping && isRayObstacle() && !GameManager.Instance.isGameOver){
            voice.PlayOneShot(vars.hitClip);//碰到障碍物音效
            GameObject go =Instantiate(ObjectPool.Instance.GetDeathEffect());
            go.SetActive(true);
            go.transform.position = gameObject.transform.position;
            GameManager.Instance.isGameOver = true;
            spriteRenderer.enabled = false;//关闭精灵渲染器即可使玩家不显示 
            StartCoroutine(DelayShowGameOverPannel());
        }

        //第三种游戏结束方法
        //当玩家距离主摄像机太远 且游戏未结束时 
        if(transform.position.y - Camera.main.transform.position.y < -7 && !GameManager.Instance.isGameOver){
            voice.PlayOneShot(vars.fallClip);//播放坠落音效
            GameManager.Instance.isGameOver = true;
            spriteRenderer.enabled = false;
            StartCoroutine(DelayShowGameOverPannel());//调用延时调用的方法
        }
    }

    //延时调用
    IEnumerator DelayShowGameOverPannel(){
        yield return new WaitForSeconds(1.5f);
        EventCenter.Broadcast(EventDefine.ShowGameOverPannnel);
    }

    //使用射线检测判断是否触碰到地面
    private bool isRayPlatform()
    {
        //射线起始点  方向(vector2)  距离   射线处理层级
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                //避免平台重复刷分
                if(lastHitGo != hit.collider.gameObject){
                    //因为初始就会接触平台 所以需要判断
                    if(lastHitGo == null){
                        lastHitGo = hit.collider.gameObject;
                        return true;//第一个必是平台  直接返回true
                    }
                    EventCenter.Broadcast(EventDefine.AddScore);//广播事件 当接触到平台时  分数增加
                    lastHitGo = hit.collider.gameObject;//将当前物体的值赋给上一个
                }
                return true;
            }
        }
        return false;
    }

    //使用射线检测判断时是否接触到障碍物
    private bool isRayObstacle(){
        RaycastHit2D hitLeft = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);
        if (hitLeft.collider != null)
        {
            if (hitLeft.collider.tag == "Obstacle")
            {
                return true;
            }
        }

        if (hitRight.collider != null)
        {
            if (hitRight.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        return false;
    }

    private void Jump()
    {
        if (isMoveLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.DOMoveX(nextPlatformPosLeft.x, 0.2f);
            transform.DOMoveY(nextPlatformPosLeft.y + 0.8f, 0.2f);
        }
        else
        {
            transform.localScale = Vector3.one;
            transform.DOMoveX(nextPlatformPosRight.x, 0.2f);
            transform.DOMoveY(nextPlatformPosRight.y + 0.8f, 0.2f);
        }

    }

    //触发检测
    private void OnTriggerEnter2D(Collider2D other)
    {
        isJumping = false;//碰到平台成为禁止状态
        if (other.tag == "Platform")
        {
            Vector3 platformPos = other.gameObject.transform.position;
            nextPlatformPosLeft = new Vector3(platformPos.x - vars.nextXpos, platformPos.y + vars.nextYpos, 0);
            nextPlatformPosRight = new Vector3(platformPos.x + vars.nextXpos, platformPos.y + vars.nextYpos, 0);
        }
    }

    //碰撞检测
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.tag == "Diamond"){
            voice.PlayOneShot(vars.diamondClip);//播放吃到钻石的特效
            EventCenter.Broadcast(EventDefine.AddDiamond);
            other.gameObject.SetActive(false);//隐藏钻石
        }
    }

    private void OnDestroy() {
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin,changeSkin);
        EventCenter.RemoveListener<bool>(EventDefine.IsMouseOn,IsMouseOn);
    }
}
