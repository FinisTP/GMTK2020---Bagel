using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;
    [SerializeField]
    private Camera uiCamera;

    private Text tooltipText;
    private RectTransform backgroundRectTransform;

    private void Awake()
    {
        instance = this;
        backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
        tooltipText = GameObject.Find("text").GetComponent<Text>();

        showTooltip("Test tooltip text");
        

    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition,
            uiCamera, out localPoint);
        transform.localPosition = localPoint;
    }
    private void showTooltip (string tooltip)
    {
        gameObject.SetActive(true);
        tooltipText.text = tooltip;
        float paddingTextSize = 4f;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + paddingTextSize * 2, 
            tooltipText.preferredHeight + paddingTextSize * 2);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void hideTooltip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipString)
    {
        instance.showTooltip(tooltipString);
    }
    public static void HideTooltip_Static()
    {
        instance.hideTooltip();
    }
}
