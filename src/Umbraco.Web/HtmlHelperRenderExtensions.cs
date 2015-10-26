using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.WebEncoders;
using Umbraco.Web.Routing;

namespace Umbraco.Web
{
    public static class HtmlHelperRenderExtensions
    {
        public static MvcForm BeginUmbracoForm(this IHtmlHelper html, 
            UmbracoContext umbCtx,
            string action, 
            string controllerName, 
            string area = null,
            object additionalRouteVals = null,
            IDictionary<string, object> htmlAttributes = null,
            FormMethod method = FormMethod.Post)
        {            
            var formAction = umbCtx.RequestPath;
            return html.RenderForm(formAction, method, htmlAttributes, controllerName, action, area, additionalRouteVals);
        }

        /// <summary>
		/// This renders out the form for us
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="formAction"></param>
		/// <param name="method"></param>
		/// <param name="htmlAttributes"></param>
		/// <param name="surfaceController"></param>
		/// <param name="surfaceAction"></param>
		/// <param name="area"></param>		
		/// <param name="additionalRouteVals"></param>
		/// <returns></returns>
		/// <remarks>
		/// This code is pretty much the same as the underlying MVC code that writes out the form
		/// </remarks>
		private static MvcForm RenderForm(this IHtmlHelper htmlHelper,
                                          string formAction,
                                          FormMethod method,
                                          IDictionary<string, object> htmlAttributes,
                                          string surfaceController,
                                          string surfaceAction,
                                          string area,
                                          object additionalRouteVals = null)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            //ensure that the multipart/form-data is added to the html attributes
            if (htmlAttributes.ContainsKey("enctype") == false)
            {
                htmlAttributes.Add("enctype", "multipart/form-data");
            }

            var tagBuilder = new TagBuilder("form");
            tagBuilder.MergeAttributes(htmlAttributes);
            // action is implicitly generated, so htmlAttributes take precedence.
            tagBuilder.MergeAttribute("action", formAction);
            // method is an explicit parameter, so it takes precedence over the htmlAttributes. 
            tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            
            tagBuilder.TagRenderMode = TagRenderMode.StartTag;
            tagBuilder.WriteTo(htmlHelper.ViewContext.Writer, new HtmlEncoder());

            //new UmbracoForm:
            var theForm = new UmbracoForm(htmlHelper.UrlEncoder, htmlHelper.ViewContext, surfaceController, surfaceAction, area, additionalRouteVals);
            
            return theForm;
        }

        /// <summary>
		/// Used for rendering out the Form for BeginUmbracoForm
		/// </summary>
		internal class UmbracoForm : MvcForm
        {
            /// <summary>
            /// Creates an UmbracoForm
            /// </summary>
            /// <param name="urlEncoder"></param>
            /// <param name="viewContext"></param>
            /// <param name="controllerName"></param>
            /// <param name="controllerAction"></param>
            /// <param name="area"></param>
            /// <param name="additionalRouteVals"></param>
            public UmbracoForm(
                IUrlEncoder urlEncoder,
                ViewContext viewContext,
                string controllerName,
                string controllerAction,
                string area,
                object additionalRouteVals = null)
                : base(viewContext)
            {
                _viewContext = viewContext;
                var surfaceFormHelper = viewContext.HttpContext.ApplicationServices.GetRequiredService<SurfaceFormHelper>();
                _encryptedString = surfaceFormHelper.CreateEncryptedRouteString(urlEncoder, controllerName, controllerAction, area, additionalRouteVals);
            }

            private readonly ViewContext _viewContext;
            private bool _disposed;
            private readonly string _encryptedString;

            protected override void Dispose(bool disposing)
            {
                if (this._disposed)
                    return;
                this._disposed = true;

                //write out the hidden surface form routes
                _viewContext.Writer.Write("<input name='ufprt' type='hidden' value='" + _encryptedString + "' />");

                base.Dispose(disposing);
            }
        }

        
        
    }
}
