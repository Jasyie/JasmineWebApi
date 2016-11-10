using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProductsApp.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Json;
using System.Web.Script.Serialization;
//using System.Web.Mvc;
//using System.Web.Http;
//using System.Web.Mvc.JsonResult;
//using System.Web.Mvc.Controller;
using System.Web.Http.Results;
//using System.Web.Http.Results.JsonResult;




namespace ProductsApp.Controllers
{
    public class ProductController : ApiController
    {
        Products[] products = new Products[]
        {
           new Products {Id=1, Name="mANGO", Category="fruit", Price=20.1m},
           new Products{Id=2, Name="muffin", Category="bakery", Price=18.9m}
        };
        
        public IEnumerable<Products> GetAllProducts()
        {
            return products;
        }
        public IHttpActionResult GetProduct(int id)
        {
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        //public IHttpActionResult xxx(int id)
        //{
        //    var product = products.FirstOrDefault((p) => p.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(product);
        //}

        //[HttpPost]
        //[Route("api/Product/Ajax")]
        //public IHttpActionResult test(Products order)
        //{
        //    return Json("ok");
        //}


        [HttpPost]
        [Route("api/Product/Search")]
        public IHttpActionResult SearchPattern(Products product)
        {
            IList<SearchInformation> SearchInfo = new List<SearchInformation>();
            //url used to search the apple information from google
             var url = "https://www.google.com.au/search?num=100&q=" + product.Name;
         
            //creating the url to send it an http request which is a web request
            var req = (HttpWebRequest)WebRequest.Create(url);

            //storing the http response after request in res varible
            var res = (HttpWebResponse)req.GetResponse();
            //creating a datastream from http response and readingdata stream to end and string it as response from server
            Stream dataStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            //pattern used to match li item with class g and pattern to match url and label
            var pattern = "(?<items><div\\sclass=\\\"g\\\".*<\\/div>)";
            var patternn = "(?<header><h3\\sclass=\\\"r\\\">)(<a\\s.+?\\\">)(?<label>.*?)(<\\/a>)(.*<cite>)(?<url>.*?)(<\\/cite>)";

            //matching the pattern and creating string collection to store their value
            var matchingListitem = Regex.Match(responseFromServer, pattern, RegexOptions.IgnoreCase);
           

            // if the pattern matches and we find the list item then add its value to resultlist
            while (matchingListitem.Success)
            {
                var matchingLabelUrl = Regex.Match(matchingListitem.Value, patternn, RegexOptions.IgnoreCase);
                SearchInformation search = new SearchInformation();
                search.Label = matchingLabelUrl.Groups["label"].Value.Replace("<b>",string.Empty).Replace("</b>",string.Empty);
                search.Url = matchingLabelUrl.Groups["url"].Value.Replace("<b>",string.Empty).Replace("</b>",string.Empty).Trim();
                if(!search.Url.StartsWith("http"))
                {
                    search.Url = @"http://" + search.Url;
                }
                //if(search.Label.StartsWith("<b>"))
                //{
                    //search.Label = search.Label.Replace("<b>",string.Empty);
               // }
                SearchInfo.Add(search);
                matchingListitem = matchingListitem.NextMatch();
            }
            return Ok(SearchInfo);
        }
    }
}
