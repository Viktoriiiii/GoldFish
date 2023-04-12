using GoldFish.Models;
using System.Collections.Generic;

namespace GoldFish.Classes
{
    public static class Helper
    {
        public static ContextFish ContextFish { get; set; }
        public static User User { get; set; }
        public static Product Product { get; set; }
        public static List<OrderProduct> ListOrderProduct { get; set; }
        public static Order Order { get; set; }
    }
}
