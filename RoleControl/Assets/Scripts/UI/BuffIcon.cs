using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    public Text textCD;
    public Image imgIcon;

    private float durationTime;
    private float curTime;

    public BuffType buffType;

    private static Sprite[] _buffSprites;

    public void LoadIcon(BuffType buffType, float duration)
    {
        durationTime = duration;
        this.buffType = buffType;

        string iconName = BuffIconMapping.GetIconName(buffType);
        if (iconName == null) return;

        // 首次加载时缓存整张图集，后续直接从缓存读取
        if (_buffSprites == null)
            _buffSprites = Resources.LoadAll<Sprite>("BuffIcon/Buff");
        if (_buffSprites == null) return;

        foreach (var sp in _buffSprites)
        {
            if (sp.name == iconName)
            {
                imgIcon.sprite = Instantiate(sp);
                break;
            }
        }
    }

    private void OnEnable()
    {
        curTime = 0;
    }

    void Update()
    {
        curTime += Time.deltaTime;
        textCD.text = (durationTime - curTime).ToString("F0");

        if (curTime > durationTime)
        {
            gameObject.SetActive(false);
            curTime = 0;
        }
    }

    public void Refresh()
    {
        curTime = 0;
    }
}
