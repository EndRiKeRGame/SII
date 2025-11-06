using System;
using System.Collections.Generic;
using Common;
using Common.Enums;
using TMPro;
using UnityEngine;
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
    public Button FilterButton;
    
    [SerializeField]
    public Button ClearButton;

    public FilterItem FilterItem { get; private set; }

    [Inject]
    public void Construct()
    {
        _tagsDropdown.options = new List<TMP_Dropdown.OptionData>();
        var all = Enum.GetValues(typeof(ItemTag));
        foreach (var one in all)
            _tagsDropdown.options.Add(new TMP_Dropdown.OptionData(one.ToString()));
        
        FilterItem = new FilterItem();
        FilterButton.onClick.AddListener(ApplyFilters);
        ClearButton.onClick.AddListener(ClearFilters);
    }
    
    public void OnDestroy()
    {
        FilterButton.onClick.RemoveListener(ApplyFilters);
        ClearButton.onClick.RemoveListener(ClearFilters);
    }

    public void ApplyFilters()
    {
        ApplyPriceFilter();
        ApplyNameFilter();
        ApplyNumOfPlayerFilter();
        ApplyMinAgeFilter();
        ApplyTagsFilter();
        ApplyAvgPlayTimeFilter();
    }
    
    public void ClearFilters()
    {
        FilterItem = new FilterItem();
        
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

    private void ApplyPriceFilter()
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
        
        FilterItem.PriceFrom = from;
        FilterItem.PriceTo = to;
    }
    
    private void ApplyNameFilter()
    {
        FilterItem.Name = _nameInput.text;
    }
    
    private void ApplyNumOfPlayerFilter()
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
        
        FilterItem.NumOfPlayersFrom = from;
        FilterItem.NumOfPlayersTo = to;
    }
    
    private void ApplyMinAgeFilter()
    {
        string minAgeFromInput = _minAgeFromInput.text;
        
        if (String.IsNullOrEmpty(minAgeFromInput))
            minAgeFromInput = "-1";
        
        if (!int.TryParse(minAgeFromInput, out var from))
            return;
        
        FilterItem.MinAge = from;
    }
    
    private void ApplyAvgPlayTimeFilter()
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
        
        FilterItem.AvgPlayTimeFrom = from;
        FilterItem.AvgPlayTimeTo = to;
    }
    
    private void ApplyTagsFilter()
    {
        FilterItem.Tags = GetSelectedTexts(_tagsDropdown, _tagsDropdown.value);
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