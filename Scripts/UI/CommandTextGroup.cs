using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandTextGroup : MonoBehaviour
{
    [NonSerialized]
    public VerticalLayoutGroup Vlg;
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private CommandButtonUI commandButtonPrefab;

    private void Awake()
    {
        Vlg = GetComponent<VerticalLayoutGroup>();
    }


    void Start()
    {
        _canvasGroup = Vlg.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = Vlg.gameObject.AddComponent<CanvasGroup>();
    }

    public void SetUpCommandTextGroup(HashSet<RightClickCommand> commands)
    {
        foreach (var command in commands)
        {
            var button = Instantiate(commandButtonPrefab, this.transform);
            button.SetCommandAndVlg(command, this);
        }
        Debug.Log("Commands count: " + commands.Count);
    }

    public void ClearCommandTextGroup()
    {
        UtilClass.DestroyChilds(this.transform);
    }

    public void TurnOff()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public void TurnOn()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }
}
