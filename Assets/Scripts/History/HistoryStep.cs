using System.Collections.Generic;
using Common;

namespace History
{
    public struct HistoryStep
    {
        public FilterItem Filter;
        public List<Item> Likes;
        public List<Item> Dislikes;
    }
}