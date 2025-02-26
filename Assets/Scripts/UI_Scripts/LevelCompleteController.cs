using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteController : MonoBehaviour
{

    private int score = 0;
    private int rank = 7;

    private TMP_Text scoreText;
    private TMP_Text statisticsText;

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
            else if (childObject.name == "StatisticsText") statisticsText = childObject.GetComponent<TMP_Text>(); 
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

        string text = "";
        text += "- Remaining Budget: "+currentBudget + "\n";
        text += "- Number of Satellites Purchased: "+numSatellites + "\n";
        text += "- Number of Satellites Destroyed: "+numSatellitesDestroyed;

        statisticsText.text = text;
        rankAnimator.SetInteger("Rank",rank);




    }

    public void CalculateScore(int startingBudget,int currentBudget,int numSatellites,int numSatellitesDestroyed)
    {   
        score = 0;

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
