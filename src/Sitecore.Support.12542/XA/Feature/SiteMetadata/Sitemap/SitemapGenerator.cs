namespace Sitecore.Support.XA.Feature.SiteMetadata.Sitemap
{
  using Sitecore.Data.Items;
  using Sitecore.Globalization;
  using Sitecore.XA.Feature.SiteMetadata.Sitemap;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;
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
      #region Modified code
      List<Item> childrenTree = ChildrenSearch(homeItem);
      #endregion
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
    #region Modified code
    protected virtual new List<Item> ChildrenSearch(Item homeItem)
    {
      List<Item> list = new List<Item>();
      Queue<Item> queue = new Queue<Item>();
      if (homeItem.HasChildren)
      {
        queue.Enqueue(homeItem);
        if (homeItem.Versions.Count > 0)
        {
          list.Add(homeItem);
        }
        list.AddRange(GetItemsForOtherLanguages(homeItem));
        while (queue.Count != 0)
        {
          foreach (Item child in queue.Dequeue().Children)
          {
            if (!list.Contains(child))
            {
              if (!ShouldBeSkipped(child))
              {
                if (child.Versions.Count > 0)
                {
                  list.Add(child);
                }
                list.AddRange(GetItemsForOtherLanguages(child));
              }
              if (child.HasChildren)
              {
                queue.Enqueue(child);
              }
            }
          }
        }
      }
      return list;
    }
    #endregion
    #region Added code
    protected virtual IEnumerable<Item> GetItemsForOtherLanguages(Item item)
    {
      foreach (Language item3 in from language in item.Languages
                                 where language != item.Language
                                 select language)
      {
        Item item2 = item.Database.GetItem(item.ID, item3);
        if (item2.Versions.Count > 0)
        {
          yield return item2;
        }
      }
    }
    #endregion
  }
}