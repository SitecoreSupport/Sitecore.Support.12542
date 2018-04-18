using Microsoft.Extensions.DependencyInjection;
using Sitecore.XA.Feature.SiteMetadata.Sitemap;
using Sitecore.XA.Foundation.IOC.Pipelines.IOC;

namespace Support
{
  public class RegisterDependencies : IocProcessor
  {
    public override void Process(IocArgs args)
    {
      args.ServiceCollection.AddTransient<ISitemapGenerator, Sitecore.Support.XA.Feature.SiteMetadata.Sitemap.SitemapGenerator>();
    }
  }
}
