using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private Image warningImage;

    private float warningSoundIntevalMax = 0.2f;
    private float warningSoundInterval;

    private const string IS_WARNING = "IsWarning";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        HideWarning();
        barImage.fillAmount = 0f;
    }
    public void SetBarProgress(float barProgress, bool isCooked)
    {
        if (barImage != null)
        {
            barImage.fillAmount = barProgress;
        }
        if (warningImage != null)
        {
            if (barProgress > 0.5f && isCooked && barProgress < 0.99f)
            {
                warningImage.gameObject.SetActive(true);
                warningSoundInterval += Time.deltaTime;
                animator.SetBool(IS_WARNING, true);
                if (warningSoundInterval > warningSoundIntevalMax) {
                    SoundManager.Instance.PlayWarningSound(gameObject.transform.position);
                    warningSoundInterval =0f;
                }

            } else 
            {
                HideWarning();

            }
        }
    }

    private void HideWarning()
    {
        if (warningImage != null)
        {
            animator.SetBool(IS_WARNING, false);
            warningImage.gameObject.SetActive(false);
        }
    }
}
