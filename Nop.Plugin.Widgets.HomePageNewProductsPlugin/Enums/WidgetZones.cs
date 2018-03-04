using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Enums
{
    public enum WidgetZones
    {
        [Display(Name = "Home page top")]
        home_page_top = 1,
        [Display(Name = "Home page bottom")]
        home_page_bottom = 2,
        [Display(Name = "Home page before categories")]
        home_page_before_categories = 3,
        [Display(Name = "Home page before products")]
        home_page_before_products = 4,
        [Display(Name = "Home page before sellers")]
        home_page_before_best_sellers = 5,
        [Display(Name = "Home page before news")]
        home_page_before_news = 6,
        [Display(Name = "Home page before poll")]
        home_page_before_poll = 7
    }
}