using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Button startGame;
    [SerializeField]
    private TMP_InputField playersAmount;
    int playersCount = 1;

    void Start()
    {
        //todo заглушка выбора героев
        ChooseHeroes();
        startGame.onClick.AddListener(StartGame);
        playersAmount.onEndEdit.AddListener(SetUpPlayersAmount);
    }

    private void SetUpPlayersAmount(string arg0)
    {
        playersCount = int.TryParse(arg0, out playersCount) ? playersCount : 1;
    }

    void ChooseHeroes()
    {
        //select hero on screen and put it in a hero list
        var player1Prefab = Resources.Load("Heroes/Armadilo") as GameObject;

        //todo refactor
        //HeroData player1 = new HeroData(1, player1Prefab, Color.blue);
        //GameManager.Instance.SetNewHero(player1);
    }

    void StartGame()
    {
        //SceneFader.Instance.StartSceneWithFaderServerRpc(CustomScene.TestScene);
        StartCoroutine(SceneFader.Instance.LoadSceneWithFader(CustomScene.TestScene));
        //CustomSceneManager.Instance.LoadScene(Scene.TestScene);
    }
}
