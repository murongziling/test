using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Maticsoft.DBUtility;//Please add references
namespace GameServer.Model
{
	/// <summary>
	/// 类account。
	/// </summary>
	[Serializable]
	public partial class AccountModel
	{
		public AccountModel()
		{}
		#region Model
		private int _id=-1;
		private string _account;
		private string _password;
		/// <summary>
		/// auto_increment
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Account
		{
			set{ _account=value;}
			get{return _account;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Password
		{
			set{ _password=value;}
			get{return _password;}
		}
		#endregion Model


		#region  Method

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public AccountModel(string account)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select Id,Account,Password ");
			strSql.Append(" FROM account ");
			strSql.Append(" where Account=@account ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@account", MySqlDbType.VarChar)};
			parameters[0].Value = account;

			DataSet ds=DbHelperMySQL.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				if(ds.Tables[0].Rows[0]["Id"]!=null && ds.Tables[0].Rows[0]["Id"].ToString()!="")
				{
					this.Id=int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Account"]!=null)
				{
					this.Account=ds.Tables[0].Rows[0]["Account"].ToString();
				}
				if(ds.Tables[0].Rows[0]["Password"]!=null)
				{
					this.Password=ds.Tables[0].Rows[0]["Password"].ToString();
				}
			}
		}

		

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int Id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from account");
			strSql.Append(" where Id=@Id ");

			MySqlParameter[] parameters = {
					new MySqlParameter("@Id", MySqlDbType.Int32)};
			parameters[0].Value = Id;

			return DbHelperMySQL.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into account (");
			strSql.Append("Account,Password)");
			strSql.Append(" values (");
			strSql.Append("@Account,@Password)");
			MySqlParameter[] parameters = {
					new MySqlParameter("@Account", MySqlDbType.VarChar,45),
					new MySqlParameter("@Password", MySqlDbType.VarChar,45)};
			parameters[0].Value = Account;
			parameters[1].Value = Password;

			DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
            getKey();

        }

        void getKey()
        {
            DataSet ds = DbHelperMySQL.Query("select @@IDENTITY  as Id");
            if (ds.Tables[0].Rows[0]["Id"] != null && ds.Tables[0].Rows[0]["Id"].ToString() != string.Empty)
            {
                this._id = int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
            }
        }


        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update account set ");
			strSql.Append("Account=@Account,");
			strSql.Append("Password=@Password");
			strSql.Append(" where Id=@Id ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@Account", MySqlDbType.VarChar,45),
					new MySqlParameter("@Password", MySqlDbType.VarChar,45),
					new MySqlParameter("@Id", MySqlDbType.Int32,11)};
			parameters[0].Value = Account;
			parameters[1].Value = Password;
			parameters[2].Value = Id;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int Id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from account ");
			strSql.Append(" where Id=@Id ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@Id", MySqlDbType.Int32)};
			parameters[0].Value = Id;

			int rows=DbHelperMySQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public void GetModel(int Id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select Id,Account,Password ");
			strSql.Append(" FROM account ");
			strSql.Append(" where Id=@Id ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@Id", MySqlDbType.Int32)};
			parameters[0].Value = Id;

			DataSet ds=DbHelperMySQL.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				if(ds.Tables[0].Rows[0]["Id"]!=null && ds.Tables[0].Rows[0]["Id"].ToString()!="")
				{
					this.Id=int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Account"]!=null )
				{
					this.Account=ds.Tables[0].Rows[0]["Account"].ToString();
				}
				if(ds.Tables[0].Rows[0]["Password"]!=null )
				{
					this.Password=ds.Tables[0].Rows[0]["Password"].ToString();
				}
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM account ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperMySQL.Query(strSql.ToString());
		}

		#endregion  Method
	}
}

