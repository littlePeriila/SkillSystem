using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 角色管理器 — 输入处理入口
/// </summary>
public class CharacterManager : MonoBehaviour
{
    private CharacterSkillSystem css;

    public void Start()
    {
        css = GetComponent<CharacterSkillSystem>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            css.AttackUseSkill(1);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            css.AttackUseSkill(2);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            css.AttackUseSkill(3);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            css.AttackUseSkill(4);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            css.AttackUseSkill(5);

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            Cursor.visible = !Cursor.visible;
    }
}
