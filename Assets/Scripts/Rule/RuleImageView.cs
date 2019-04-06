using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class RuleImageView : MonoBehaviour {
    
    public Image ruleImage1, ruleImage2, ruleImage3;
    private int rulePage = 0;

	void Start () {
        DiactivateRuleImage();
	}
	
    public void ActivateRuleImage()
    {
        gameObject.SetActive(true);
        rulePage = 0;
        ResetRuleImage();
    }

    public void DiactivateRuleImage()
    {
        gameObject.SetActive(false);
    }

    public void PushNextPage()
    {
        rulePage += 1;
        ResetRuleImage();
    }

    public void PushBackPage()
    {
        rulePage -= 1;
        ResetRuleImage();
    }

    void ResetRuleImage()
    {
        if (rulePage == 0)
        {
            ruleImage1.gameObject.SetActive(true);
            ruleImage2.gameObject.SetActive(false);
            ruleImage3.gameObject.SetActive(false);
        }
        else if (rulePage == 1)
        {
            ruleImage1.gameObject.SetActive(false);
            ruleImage2.gameObject.SetActive(true);
            ruleImage3.gameObject.SetActive(false);
        }
        else if (rulePage == 2)
        {
            ruleImage1.gameObject.SetActive(false);
            ruleImage2.gameObject.SetActive(false);
            ruleImage3.gameObject.SetActive(true);
        }
    }
}
