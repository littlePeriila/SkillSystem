using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPortrait : MonoBehaviour
{
    public Transform buffContent;
    public Slider silderHP;
    public Slider silderMP;
    public GameObject buffItem;

    public CharacterStatus cstatus;

    private List<GameObject> buffItems = new List<GameObject>();
    private float curTime;
    private Vector2 defaultPos;
    private Vector2 hidePos = new Vector2(1600, 444);

    private bool isHero;

    void Start()
    {
        isHero = gameObject.name == "UIHeroPortrait";
        defaultPos = GetComponent<RectTransform>().anchoredPosition;
        if (!isHero)
            GetComponent<RectTransform>().anchoredPosition = hidePos;

        silderHP.maxValue = 1;
        silderMP.maxValue = 1;

        // 订阅事件
        ObserverMa.I.Register(SkillEventKeys.PortraitShow, OnPortraitShow);
        ObserverMa.I.Register(SkillEventKeys.PortraitsHide, OnPortraitsHide);
        ObserverMa.I.Register(SkillEventKeys.HPChanged, OnHPChanged);
        ObserverMa.I.Register(SkillEventKeys.SPChanged, OnSPChanged);
        ObserverMa.I.Register(SkillEventKeys.BuffApplied, OnBuffApplied);
    }

    private void OnEnable()
    {
        curTime = 0;
    }

    private void Update()
    {
        if (isHero) return;

        curTime += Time.deltaTime;
        if (curTime > 40.0f)
            GetComponent<RectTransform>().anchoredPosition = hidePos;
    }

    public void RefreshHpMp()
    {
        silderHP.value = cstatus.HP / cstatus.MaxHP;
        silderMP.value = cstatus.SP / cstatus.MaxSP;
    }

    // ===== 事件回调 =====

    private void OnPortraitShow(object args)
    {
        var target = args as GameObject;
        if (target == null) return;
        if (cstatus == null || target != cstatus.gameObject) return;
        GetComponent<RectTransform>().anchoredPosition = defaultPos;
        curTime = 0;
    }

    private void OnPortraitsHide()
    {
        if (isHero) return;
        GetComponent<RectTransform>().anchoredPosition = hidePos;
    }

    private void OnHPChanged(object args)
    {
        var e = (ResourceChangedArgs)args;
        if (cstatus == null || e.Target != cstatus.gameObject) return;
        silderHP.value = e.Current / e.Max;
    }

    private void OnSPChanged(object args)
    {
        var e = (ResourceChangedArgs)args;
        if (cstatus == null || e.Target != cstatus.gameObject) return;
        silderMP.value = e.Current / e.Max;
    }

    private void OnBuffApplied(object args)
    {
        var e = (BuffAppliedArgs)args;
        if (cstatus == null || e.Target != cstatus.gameObject) return;
        AddBuffIcon(e.BuffType, e.Duration);
    }

    // ===== Buff 图标管理 =====

    public void AddBuffIcon(BuffType buffType, float duration)
    {
        curTime = 0;
        BuffIcon curBuff = null;
        foreach (var item in buffItems)
        {
            if (item.activeSelf)
            {
                curBuff = item.GetComponent<BuffIcon>();
                if (curBuff.buffType == buffType)
                {
                    curBuff.Refresh();
                    return;
                }
            }
        }

        if (buffType == BuffType.BeatBack || buffType == BuffType.BeatUp || buffType == BuffType.Pull)
            duration = 2f;

        GameObject go = GetChild();
        buffItems.Add(go);
        curBuff = go.GetComponent<BuffIcon>();
        curBuff.LoadIcon(buffType, duration);
    }

    private GameObject GetChild()
    {
        foreach (var item in buffItems)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return item;
            }
        }
        return Instantiate<GameObject>(buffItem, buffContent);
    }

    public void Reset()
    {
        curTime = 0;
    }

    public void ShowPortrait()
    {
        GetComponent<RectTransform>().anchoredPosition = defaultPos;
    }
}
