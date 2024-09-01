using MasterDetailsAuthentication.DAL;
using MasterDetailsAuthentication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MasterDetailsAuthentication.Controllers
{
    [Authorize]
    public class OrderController : ApiController
    {
        private MyDbContext _db = new MyDbContext();

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetOrder()
        {
            var data = _db.OrderMasters.ToList();
            var jsonset = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            var serial = JsonConvert.SerializeObject(data, Formatting.None, jsonset);
            var jsonObj = JsonConvert.DeserializeObject(serial);
            return Ok(jsonObj);
        }
        [Authorize]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetOrder(int id)
        {
            OrderMaster order = _db.OrderMasters.FirstOrDefault(o => o.OrderId == id);
            var jsonset = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            var serial = JsonConvert.SerializeObject(order, Formatting.None, jsonset);
            var jsonObj = JsonConvert.DeserializeObject(serial);
            return Ok(jsonObj);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult PostOrder()
        {

            if (ModelState.IsValid)
            {
                var customerName = HttpContext.Current.Request.Form["CustomerName"];
                var order = new OrderMaster()
                {
                    CustomerName = customerName,
                };
                var imageFile = HttpContext.Current.Request.Files["ImageFile"];
                if (imageFile != null)
                {
                    var filename = Guid.NewGuid().ToString() + Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Images"), filename);
                    order.ImagePath = path;
                    imageFile.SaveAs(path);

                    string orderDetailJson = HttpContext.Current.Request.Form["OrderDetail"];
                    if (!string.IsNullOrEmpty(orderDetailJson))
                    {
                        List<OrderDetail> orderDetailList = JsonConvert.DeserializeObject<List<OrderDetail>>(orderDetailJson);
                        order.OrderDetail.AddRange(orderDetailList);
                    }
                }
                _db.OrderMasters.Add(order);
                _db.SaveChanges();
            }
            return Ok();

        }

        [System.Web.Http.HttpPut]
        public IHttpActionResult PutOrder(int id)
        {
            var order = _db.OrderMasters.Include(o => o.OrderDetail).FirstOrDefault(o => o.OrderId == id);
            order.CustomerName = HttpContext.Current.Request.Form["CustomerName"];
            var ImageFile = HttpContext.Current.Request.Files["ImageFile"];
            if (ImageFile != null)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetFileName(ImageFile.FileName);
                var imagepath = Path.Combine(HttpContext.Current.Server.MapPath("~/Images"), filename);
                order.ImagePath = imagepath;
                ImageFile.SaveAs(imagepath);
            }

            string orderDetailJson = HttpContext.Current.Request.Form["OrderDetail"];
            if (!string.IsNullOrEmpty(orderDetailJson))
            {
                List<OrderDetail> orderDetailList = JsonConvert.DeserializeObject<List<OrderDetail>>(orderDetailJson);
                order.OrderDetail.Clear();
                order.OrderDetail.AddRange(orderDetailList);
            }
            _db.Entry(order).State = EntityState.Modified;
            _db.SaveChanges();
            return Ok();
        }

        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteOrder(int id)
        {
            var order = _db.OrderMasters.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            _db.OrderMasters.Remove(order);
            _db.SaveChanges();
            return Ok();
        }
    }
}
