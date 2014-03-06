using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///AddressBook 的摘要说明
/// </summary>
public class Model
{
    public class ContactBook 
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nick { get; set; }
        public string uin { get; set; }
        public string qqNumber { get; set; }
        public string phoneNumber { get; set; }
        public string city { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
    }

    public class JsonResult
    {
        public int retcode { get; set; }
        public Object result { get; set; }

    }
}