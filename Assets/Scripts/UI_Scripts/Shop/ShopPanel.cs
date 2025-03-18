using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class ShopPanel : MonoBehaviour
{
    private Language _language = Language.English;

    private GameController _gameController;
    private float _currentBudget = 0;

    private TMP_Text _budgetText;



    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        var childTextList = gameObject.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text childText in childTextList)
        {
            var childObject = childText.gameObject;

            // filter for log text and progress text
            if (childObject.name == "BudgetText")
            {
                _budgetText = childText;
                break;
            }

        }
   
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameController.currentBudget != _currentBudget)
        {
            _currentBudget = _gameController.currentBudget;
            // Update text
            if (_language == Language.English) _budgetText.text = "Budget: £"+_currentBudget;
            else if (_language == Language.Welsh) _budgetText.text = "NT: £"+_currentBudget;
        }
        // Update budget whenever the game controller budget changes
    }
}
