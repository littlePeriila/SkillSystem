using UnityEngine;

/// <summary>角色头像管理器 — 负责创建和绑定角色 UI 头像，与 CharacterStatus 数据分离</summary>
public class PortraitManager : MonoBehaviour
{
    private void Start()
    {
        var status = GetComponent<CharacterStatus>();
        if (status == null) return;

        UIPortrait portrait;
        if (CompareTag("Player"))
        {
            var heroHead = GameObject.FindGameObjectWithTag("HeroHead");
            if (heroHead == null) return;
            portrait = heroHead.GetComponent<UIPortrait>();
        }
        else if (CompareTag("Enemy"))
        {
            var canvas = GameObject.FindGameObjectWithTag("Canvas");
            if (canvas == null) return;
            var prefab = Resources.Load<GameObject>("UIEnemyPortrait");
            portrait = Instantiate(prefab, canvas.transform).GetComponent<UIPortrait>();
            MonsterMgr.I.AddEnemyPortraits(portrait);
        }
        else
        {
            return;
        }

        portrait.cstatus = status;
        portrait.RefreshHpMp();
    }
}
