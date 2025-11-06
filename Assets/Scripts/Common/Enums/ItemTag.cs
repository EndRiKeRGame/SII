namespace Common.Enums
{
    public enum ItemTag
    {
        Категория = 0,
        
        Стратегия,
        Семейная,
        Вечериночные,
        Хардкорная,
        Настольная_Ролевая_Игра,
        RPG,
        
        
        Ужас_Аркхема,
        Живая_Карточная_Игра,
        Игра_по_Вселенной,
        Ужас_Аркхема_Карточная_Игра,
        
        
        Gloomhaven,
        Древний_Ужас,
        
        Dangeons_and_Dragons,
        Каркасон,
        Fallout,
        Игра_Престолов,
        
        Манчкин,
        Ticket_to_Ride,
        Космический_контакт,
        Варгейм,
        Место_Преступления,
        Цитадели,
        Подарочное_Издание,
        Семь_чудес,
        Мачи_Коро,
        Бэнг,
        
    }
    
    public static class StringToTag
    {
        public static ItemTag Convert(this string str)
        {
            return (ItemTag)System.Enum.Parse(typeof(ItemTag), str);
        }
    }
}