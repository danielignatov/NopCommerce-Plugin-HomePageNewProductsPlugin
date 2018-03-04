using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Enums;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.HomePageNewProductsPlugin.WidgetZone")]
        public WidgetZones WidgetZone  { get; set; }
        public bool WidgetZone_OverrideForStore { get; set; }

        [Range(1, 12)]
        [NopResourceDisplayName("Plugins.Widgets.HomePageNewProductsPlugin.NumberOfProductsToDisplay")]
        public int NumberOfProductsToDisplay { get; set; }
        public bool NumberOfProductsToDisplay_OverrideForStore { get; set; }
    }
}