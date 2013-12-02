using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LCDDisplay
{
    class rssService
    {
        public string rssUrl;
        public int itemCount;
        public rssService(){}
        public rssService(string url,int count)
        {
            rssUrl = url;
            itemCount = count;
        }
        public ArrayList getRssFeed()
        {
            ArrayList titleList=new ArrayList();
            System.Net.WebRequest myRequest = System.Net.WebRequest.Create(rssUrl);
            System.Net.WebResponse myResponse = myRequest.GetResponse();
            System.IO.Stream rssStream = myResponse.GetResponseStream();
            System.Xml.XmlDocument rssDoc = new System.Xml.XmlDocument();
            rssDoc.Load(rssStream);

            System.Xml.XmlNodeList rssItems = rssDoc.SelectNodes("rss/channel/item");
            for (int i = 0; i < itemCount;i++ )
            {
                System.Xml.XmlNode rssDetail;
                rssDetail=rssItems.Item(i).SelectSingleNode("title");
                if (rssDetail!=null)
                {
                    titleList.Add(rssDetail.InnerText);
                }
            }
            return titleList;
        }
    }
}
