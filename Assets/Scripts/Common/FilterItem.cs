using System.Collections.Generic;
using Common.Enums;

namespace Common
{
    public class FilterItem
    {
        public string Name { get; set; } = "";
        public int PriceFrom { get; set; } = -1;
        public int PriceTo { get; set; } = int.MaxValue;
        public int NumOfPlayersFrom { get; set; } = -1;
        public int NumOfPlayersTo { get; set; } = int.MaxValue;
        public int MinAge { get; set; } = -1;
        public int AvgPlayTimeFrom { get; set; } = -1;
        public int AvgPlayTimeTo { get; set; } = int.MaxValue;
        public List<ItemTag> Tags { get; set; } = new();
    }
}