namespace WISHLIST.Models.Domain
{
    public class InteractionModel
    {
        public string Id { get; set; }

        public string FirstUsername { get; set; }

        public string SecondUsername { get; set; }

        public InteractionType InteractionType { get; set; }
    }
}
