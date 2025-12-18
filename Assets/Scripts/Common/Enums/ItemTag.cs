namespace Common.Enums
{
    public enum ItemTag
    {
        Категория = 0,
        
        Стратегия = 1,
        Семейная,
        Вечериночные,
        Хардкорная,
        Настольная_Ролевая_Игра,
        RPG = 6,
        
        
        Ужас_Аркхема,
        Живая_Карточная_Игра,
        Игра_по_Вселенной,
        Ужас_Аркхема_Карточная_Игра = 10,
        
        
        Gloomhaven,
        Древний_Ужас,
        
        Dangeons_and_Dragons,
        Каркасон,
        Fallout,
        Игра_Престолов = 16,
        
        Манчкин,
        Ticket_to_Ride,
        Космический_контакт,
        Варгейм,
        Место_Преступления,
        Цитадели,
        Подарочное_Издание,
        Семь_чудес,
        Мачи_Коро,
        Бэнг = 26,
        
    }
    
    public static class StringToTag
    {
        public static ItemTag Convert(string str)
        {
            return (ItemTag)System.Enum.Parse(typeof(ItemTag), str);
        }
    }
}