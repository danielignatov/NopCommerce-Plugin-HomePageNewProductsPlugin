using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Models
{
    public class LatestProductModel : BaseNopModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ProductPicture Picture { get; set; }

        // Url
        public string SeName { get; set; }
    }
}