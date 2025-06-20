﻿namespace PassionStore.Web.Models.Views
{
    public class ProductDetailsView
    {
        public ProductView Product { get; set; }
        public List<ProductView> RelatedProducts { get; set; }
        public List<ProductRatingView> Ratings { get; set; }
        public ProductRatingView UserRating { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
