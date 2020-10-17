namespace Protocol.Code
{
    public   class AccountCode
    {
        //注册的操作码
        public const int REGIST_CREQ = 0;  //客户端请求  参数：accountDto
        public const int REGIST_SRES = 1;  //服务器响应

        //登陆的操作码
        public const int LOGIN = 2;   //参数： accountDto

    }
}
