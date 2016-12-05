

namespace MoviesBot.Data.DTO
{
    class Message
    {
        public string Id { get; set; }
        public User User { get; set; }
        public Chat Chat { get; set; }
        public string Text { get; set; }
    }
}
