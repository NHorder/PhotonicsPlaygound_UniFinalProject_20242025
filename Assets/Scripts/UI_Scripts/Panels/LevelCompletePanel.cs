using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class LevelCompletePanel : MonoBehaviour
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
        // Collect game controller
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        // Collect child gameObjects
        var childTransformsList = gameObject.GetComponentsInChildren<RectTransform>();

        // Loop through and sort to find needed objects to update 
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
        // Collect information from game controller needed to clacualte the score
        var startingBudget = _gameController.startingBudget;
        var currentBudget = _gameController.currentBudget;
        var numSatellites = _gameController.worldInfo.numSatellites;
        var numSatellitesDestroyed = _gameController.worldInfo.numSatellitesDestroyed;

        // Calculate score and rank - score is saved as private variable
        CalculateScore(startingBudget,currentBudget,numSatellites,numSatellitesDestroyed);
        CalculateRank();

        // Update the score text
        _scoreText.text  = $"Score: {_score}";

        // Update the satistics text
        _statisticsText.text = $"- Remaining Budget: {currentBudget}\n- Number of Satellites Purchased: {numSatellites}\n- Number of Satellites Destroyed: {numSatellitesDestroyed}";
        
        // Notify animator to change visual dependent on rank (1 = S, 2 = A, 3 = B, 4 = C, 5 = D, 6 = E, > 7 = F)
        _rankAnimator.SetInteger("Rank",_rank);

    }

    public void CalculateScore(int startingBudget,int currentBudget,int numSatellites,int numSatellitesDestroyed)
    {   
        // Calculate score using an equation
        _score = 0;
    }

    private void CalculateRank()
    {
        // Calcualte rank based on score
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
        // Transition to level selection scene - called by button
        SceneController.ToLevelSelection();
    }

    public void Retry()
    {
        // Call to game controller to reset the level - called by a button
        _gameController.ResetLevel();
    }



}
