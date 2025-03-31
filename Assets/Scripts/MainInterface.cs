using UnityEngine;
using UnityEngine.UI;

public class MainInterface : MonoBehaviour
{
    public bool isPublic = true;

    public Image PrivacyCheckbox;
    public Sprite[] CheckboxSprites;
    public GameObject[] Canvases;
    public GameObject ChatCreationPanel;

    private string lastCanvas = "Main Canvas";

    private DataManager dataManager;

    private void Awake()
    {
        dataManager = GetComponent<DataManager>();
    }

    public void ToggleInterface(string currentCanvasName)
    {
        if (lastCanvas == currentCanvasName || dataManager.isChangeUsernamePanelOpened)
            return;

        foreach (GameObject element in Canvases)
        {
            if (element.name == currentCanvasName)
            {
                element.GetComponent<CanvasGroup>().blocksRaycasts = true;
                element.GetComponent<Animator>().SetTrigger("Fade In");
            }
            else
            {
                if (element.GetComponent<CanvasGroup>().blocksRaycasts == true)
                {
                    element.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    element.GetComponent<Animator>().SetTrigger("Fade Out");
                }
            }
        }

        lastCanvas = currentCanvasName;
    }

    public void ToggleChatCreationPanelButton(bool action)
    {
        ChatCreationPanel.SetActive(action);
    }

    public void TogglePrivacyButton()
    {
        if (isPublic)
        {
            isPublic = false;
            PrivacyCheckbox.sprite = CheckboxSprites[0];
        }
        else
        {
            isPublic = true;
            PrivacyCheckbox.sprite = CheckboxSprites[1];
        }
    }
}