using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteController : MonoBehaviour
{

    private int _score = 0;
    private int _rank = 7;

    private TMP_Text _scoreText;
    private TMP_Text _statisticsText;

    private Animator _rankAnimator;

    private GameController _gameController;


    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        var childTransformsList = gameObject.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform childTransform in childTransformsList)
        {
            var childObject = childTransform.gameObject;

            if (childObject.name == "ScoreText") _scoreText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "StatisticsText") _statisticsText = childObject.GetComponent<TMP_Text>(); 
            else if (childObject.name == "FinalRatingSprite") _rankAnimator = childObject.GetComponent<Animator>();
        }
    }

    public void GameComplete()
    {
        var startingBudget = _gameController.startingBudget;
        var currentBudget = _gameController.currentBudget;
        var numSatellites = _gameController.worldInfo.numSatellites;
        var numSatellitesDestroyed = _gameController.worldInfo.numSatellitesDestroyed;

        CalculateScore(startingBudget,currentBudget,numSatellites,numSatellitesDestroyed);
        CalculateRank();

        _scoreText.text  = "Score: "+_score;

        var text = $"- Remaining Budget: {currentBudget}\n- Number of Satellites Purchased: {numSatellites}\n- Number of Satellites Destroyed: {numSatellitesDestroyed}";

        _statisticsText.text = text;
        _rankAnimator.SetInteger("Rank",_rank);




    }

    public void CalculateScore(int startingBudget,int currentBudget,int numSatellites,int numSatellitesDestroyed)
    {   
        _score = 0;
    }

    private void CalculateRank()
    {
        if (_score > 900) _rank = 1;
        else if (_score > 800) _rank = 2;
        else if (_score > 700) _rank = 3;
        else if (_score > 600) _rank = 4;
        else if (_score > 500) _rank = 5;
        else if (_score > 400) _rank = 6;
        else _rank = 7;
    }

    public void ToLevelSelect()
    {
        SceneController.ToLevelSelection();
    }

    public void Retry()
    {
        _gameController.ResetLevel();
    }



}
