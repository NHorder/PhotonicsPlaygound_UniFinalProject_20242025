using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteController : MonoBehaviour
{

    private int score;
    private int rank;

    private TMP_Text scoreText;
    private TMP_Text remainingBudgetText;
    private TMP_Text numSatelliteText;
    private TMP_Text numSatelliteDestroyedText;

    private Animator rankAnimator;

    private GameController gameController;


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        RectTransform[] childTransforms = gameObject.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform childTransform in childTransforms)
        {
            GameObject childObject = childTransform.gameObject;

            if (childObject.name == "ScoreText") scoreText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "BudgetRemaining") remainingBudgetText = childObject.GetComponent<TMP_Text>(); 
            else if (childObject.name == "NumberSatellites") numSatelliteText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "NumSatellitesDestroyed") numSatelliteDestroyedText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "FinalRatingSprite") rankAnimator = childObject.GetComponent<Animator>();
        }
    }

    public void GameComplete()
    {
        int startingBudget = gameController.startingBudget;
        int currentBudget = gameController.currentBudget;
        int numSatellites = gameController.worldInfo.numSatellites;
        int numSatellitesDestroyed = gameController.worldInfo.numSatellitesDestroyed;

        CalculateScore(startingBudget,currentBudget,numSatellites,numSatellitesDestroyed);
        CalculateRank();

        scoreText.text  = "Score: "+score;
        remainingBudgetText.text = "Remaining Budget: "+currentBudget;
        numSatelliteText.text = "Number of Satellites Purchased: "+numSatellites;
        numSatelliteDestroyedText.text  = "Number of Satellites Destroyed: "+numSatellitesDestroyed;
        rankAnimator.SetInteger("rank",rank);




    }

    public void CalculateScore(int startingBudget,int currentBudget,int numSatellites,int numSatellitesDestroyed)
    {   
        score = 1000;

    }

    private void CalculateRank()
    {
        if (score > 900) rank = 1;
        else if (score > 800) rank = 2;
        else if (score > 700) rank = 3;
        else if (score > 600) rank = 4;
        else if (score > 500) rank = 5;
        else if (score > 400) rank = 6;
        else rank = 7;
    }

    public void ToLevelSelect()
    {
        SceneController.To_LevelSelection();
    }

    public void Retry()
    {
        gameController.ResetLevel();
    }



}
