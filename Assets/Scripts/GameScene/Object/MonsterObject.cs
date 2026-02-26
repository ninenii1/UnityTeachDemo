using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterObject : MonoBehaviour
{
    //动画相关
    private Animator animator;
    //位移相关 寻路组件
    private NavMeshAgent agent;
    //一些不变的基础数据
    private MonsterInfo monsterInfo;

    //当前血量
    private int hp;
    //怪物是否死亡
    public bool isDead = false;

    //上一次攻击的时间
    private float frontTime;

    // Start is called before the first frame update
    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
    }

    //初始化
    public void InitInfo(MonsterInfo info)
    {
        monsterInfo = info;
        //状态机加载
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(info.animator);
        //要变的当前血量
        hp = info.hp;
        //速度和加速度赋值 之所以赋值一样 是希望没有 明显的加速运动 而是一个匀速运动 初始化
        agent.speed = agent.acceleration = info.moveSpeed;
        //旋转速度
        agent.angularSpeed = info.roundSpeed;
    }

    //受伤
    public void Wound(int dmg)
    {
        if (isDead)
            return;

        //减少血量
        hp -= dmg;
        //播放受伤动画
        animator.SetTrigger("Wound");

        if( hp <= 0 )
        {
            //死亡
            Dead();
        }
        else
        {
            //播放音效
            GameDataMgr.Instance.PlaySound("Music/Wound");
        }
    }

    //死亡
    public void Dead()
    {
        isDead = true;
        //停止移动
        //agent.isStopped = true;
        agent.enabled = false;
        //播放死亡动画
        animator.SetBool("Dead", true);

        //播放音效
        GameDataMgr.Instance.PlaySound("Music/dead");
        //加钱——我们之后通过关卡管理类 来管理游戏中的对象 通过它来让玩家加钱 
        GameLevelMgr.Instance.player.AddMoney(10);
    }
    
    //死亡动画播放完毕后 会调用的事件方法
    public void DeadEvent()
    {
        //死亡动画播放完毕后移除对象
        //之后有了关卡管理器再来处理
        //GameLevelMgr.Instance.ChangeMonsterNum(-1);

        //从列表中移除怪物
        GameLevelMgr.Instance.RemoveMonster(this);

        //在场景中移除已经死亡的对象
        Destroy(this.gameObject);

        //怪物死亡时 检测 游戏是否胜利
        if(GameLevelMgr.Instance.CheckOver())
        {
            //显示结束界面
            GameOverPanel panel = UIManager.Instance.ShowPanel<GameOverPanel>();
            panel.InitInfo(GameLevelMgr.Instance.player.money, true);
        }
    }

    //出生过后再移动
    //移动——寻路组件
    public void BornOver()
    {
        //出生结束后 再让怪物朝目标点移动
        agent.SetDestination(MainTowerObject.Instance.transform.position);
        //播放移动动画
        animator.SetBool("Run", true);
    }

    //攻击
    void Update()
    {
        //检测什么时候停下来攻击
        if (isDead)
            return;
        //根据速度 来决定动画播放什么
        animator.SetBool("Run", agent.velocity != Vector3.zero);
        //检测和目标点达到移动条件时 就攻击
        if( Vector3.Distance(this.transform.position, MainTowerObject.Instance.transform.position ) < 5 &&
            Time.time - frontTime >= monsterInfo.atkOffset)
        {
            //记录这次攻击时的时间
            frontTime = Time.time;
            animator.SetTrigger("Atk");
        }
    }

    //伤害检测
    public void AtkEvent()
    {
        //范围检测 进行伤害判断
        Collider[] colliders = Physics.OverlapSphere(this.transform.position + this.transform.forward + this.transform.up, 1, 1 << LayerMask.NameToLayer("MainTower"));

        //播放音效
        GameDataMgr.Instance.PlaySound("Music/Eat");

        for (int i = 0; i < colliders.Length; i++)
        {
            if( MainTowerObject.Instance.gameObject == colliders[i].gameObject)
            {
                //让保护区域受到伤害
                MainTowerObject.Instance.Wound(monsterInfo.atk);
            }
        }
    }
}
