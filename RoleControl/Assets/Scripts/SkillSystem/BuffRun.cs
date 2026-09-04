using System.Collections;
using UnityEngine;

/// <summary>Buff 运行时实例 — 仅管理生命周期，效果委托给 IBuffEffect 策略</summary>
public class BuffRun : MonoBehaviour
{
    private IBuffEffect _effect;
    private CharacterStatus _target;
    private GameObject _caster;
    private float _value;
    private float _duration;
    private float _interval;
    private float _timer;
    private bool _cleanedUp;

    public BuffType bufftype => _effect?.BuffType ?? BuffType.None;

    public void InitBuff(BuffType buffType, float duration, float value, float interval, GameObject caster = null)
    {
        _effect = BuffFactory.Create(buffType);
        if (_effect == null)
        {
            Destroy(this);
            return;
        }

        _caster = caster;

        // 位移类 buff 固定 2 秒
        _duration = (buffType == BuffType.BeatBack || buffType == BuffType.BeatUp || buffType == BuffType.Pull)
            ? 2f
            : duration;
        _value = value;
        _interval = interval;
        _target = GetComponent<CharacterStatus>();

        StartCoroutine(Execute());
    }

    public void Reset()
    {
        _timer = 0;
    }

    public float GetRemainTime()
    {
        return _duration - _timer;
    }

    private IEnumerator Execute()
    {
        do
        {
            _effect.Apply(_target, _value, _caster);
            SpawnFx();
            yield return new WaitForSeconds(_interval);
            _timer += _interval;
        } while (_timer < _duration);

        Cleanup();
        Destroy(this);
    }

    private void SpawnFx()
    {
        if (string.IsNullOrEmpty(_effect.FxPrefabName))
            return;

        var fxPos = _effect.FxOnRoot ? _target.transform : _target.HitFxPos;
        var prefab = Resources.Load<GameObject>($"Skill/{_effect.FxPrefabName}");
        if (prefab == null) return;

        var fx = GameObjectPool.I.CreateObject(
            _effect.FxPrefabName, prefab, fxPos.position, fxPos.rotation);
        fx.transform.SetParent(fxPos);
        GameObjectPool.I.Destory(fx, _interval);
    }

    private void OnDisable()
    {
        Cleanup();
    }

    /// <summary>确保 OnRemove 只执行一次</summary>
    private void Cleanup()
    {
        if (_cleanedUp) return;
        _cleanedUp = true;

        if (_effect != null && _target != null)
            _effect.OnRemove(_target, _value);
    }
}
