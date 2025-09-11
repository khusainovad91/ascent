using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiQuestsManager : PersistentSingleton<UiQuestsManager>
{
    [SerializeField]
    private RectTransform _mainQuestTab;
    [SerializeField]
    private RectTransform _subQuestsTab;
    [SerializeField]
    private VerticalLayoutGroup _mainQuestsVLG;
    [SerializeField]
    private VerticalLayoutGroup _subQuestsVLG;

    [SerializeField]
    private TextMeshProUGUI questPrefabText;

    public TextMeshProUGUI AddNewQuest(Quest quest)
    {
        var newText = GameObject.Instantiate(questPrefabText);
        newText.text = quest.Description.Value.ToString();

        if (quest.IsMainQuest.Value)
        {
            newText.transform.SetParent(_mainQuestsVLG.transform, false);
            _subQuestsTab.position = _subQuestsTab.position + new Vector3(0, -(_mainQuestTab.transform.childCount - 1) * 60);
        } else
        {
            newText.transform.SetParent(_subQuestsVLG.transform, false);
        }

        return newText;
    }
}
