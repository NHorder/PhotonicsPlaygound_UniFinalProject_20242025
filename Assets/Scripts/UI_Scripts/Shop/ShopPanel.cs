using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class ShopPanel : MonoBehaviour
{
    /// <summary>
    /// Class to handle shop panel interactions
    /// </summary>
    
    private Language _language = Language.English;

    private GameController _gameController;
    private float _currentBudget = 0;

    private TMP_Text _budgetText;

    private bool _forceUpdate = false;

    private ShopDropDownHandler[] _dropdowns;


    // Start is called before the first frame update
    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        // Locate and find the budget text within child components
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
   
        _dropdowns = gameObject.GetComponentsInChildren<ShopDropDownHandler>();
    }

    /// <summary>
    /// Method called once per frame
    /// </summary>
    void Update()
    {
        // Check 
        if (_gameController.currentBudget != _currentBudget || _forceUpdate)
        {
            _forceUpdate = false;
            _currentBudget = _gameController.currentBudget;
            // Update text
            if (_language == Language.English) _budgetText.text = "Budget: £"+_currentBudget;
            else if (_language == Language.Welsh) _budgetText.text = "Cyllid: £"+_currentBudget;
        }
        // Update budget whenever the game controller budget changes
    }

    /// <summary>
    /// Method to update language
    /// </summary>
    /// <param name="newLanguage"></param>
    public void UpdateLanguage(Language newLanguage)
    {
        _language = newLanguage;
        _forceUpdate = true;

        if (_dropdowns.Length > 0)
        {
            // Loop through each dropdown and notify it to update language
            foreach (ShopDropDownHandler dropdown in _dropdowns)
            {
                dropdown.UpdateLanguage(newLanguage);
            }
        }
    }
}
