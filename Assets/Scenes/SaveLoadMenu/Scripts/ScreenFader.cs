using UnityEngine.UI;
using UnityEngine;

public class ScreenFader : MonoBehaviour {

    public Image image;
    private Color faderColor;
    private float maxTime, curTime, volume;
    public static bool isColor { get; private set; }
    public static bool isClear { get; private set; }
    private bool fade;
    private static ScreenFader _internal;

    public static void Fader(float time, Color color)
    {
        _internal.Fader_internal(time, color);
    }

    public static void Fader(float time)
    {
        _internal.Fader_internal(time, Color.black);
    }

    void Fader_internal(float time, Color color)
    {
        if (curTime != 0) return;
        isClear = false;
        isColor = false;
        enabled = true;
        fade = !fade;
        faderColor = color;
        maxTime = time;
        volume = AudioListener.volume;
        image.gameObject.SetActive(true);
    }


    public void Init()
    {
        _internal = this;
        fade = true;
        image.raycastTarget = true;
        image.gameObject.SetActive(true);
        AudioListener.volume = 0;
        Fader(3, Color.black);
    }

    void SetClear()
    {
        if (isClear) return;
        faderColor.a = 1 - GetValue();
        
        AudioListener.volume = Mathf.Clamp(GetValue(), 0, volume);
        image.color = faderColor;

        if (image.color.a <= 0)
        {
            image.color = faderColor;
            image.raycastTarget = false;
            isClear = true;
            curTime = 0;
            enabled = false;
        }
    }

    void SetColor()
    {
        if (isColor) return;
        faderColor.a = GetValue();
        AudioListener.volume = Mathf.Clamp(volume - GetValue(), 0, volume);
        image.color = faderColor;
        image.raycastTarget = true;
        
        if (image.color.a >= 1)
        {
            image.color = faderColor;
            isColor = true;
            curTime = 0;
            enabled = false;
        }
    }

    float GetValue()
    {
        curTime += Time.unscaledDeltaTime;
        return curTime / maxTime;
    }

    void LateUpdate()
    {
        if (Time.unscaledDeltaTime > 1) return;
        if (fade) SetColor(); else SetClear();
    }
}
