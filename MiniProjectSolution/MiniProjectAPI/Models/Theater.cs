namespace MiniProjectAPI.Models
{
    public class Theater
    {
        public int TheaterId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public ICollection<Screen> Screens { get; set; }
    }
}