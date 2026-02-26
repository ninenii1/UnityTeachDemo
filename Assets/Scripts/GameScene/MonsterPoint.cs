using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPoint : MonoBehaviour
{
    //怪物有多少波
    public int maxWave;
    //每波怪物有多少只
    public int monsterNumOneWave;
    //用于记录 当前波的怪物还有多少只没有创建
    private int nowNum;

    //怪物ID 允许有多个 这样就可以随机创建不同的怪物 更具多样性
    public List<int> monsterIDs;
    //用于记录 当前波 要创建什么ID的怪物
    private int nowID;

    //单只怪物创建间隔时间
    public float createOffsetTime;

    //波与波之间的间隔时间
    public float delayTime;

    //第一波怪物创建的间隔时间
    public float firstDelayTime;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("CreateWave", firstDelayTime);

        //记录出怪点
        GameLevelMgr.Instance.AddMonsterPoint(this);
        //更新最大波数
        GameLevelMgr.Instance.UpdatgeMaxNum(maxWave);
    }

    /// <summary>
    /// 开始创建一波的怪物
    /// </summary>
    private void CreateWave()
    {
        //得到当前波怪物的ID是什么
        nowID = monsterIDs[Random.Range(0, monsterIDs.Count)];
        //当前波怪物有多少只
        nowNum = monsterNumOneWave;
        //创建怪物
        CreateMonster();
        //减少波数
        --maxWave;
        //通知关卡管理器 出了一波怪
        GameLevelMgr.Instance.ChangeNowWaveNum(1);
    }

    /// <summary>
    /// 创建怪物
    /// </summary>
    private void CreateMonster()
    {
        //直接创建怪物
        //取出怪物数据
        MonsterInfo info = GameDataMgr.Instance.monsterInfoList[nowID - 1];

        //创建怪物预设体
        GameObject obj = Instantiate(Resources.Load<GameObject>(info.res), this.transform.position, Quaternion.identity);
        //为我们创建出的怪物预设体 添加怪物脚本 进行初始化
        MonsterObject monsterObj = obj.AddComponent<MonsterObject>();
        monsterObj.InitInfo(info);

        //告诉管理器 怪物数量加1
        //GameLevelMgr.Instance.ChangeMonsterNum(1);
        //记录怪物到列表中
        GameLevelMgr.Instance.AddMonster(monsterObj);

        //创建完一只怪物后 减去要创建的怪物数量1
        --nowNum;
        if( nowNum == 0 )
        {
            if (maxWave > 0)
                Invoke("CreateWave", delayTime);
        }
        else
        {
            Invoke("CreateMonster", createOffsetTime);
        }
    }

    /// <summary>
    /// 出怪点是否出怪结束
    /// </summary>
    /// <returns></returns>
    public bool CheckOver()
    {
        return nowNum == 0 && maxWave == 0;
    }
}
