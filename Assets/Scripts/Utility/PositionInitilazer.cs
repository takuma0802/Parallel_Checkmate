using UnityEngine;
using UnityEngine.UI;

public class PositionInitilazer
{
	public void FixedCenter(RectTransform rect)
    {
        rect.anchoredPosition3D = Vector3.zero;
        rect.sizeDelta = Vector3.zero;
        rect.localScale = Vector3.one;
    }

    public void FixedCenterKeepSize(RectTransform rect)
    {
        rect.anchoredPosition3D = Vector3.zero;
        rect.localScale = Vector3.one;
    }
}
