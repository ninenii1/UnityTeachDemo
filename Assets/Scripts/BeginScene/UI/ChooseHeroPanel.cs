using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChooseHeroPanel : BasePanel
{
    //左右键
    public Button btnLeft;
    public Button btnRight;

    //购买按钮
    public Button btnUnLock;
    public Text txtUnLock;

    //开始和返回
    public Button btnStart;
    public Button btnBack;

    //左上角拥有的钱
    public Text txtMoney;

    //角色信息
    public Text txtName;

    //英雄预设体需要创建在的位置
    private Transform heroPos;

    //当前场景中显示的对象
    private GameObject heroObj;
    //当前使用的数据
    private RoleInfo nowRoleData;
    //当前使用数据的索引
    private int nowIndex;

    public override void Init()
    {
        //找到场景中 放置对象预设体的位置
        heroPos = GameObject.Find("HeroPos").transform;

        //更新左上角玩家拥有的钱
        txtMoney.text = GameDataMgr.Instance.playerData.haveMoney.ToString();

        btnLeft.onClick.AddListener(() =>
        {
            --nowIndex;
            if (nowIndex < 0)
                nowIndex = GameDataMgr.Instance.roleInfoList.Count - 1;
            //模型的更新
            ChangeHero();
        });

        btnRight.onClick.AddListener(() =>
        {
            ++nowIndex;
            if (nowIndex >= GameDataMgr.Instance.roleInfoList.Count)
                nowIndex = 0;
            //模型的更新
            ChangeHero();
        });

        btnUnLock.onClick.AddListener(() =>
        {
            //点击解锁按钮的逻辑
            PlayerData data = GameDataMgr.Instance.playerData;
            //当有钱时
            if(data.haveMoney >= nowRoleData.lockMoney)
            {
                //购买逻辑
                //减去花费
                data.haveMoney -= nowRoleData.lockMoney;
                //更新界面显示
                txtMoney.text = data.haveMoney.ToString();
                //记录购买的id
                data.buyHero.Add(nowRoleData.id);
                //保存数据
                GameDataMgr.Instance.SavePlayerData();

                //更新解锁按钮
                UpdateLockBtn();

                //提示面板 显示购买成功
                UIManager.Instance.ShowPanel<TipPanel>().ChangeInfo("购买成功");
            }
            else
            {
                //提示面板 显示 金钱不足
                UIManager.Instance.ShowPanel<TipPanel>().ChangeInfo("金钱不足");
            }
        });

        btnStart.onClick.AddListener(() =>
        {
            //第一 是记录当前选择的角色
            GameDataMgr.Instance.nowSelRole = nowRoleData;

            //第二 是隐藏自己 显示场景选择界面
            UIManager.Instance.HidePanel<ChooseHeroPanel>();
            UIManager.Instance.ShowPanel<ChooseScenePanel>();
        });

        btnBack.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ChooseHeroPanel>();
            //让摄像机转回去后再显示开始界面
            Camera.main.GetComponent<CameraAnimator>().TurnRgiht(() =>
            {
                UIManager.Instance.ShowPanel<BeginPanel>();
            });
        });

        //更新模型显示
        ChangeHero();
    }

    /// <summary>
    /// 更新场景上要显示的模型的
    /// </summary>
    private void ChangeHero()
    {
        if(heroObj != null)
        {
            Destroy(heroObj);
            heroObj = null;
        }

        //取出数据的一条 根据索引值
        nowRoleData = GameDataMgr.Instance.roleInfoList[nowIndex];
        //实例化对象 并记录下来 用于下次切换时 删除
        heroObj = Instantiate(Resources.Load<GameObject>(nowRoleData.res), heroPos.position, heroPos.rotation);
        //由于我们现在在对象上挂载了PlayerObject 但是在开始场景 不需要
        Destroy(heroObj.GetComponent<PlayerObject>());

        //更新上方显示的 描述信息
        txtName.text = nowRoleData.tips;

        //根据解锁相关数据 来决定是否显示解锁按钮
        UpdateLockBtn();
    }

    /// <summary>
    /// 更新解锁按钮显示情况
    /// </summary>
    private void UpdateLockBtn()
    {
        //如果该角色 需要解锁 并且没有解锁的话 就应该显示解锁按钮 并且隐藏开始按钮
        if( nowRoleData.lockMoney > 0 && !GameDataMgr.Instance.playerData.buyHero.Contains(nowRoleData.id) )
        {
            //更新解锁按钮显示 并更新上面的钱
            btnUnLock.gameObject.SetActive(true);
            txtUnLock.text = "￥" + nowRoleData.lockMoney;
            //隐藏开始按钮 因为该角色没有解锁
            btnStart.gameObject.SetActive(false);
        }
        else
        {
            btnUnLock.gameObject.SetActive(false);
            btnStart.gameObject.SetActive(true);
        }
    }

    public override void HideMe(UnityAction callBack)
    {
        base.HideMe(callBack);
        //每次隐藏自己时  要把当前显示的3D模型角色 删除掉
        if(heroObj != null)
        {
            DestroyImmediate(heroObj);
            heroObj = null;
        }
    }
}
