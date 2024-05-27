namespace MiniProjectAPI.Models
{
    public class Snack
    {
            public int SnackId { get; set; }
            public decimal Price { get; set; }
            public string Name { get; set; }

            //public int Quantity { get; set; }
            public ICollection<BookingSnack> BookingSnacks { get; set; }
    }
}