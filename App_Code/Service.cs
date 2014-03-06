using System.Collections.Generic;
using System.Web.Services;
using yiwoSDK;//using yiwoSDK
using System.Web.Script.Serialization;
using System;//添加它为了方便序列化

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class Service : System.Web.Services.WebService
{
    bool IsNeedCode = false;
    string CurCodeQQ = "";//当前检测过是否有验证码的QQ
    public  CWebQQ cw ;


    public Service () {
        CFun.I_LOVE_BBS("yiwowang.com");

        

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public string HelloWorld() {
        return "Hello World";
    }

    [WebMethod(EnableSession = true)]
    public string isLogin()
    {
        getSession();
        return cw.GetMyQQ() == "" ? "0" : cw.GetMyQQ();
    }

    [WebMethod(Description = "返回验证码",EnableSession = true)]
    public string VCode(string LoginQQ)
    {
        getSession();

        string tmp="";
        if (!CFun.IsQQnumber(ref LoginQQ)) return "不是qq帐号";
        if (LoginQQ != CurCodeQQ || !IsNeedCode){
            tmp = cw.GetLoginVC(LoginQQ);//根据QQ号获取默认验证码

           
         }
        return tmp;
    }


    [WebMethod(Description = "验证码图片", EnableSession = true)]
    public void getVCode2(string LoginQQ)
    {
        getSession();
        System.IO.MemoryStream bstream = new System.IO.MemoryStream();
        cw.GetLoginVCImage(LoginQQ).Save(bstream, System.Drawing.Imaging.ImageFormat.Jpeg);

        byte[] byteReturn = bstream.ToArray();
        bstream.Close();
        this.Context.Response.OutputStream.Write(byteReturn, 0, byteReturn.Length);
    }


    [WebMethod(Description = "登录", EnableSession = true)]
    public Model.JsonResult login(string QQnumber, string password, string verifyCode, string status)
    {
        getSession();
        //登陆QQ
        /**
         * 
         * -1：输入账号位数或密码位数错误。0：登陆成功1:账号或密码错误2:验证码错误3：未知的失败
         * 
         * */
        int ret = cw.Login(QQnumber, password, verifyCode, status,false);
        string result = "";
        switch (ret)
        {
            case -1: result = "输入账号位数或密码位数错误"; break;
            case 0: result = "登陆成功"; break;
            case 1: result = "账号或密码错误"; break;
            case 2: result = "验证码错误"; break;
            default: result = "未知错误"; break;

        }

        return new Model.JsonResult()
        {
            retcode = ret,
            result = result
        };
            
    }


    [WebMethod(EnableSession = true)]
    public string GetFriedsList()
    {
        getSession();
        return  cw.GetFriendList();
    }


    [WebMethod(EnableSession = true)]
    public string GetMemberListByGroup(string code)
    {
        getSession();
        return cw.GetGroupMemberList(code);
    }

    [WebMethod(EnableSession = true)]
    public string GetGroupList()
    {
        getSession();
        return cw.GetGroupList();
    }


    [WebMethod(Description = "讨论组列表", EnableSession = true)]
    public string GetDiscuList()
    {
        getSession();
        return cw.GetDiscuList();
    }



    [WebMethod(Description = "给好友发消息", EnableSession = true)]
    public void sendMsgToFriend(string uin,string content)
    {
        //SendMsgToFriend(string uin, string Content, string FullFilePath="",string FontName = "宋体", string FontSize = "10", string FontColor = "000000", string BIU_SetValue = "0,0,0")
        getSession();
        cw.SendMsgToFriend(uin,content);
        
    }

    [WebMethod(Description = "给多个好友(,隔开)群发消息", EnableSession = true)]
    public Model.JsonResult sendMsgToFriends(string uinStr, string content)
    {
        //SendMsgToFriend(string uin, string Content, string FullFilePath="",string FontName = "宋体", string FontSize = "10", string FontColor = "000000", string BIU_SetValue = "0,0,0")
        getSession();
        string [] qqs=uinStr.Split(',');
        for (int i = 0; i < qqs.Length; i++)
        {
            cw.SendMsgToFriend(qqs[i], content);
        }


        return new Model.JsonResult()
        {
            retcode = 0,
            result = "向所选择好友发送成功！"
        };

    }


    [WebMethod(Description = "给多个群(,隔开)群发消息", EnableSession = true)]
    public Model.JsonResult sendMsgToGroups(string uinStr, string content)
    {
        //SendMsgToFriend(string uin, string Content, string FullFilePath="",string FontName = "宋体", string FontSize = "10", string FontColor = "000000", string BIU_SetValue = "0,0,0")
        getSession();
        string[] qqs = uinStr.Split(',');
        for (int i = 0; i < qqs.Length; i++)
        {
            sendMsgToAllMemberByGroup(qqs[i], content);
        }

        return new Model.JsonResult()
        {
            retcode = 0,
            result = "向群里所有成员发送成功！"
        };

    }



    [WebMethod(Description = "给群里所有好友群发消息", EnableSession = true)]
    public Model.JsonResult sendMsgToAllMemberByGroup(string qqGroupNumber, string content)
    {
        //SendMsgToFriend(string uin, string Content, string FullFilePath="",string FontName = "宋体", string FontSize = "10", string FontColor = "000000", string BIU_SetValue = "0,0,0")
        //public string GetGroupNumber(string code)

        getSession();
        string code = qqGroupNumber;
         //string groupListStr=GetGroupList();
         //string code = "";
         CData.json myjson = new CData.json();
         //myjson.SetData(groupListStr);
         //while (!myjson.isEnd())
         //{

         //    code= qqGroupNumber==cw.GetGroupNumber(myjson.GetValue("code"))?myjson.GetValue("code"):"";
         //    if (code != "")
         //    {
         //        break;
         //    }
         //    code = qqGroupNumber == myjson.GetValue("name") ? myjson.GetValue("code") : "";
         //    if (code != "")
         //    {
         //        break;
         //    }

         //    myjson.MoveNext();
         //}

         //if (code == "")
         //{
         //    return  new Model.JsonResult(){
         //        retcode=0,
         //        result="你已经掉线或者群号不对！"};
         //}

        string jsonstr = GetMemberListByGroup(code);

        myjson.SetData(jsonstr);
        if (myjson.GetValue("retcode") == "100101")
        {
            return new Model.JsonResult()
            {
                retcode = 0,
                result = "你已经掉线或者群号不对！"
            };
        }


        while (!myjson.isEnd())
        {
            cw.SendMsgToFriend(myjson.GetValue("minfo.uin"), content);
            myjson.MoveNext();
        }

        return  new Model.JsonResult(){
                 retcode=0,
                 result="发送成功！"};


    }

    /**
     * 
     * gid和code参数是什么？在获取群列表的时候（GetGroupList()）返回数据中，每个群数据中含有这两个数值，提取出来就可以了。
     * */
    [WebMethod(Description = "给群发消息", EnableSession = true)]
    public void sendMsgToGroup(string gid,string code,string content)
    {
        //SendMsgToFriend(string uin, string Content, string FullFilePath="",string FontName = "宋体", string FontSize = "10", string FontColor = "000000", string BIU_SetValue = "0,0,0")
        getSession();
        cw.SendMsgToGroup(gid, code,content);
    }

    [WebMethod(EnableSession = true)]
    public string GetFriendInfo(string uin)
    {
        getSession();
        return cw.GetPersionCard(uin);
    }

    [WebMethod(Description = "返回群通讯录json", EnableSession = true)]
    public string GetAddressBookByGroup(string code)
    {
        getSession();
       List<Model.ContactBook> contactList = new List<Model.ContactBook>(); 
       string strJson = GetMemberListByGroup(code);
       CData.json myjson = new CData.json();
       myjson.SetData(strJson);
       int id = 1;

       CData.json friendInfoJson = new CData.json();
       while (!myjson.isEnd())
       {
           friendInfoJson.SetData(GetFriendInfo(myjson.GetValue("info.uin")));
           /**
            * {"retcode":0,"result":{"face":0,"birthday":{"month":1,"year":1987,"day":1},"occupation":"","phone":"","allow":4,"college":"","uin":4261612539,"constel":12,"blood":0,"homepage":"","stat":10,"vip_info":1,"country":"中国","city":"长沙","personal":"","nick":"Felix","shengxiao":3,"email":"","client_type":1,"province":"湖南","gender":"male","mobile":"156********"}}
            **/
           Model.ContactBook contact = new Model.ContactBook()
           {
               id=id,
               name="",
               qqNumber="",
               phoneNumber=""
           };
           id++;
           contactList.Add(contact);
           myjson.MoveNext();
       }
       return ToJSON(contactList);
    }



    [WebMethod(Description = "返回整理好的好友信息json", EnableSession = true)]
    public string GetFriendsInfoList()//filter=1 排除没有电话号码的好友
    {
        getSession();
        List<Model.ContactBook> contactList = new List<Model.ContactBook>();
        string strJson = GetFriedsList();//没有处理的数据
        //string strJson = "{\"retcode\":0,\"result\":{\"friends\":[{\"flag\":0,\"uin\":3619816485,\"categories\":0},{\"flag\":0,\"uin\":839237115,\"categories\":0},{\"flag\":0,\"uin\":3368009916,\"categories\":0},{\"flag\":0,\"uin\":194439327,\"categories\":0},{\"flag\":0,\"uin\":3302200792,\"categories\":0},{\"flag\":0,\"uin\":3439779375,\"categories\":0},{\"flag\":0,\"uin\":2258949924,\"categories\":0},{\"flag\":0,\"uin\":3150724686,\"categories\":0},{\"flag\":0,\"uin\":1094860161,\"categories\":0},{\"flag\":0,\"uin\":3386579765,\"categories\":0},{\"flag\":0,\"uin\":2994289004,\"categories\":0},{\"flag\":0,\"uin\":3653433459,\"categories\":0},{\"flag\":0,\"uin\":2887700961,\"categories\":0},{\"flag\":0,\"uin\":2610542872,\"categories\":0},{\"flag\":0,\"uin\":2290583942,\"categories\":0},{\"flag\":0,\"uin\":368006300,\"categories\":0},{\"flag\":0,\"uin\":1713997691,\"categories\":0},{\"flag\":128,\"uin\":3505361440,\"categories\":0},{\"flag\":0,\"uin\":3099816534,\"categories\":0},{\"flag\":0,\"uin\":3173920123,\"categories\":0},{\"flag\":0,\"uin\":1914109736,\"categories\":0},{\"flag\":0,\"uin\":2121559861,\"categories\":0},{\"flag\":0,\"uin\":3929281960,\"categories\":0},{\"flag\":0,\"uin\":3786432353,\"categories\":0},{\"flag\":0,\"uin\":1069349468,\"categories\":0},{\"flag\":0,\"uin\":3507577234,\"categories\":0},{\"flag\":0,\"uin\":142345338,\"categories\":0},{\"flag\":0,\"uin\":3621243390,\"categories\":0},{\"flag\":0,\"uin\":1147379724,\"categories\":0},{\"flag\":0,\"uin\":2816455943,\"categories\":0},{\"flag\":0,\"uin\":4225765766,\"categories\":0},{\"flag\":0,\"uin\":1739807784,\"categories\":0},{\"flag\":0,\"uin\":3303464632,\"categories\":0},{\"flag\":0,\"uin\":432341271,\"categories\":0},{\"flag\":0,\"uin\":772153653,\"categories\":0},{\"flag\":0,\"uin\":1540612013,\"categories\":0},{\"flag\":0,\"uin\":153021223,\"categories\":0},{\"flag\":0,\"uin\":3461682966,\"categories\":0},{\"flag\":0,\"uin\":829782734,\"categories\":0},{\"flag\":0,\"uin\":4018657061,\"categories\":0},{\"flag\":0,\"uin\":2003892327,\"categories\":0},{\"flag\":0,\"uin\":3340235915,\"categories\":0},{\"flag\":0,\"uin\":3121898931,\"categories\":0},{\"flag\":0,\"uin\":1295827351,\"categories\":0},{\"flag\":0,\"uin\":3878201819,\"categories\":0},{\"flag\":0,\"uin\":4202687605,\"categories\":0},{\"flag\":0,\"uin\":772110702,\"categories\":0},{\"flag\":0,\"uin\":1679896435,\"categories\":0},{\"flag\":0,\"uin\":2830326194,\"categories\":0},{\"flag\":0,\"uin\":4249092032,\"categories\":0},{\"flag\":0,\"uin\":471305557,\"categories\":0},{\"flag\":0,\"uin\":232585071,\"categories\":0},{\"flag\":0,\"uin\":2896419008,\"categories\":0},{\"flag\":0,\"uin\":3813285728,\"categories\":0},{\"flag\":0,\"uin\":439555365,\"categories\":0},{\"flag\":0,\"uin\":1912973514,\"categories\":0},{\"flag\":0,\"uin\":1547625012,\"categories\":0},{\"flag\":0,\"uin\":3994829056,\"categories\":0},{\"flag\":0,\"uin\":2493138128,\"categories\":0},{\"flag\":0,\"uin\":178120546,\"categories\":0},{\"flag\":0,\"uin\":264139557,\"categories\":0},{\"flag\":0,\"uin\":4067550572,\"categories\":0},{\"flag\":0,\"uin\":2247237876,\"categories\":0},{\"flag\":0,\"uin\":364078635,\"categories\":0},{\"flag\":0,\"uin\":1094752867,\"categories\":0},{\"flag\":0,\"uin\":959516840,\"categories\":0}],\"marknames\":[{\"uin\":839237115,\"markname\":\"张上老师\",\"type\":0},{\"uin\":3368009916,\"markname\":\"10-罗远飏\",\"type\":0},{\"uin\":3302200792,\"markname\":\"JAV.舒德伟\",\"type\":0},{\"uin\":3439779375,\"markname\":\"C.梁善学\",\"type\":0},{\"uin\":2258949924,\"markname\":\"10-李书鹏\",\"type\":0},{\"uin\":3150724686,\"markname\":\"10-刘天文\",\"type\":0},{\"uin\":1094860161,\"markname\":\"08-蔡大伟\",\"type\":0},{\"uin\":3386579765,\"markname\":\"10-王千\",\"type\":0},{\"uin\":2994289004,\"markname\":\"10计算机覃双盼\",\"type\":0},{\"uin\":3653433459,\"markname\":\"09-姚威\",\"type\":0},{\"uin\":2887700961,\"markname\":\"10-张培\",\"type\":0},{\"uin\":2610542872,\"markname\":\"07-帅强强\",\"type\":0},{\"uin\":2290583942,\"markname\":\"07计算机吴平平\",\"type\":0},{\"uin\":368006300,\"markname\":\"07-高秉文\",\"type\":0},{\"uin\":1713997691,\"markname\":\"C.郭俊杰\",\"type\":0},{\"uin\":3099816534,\"markname\":\"08-陈娟\",\"type\":0},{\"uin\":3173920123,\"markname\":\"10-刘文\",\"type\":0},{\"uin\":1914109736,\"markname\":\"07-肖惠中\",\"type\":0},{\"uin\":2121559861,\"markname\":\"夏权\",\"type\":0},{\"uin\":3929281960,\"markname\":\"11朱伟杰\",\"type\":0},{\"uin\":3786432353,\"markname\":\"08-艾虎\",\"type\":0},{\"uin\":1069349468,\"markname\":\"10-张高伟\",\"type\":0},{\"uin\":3507577234,\"markname\":\"07-管春燕\",\"type\":0},{\"uin\":142345338,\"markname\":\"09-杨益浩\",\"type\":0},{\"uin\":3621243390,\"markname\":\"09-张国庆\",\"type\":0},{\"uin\":1147379724,\"markname\":\"JAV.徐钊\",\"type\":0},{\"uin\":4225765766,\"markname\":\"10-邱旭\",\"type\":0},{\"uin\":3303464632,\"markname\":\"徐义春老师\",\"type\":0},{\"uin\":432341271,\"markname\":\"C谭宇源\",\"type\":0},{\"uin\":1540612013,\"markname\":\"张洪\",\"type\":0},{\"uin\":153021223,\"markname\":\"胡刚老师\",\"type\":0},{\"uin\":3461682966,\"markname\":\"06-刘兵\",\"type\":0},{\"uin\":829782734,\"markname\":\"08计算机许文乔\",\"type\":0},{\"uin\":4018657061,\"markname\":\"07-周程\",\"type\":0},{\"uin\":2003892327,\"markname\":\"10级通信陈晓思\",\"type\":0},{\"uin\":3340235915,\"markname\":\"1121高骞\",\"type\":0},{\"uin\":3121898931,\"markname\":\"07-陈兰花\",\"type\":0},{\"uin\":1295827351,\"markname\":\"章冲\",\"type\":0},{\"uin\":3878201819,\"markname\":\"07-邱芹军\",\"type\":0},{\"uin\":4202687605,\"markname\":\"06-肖先霞\",\"type\":0},{\"uin\":1679896435,\"markname\":\"07-陈典\",\"type\":0},{\"uin\":2830326194,\"markname\":\"c 覃玉红\",\"type\":0},{\"uin\":4249092032,\"markname\":\"10-李亮\",\"type\":0},{\"uin\":471305557,\"markname\":\"08-朱海波\",\"type\":0},{\"uin\":232585071,\"markname\":\"10通信邓佑志\",\"type\":0},{\"uin\":2896419008,\"markname\":\"07-周刚\",\"type\":0},{\"uin\":3813285728,\"markname\":\"10-陈杰\",\"type\":0},{\"uin\":439555365,\"markname\":\"10-朱诚\",\"type\":0},{\"uin\":1912973514,\"markname\":\"08-陈文凯\",\"type\":0},{\"uin\":1547625012,\"markname\":\"王杰\",\"type\":0},{\"uin\":3994829056,\"markname\":\"C.孙育林\",\"type\":0},{\"uin\":2493138128,\"markname\":\"09-刘飞\",\"type\":0},{\"uin\":4067550572,\"markname\":\"10-杨帆\",\"type\":0},{\"uin\":2247237876,\"markname\":\"王青\",\"type\":0},{\"uin\":364078635,\"markname\":\"居伟杰\",\"type\":0},{\"uin\":1094752867,\"markname\":\"10 汪勇\",\"type\":0}],\"categories\":[{\"index\":1,\"sort\":1,\"name\":\"朋友\"},{\"index\":2,\"sort\":2,\"name\":\"家人\"},{\"index\":3,\"sort\":3,\"name\":\"同学\"}],\"vipinfo\":[{\"vip_level\":3,\"u\":3619816485,\"is_vip\":1},{\"vip_level\":0,\"u\":839237115,\"is_vip\":0},{\"vip_level\":0,\"u\":3368009916,\"is_vip\":0},{\"vip_level\":1,\"u\":194439327,\"is_vip\":1},{\"vip_level\":0,\"u\":3302200792,\"is_vip\":0},{\"vip_level\":6,\"u\":3439779375,\"is_vip\":1},{\"vip_level\":0,\"u\":2258949924,\"is_vip\":0},{\"vip_level\":0,\"u\":3150724686,\"is_vip\":0},{\"vip_level\":0,\"u\":1094860161,\"is_vip\":0},{\"vip_level\":0,\"u\":3386579765,\"is_vip\":0},{\"vip_level\":0,\"u\":2994289004,\"is_vip\":0},{\"vip_level\":0,\"u\":3653433459,\"is_vip\":0},{\"vip_level\":0,\"u\":2887700961,\"is_vip\":0},{\"vip_level\":0,\"u\":2610542872,\"is_vip\":0},{\"vip_level\":0,\"u\":2290583942,\"is_vip\":0},{\"vip_level\":0,\"u\":368006300,\"is_vip\":0},{\"vip_level\":0,\"u\":1713997691,\"is_vip\":0},{\"vip_level\":0,\"u\":3505361440,\"is_vip\":0},{\"vip_level\":1,\"u\":3099816534,\"is_vip\":1},{\"vip_level\":6,\"u\":3173920123,\"is_vip\":1},{\"vip_level\":6,\"u\":1914109736,\"is_vip\":1},{\"vip_level\":0,\"u\":2121559861,\"is_vip\":0},{\"vip_level\":0,\"u\":3929281960,\"is_vip\":0},{\"vip_level\":0,\"u\":3786432353,\"is_vip\":0},{\"vip_level\":0,\"u\":1069349468,\"is_vip\":0},{\"vip_level\":0,\"u\":3507577234,\"is_vip\":0},{\"vip_level\":6,\"u\":142345338,\"is_vip\":1},{\"vip_level\":0,\"u\":3621243390,\"is_vip\":0},{\"vip_level\":0,\"u\":1147379724,\"is_vip\":0},{\"vip_level\":0,\"u\":2816455943,\"is_vip\":0},{\"vip_level\":0,\"u\":4225765766,\"is_vip\":0},{\"vip_level\":0,\"u\":1739807784,\"is_vip\":0},{\"vip_level\":0,\"u\":3303464632,\"is_vip\":0},{\"vip_level\":0,\"u\":432341271,\"is_vip\":0},{\"vip_level\":6,\"u\":772153653,\"is_vip\":1},{\"vip_level\":0,\"u\":1540612013,\"is_vip\":0},{\"vip_level\":0,\"u\":153021223,\"is_vip\":0},{\"vip_level\":0,\"u\":3461682966,\"is_vip\":0},{\"vip_level\":0,\"u\":829782734,\"is_vip\":0},{\"vip_level\":0,\"u\":4018657061,\"is_vip\":0},{\"vip_level\":0,\"u\":2003892327,\"is_vip\":0},{\"vip_level\":0,\"u\":3340235915,\"is_vip\":0},{\"vip_level\":0,\"u\":3121898931,\"is_vip\":0},{\"vip_level\":0,\"u\":1295827351,\"is_vip\":0},{\"vip_level\":0,\"u\":3878201819,\"is_vip\":0},{\"vip_level\":0,\"u\":4202687605,\"is_vip\":0},{\"vip_level\":0,\"u\":772110702,\"is_vip\":0},{\"vip_level\":0,\"u\":1679896435,\"is_vip\":0},{\"vip_level\":0,\"u\":2830326194,\"is_vip\":0},{\"vip_level\":0,\"u\":4249092032,\"is_vip\":0},{\"vip_level\":0,\"u\":471305557,\"is_vip\":0},{\"vip_level\":0,\"u\":232585071,\"is_vip\":0},{\"vip_level\":0,\"u\":2896419008,\"is_vip\":0},{\"vip_level\":0,\"u\":3813285728,\"is_vip\":0},{\"vip_level\":0,\"u\":439555365,\"is_vip\":0},{\"vip_level\":0,\"u\":1912973514,\"is_vip\":0},{\"vip_level\":0,\"u\":1547625012,\"is_vip\":0},{\"vip_level\":0,\"u\":3994829056,\"is_vip\":0},{\"vip_level\":0,\"u\":2493138128,\"is_vip\":0},{\"vip_level\":6,\"u\":178120546,\"is_vip\":1},{\"vip_level\":0,\"u\":264139557,\"is_vip\":0},{\"vip_level\":0,\"u\":4067550572,\"is_vip\":0},{\"vip_level\":0,\"u\":2247237876,\"is_vip\":0},{\"vip_level\":0,\"u\":364078635,\"is_vip\":0},{\"vip_level\":0,\"u\":1094752867,\"is_vip\":0},{\"vip_level\":0,\"u\":959516840,\"is_vip\":0}],\"info\":[{\"face\":114,\"flag\":16777798,\"nick\":\"LuckyQiMa\",\"uin\":3619816485},{\"face\":678,\"flag\":4719106,\"nick\":\"深海\",\"uin\":839237115},{\"face\":225,\"flag\":512,\"nick\":\"天堂鱼\",\"uin\":3368009916},{\"face\":0,\"flag\":289964614,\"nick\":\"Felix\",\"uin\":194439327},{\"face\":639,\"flag\":294126146,\"nick\":\"伟\",\"uin\":3302200792},{\"face\":585,\"flag\":8913478,\"nick\":\"天长地久\",\"uin\":3439779375},{\"face\":558,\"flag\":289931842,\"nick\":\"期待\",\"uin\":2258949924},{\"face\":588,\"flag\":25690624,\"nick\":\"Steven-丶卩\",\"uin\":3150724686},{\"face\":279,\"flag\":4719104,\"nick\":\"嚜鴻箛鶴\",\"uin\":1094860161},{\"face\":0,\"flag\":13107712,\"nick\":\"某❤尛千\",\"uin\":3386579765},{\"face\":603,\"flag\":285737472,\"nick\":\"期/盼\",\"uin\":2994289004},{\"face\":603,\"flag\":294126146,\"nick\":\"Smart_Array!\",\"uin\":3653433459},{\"face\":279,\"flag\":298320386,\"nick\":\"一直奔跑的蜗牛\",\"uin\":2887700961},{\"face\":0,\"flag\":13107712,\"nick\":\"qqshuai\",\"uin\":2610542872},{\"face\":252,\"flag\":294142464,\"nick\":\"吴平平\",\"uin\":2290583942},{\"face\":108,\"flag\":8913408,\"nick\":\"梧桐庭锁清秋\",\"uin\":368006300},{\"face\":522,\"flag\":297796162,\"nick\":\"☆、yummyˇ\",\"uin\":1713997691},{\"face\":525,\"flag\":285212672,\"nick\":\"_,_ , (●.●)(●﹏●) /\",\"uin\":3505361440},{\"face\":33,\"flag\":4719108,\"nick\":\"属于-UNE VIE\",\"uin\":3099816534},{\"face\":603,\"flag\":12583494,\"nick\":\"冰凉咖啡\",\"uin\":3173920123},{\"face\":0,\"flag\":289964646,\"nick\":\"惠中\",\"uin\":1914109736},{\"face\":612,\"flag\":8913408,\"nick\":\"\",\"uin\":2121559861},{\"face\":390,\"flag\":25690722,\"nick\":\"M.Chicagos\",\"uin\":3929281960},{\"face\":336,\"flag\":8913408,\"nick\":\"溺水的鱼儿\",\"uin\":3786432353},{\"face\":48,\"flag\":25690722,\"nick\":\"执笔写空白\",\"uin\":1069349468},{\"face\":342,\"flag\":4719104,\"nick\":\"guan\",\"uin\":3507577234},{\"face\":306,\"flag\":294158950,\"nick\":\"Mr.Demon\",\"uin\":142345338},{\"face\":0,\"flag\":8913506,\"nick\":\"张国庆\",\"uin\":3621243390},{\"face\":645,\"flag\":13107712,\"nick\":\"天涯\",\"uin\":1147379724},{\"face\":306,\"flag\":29885024,\"nick\":\"皙华\",\"uin\":2816455943},{\"face\":108,\"flag\":8389120,\"nick\":\"catch dream\",\"uin\":4225765766},{\"face\":279,\"flag\":12583426,\"nick\":\"飞来飞去的鸟\",\"uin\":1739807784},{\"face\":552,\"flag\":8913408,\"nick\":\"北京吉普车\",\"uin\":3303464632},{\"face\":0,\"flag\":13107712,\"nick\":\"unAlias\",\"uin\":432341271},{\"face\":603,\"flag\":17334790,\"nick\":\"Yinshawn❤Rao\",\"uin\":772153653},{\"face\":609,\"flag\":8913408,\"nick\":\"蓝色梦想\",\"uin\":1540612013},{\"face\":294,\"flag\":8913408,\"nick\":\"老虎油\",\"uin\":153021223},{\"face\":285,\"flag\":557568,\"nick\":\"天空\",\"uin\":3461682966},{\"face\":480,\"flag\":8913408,\"nick\":\"  这个冬季~\",\"uin\":829782734},{\"face\":603,\"flag\":4194816,\"nick\":\"清风\\u0026朗月\",\"uin\":4018657061},{\"face\":633,\"flag\":524864,\"nick\":\"俞慢俞美丽。\",\"uin\":2003892327},{\"face\":546,\"flag\":297796096,\"nick\":\"·\",\"uin\":3340235915},{\"face\":309,\"flag\":524800,\"nick\":\"蒲公英\",\"uin\":3121898931},{\"face\":609,\"flag\":524802,\"nick\":\"向左走 向右走\",\"uin\":1295827351},{\"face\":582,\"flag\":13107714,\"nick\":\"QQJ\",\"uin\":3878201819},{\"face\":0,\"flag\":289407552,\"nick\":\"碧玉之家\",\"uin\":4202687605},{\"face\":0,\"flag\":16777792,\"nick\":\"心海云曦\",\"uin\":772110702},{\"face\":546,\"flag\":285737472,\"nick\":\"九\",\"uin\":1679896435},{\"face\":180,\"flag\":4194880,\"nick\":\"♂^(oo)^\",\"uin\":2830326194},{\"face\":237,\"flag\":8913504,\"nick\":\"大漠\",\"uin\":4249092032},{\"face\":522,\"flag\":8389216,\"nick\":\"busy\",\"uin\":471305557},{\"face\":339,\"flag\":25166402,\"nick\":\"相忘于江湖\",\"uin\":232585071},{\"face\":786,\"flag\":8913408,\"nick\":\"♡流动的阳光\",\"uin\":2896419008},{\"face\":558,\"flag\":8389186,\"nick\":\"杰\",\"uin\":3813285728},{\"face\":0,\"flag\":8389120,\"nick\":\"飘往江南的雨\",\"uin\":439555365},{\"face\":339,\"flag\":25690624,\"nick\":\"含着泪的星星\",\"uin\":1912973514},{\"face\":147,\"flag\":8913408,\"nick\":\"want you\",\"uin\":1547625012},{\"face\":237,\"flag\":8913408,\"nick\":\"猿大人\",\"uin\":3994829056},{\"face\":561,\"flag\":13107712,\"nick\":\"陌生老朋友\",\"uin\":2493138128},{\"face\":0,\"flag\":289931846,\"nick\":\"月魄霜\",\"uin\":178120546},{\"face\":606,\"flag\":297796160,\"nick\":\"星晴之川\",\"uin\":264139557},{\"face\":480,\"flag\":20972130,\"nick\":\"暮憶晨曦\",\"uin\":4067550572},{\"face\":585,\"flag\":8913408,\"nick\":\"印迹\",\"uin\":2247237876},{\"face\":0,\"flag\":8946176,\"nick\":\"回首 漠然\",\"uin\":364078635},{\"face\":567,\"flag\":285737538,\"nick\":\" 蓝色角落\",\"uin\":1094752867},{\"face\":546,\"flag\":25165824,\"nick\":\"。。。\",\"uin\":959516840}]}}";
        CData.json myjson = new CData.json();
        string marknamesStr = myjson.GetValue("info", strJson, false);
        marknamesStr = marknamesStr.Substring(1, marknamesStr.Length - 2);
        myjson.SetData(marknamesStr);
        int id = 1;

        CData.json friendInfoJson = new CData.json();
        while (!myjson.isEnd())
        {
                Model.ContactBook contact = new Model.ContactBook()
                {
                    id = id,
                    nick = myjson.GetValue("nick"),
                    qqNumber = cw.GetQQnumber(myjson.GetValue("uin")),
                    uin = myjson.GetValue("uin"),
                };
                id++;
                contactList.Add(contact);
            myjson.MoveNext();
        }
        return ToJSON(contactList);
    }


    [WebMethod(Description = "返回整理好的好友信息json(参数：filter=1 排除没有电话号码的好友)", EnableSession = true)]
    public string GetFriendsInfo(string filter="0")//filter=1 排除没有电话号码的好友
    {
        getSession();
        List<Model.ContactBook> contactList = new List<Model.ContactBook>();
        string strJson = GetFriedsList();//没有处理的数据
        //string strJson = "{\"retcode\":0,\"result\":{\"friends\":[{\"flag\":0,\"uin\":3619816485,\"categories\":0},{\"flag\":0,\"uin\":839237115,\"categories\":0},{\"flag\":0,\"uin\":3368009916,\"categories\":0},{\"flag\":0,\"uin\":194439327,\"categories\":0},{\"flag\":0,\"uin\":3302200792,\"categories\":0},{\"flag\":0,\"uin\":3439779375,\"categories\":0},{\"flag\":0,\"uin\":2258949924,\"categories\":0},{\"flag\":0,\"uin\":3150724686,\"categories\":0},{\"flag\":0,\"uin\":1094860161,\"categories\":0},{\"flag\":0,\"uin\":3386579765,\"categories\":0},{\"flag\":0,\"uin\":2994289004,\"categories\":0},{\"flag\":0,\"uin\":3653433459,\"categories\":0},{\"flag\":0,\"uin\":2887700961,\"categories\":0},{\"flag\":0,\"uin\":2610542872,\"categories\":0},{\"flag\":0,\"uin\":2290583942,\"categories\":0},{\"flag\":0,\"uin\":368006300,\"categories\":0},{\"flag\":0,\"uin\":1713997691,\"categories\":0},{\"flag\":128,\"uin\":3505361440,\"categories\":0},{\"flag\":0,\"uin\":3099816534,\"categories\":0},{\"flag\":0,\"uin\":3173920123,\"categories\":0},{\"flag\":0,\"uin\":1914109736,\"categories\":0},{\"flag\":0,\"uin\":2121559861,\"categories\":0},{\"flag\":0,\"uin\":3929281960,\"categories\":0},{\"flag\":0,\"uin\":3786432353,\"categories\":0},{\"flag\":0,\"uin\":1069349468,\"categories\":0},{\"flag\":0,\"uin\":3507577234,\"categories\":0},{\"flag\":0,\"uin\":142345338,\"categories\":0},{\"flag\":0,\"uin\":3621243390,\"categories\":0},{\"flag\":0,\"uin\":1147379724,\"categories\":0},{\"flag\":0,\"uin\":2816455943,\"categories\":0},{\"flag\":0,\"uin\":4225765766,\"categories\":0},{\"flag\":0,\"uin\":1739807784,\"categories\":0},{\"flag\":0,\"uin\":3303464632,\"categories\":0},{\"flag\":0,\"uin\":432341271,\"categories\":0},{\"flag\":0,\"uin\":772153653,\"categories\":0},{\"flag\":0,\"uin\":1540612013,\"categories\":0},{\"flag\":0,\"uin\":153021223,\"categories\":0},{\"flag\":0,\"uin\":3461682966,\"categories\":0},{\"flag\":0,\"uin\":829782734,\"categories\":0},{\"flag\":0,\"uin\":4018657061,\"categories\":0},{\"flag\":0,\"uin\":2003892327,\"categories\":0},{\"flag\":0,\"uin\":3340235915,\"categories\":0},{\"flag\":0,\"uin\":3121898931,\"categories\":0},{\"flag\":0,\"uin\":1295827351,\"categories\":0},{\"flag\":0,\"uin\":3878201819,\"categories\":0},{\"flag\":0,\"uin\":4202687605,\"categories\":0},{\"flag\":0,\"uin\":772110702,\"categories\":0},{\"flag\":0,\"uin\":1679896435,\"categories\":0},{\"flag\":0,\"uin\":2830326194,\"categories\":0},{\"flag\":0,\"uin\":4249092032,\"categories\":0},{\"flag\":0,\"uin\":471305557,\"categories\":0},{\"flag\":0,\"uin\":232585071,\"categories\":0},{\"flag\":0,\"uin\":2896419008,\"categories\":0},{\"flag\":0,\"uin\":3813285728,\"categories\":0},{\"flag\":0,\"uin\":439555365,\"categories\":0},{\"flag\":0,\"uin\":1912973514,\"categories\":0},{\"flag\":0,\"uin\":1547625012,\"categories\":0},{\"flag\":0,\"uin\":3994829056,\"categories\":0},{\"flag\":0,\"uin\":2493138128,\"categories\":0},{\"flag\":0,\"uin\":178120546,\"categories\":0},{\"flag\":0,\"uin\":264139557,\"categories\":0},{\"flag\":0,\"uin\":4067550572,\"categories\":0},{\"flag\":0,\"uin\":2247237876,\"categories\":0},{\"flag\":0,\"uin\":364078635,\"categories\":0},{\"flag\":0,\"uin\":1094752867,\"categories\":0},{\"flag\":0,\"uin\":959516840,\"categories\":0}],\"marknames\":[{\"uin\":839237115,\"markname\":\"张上老师\",\"type\":0},{\"uin\":3368009916,\"markname\":\"10-罗远飏\",\"type\":0},{\"uin\":3302200792,\"markname\":\"JAV.舒德伟\",\"type\":0},{\"uin\":3439779375,\"markname\":\"C.梁善学\",\"type\":0},{\"uin\":2258949924,\"markname\":\"10-李书鹏\",\"type\":0},{\"uin\":3150724686,\"markname\":\"10-刘天文\",\"type\":0},{\"uin\":1094860161,\"markname\":\"08-蔡大伟\",\"type\":0},{\"uin\":3386579765,\"markname\":\"10-王千\",\"type\":0},{\"uin\":2994289004,\"markname\":\"10计算机覃双盼\",\"type\":0},{\"uin\":3653433459,\"markname\":\"09-姚威\",\"type\":0},{\"uin\":2887700961,\"markname\":\"10-张培\",\"type\":0},{\"uin\":2610542872,\"markname\":\"07-帅强强\",\"type\":0},{\"uin\":2290583942,\"markname\":\"07计算机吴平平\",\"type\":0},{\"uin\":368006300,\"markname\":\"07-高秉文\",\"type\":0},{\"uin\":1713997691,\"markname\":\"C.郭俊杰\",\"type\":0},{\"uin\":3099816534,\"markname\":\"08-陈娟\",\"type\":0},{\"uin\":3173920123,\"markname\":\"10-刘文\",\"type\":0},{\"uin\":1914109736,\"markname\":\"07-肖惠中\",\"type\":0},{\"uin\":2121559861,\"markname\":\"夏权\",\"type\":0},{\"uin\":3929281960,\"markname\":\"11朱伟杰\",\"type\":0},{\"uin\":3786432353,\"markname\":\"08-艾虎\",\"type\":0},{\"uin\":1069349468,\"markname\":\"10-张高伟\",\"type\":0},{\"uin\":3507577234,\"markname\":\"07-管春燕\",\"type\":0},{\"uin\":142345338,\"markname\":\"09-杨益浩\",\"type\":0},{\"uin\":3621243390,\"markname\":\"09-张国庆\",\"type\":0},{\"uin\":1147379724,\"markname\":\"JAV.徐钊\",\"type\":0},{\"uin\":4225765766,\"markname\":\"10-邱旭\",\"type\":0},{\"uin\":3303464632,\"markname\":\"徐义春老师\",\"type\":0},{\"uin\":432341271,\"markname\":\"C谭宇源\",\"type\":0},{\"uin\":1540612013,\"markname\":\"张洪\",\"type\":0},{\"uin\":153021223,\"markname\":\"胡刚老师\",\"type\":0},{\"uin\":3461682966,\"markname\":\"06-刘兵\",\"type\":0},{\"uin\":829782734,\"markname\":\"08计算机许文乔\",\"type\":0},{\"uin\":4018657061,\"markname\":\"07-周程\",\"type\":0},{\"uin\":2003892327,\"markname\":\"10级通信陈晓思\",\"type\":0},{\"uin\":3340235915,\"markname\":\"1121高骞\",\"type\":0},{\"uin\":3121898931,\"markname\":\"07-陈兰花\",\"type\":0},{\"uin\":1295827351,\"markname\":\"章冲\",\"type\":0},{\"uin\":3878201819,\"markname\":\"07-邱芹军\",\"type\":0},{\"uin\":4202687605,\"markname\":\"06-肖先霞\",\"type\":0},{\"uin\":1679896435,\"markname\":\"07-陈典\",\"type\":0},{\"uin\":2830326194,\"markname\":\"c 覃玉红\",\"type\":0},{\"uin\":4249092032,\"markname\":\"10-李亮\",\"type\":0},{\"uin\":471305557,\"markname\":\"08-朱海波\",\"type\":0},{\"uin\":232585071,\"markname\":\"10通信邓佑志\",\"type\":0},{\"uin\":2896419008,\"markname\":\"07-周刚\",\"type\":0},{\"uin\":3813285728,\"markname\":\"10-陈杰\",\"type\":0},{\"uin\":439555365,\"markname\":\"10-朱诚\",\"type\":0},{\"uin\":1912973514,\"markname\":\"08-陈文凯\",\"type\":0},{\"uin\":1547625012,\"markname\":\"王杰\",\"type\":0},{\"uin\":3994829056,\"markname\":\"C.孙育林\",\"type\":0},{\"uin\":2493138128,\"markname\":\"09-刘飞\",\"type\":0},{\"uin\":4067550572,\"markname\":\"10-杨帆\",\"type\":0},{\"uin\":2247237876,\"markname\":\"王青\",\"type\":0},{\"uin\":364078635,\"markname\":\"居伟杰\",\"type\":0},{\"uin\":1094752867,\"markname\":\"10 汪勇\",\"type\":0}],\"categories\":[{\"index\":1,\"sort\":1,\"name\":\"朋友\"},{\"index\":2,\"sort\":2,\"name\":\"家人\"},{\"index\":3,\"sort\":3,\"name\":\"同学\"}],\"vipinfo\":[{\"vip_level\":3,\"u\":3619816485,\"is_vip\":1},{\"vip_level\":0,\"u\":839237115,\"is_vip\":0},{\"vip_level\":0,\"u\":3368009916,\"is_vip\":0},{\"vip_level\":1,\"u\":194439327,\"is_vip\":1},{\"vip_level\":0,\"u\":3302200792,\"is_vip\":0},{\"vip_level\":6,\"u\":3439779375,\"is_vip\":1},{\"vip_level\":0,\"u\":2258949924,\"is_vip\":0},{\"vip_level\":0,\"u\":3150724686,\"is_vip\":0},{\"vip_level\":0,\"u\":1094860161,\"is_vip\":0},{\"vip_level\":0,\"u\":3386579765,\"is_vip\":0},{\"vip_level\":0,\"u\":2994289004,\"is_vip\":0},{\"vip_level\":0,\"u\":3653433459,\"is_vip\":0},{\"vip_level\":0,\"u\":2887700961,\"is_vip\":0},{\"vip_level\":0,\"u\":2610542872,\"is_vip\":0},{\"vip_level\":0,\"u\":2290583942,\"is_vip\":0},{\"vip_level\":0,\"u\":368006300,\"is_vip\":0},{\"vip_level\":0,\"u\":1713997691,\"is_vip\":0},{\"vip_level\":0,\"u\":3505361440,\"is_vip\":0},{\"vip_level\":1,\"u\":3099816534,\"is_vip\":1},{\"vip_level\":6,\"u\":3173920123,\"is_vip\":1},{\"vip_level\":6,\"u\":1914109736,\"is_vip\":1},{\"vip_level\":0,\"u\":2121559861,\"is_vip\":0},{\"vip_level\":0,\"u\":3929281960,\"is_vip\":0},{\"vip_level\":0,\"u\":3786432353,\"is_vip\":0},{\"vip_level\":0,\"u\":1069349468,\"is_vip\":0},{\"vip_level\":0,\"u\":3507577234,\"is_vip\":0},{\"vip_level\":6,\"u\":142345338,\"is_vip\":1},{\"vip_level\":0,\"u\":3621243390,\"is_vip\":0},{\"vip_level\":0,\"u\":1147379724,\"is_vip\":0},{\"vip_level\":0,\"u\":2816455943,\"is_vip\":0},{\"vip_level\":0,\"u\":4225765766,\"is_vip\":0},{\"vip_level\":0,\"u\":1739807784,\"is_vip\":0},{\"vip_level\":0,\"u\":3303464632,\"is_vip\":0},{\"vip_level\":0,\"u\":432341271,\"is_vip\":0},{\"vip_level\":6,\"u\":772153653,\"is_vip\":1},{\"vip_level\":0,\"u\":1540612013,\"is_vip\":0},{\"vip_level\":0,\"u\":153021223,\"is_vip\":0},{\"vip_level\":0,\"u\":3461682966,\"is_vip\":0},{\"vip_level\":0,\"u\":829782734,\"is_vip\":0},{\"vip_level\":0,\"u\":4018657061,\"is_vip\":0},{\"vip_level\":0,\"u\":2003892327,\"is_vip\":0},{\"vip_level\":0,\"u\":3340235915,\"is_vip\":0},{\"vip_level\":0,\"u\":3121898931,\"is_vip\":0},{\"vip_level\":0,\"u\":1295827351,\"is_vip\":0},{\"vip_level\":0,\"u\":3878201819,\"is_vip\":0},{\"vip_level\":0,\"u\":4202687605,\"is_vip\":0},{\"vip_level\":0,\"u\":772110702,\"is_vip\":0},{\"vip_level\":0,\"u\":1679896435,\"is_vip\":0},{\"vip_level\":0,\"u\":2830326194,\"is_vip\":0},{\"vip_level\":0,\"u\":4249092032,\"is_vip\":0},{\"vip_level\":0,\"u\":471305557,\"is_vip\":0},{\"vip_level\":0,\"u\":232585071,\"is_vip\":0},{\"vip_level\":0,\"u\":2896419008,\"is_vip\":0},{\"vip_level\":0,\"u\":3813285728,\"is_vip\":0},{\"vip_level\":0,\"u\":439555365,\"is_vip\":0},{\"vip_level\":0,\"u\":1912973514,\"is_vip\":0},{\"vip_level\":0,\"u\":1547625012,\"is_vip\":0},{\"vip_level\":0,\"u\":3994829056,\"is_vip\":0},{\"vip_level\":0,\"u\":2493138128,\"is_vip\":0},{\"vip_level\":6,\"u\":178120546,\"is_vip\":1},{\"vip_level\":0,\"u\":264139557,\"is_vip\":0},{\"vip_level\":0,\"u\":4067550572,\"is_vip\":0},{\"vip_level\":0,\"u\":2247237876,\"is_vip\":0},{\"vip_level\":0,\"u\":364078635,\"is_vip\":0},{\"vip_level\":0,\"u\":1094752867,\"is_vip\":0},{\"vip_level\":0,\"u\":959516840,\"is_vip\":0}],\"info\":[{\"face\":114,\"flag\":16777798,\"nick\":\"LuckyQiMa\",\"uin\":3619816485},{\"face\":678,\"flag\":4719106,\"nick\":\"深海\",\"uin\":839237115},{\"face\":225,\"flag\":512,\"nick\":\"天堂鱼\",\"uin\":3368009916},{\"face\":0,\"flag\":289964614,\"nick\":\"Felix\",\"uin\":194439327},{\"face\":639,\"flag\":294126146,\"nick\":\"伟\",\"uin\":3302200792},{\"face\":585,\"flag\":8913478,\"nick\":\"天长地久\",\"uin\":3439779375},{\"face\":558,\"flag\":289931842,\"nick\":\"期待\",\"uin\":2258949924},{\"face\":588,\"flag\":25690624,\"nick\":\"Steven-丶卩\",\"uin\":3150724686},{\"face\":279,\"flag\":4719104,\"nick\":\"嚜鴻箛鶴\",\"uin\":1094860161},{\"face\":0,\"flag\":13107712,\"nick\":\"某❤尛千\",\"uin\":3386579765},{\"face\":603,\"flag\":285737472,\"nick\":\"期/盼\",\"uin\":2994289004},{\"face\":603,\"flag\":294126146,\"nick\":\"Smart_Array!\",\"uin\":3653433459},{\"face\":279,\"flag\":298320386,\"nick\":\"一直奔跑的蜗牛\",\"uin\":2887700961},{\"face\":0,\"flag\":13107712,\"nick\":\"qqshuai\",\"uin\":2610542872},{\"face\":252,\"flag\":294142464,\"nick\":\"吴平平\",\"uin\":2290583942},{\"face\":108,\"flag\":8913408,\"nick\":\"梧桐庭锁清秋\",\"uin\":368006300},{\"face\":522,\"flag\":297796162,\"nick\":\"☆、yummyˇ\",\"uin\":1713997691},{\"face\":525,\"flag\":285212672,\"nick\":\"_,_ , (●.●)(●﹏●) /\",\"uin\":3505361440},{\"face\":33,\"flag\":4719108,\"nick\":\"属于-UNE VIE\",\"uin\":3099816534},{\"face\":603,\"flag\":12583494,\"nick\":\"冰凉咖啡\",\"uin\":3173920123},{\"face\":0,\"flag\":289964646,\"nick\":\"惠中\",\"uin\":1914109736},{\"face\":612,\"flag\":8913408,\"nick\":\"\",\"uin\":2121559861},{\"face\":390,\"flag\":25690722,\"nick\":\"M.Chicagos\",\"uin\":3929281960},{\"face\":336,\"flag\":8913408,\"nick\":\"溺水的鱼儿\",\"uin\":3786432353},{\"face\":48,\"flag\":25690722,\"nick\":\"执笔写空白\",\"uin\":1069349468},{\"face\":342,\"flag\":4719104,\"nick\":\"guan\",\"uin\":3507577234},{\"face\":306,\"flag\":294158950,\"nick\":\"Mr.Demon\",\"uin\":142345338},{\"face\":0,\"flag\":8913506,\"nick\":\"张国庆\",\"uin\":3621243390},{\"face\":645,\"flag\":13107712,\"nick\":\"天涯\",\"uin\":1147379724},{\"face\":306,\"flag\":29885024,\"nick\":\"皙华\",\"uin\":2816455943},{\"face\":108,\"flag\":8389120,\"nick\":\"catch dream\",\"uin\":4225765766},{\"face\":279,\"flag\":12583426,\"nick\":\"飞来飞去的鸟\",\"uin\":1739807784},{\"face\":552,\"flag\":8913408,\"nick\":\"北京吉普车\",\"uin\":3303464632},{\"face\":0,\"flag\":13107712,\"nick\":\"unAlias\",\"uin\":432341271},{\"face\":603,\"flag\":17334790,\"nick\":\"Yinshawn❤Rao\",\"uin\":772153653},{\"face\":609,\"flag\":8913408,\"nick\":\"蓝色梦想\",\"uin\":1540612013},{\"face\":294,\"flag\":8913408,\"nick\":\"老虎油\",\"uin\":153021223},{\"face\":285,\"flag\":557568,\"nick\":\"天空\",\"uin\":3461682966},{\"face\":480,\"flag\":8913408,\"nick\":\"  这个冬季~\",\"uin\":829782734},{\"face\":603,\"flag\":4194816,\"nick\":\"清风\\u0026朗月\",\"uin\":4018657061},{\"face\":633,\"flag\":524864,\"nick\":\"俞慢俞美丽。\",\"uin\":2003892327},{\"face\":546,\"flag\":297796096,\"nick\":\"·\",\"uin\":3340235915},{\"face\":309,\"flag\":524800,\"nick\":\"蒲公英\",\"uin\":3121898931},{\"face\":609,\"flag\":524802,\"nick\":\"向左走 向右走\",\"uin\":1295827351},{\"face\":582,\"flag\":13107714,\"nick\":\"QQJ\",\"uin\":3878201819},{\"face\":0,\"flag\":289407552,\"nick\":\"碧玉之家\",\"uin\":4202687605},{\"face\":0,\"flag\":16777792,\"nick\":\"心海云曦\",\"uin\":772110702},{\"face\":546,\"flag\":285737472,\"nick\":\"九\",\"uin\":1679896435},{\"face\":180,\"flag\":4194880,\"nick\":\"♂^(oo)^\",\"uin\":2830326194},{\"face\":237,\"flag\":8913504,\"nick\":\"大漠\",\"uin\":4249092032},{\"face\":522,\"flag\":8389216,\"nick\":\"busy\",\"uin\":471305557},{\"face\":339,\"flag\":25166402,\"nick\":\"相忘于江湖\",\"uin\":232585071},{\"face\":786,\"flag\":8913408,\"nick\":\"♡流动的阳光\",\"uin\":2896419008},{\"face\":558,\"flag\":8389186,\"nick\":\"杰\",\"uin\":3813285728},{\"face\":0,\"flag\":8389120,\"nick\":\"飘往江南的雨\",\"uin\":439555365},{\"face\":339,\"flag\":25690624,\"nick\":\"含着泪的星星\",\"uin\":1912973514},{\"face\":147,\"flag\":8913408,\"nick\":\"want you\",\"uin\":1547625012},{\"face\":237,\"flag\":8913408,\"nick\":\"猿大人\",\"uin\":3994829056},{\"face\":561,\"flag\":13107712,\"nick\":\"陌生老朋友\",\"uin\":2493138128},{\"face\":0,\"flag\":289931846,\"nick\":\"月魄霜\",\"uin\":178120546},{\"face\":606,\"flag\":297796160,\"nick\":\"星晴之川\",\"uin\":264139557},{\"face\":480,\"flag\":20972130,\"nick\":\"暮憶晨曦\",\"uin\":4067550572},{\"face\":585,\"flag\":8913408,\"nick\":\"印迹\",\"uin\":2247237876},{\"face\":0,\"flag\":8946176,\"nick\":\"回首 漠然\",\"uin\":364078635},{\"face\":567,\"flag\":285737538,\"nick\":\" 蓝色角落\",\"uin\":1094752867},{\"face\":546,\"flag\":25165824,\"nick\":\"。。。\",\"uin\":959516840}]}}";
        CData.json myjson = new CData.json();
        string marknamesStr = myjson.GetValue("info", strJson, false);
        marknamesStr = marknamesStr.Substring(1, marknamesStr.Length - 2);
        myjson.SetData(marknamesStr);
        int id = 1;

        CData.json friendInfoJson = new CData.json();
        while (!myjson.isEnd())
        {
            friendInfoJson.SetData(GetFriendInfo(myjson.GetValue("uin")));
            /**
             * {"retcode":0,"result":{"face":0,"birthday":{"month":1,"year":1987,"day":1},"occupation":"","phone":"","allow":4,"college":"","uin":4261612539,"constel":12,"blood":0,"homepage":"","stat":10,"vip_info":1,"country":"中国","city":"长沙","personal":"","nick":"Felix","shengxiao":3,"email":"","client_type":1,"province":"湖南","gender":"male","mobile":"156********"}}
             **/
            if (friendInfoJson.GetValue("result.phone") != ""&&filter=="1"||filter=="0")
            {
                Model.ContactBook contact = new Model.ContactBook()
                {
                    id = id,
                    name = friendInfoJson.GetValue("result.markname"),
                    nick = myjson.GetValue("nick"),
                    qqNumber = cw.GetQQnumber(myjson.GetValue("uin")),
                    uin = myjson.GetValue("uin"),
                    phoneNumber = friendInfoJson.GetValue("result.phone"),
                    city = friendInfoJson.GetValue("result.city"),
                    birthday = friendInfoJson.GetValue("result.birthday"),
                    email = friendInfoJson.GetValue("result.email"),
                };
                id++;
                contactList.Add(contact);
            }
            myjson.MoveNext();
        }
        return ToJSON(contactList);
    }





    //对数据序列化，返回JSON格式 
    public string ToJSON(object obj)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return serializer.Serialize(obj);
    }

    public byte[] getVCode(string LoginQQ)
    {
        //加载验证码图片
        // 5. 输出字节流
        string yzmUrl = "https://ssl.captcha.qq.com/getimage?aid=501004106&r=" + new Random().Next() + "&uin=" + LoginQQ;
        System.IO.Stream stream = QQHelper.HttpHelper.GetResponseImage(yzmUrl, "", "", "");
        System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
        System.IO.MemoryStream bstream = new System.IO.MemoryStream();
        img.Save(bstream, System.Drawing.Imaging.ImageFormat.Jpeg);


        byte[] byteReturn = bstream.ToArray();
        bstream.Close();
        return byteReturn;
    }

     [WebMethod(EnableSession = true)]
    public void getSession(){
         if (Context.Session["cw"] == null) Context.Session["cw"] = new CWebQQ();
         cw = (CWebQQ)Context.Session["cw"];
    }
    
}