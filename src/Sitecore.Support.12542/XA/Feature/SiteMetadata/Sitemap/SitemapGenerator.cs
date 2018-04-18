namespace Sitecore.Support.XA.Feature.SiteMetadata.Sitemap
{
  using Sitecore.Data.Items;
  using Sitecore.XA.Feature.SiteMetadata.Sitemap;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Text;
  using System.Threading.Tasks;
  using System.Xml;

  public class SitemapGenerator : Sitecore.XA.Feature.SiteMetadata.Sitemap.SitemapGenerator
  {
    public SitemapGenerator() : base() { }
    public SitemapGenerator(XmlWriterSettings xmlWriterSettings) : base(xmlWriterSettings) { }

    public override string GenerateSitemap(Item homeItem, NameValueCollection externalSitemaps, SitemapLinkOptions sitemapLinkOptions)
    {
      List<string> externalXmls = null;
      Task task = null;
      if (externalSitemaps != null && externalSitemaps.Count > 0)
      {
        task = new Task(delegate
        {
          DownloadExternalSitemaps(externalSitemaps, out externalXmls);
        });
        task.Start();
      }
      HashSet<Item> childrenTree = ChildrenSearch(homeItem);
      StringBuilder stringBuilder = BuildSitemap(childrenTree, sitemapLinkOptions);
      task?.Wait();
      if (externalXmls != null && externalXmls.Count > 0)
      {
        string value = JoinXmls(stringBuilder.ToString(), externalXmls);
        stringBuilder.Clear();
        stringBuilder.Append(value);
      }
      return FixEncoding(stringBuilder);
    }

    protected override HashSet<Item> ChildrenSearch(Item homeItem)
    {
      HashSet<Item> hashSet = new HashSet<Item>();
      Queue<Item> queue = new Queue<Item>();
      if (homeItem.HasChildren)
      {
        queue.Enqueue(homeItem);
        hashSet.Add(homeItem);
        while (queue.Count != 0)
        {
          foreach (Item child in queue.Dequeue().Children)
          {
            if (!hashSet.Contains(child))
            {
              if (!ShouldBeSkipped(child))
              {
                hashSet.Add(child);
              }
              if (child.HasChildren)
              {
                queue.Enqueue(child);
              }
            }
          }
        }
      }
      return hashSet;
    }
    
  }
}