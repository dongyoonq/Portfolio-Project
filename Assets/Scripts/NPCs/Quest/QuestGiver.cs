using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
    QuestData quest;

    [NonSerialized] public GameObject questDescriptionPanel;
    GameObject questPanel;
    GameObject questContent;

    GameObject contentPrefab;
    GameObject instanceContent;

    TMP_Text titleText;
    TMP_Text descriptionText;
    TMP_Text rewardText;
    Button acceptButton;

    private void Start()
    {
        questPanel = GameObject.Find("QuestUI");
        questContent = questPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;
        questDescriptionPanel = questPanel.transform.GetChild(1).gameObject;
        acceptButton = questDescriptionPanel.transform.GetChild(1).GetComponent<Button>();
        titleText = questDescriptionPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        descriptionText = questDescriptionPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        rewardText = questDescriptionPanel.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetComponent<TMP_Text>();
        contentPrefab = GameManager.Resource.Load<GameObject>("UI/QuestSlot");
    }

    private void OnEnable()
    {
        quest = GetComponent<NPC>().data.quest;
    }

    public void OpenQuestfromNPC()
    {
        questDescriptionPanel.SetActive(true);
        titleText.text = quest.Title;
        rewardText.text = $"\nReward : Exp {quest.ExpReward}";

        if (!quest.isActive)
        {
            descriptionText.text = quest.Description;
            acceptButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Accept";
            acceptButton.gameObject.SetActive(true);
        }
        else
        {
            if (!quest.Goal.IsReached())
            {
                descriptionText.text = $"<color=#FFFF00>{name}</color>\n{quest.Description}\n<color=#FF0000>Quest is Not finished yet</color>\n<color=#04E2FD>{quest.Target} : {quest.Goal.currentAmount}/{quest.Goal.requiredAmount}</color>";
                acceptButton.gameObject.SetActive(false);
            }
            else
            {
                descriptionText.text = $"<color=#FFFF00>{name}</color>\n{quest.Description}\n<color=#00FF00>Quest completed. Click OK to complete</color>";
                acceptButton.gameObject.SetActive(true);
                acceptButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Ok";
            }
        }
    }

    public void OpenQuest()
    {
        questDescriptionPanel.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        titleText.text = quest.Title;
        rewardText.text = $"\nReward : Exp {quest.ExpReward}";

        if (!quest.isActive)
        {
            descriptionText.text = quest.Description;
        }
        else
        {
            if (!quest.Goal.IsReached())
            {
                descriptionText.text = $"<color=#FFFF00>{name}</color>\n{quest.Description}\n<color=#FF0000>Quest is Not finished yet</color>\n<color=#04E2FD>{quest.Target} : {quest.Goal.currentAmount}/{quest.Goal.requiredAmount}</color>";
            }
            else
            {
                descriptionText.text = $"<color=#FFFF00>{name}</color>\n{quest.Description}\n<color=#00FF00>Quest completed. Return to that NPC</color>";
            }
        }
    }

    public void AcceptQuest()
    {
        if (!quest.isActive)
        {
            questDescriptionPanel.SetActive(false);
            quest.isActive = true;
            Player.Instance.questList.Add(quest);

            instanceContent = Instantiate(contentPrefab, questContent.transform);
            instanceContent.transform.GetChild(0).GetComponent<Text>().text = quest.Title;
            instanceContent.GetComponent<Button>().onClick.AddListener(OpenQuest);
            instanceContent.transform.GetChild(1).gameObject.SetActive(true); // Progress Active

            acceptButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    public void CompleteQuest()
    {
        if (quest.Goal.IsReached())
        {
            Player.Instance.Exp += quest.ExpReward;
            questDescriptionPanel.SetActive(false);
            quest.isActive = false;
            Player.Instance.questList.Remove(quest);
            quest.Goal.currentAmount = 0;
            Destroy(instanceContent);
            acceptButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}