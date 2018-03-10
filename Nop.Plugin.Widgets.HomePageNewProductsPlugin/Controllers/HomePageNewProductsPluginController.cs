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
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using Nop.Web.Extensions;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Controllers
{
    public class HomePageNewProductsPluginController : BasePluginController
    {
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;

        public HomePageNewProductsPluginController(ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ICategoryService categoryService,
            IWorkContext workContext,
            IProductService productService,
            IPermissionService permissionService,
            IStoreService storeService,
            IStoreContext storeContext,
            ISettingService settingService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper,
            ISpecificationAttributeService specificationAttributeService,
            ILocalizationService localizationService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            ICacheManager cacheManager)
        {
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._categoryService = categoryService;
            this._workContext = workContext;
            this._productService = productService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._specificationAttributeService = specificationAttributeService;
            this._localizationService = localizationService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._cacheManager = cacheManager;
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
        public ActionResult PublicInfo(int? productThumbPictureSize, string widgetZone, object additionalData = null)
        {
            var numberOfProducts = this._settingService.GetSettingByKey<int>("HomePageNewProductsPluginSettings.NumberOfProductsToDisplay");
            var products = _productService.SearchProducts(orderBy: ProductSortingEnum.CreatedOn).Where(i => i.ProductPictures.FirstOrDefault() != null).Take(numberOfProducts);
            
            bool preparePriceModel = true;
            bool preparePictureModel = true;
            bool prepareSpecificationAttributes = false;
            bool forceRedirectionAfterAddingToCart = false;

            IList<ProductOverviewModel> productOverviewModelList = this.PrepareProductOverviewModels(_workContext,
                _storeContext, _categoryService, _productService, _specificationAttributeService,
                _priceCalculationService, _priceFormatter, _permissionService,
                _localizationService, _taxService, _currencyService,
                _pictureService, _webHelper, _cacheManager,
                _catalogSettings, _mediaSettings, products,
                preparePriceModel, preparePictureModel,
                productThumbPictureSize, prepareSpecificationAttributes,
                forceRedirectionAfterAddingToCart).ToList();

            return View("~/Plugins/Widgets.HomePageNewProductsPlugin/Views/HomePageNewProductsPlugin/PublicInfo.cshtml", productOverviewModelList);
        }
    }
}