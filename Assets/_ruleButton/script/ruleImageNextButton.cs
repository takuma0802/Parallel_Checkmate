using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ruleImageNextButton : MonoBehaviour
{
    public Image ruleImage1;
    public Image ruleIMage2;
    public Image ruleImage3;

    int rulePage = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (rulePage == 0)
        {
            ruleImage1.gameObject.SetActive(true);
            ruleIMage2.gameObject.SetActive(false);
            ruleImage3.gameObject.SetActive(false);
        }
        else if (rulePage == 1)
        {
            ruleImage1.gameObject.SetActive(false);
            ruleIMage2.gameObject.SetActive(true);
            ruleImage3.gameObject.SetActive(false);
        }
        else if (rulePage == 2)
        {
            ruleImage1.gameObject.SetActive(false);
            ruleIMage2.gameObject.SetActive(false);
            ruleImage3.gameObject.SetActive(true);
        }
    }

    public void PushRuleButton()
    {
        rulePage = 0;
    }
    public void PushNextPage()
    {
        rulePage += 1;
    }
    public void PushBackPage()
    {
        rulePage -= 1;
    }
}
