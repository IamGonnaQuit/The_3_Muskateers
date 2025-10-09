using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PotionNameUI : MonoBehaviour
{
    public static PotionNameUI Instance;

    [Header("UI Text References")]
    public TextMeshProUGUI leftHandText;
    public TextMeshProUGUI rightHandText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        ClearText();
    }

    public void ShowPotionName(string potionName, XRBaseInteractor interactor)
    {
        if (interactor is XRDirectInteractor)
        {
            // Check which controller this belongs to (by name)
            if (interactor.name.ToLower().Contains("left"))
                leftHandText.text = potionName;
            else if (interactor.name.ToLower().Contains("right"))
                rightHandText.text = potionName;
        }
    }

    public void ClearText(XRBaseInteractor interactor = null)
    {
        // Clear specific hand if known
        if (interactor != null)
        {
            if (interactor.name.ToLower().Contains("left"))
                leftHandText.text = "";
            else if (interactor.name.ToLower().Contains("right"))
                rightHandText.text = "";
        }
        else
        {
            // Clear both
            leftHandText.text = "";
            rightHandText.text = "";
        }
    }
}
