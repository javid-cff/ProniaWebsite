namespace Pronia.ViewModels.BasketViewModels
{
    public class BasketVM
    {
        public List<BasketItemVM> Items { get; set; } = new List<BasketItemVM>();
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Count);
    }
}
