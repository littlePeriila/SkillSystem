using UnityEngine;

/// <summary>位移执行器 — 管理位移生命周期，同一角色同一时间只允许一个位移</summary>
public class DisplacementExecutor : MonoBehaviour
{
    private IDisplacement _displacement;
    private DisplacementContext _ctx;
    private float _timer;
    private bool _running;

    public bool IsRunning => _running;

    public void StartDisplacement(IDisplacement displacement, DisplacementContext ctx)
    {
        if (_running) return;

        _displacement = displacement;
        _ctx = ctx;
        _timer = 0;
        _running = true;
        _displacement.OnStart(ref _ctx);
    }

    private void Update()
    {
        if (!_running) return;

        _timer += Time.deltaTime;

        float duration = _displacement.Duration;
        float t = duration > 0 ? Mathf.Clamp01(_timer / duration) : 1f;

        if (t >= 1f)
        {
            Finish();
            return;
        }

        _displacement.OnUpdate(ref _ctx, t);
    }

    /// <summary>外部强制中断（如晕眩时调用）</summary>
    public void Cancel()
    {
        if (!_running) return;
        Finish();

        // 打断自身技能施放
        var ctrl = GetComponent<SkillCastController>();
        if (ctrl != null && ctrl.IsCasting)
            ctrl.Interrupt(InterruptType.Displace);
    }

    private void Finish()
    {
        _displacement?.OnEnd(ref _ctx);
        _running = false;
        _displacement = null;
    }
}
