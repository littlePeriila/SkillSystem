using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>简单工厂 — 创建攻击目标选择器，带缓存和安全检查</summary>
public class SelectorFactory
{
    private static readonly Dictionary<string, IAttackSelector> cache = new Dictionary<string, IAttackSelector>();

    public static IAttackSelector CreateSelector(DamageMode mode)
    {
        string key = mode.ToString();

        if (cache.TryGetValue(key, out var cached))
            return cached;

        var nameSpace = typeof(SelectorFactory).Namespace;
        string classFullName = string.Format("{0}AttackSelector", key);

        if (!string.IsNullOrEmpty(nameSpace))
            classFullName = nameSpace + "." + classFullName;

        Type type = Type.GetType(classFullName);
        if (type == null)
        {
            Debug.LogError($"[SelectorFactory] 找不到选择器类型: {classFullName}");
            return null;
        }

        var selector = Activator.CreateInstance(type) as IAttackSelector;
        if (selector == null)
        {
            Debug.LogError($"[SelectorFactory] 类型 {classFullName} 未实现 IAttackSelector");
            return null;
        }

        cache.Add(key, selector);
        return selector;
    }
}
