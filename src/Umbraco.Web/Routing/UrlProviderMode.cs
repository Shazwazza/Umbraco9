namespace Umbraco.Web.Routing
{
    /// <summary>
    /// Specifies the type of urls that the url provider should produce.
    /// </summary>
    public enum UrlProviderMode
    {
       
        /// <summary>
        /// Indicates that the url provider should produce relative urls exclusively.
        /// </summary>
        Relative,

        /// <summary>
        /// Indicates that the url provider should produce absolute urls exclusively.
        /// </summary>
        Absolute,

        /// <summary>
        /// Indicates that the url provider should determine automatically whether to return relative or absolute urls.
        /// </summary>
        Auto
    }
}