using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Web.Models;

namespace Umbraco.Web.Controllers
{
    public class PublishedContentModelBinder : IModelBinder
    {   
        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        { 
            if (bindingContext.ModelType != typeof(IPublishedContent) || bindingContext.FieldName != "publishedContent")
            {
                // let other binders run.
                return ModelBindingResult.NoResultAsync;
            }

            var requestServices = bindingContext.OperationBindingContext.HttpContext.RequestServices;
            var umbCtx = (UmbracoContext)requestServices.GetRequiredService(typeof(UmbracoContext));

            var model = umbCtx.PublishedContent;
            if (model == null)
            {
                // let other binders run.
                return ModelBindingResult.NoResultAsync;
            }

            bindingContext.ValidationState.Add(model, new ValidationStateEntry() { SuppressValidation = true });

            return ModelBindingResult.SuccessAsync(bindingContext.FieldName, model);
        }
    }
}