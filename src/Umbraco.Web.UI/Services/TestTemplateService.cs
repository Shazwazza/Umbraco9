using System;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Umbraco.Services
{
    public class TestTemplateService : ITemplateService
    {
        public const string Template1Id = "33FDCCA6-F563-4686-BA3D-1BFC4575ED21";
        public const string Template2Id = "C77407DF-AD40-4149-B609-7FBB053E5EEC";
        public const string Template3Id = "39A5FE40-1AF0-4293-A2F9-651B3C3454E9";

        public ITemplate GetTemplate(string alias)
        {
            switch (alias)
            {
                case "UmbracoHome":
                    return new Template
                    {
                        Alias = alias,
                        Id = new Guid(Template1Id),
                        Name = alias
                    };
                case "BlogPost":
                    return new Template
                    {
                        Alias = alias,
                        Id = new Guid(Template2Id),
                        Name = alias
                    };
                case "BlogList":
                    return new Template
                    {
                        Alias = alias,
                        Id = new Guid(Template3Id),
                        Name = alias
                    };
                default:
                    throw new IndexOutOfRangeException();
            }

            
        }

        public ITemplate GetTemplate(Guid id)
        {
            switch (id.ToString().ToUpperInvariant())
            {
                case Template1Id:
                    return new Template
                    {
                        Alias = "UmbracoHome",
                        Id = new Guid(Template1Id),
                        Name = "Home"
                    };
                case Template2Id:
                    return new Template
                    {
                        Alias = "BlogPost",
                        Id = new Guid(Template2Id),
                        Name = "Blog Post"
                    };
                case Template3Id:
                    return new Template
                    {
                        Alias = "BlogList",
                        Id = new Guid(Template3Id),
                        Name = "Blog List"
                    };
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}