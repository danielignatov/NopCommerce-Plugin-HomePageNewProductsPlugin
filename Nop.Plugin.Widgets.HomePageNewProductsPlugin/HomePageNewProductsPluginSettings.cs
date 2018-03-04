using Nop.Core.Configuration;
using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Enums;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin
{
    public class HomePageNewProductsPluginSettings : ISettings
    {
        public WidgetZones WidgetZone { get; set; }

        public int NumberOfProductsToDisplay { get; set; }
    }
}