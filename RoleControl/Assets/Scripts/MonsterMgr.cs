using System.Collections.Generic;
using UnityEngine;

/// <summary>敌人头像管理单例 — 维护所有敌人头像引用</summary>
public class MonsterMgr
{
    private static MonsterMgr instance;

    private MonsterMgr() { }

    public static MonsterMgr I
    {
        get
        {
            if (instance == null)
                instance = new MonsterMgr();
            return instance;
        }
    }

    private List<UIPortrait> allEnemyPortraits = new List<UIPortrait>();

    public void AddEnemyPortraits(UIPortrait uiPortrait)
    {
        allEnemyPortraits.Add(uiPortrait);
    }

    public void RemoveEnemyPortraits(UIPortrait uiPortrait)
    {
        allEnemyPortraits.Remove(uiPortrait);
    }
}
