using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonStateChanger
{
    private bool            pressed;
    private bool            setBackgroundColor; //false = set image tint instead
    private VisualElement   button;
    private Color           originalColor   = Color.white;
    private Color           pressedColor    = new Color(.95f, .95f, .95f, 1f);

    private bool            ignore          = false;

    public bool Ignore      { set { ignore = value; } get { return ignore; } }

    public ButtonStateChanger(VisualElement button, Color originalColor, Color pressedColor, bool setBGColor = true)
    {
        this.button         = button;
        this.originalColor  = originalColor;
        this.pressedColor   = pressedColor;
        pressed             = false;
        setBackgroundColor  = setBGColor;
    }

    public ButtonStateChanger(VisualElement button, bool setBGColor = true)
    {
        this.button         = button;
        pressed             = false;
        setBackgroundColor  = setBGColor;
    }

    public void OnPointerDown(PointerDownEvent evt)
    {
        if (Ignore)
            return;

        button.style.right  = -4f;
        button.style.bottom = -4f;

        if (setBackgroundColor)
            button.SetColor(pressedColor);
        else
            button.style.unityBackgroundImageTintColor = new StyleColor(pressedColor);

        pressed             = true;
    }

    public void OnPointerUp(PointerUpEvent evt)
    {
        if (Ignore)
            return;

        if (pressed == false)
            return;

        if (setBackgroundColor)
            button.SetColor(originalColor);
        else
            button.style.unityBackgroundImageTintColor = new StyleColor(originalColor);

        StyleLength s       = new StyleLength(StyleKeyword.Auto);
        button.style.right  = s;
        button.style.bottom = s;

        pressed             = false;
    }

    public void OnPointerOff(PointerLeaveEvent evt)
    {
        OnPointerUp(null);
    }
}