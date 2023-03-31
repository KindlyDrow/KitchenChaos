using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryImageUI : MonoBehaviour
{

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

    private const string IS_DELIVERED = "IsDelivered";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManger_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManger_OnRecipeFailed;
        gameObject.SetActive(false);
    }

    private void DeliveryManger_OnRecipeFailed(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        backgroundImage.color = failedColor;
        iconImage.sprite = failedSprite;
        messageText.text = "DELIVERY\nFAILED";
        animator.SetTrigger(IS_DELIVERED);
    }

    private void DeliveryManger_OnRecipeSuccess(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        backgroundImage.color = successColor;
        iconImage.sprite = successSprite;
        messageText.text = "DELIVERY\nSUCCESS";
        animator.SetTrigger(IS_DELIVERED);
    }
}
