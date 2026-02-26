using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTowerObject : MonoBehaviour
{
    //血量相关
    private int hp;
    private int maxHp;
    //是否死亡
    private bool isDead;

    //能够被别人快速获取到位置
    private static MainTowerObject instance;
    public static MainTowerObject Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    //更新血量
    public void UpdateHp(int hp, int maxHP)
    {
        this.hp = hp;
        this.maxHp = maxHP;

        //更新界面上血量的显示
        UIManager.Instance.GetPanel<GamePanel>().UpdateTowerHp(hp, maxHP);
    }

    //自己受到伤害
    public void Wound(int dmg)
    {
        //如果保护区域已经被打死 就没有必要再减血了
        if (isDead)
            return;
        //受到伤害
        hp -= dmg;
        //死亡逻辑
        if( hp <= 0 )
        {
            hp = 0;
            isDead = true;
            //游戏结束
            GameOverPanel panel = UIManager.Instance.ShowPanel<GameOverPanel>();
            //得到奖励的一半
            panel.InitInfo((int)(GameLevelMgr.Instance.player.money * 0.5f), false);
        }

        //更新血量
        UpdateHp(hp, maxHp);
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
