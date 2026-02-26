using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseScenePanel : BasePanel
{
    //四个按钮
    public Button btnLeft;
    public Button btnRight;
    public Button btnStart;
    public Button btnBack;

    //用于文本和图片更新
    public Text txtInfo;
    public Image imgScene;

    //记录当前数据索引
    private int nowIndex;
    //记录当前选择的数据
    private SceneInfo nowSceneInfo;

    public override void Init()
    {
        btnLeft.onClick.AddListener(() =>
        {
            --nowIndex;
            if (nowIndex < 0)
                nowIndex = GameDataMgr.Instance.sceneInfoList.Count - 1;
            ChangeScene();
        });
        btnRight.onClick.AddListener(() =>
        {
            ++nowIndex;
            if (nowIndex >= GameDataMgr.Instance.sceneInfoList.Count)
                nowIndex = 0;
            ChangeScene();
        });

        btnStart.onClick.AddListener(() =>
        {
            //隐藏当前面板 
            UIManager.Instance.HidePanel<ChooseScenePanel>();
            //切换场景
            AsyncOperation ao = SceneManager.LoadSceneAsync(nowSceneInfo.sceneName);
            //进行关卡初始化
            ao.completed += (obj) =>
            {
                GameLevelMgr.Instance.InitInfo(nowSceneInfo);
            };
        });

        btnBack.onClick.AddListener(() =>
        {
            //隐藏自己
            UIManager.Instance.HidePanel<ChooseScenePanel>();
            //返回上级面板 显示选角面板
            UIManager.Instance.ShowPanel<ChooseHeroPanel>();
        });

        //一打开面板 初始化时 也应该把数据更新了
        ChangeScene();
    }

    /// <summary>
    /// 切换界面显示的场景信息
    /// </summary>
    public void ChangeScene()
    {
        nowSceneInfo = GameDataMgr.Instance.sceneInfoList[nowIndex];
        //更新图片和显示的文字信息
        imgScene.sprite = Resources.Load<Sprite>(nowSceneInfo.imgRes);
        txtInfo.text = "名称:\n" + nowSceneInfo.name + "\n" + "描述:\n" + nowSceneInfo.tips;
    }
}
