using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }  // 商品的ID

        public string ItemEName { get; set; }  // 商品英文名稱 (新增)
        public string ProductName { get; set; }  // 商品名稱
        public decimal Price { get; set; }  // 商品單價
        public int Quantity { get; set; }  // 購買數量
        public string ImageFileName { get; set; }  // 商品圖片檔案名

        public string Remark { get; set; }  // 備註

        // 計算商品小計
        public decimal TotalPrice
        {
            get
            {
                return Price * Quantity;
            }
            set
            {

            }
        }
    }
}
