using System.Collections.Generic;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using System.Web.Routing;
using Nop.Services.Localization;
using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Enums;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin
{
    /// <summary>
    /// Plugin
    /// </summary>
    public class HomePageNewProductsPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;

        public HomePageNewProductsPlugin(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// Widget zones enumerable is contained in this project under Enums/WidgetZones
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { this._settingService.GetSettingByKey<string>("HomePageNewProductsPluginSettings.WidgetZone") };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "HomePageNewProductsPlugin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.HomePageNewProductsPlugin.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "HomePageNewProductsPlugin";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.HomePageNewProductsPlugin.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            // Default settings
            var settings = new HomePageNewProductsPluginSettings
            {
                WidgetZone = WidgetZones.home_page_top,
                NumberOfProductsToDisplay = 4
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.WidgetZone", "Widget zone");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.WidgetZone", "Зона за показ", "bg-BG");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.WidgetZone.Hint", "Select which area of the site it will cover");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.WidgetZone.Hint", "Изберете зона за показване", "bg-BG");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.NumberOfProductsToDisplay", "Number of products to display");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.NumberOfProductsToDisplay", "Брой на показваните продукти", "bg-BG");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.NumberOfProductsToDisplay.Hint", "Select between 1 and 12");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.NumberOfProductsToDisplay.Hint", "Изберете между 1 и 12", "bg-BG");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.LatestProducts", "Latest Products");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.LatestProducts", "Последни Продукти", "bg-BG");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<HomePageNewProductsPluginSettings>();

            this.DeletePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.WidgetZone");
            this.DeletePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.WidgetZone.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.NumberOfProductsToDisplay");
            this.DeletePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.NumberOfProductsToDisplay.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.HomePageNewProductsPlugin.LatestProducts");

            base.Uninstall();
        }
    }
}