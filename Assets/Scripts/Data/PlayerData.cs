using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家数据
/// </summary>
public class PlayerData 
{
    //当前拥有多少游戏币
    public int haveMoney = 300;
    //当前解锁了哪些角色
    public List<int> buyHero = new List<int>();
}
