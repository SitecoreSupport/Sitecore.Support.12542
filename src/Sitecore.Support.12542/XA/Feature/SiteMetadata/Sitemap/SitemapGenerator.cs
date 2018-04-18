namespace Sitecore.Support.XA.Feature.SiteMetadata.Sitemap
{
  using Sitecore.Data.Items;
  using System.Collections.Generic;
  using System.Xml;

  public class SitemapGenerator : Sitecore.XA.Feature.SiteMetadata.Sitemap.SitemapGenerator
  {
    public SitemapGenerator() : base() { }
    public SitemapGenerator(XmlWriterSettings xmlWriterSettings) : base(xmlWriterSettings) { }

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