namespace MovieBookingAPI.Models.DTOs.Seat
{
    public class SeatGenerationInputDTO
    {
        public int ScreenId { get; set; }
        public string Rows { get; set; }  //ROws like "A-Z"
        public Dictionary<string, int> ColumnsPerRow { get; set; } // { "A-W" : 20, "X-Z" : 15  }
        public Dictionary<string, decimal> RowPrices { get; set; } // { "A-W" : 170 , "X-Z" : 60 } 
    }
}