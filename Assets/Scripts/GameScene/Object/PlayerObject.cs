using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    private Animator animator;

    //1.玩家属性的初始化
    //玩家攻击力
    private int atk;
    //玩家拥有的钱
    public int money;
    //旋转的速度
    private float roundSpeed = 50;

    //持枪对象才有的开火点
    public Transform gunPoint;

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    /// <summary>
    /// 初始化玩家基础属性
    /// </summary>
    /// <param name="atk"></param>
    /// <param name="money"></param>
    public void InitPlayerInfo(int atk, int money)
    {
        this.atk = atk;
        this.money = money;
        //更新界面上钱的数量
        UpdateMoney();
    }

    void Update()
    {
        //2.移动变化、动作变化
        //移动动作的变化：由于动作有位移，我们也应用了动作的位移，所以只要改变这两个值，就会有动作的变化和速度的变化
        animator.SetFloat("VSpeed", Input.GetAxis("Vertical"));
        animator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));
        //旋转
        this.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * roundSpeed * Time.deltaTime);

        //按下左Shift键蹲下
        if( Input.GetKeyDown(KeyCode.LeftShift) )
        {
            animator.SetLayerWeight(1, 1); //设置层级的权重，权重为1时下蹲
        }
        //抬起左Shift键站立
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetLayerWeight(1, 0);
        }
        //按下R键打滚
        if (Input.GetKeyDown(KeyCode.R))
            animator.SetTrigger("Roll");
        //按下鼠标左键开枪
        if (Input.GetMouseButtonDown(0))
            animator.SetTrigger("Fire");
            
    }

    //3.攻击动作的不同处理
    /// <summary>
    /// 专门用于处理刀武器攻击动作的伤害检测事件
    /// </summary>
    //要先在攻击动画中刺向敌人的那一帧添加事件，并将方法名KnifeEvent填写在事件的Function中
    public void KnifeEvent()
    {
        //进行伤害检测（球形范围检测）
        //角色脚下位置+角色前方偏移值+上方偏移值 = 角色胸口前方的位置，在这个位置创建一个圆形检测范围，判断有没有敌人在这个范围内
        //有敌人在这个范围就可以得到敌人的碰撞器，通过碰撞器得到身上的脚本，实现对敌人的伤害
        //范围半径为1，通过LayerMask层级遮罩只检测"Monster"层的对象，避免误判其他碰撞器
        Collider[] colliders = Physics.OverlapSphere(this.transform.position + this.transform.forward + this.transform.up, 1, 1 << LayerMask.NameToLayer("Monster"));

        //播放音效
        GameDataMgr.Instance.PlaySound("Music/Knife");

        //暂时无法继续写逻辑了 因为 我们没有怪物对应的脚本
        for (int i = 0; i < colliders.Length; i++)
        {
            //遍历数组，得到碰撞到的对象上的怪物脚本，让其受伤
            MonsterObject monster = colliders[i].gameObject.GetComponent<MonsterObject>();
            if (monster != null && !monster.isDead)
            {
                monster.Wound(this.atk);
                break;
            }
        }
    }

    /// <summary>
    /// 专门用于处理枪武器攻击动作的伤害检测事件
    /// </summary>
    //在射击动画中射向敌人的那一帧添加事件，并将方法名ShootEvent填写在事件的Function中
    public void ShootEvent()
    {
        //进行射线检测 
        //前提是需要在枪下方创建空对象作为开火点，z轴朝向枪口正方向（这里旋转Y轴为-90度）
        //参数：（开火点，方向），最大检测距离，检测层级
        //返回值为碰撞信息
        RaycastHit[] hits = Physics.RaycastAll(new Ray(gunPoint.position, this.transform.forward), 1000, 1 << LayerMask.NameToLayer("Monster"));

        //播放开枪音效
        GameDataMgr.Instance.PlaySound("Music/Gun");

        for (int i = 0; i < hits.Length; i++)
        {
            //得到碰撞到的对象上的怪物脚本，让其受伤
            MonsterObject monster = hits[i].collider.gameObject.GetComponent<MonsterObject>();
            if (monster != null && !monster.isDead)
            {
                //进行打击特效的创建
                GameObject effObj = Instantiate(Resources.Load<GameObject>(GameDataMgr.Instance.nowSelRole.hitEff));
                effObj.transform.position = hits[i].point;
                effObj.transform.rotation = Quaternion.LookRotation(hits[i].normal);
                Destroy(effObj, 1);

                monster.Wound(this.atk);
                break;
            }
        }
    }

    //4.钱变化的逻辑，一开始的金钱数量以及杀死敌人增加金钱
    public void UpdateMoney()
    {
        //间接地更新界面上钱的数量
        UIManager.Instance.GetPanel<GamePanel>().UpdateMoney(money);
    }

    /// <summary>
    /// 提供给外部加钱的方法
    /// </summary>
    /// <param name="money"></param>
    public void AddMoney(int money)
    {
        //加钱
        this.money += money;
        UpdateMoney();
    }
}
