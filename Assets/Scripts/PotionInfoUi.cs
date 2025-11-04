using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PotionInfoUI : MonoBehaviour
{
    public static PotionInfoUI Instance;

    [Header("UI References")]
    public GameObject leftHandPanel;
    public GameObject rightHandPanel;

    [Header("Text References")]
    public TextMeshProUGUI leftHandTitle;
    public TextMeshProUGUI leftHandDescription;
    public TextMeshProUGUI rightHandTitle;
    public TextMeshProUGUI rightHandDescription;

    [Header("Color References")]
    public Renderer leftColorSwatch;
    public Renderer rightColorSwatch;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HideBoth();
    }

    public void ShowPotionInfo(PotionData data, XRBaseInteractor interactor)
    {
        if (data == null || interactor == null) return;

        bool isLeft = interactor.name.ToLower().Contains("left");

        if (isLeft)
        {
            if (leftHandPanel != null) leftHandPanel.SetActive(true);
            if (leftHandTitle != null) leftHandTitle.text = data.potionName;
            if (leftHandDescription != null) leftHandDescription.text = data.potionDescription;
            if (leftColorSwatch != null) leftColorSwatch.material.color = data.potionColor;
        }
        else
        {
            if (rightHandPanel != null) rightHandPanel.SetActive(true);
            if (rightHandTitle != null) rightHandTitle.text = data.potionName;
            if (rightHandDescription != null) rightHandDescription.text = data.potionDescription;
            if (rightColorSwatch != null) rightColorSwatch.material.color = data.potionColor;
        }
    }

    public void HidePotionInfo(XRBaseInteractor interactor)
    {
        if (interactor == null) return;
        bool isLeft = interactor.name.ToLower().Contains("left");

        if (isLeft)
        {
            if (leftHandPanel != null) leftHandPanel.SetActive(false);
        }
        else
        {
            if (rightHandPanel != null) rightHandPanel.SetActive(false);
        }
    }

    public void HideBoth()
    {
        if (leftHandPanel != null) leftHandPanel.SetActive(false);
        if (rightHandPanel != null) rightHandPanel.SetActive(false);
    }
}
