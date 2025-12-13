using System;
using System.Collections.Generic;
using Common;
using Common.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

public class FiltersView : MonoBehaviour
{
    [Header("Цена")]
    [SerializeField]
    private TMP_InputField _priceFromInput;
    
    [SerializeField]
    private TMP_InputField _priceToInput;
    
    [Header("Название")]
    [SerializeField]
    private TMP_InputField _nameInput;
    
    [Header("Количество игроков")]
    [SerializeField]
    private TMP_InputField _numOfPlayersFromInput;
    
    [SerializeField]
    private TMP_InputField _numOfPlayersToInput;
    
    [Header("Минимальный возраст")]
    [SerializeField]
    private TMP_InputField _minAgeFromInput;
    
    [Header("Среднее время игры")]
    [SerializeField]
    private TMP_InputField _avgPlayTimeFromInput;
    
    [SerializeField]
    private TMP_InputField _avgPlayTimeToInput;
    
    [Header("Теги")]
    [SerializeField]
    private TMP_Dropdown _tagsDropdown;
    
    [Space(5)]
    [Header("Кнопки")]
    [SerializeField]
    private Button _filterButton;
    
    [SerializeField]
    private Button _clearButton;

    [Inject]
    public void Construct()
    {
        _tagsDropdown.options = new List<TMP_Dropdown.OptionData>();
        var all = Enum.GetValues(typeof(ItemTag));
        foreach (var one in all)
            _tagsDropdown.options.Add(new TMP_Dropdown.OptionData(one.ToString()));
        
        _clearButton.onClick.AddListener(ClearFilters);
    }

    public void AddButtons(Action onSubmit, Action onClear)
    {
        _filterButton.onClick.AddListener(new UnityAction(onSubmit));
        _clearButton.onClick.AddListener(new UnityAction(onClear));
    }
    
    public void OnDestroy()
    {
        _filterButton.onClick.RemoveAllListeners();
        _clearButton.onClick.RemoveAllListeners();
    }

    public void ApplyFilters(ref FilterItem filter)
    {
        ApplyPriceFilter(ref filter);
        ApplyNameFilter(ref filter);
        ApplyNumOfPlayerFilter(ref filter);
        ApplyMinAgeFilter(ref filter);
        ApplyTagsFilter(ref filter);
        ApplyAvgPlayTimeFilter(ref filter);
    }

    public void ClearFilters()
    {
        _priceFromInput.text = "";
        _priceToInput.text = "";
        _nameInput.text = "";
        _numOfPlayersFromInput.text = "";
        _numOfPlayersToInput.text = "";
        _minAgeFromInput.text = "";
        _avgPlayTimeFromInput.text = "";
        _avgPlayTimeToInput.text = "";
        _tagsDropdown.value = 0;
    }

    private void ApplyPriceFilter(ref FilterItem filter)
    {
        string priceFromInput = _priceFromInput.text;
        string priceToInput = _priceToInput.text;
        
        if (String.IsNullOrEmpty(priceFromInput))
            priceFromInput = "-1";
        
        if (String.IsNullOrEmpty(priceToInput))
            priceToInput = $"{int.MaxValue}";
        
        if (!int.TryParse(priceFromInput, out var from) ||
            !int.TryParse(priceToInput, out var to))
            return;
        
        filter.PriceFrom = from;
        filter.PriceTo = to;
    }
    
    private void ApplyNameFilter(ref FilterItem filter)
    {
        filter.Name = _nameInput.text;
    }
    
    private void ApplyNumOfPlayerFilter(ref FilterItem filter)
    {
        string numOfPlayersFromInput = _numOfPlayersFromInput.text;
        string numOfPlayersToInput = _numOfPlayersToInput.text;
        
        if (String.IsNullOrEmpty(numOfPlayersFromInput))
            numOfPlayersFromInput = "-1";
        
        if (String.IsNullOrEmpty(numOfPlayersToInput))
            numOfPlayersToInput = $"{int.MaxValue}";
        
        if (!int.TryParse(numOfPlayersFromInput, out var from) ||
            !int.TryParse(numOfPlayersToInput, out var to))
            return;
        
        filter.NumOfPlayersFrom = from;
        filter.NumOfPlayersTo = to;
    }
    
    private void ApplyMinAgeFilter(ref FilterItem filter)
    {
        string minAgeFromInput = _minAgeFromInput.text;
        
        if (String.IsNullOrEmpty(minAgeFromInput))
            minAgeFromInput = "-1";
        
        if (!int.TryParse(minAgeFromInput, out var from))
            return;
        
        filter.MinAge = from;
    }
    
    private void ApplyAvgPlayTimeFilter(ref FilterItem filter)
    {
        string avgAgePlayFromInput = _avgPlayTimeFromInput.text;
        string avgAgePlayToInput = _avgPlayTimeToInput.text;
        
        if (String.IsNullOrEmpty(avgAgePlayFromInput))
            avgAgePlayFromInput = "-1";
        
        if (String.IsNullOrEmpty(avgAgePlayToInput))
            avgAgePlayFromInput = $"{int.MaxValue}";
        
        if (!int.TryParse(avgAgePlayFromInput, out var from) ||
            !int.TryParse(avgAgePlayToInput, out var to))
            return;
        
        filter.AvgPlayTimeFrom = from;
        filter.AvgPlayTimeTo = to;
    }
    
    private void ApplyTagsFilter(ref FilterItem filter)
    {
        filter.Tags = GetSelectedTexts(_tagsDropdown, _tagsDropdown.value);
    }
    
    private List<ItemTag> GetSelectedTexts(TMP_Dropdown dropdown, int maskValue)
    {
        List<ItemTag> selectedTexts = new List<ItemTag>();
    
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if ((maskValue & (1 << i)) != 0)
            {
                selectedTexts.Add(StringToTag.Convert(dropdown.options[i].text));
            }
        }
    
        return selectedTexts;
    }
}