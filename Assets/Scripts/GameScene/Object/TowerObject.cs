using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerObject : MonoBehaviour
{
    //炮台头部 用于旋转 指向目标
    public Transform head;
    //开火点 用于释放攻击特效的位置
    public Transform gunPoint;
    //炮台头部旋转速度 可以写死 也可以配在表中
    private float roundSpeed = 20;
    
    //炮台关联的数据
    private TowerInfo info;

    //当前要攻击的目标
    private MonsterObject targetObj;
    //当前要攻击的目标们
    private List<MonsterObject> targetObjs;

    //用于计时的 用来判断攻击间隔时间
    private float nowTime;

    //用于记录怪物位置
    private Vector3 monsterPos;

    /// <summary>
    /// 初始化炮台相关数据
    /// </summary>
    /// <param name="info"></param>
    public void InitInfo(TowerInfo info)
    {
        this.info = info;
    }

    // Update is called once per frame
    void Update()
    {
        //单体攻击逻辑
        if (info.atkType == 1)
        {
            //没有目标 或者 目标死亡 或者 目标超出攻击距离
            if( targetObj == null ||
                targetObj.isDead ||
                Vector3.Distance(this.transform.position, targetObj.transform.position) > info.atkRange)
            {
                targetObj = GameLevelMgr.Instance.FindMonster(this.transform.position, info.atkRange);
            }

            //如果没有找到任何可以攻击的对象 那么炮台就不应该旋转
            if (targetObj == null)
                return;

            //得到怪物位置，偏移Y的目标是希望 炮台头部不要上下倾斜
            monsterPos = targetObj.transform.position;
            monsterPos.y = head.position.y;
            //让炮台的头部 旋转起来
            head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(monsterPos - head.position), roundSpeed * Time.deltaTime);
            
            //判断 两个对象之间的夹角 小于一定范围时  才能让目标受伤 并且攻击间隔条件要满足
            if( Vector3.Angle(head.forward, monsterPos - head.position) < 5 &&
                Time.time - nowTime >= info.offsetTime )
            {
                //让目标受伤
                targetObj.Wound(info.atk);
                //播放音效
                GameDataMgr.Instance.PlaySound("Music/Tower");
                //创建开火特效
                GameObject effObj = Instantiate(Resources.Load<GameObject>(info.eff), gunPoint.position, gunPoint.rotation);
                //延迟移除特效
                Destroy(effObj, 0.2f);

                //记录开火时间
                nowTime = Time.time;
            }
        }
        //群体攻击逻辑
        else
        {
            targetObjs = GameLevelMgr.Instance.FindMonsters(this.transform.position, info.atkRange);

            if( targetObjs.Count > 0 &&
                Time.time - nowTime >= info.offsetTime)
            {
                //创建开火特效
                GameObject effObj = Instantiate(Resources.Load<GameObject>(info.eff), gunPoint.position, gunPoint.rotation);
                //延迟移除特效
                Destroy(effObj, 0.2f);

                //让目标们受伤
                for (int i = 0; i < targetObjs.Count; i++)
                {
                    targetObjs[i].Wound(info.atk);
                }

                //记录开火时间
                nowTime = Time.time;
            }
        }
    }
}
