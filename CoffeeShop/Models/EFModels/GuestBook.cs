namespace CoffeeShop.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GuestBook
    {
    public int Id { get; set; }

        [Required(ErrorMessage = "姓名是必填的。")]
        [StringLength(10, ErrorMessage = "姓名不能超過10個字符。")]
        [RegularExpression(@"^[\u4e00-\u9fa5a-zA-Z]+$", ErrorMessage = "姓名只能包含中文和英文字符。")]
        public string Name { get; set; }

    [Required(ErrorMessage = "電子郵件是必填的。")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址。")]
    public string Email { get; set; }

        // 使用正則表達式來驗證台灣手機和市話號碼
        [RegularExpression(@"^(09\d{8}|0[2-9]{1}\d{1,2}[- ]?\d{7})$",
            ErrorMessage = "請輸入有效的台灣手機或市話號碼。")]
        [StringLength(15, ErrorMessage = "電話號碼不能超過15個字符。")]
        public string Phone { get; set; }

        public string Message { get; set; }

        public DateTime Createdtime { get; set; }
    }
}
