using UnityEngine;

/// <summary>伤害飘字监听器 — 订阅 DamageDealt 事件，自动注册到场景</summary>
public class DamagePopupListener : MonoBehaviour
{
    private GameObject damagePopupPrefab;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInit()
    {
        var go = new GameObject(nameof(DamagePopupListener));
        go.AddComponent<DamagePopupListener>();
        DontDestroyOnLoad(go);
    }

    void Start()
    {
        damagePopupPrefab = Resources.Load<GameObject>("HUD");
        ObserverMa.I.Register(SkillEventKeys.DamageDealt, OnDamageDealt);
    }

    private void OnDamageDealt(object args)
    {
        var e = (DamageDealtArgs)args;
        if (e.Target == null) return;

        var status = e.Target.GetComponent<CharacterStatus>();
        Transform hudPos = status != null ? status.hudPos : e.Target.transform;

        var popup = Instantiate(damagePopupPrefab).GetComponent<DamagePopup>();
        popup.target = hudPos;
        popup.transform.rotation = Quaternion.identity;
        popup.Value = ((int)e.Damage).ToString();
    }
}
