using System.Collections.Generic;
using Common.Enums;

namespace Common
{
    public struct FilterItem
    {
        public string Name;
        public int PriceFrom;
        public int PriceTo;
        public int NumOfPlayersFrom;
        public int NumOfPlayersTo;
        public int MinAge;
        public int AvgPlayTimeFrom;
        public int AvgPlayTimeTo;
        public List<ItemTag> Tags;
    }
}