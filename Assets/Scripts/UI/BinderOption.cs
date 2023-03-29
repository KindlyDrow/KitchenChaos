using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BinderOption : MonoBehaviour
{
    
    [SerializeField] private GameInput.Binders binders;

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI text; 
    [SerializeField] private GameObject pressAny;

    private void Start()
    {
        button.onClick.AddListener(OnBindClick);
        UpadateVisuale();
    }

    private void OnBindClick()
    {
        ShowRebindKey();
        GameInput.Instanse.RebindBinding(binders, HideRebindKey);
        UpadateVisuale();
    }

    private void UpadateVisuale()
    {
        buttonText.text = GameInput.Instanse.GetBindingText(binders);
    }

    private void ShowRebindKey()
    {
        button.gameObject.SetActive(false);
        pressAny.SetActive(true);
    }

    private void HideRebindKey()
    {
        button.gameObject.SetActive(true);
        pressAny.SetActive(false);
        UpadateVisuale();
    }
}
