using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Models;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Seo;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Controllers
{
    public class HomePageNewProductsPluginController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public HomePageNewProductsPluginController(IWorkContext workContext,
            IProductService productService,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._productService = productService;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var homePageNewProductsPluginSettings = _settingService.LoadSetting<HomePageNewProductsPluginSettings>(storeScope);
            var model = new ConfigurationModel();
            model.WidgetZone = homePageNewProductsPluginSettings.WidgetZone;
            model.NumberOfProductsToDisplay = homePageNewProductsPluginSettings.NumberOfProductsToDisplay;
            
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.WidgetZone_OverrideForStore = _settingService.SettingExists(homePageNewProductsPluginSettings, x => x.WidgetZone, storeScope);
                model.NumberOfProductsToDisplay_OverrideForStore = _settingService.SettingExists(homePageNewProductsPluginSettings, x => x.NumberOfProductsToDisplay, storeScope);
            }

            return View("~/Plugins/Widgets.HomePageNewProductsPlugin/Views/HomePageNewProductsPlugin/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var homePageNewProductsPluginSettings = _settingService.LoadSetting<HomePageNewProductsPluginSettings>(storeScope);
            homePageNewProductsPluginSettings.WidgetZone = model.WidgetZone;
            homePageNewProductsPluginSettings.NumberOfProductsToDisplay = model.NumberOfProductsToDisplay;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.WidgetZone_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(homePageNewProductsPluginSettings, x => x.WidgetZone, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(homePageNewProductsPluginSettings, x => x.WidgetZone, storeScope);
            
            if (model.NumberOfProductsToDisplay_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(homePageNewProductsPluginSettings, x => x.NumberOfProductsToDisplay, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(homePageNewProductsPluginSettings, x => x.NumberOfProductsToDisplay, storeScope);
            
            //now clear settings cache
            _settingService.ClearCache();
            
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            
            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            var numberOfProducts = this._settingService.GetSettingByKey<int>("HomePageNewProductsPluginSettings.NumberOfProductsToDisplay");
            var products = _productService.SearchProducts(orderBy: ProductSortingEnum.CreatedOn).Take(numberOfProducts);
            var productsModel = products
              .Select(x =>
              {
                  var latestModel = new LatestProductModel
                  {
                      Id = x.Id,
                      Name = x.Name,
                      Picture = x.ProductPictures.FirstOrDefault(),
                      SeName = x.GetSeName()
                  };
                  return latestModel;
              })
              .ToList();

            return View("~/Plugins/Widgets.HomePageNewProductsPlugin/Views/HomePageNewProductsPlugin/PublicInfo.cshtml", productsModel);
        }
    }
}