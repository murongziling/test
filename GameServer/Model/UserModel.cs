using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Maticsoft.DBUtility;//Please add references
namespace GameServer.Model
{
	/// <summary>
	/// 类user。
	/// </summary>
	[Serializable]
	public partial class UserModel
	{
		public UserModel()
		{}
		#region Model
		private int _id=-1;
		private string _name;
		private int _been;
		private int _wincount=0;
		private int _losecount=0;
		private int _runcount=0;
		private int _winbeen;
		private int _lv=1;
		private int _exp;
		private int _accountid;
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
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Been
		{
			set{ _been=value;}
			get{return _been;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int WinCount
		{
			set{ _wincount=value;}
			get{return _wincount;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int LoseCount
		{
			set{ _losecount=value;}
			get{return _losecount;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int RunCount
		{
			set{ _runcount=value;}
			get{return _runcount;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int winBeen
		{
			set{ _winbeen=value;}
			get{return _winbeen;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Lv
		{
			set{ _lv=value;}
			get{return _lv;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Exp
		{
			set{ _exp=value;}
			get{return _exp;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int AccountId
		{
			set{ _accountid=value;}
			get{return _accountid;}
		}
		#endregion Model


		#region  Method

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public UserModel(int accountId)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select Id,Name,Been,WinCount,LoseCount,RunCount,winBeen,Lv,Exp,AccountId ");
			strSql.Append(" FROM user ");
			strSql.Append(" where AccountId=@accountId ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@accountId", MySqlDbType.Int32)};
			parameters[0].Value = accountId;

			DataSet ds=DbHelperMySQL.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				if(ds.Tables[0].Rows[0]["Id"]!=null && ds.Tables[0].Rows[0]["Id"].ToString()!="")
				{
					this.Id=int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Name"]!=null)
				{
					this.Name=ds.Tables[0].Rows[0]["Name"].ToString();
				}
				if(ds.Tables[0].Rows[0]["Been"]!=null && ds.Tables[0].Rows[0]["Been"].ToString()!="")
				{
					this.Been=int.Parse(ds.Tables[0].Rows[0]["Been"].ToString());
				}
				if(ds.Tables[0].Rows[0]["WinCount"]!=null && ds.Tables[0].Rows[0]["WinCount"].ToString()!="")
				{
					this.WinCount=int.Parse(ds.Tables[0].Rows[0]["WinCount"].ToString());
				}
				if(ds.Tables[0].Rows[0]["LoseCount"]!=null && ds.Tables[0].Rows[0]["LoseCount"].ToString()!="")
				{
					this.LoseCount=int.Parse(ds.Tables[0].Rows[0]["LoseCount"].ToString());
				}
				if(ds.Tables[0].Rows[0]["RunCount"]!=null && ds.Tables[0].Rows[0]["RunCount"].ToString()!="")
				{
					this.RunCount=int.Parse(ds.Tables[0].Rows[0]["RunCount"].ToString());
				}
				if(ds.Tables[0].Rows[0]["winBeen"]!=null && ds.Tables[0].Rows[0]["winBeen"].ToString()!="")
				{
					this.winBeen=int.Parse(ds.Tables[0].Rows[0]["winBeen"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Lv"]!=null && ds.Tables[0].Rows[0]["Lv"].ToString()!="")
				{
					this.Lv=int.Parse(ds.Tables[0].Rows[0]["Lv"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Exp"]!=null && ds.Tables[0].Rows[0]["Exp"].ToString()!="")
				{
					this.Exp=int.Parse(ds.Tables[0].Rows[0]["Exp"].ToString());
				}
				if(ds.Tables[0].Rows[0]["AccountId"]!=null && ds.Tables[0].Rows[0]["AccountId"].ToString()!="")
				{
					this.AccountId=int.Parse(ds.Tables[0].Rows[0]["AccountId"].ToString());
				}
			}
		}


		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int Id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from user");
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
			strSql.Append("insert into user (");
			strSql.Append("Name,Been,WinCount,LoseCount,RunCount,winBeen,Lv,Exp,AccountId)");
			strSql.Append(" values (");
			strSql.Append("@Name,@Been,@WinCount,@LoseCount,@RunCount,@winBeen,@Lv,@Exp,@AccountId)");
			MySqlParameter[] parameters = {
					new MySqlParameter("@Name", MySqlDbType.VarChar,45),
					new MySqlParameter("@Been", MySqlDbType.Int32,11),
					new MySqlParameter("@WinCount", MySqlDbType.Int32,11),
					new MySqlParameter("@LoseCount", MySqlDbType.Int32,11),
					new MySqlParameter("@RunCount", MySqlDbType.Int32,11),
					new MySqlParameter("@winBeen", MySqlDbType.Int32,11),
					new MySqlParameter("@Lv", MySqlDbType.Int32,11),
					new MySqlParameter("@Exp", MySqlDbType.Int32,11),
					new MySqlParameter("@AccountId", MySqlDbType.Int32,11)};
			parameters[0].Value = Name;
			parameters[1].Value = Been;
			parameters[2].Value = WinCount;
			parameters[3].Value = LoseCount;
			parameters[4].Value = RunCount;
			parameters[5].Value = winBeen;
			parameters[6].Value = Lv;
			parameters[7].Value = Exp;
			parameters[8].Value = AccountId;

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
			strSql.Append("update user set ");
			strSql.Append("Name=@Name,");
			strSql.Append("Been=@Been,");
			strSql.Append("WinCount=@WinCount,");
			strSql.Append("LoseCount=@LoseCount,");
			strSql.Append("RunCount=@RunCount,");
			strSql.Append("winBeen=@winBeen,");
			strSql.Append("Lv=@Lv,");
			strSql.Append("Exp=@Exp,");
			strSql.Append("AccountId=@AccountId");
			strSql.Append(" where Id=@Id ");
			MySqlParameter[] parameters = {
					new MySqlParameter("@Name", MySqlDbType.VarChar,45),
					new MySqlParameter("@Been", MySqlDbType.Int32,11),
					new MySqlParameter("@WinCount", MySqlDbType.Int32,11),
					new MySqlParameter("@LoseCount", MySqlDbType.Int32,11),
					new MySqlParameter("@RunCount", MySqlDbType.Int32,11),
					new MySqlParameter("@winBeen", MySqlDbType.Int32,11),
					new MySqlParameter("@Lv", MySqlDbType.Int32,11),
					new MySqlParameter("@Exp", MySqlDbType.Int32,11),
					new MySqlParameter("@AccountId", MySqlDbType.Int32,11),
					new MySqlParameter("@Id", MySqlDbType.Int32,11)};
			parameters[0].Value = Name;
			parameters[1].Value = Been;
			parameters[2].Value = WinCount;
			parameters[3].Value = LoseCount;
			parameters[4].Value = RunCount;
			parameters[5].Value = winBeen;
			parameters[6].Value = Lv;
			parameters[7].Value = Exp;
			parameters[8].Value = AccountId;
			parameters[9].Value = Id;

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
			strSql.Append("delete from user ");
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
			strSql.Append("select Id,Name,Been,WinCount,LoseCount,RunCount,winBeen,Lv,Exp,AccountId ");
			strSql.Append(" FROM user ");
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
				if(ds.Tables[0].Rows[0]["Name"]!=null )
				{
					this.Name=ds.Tables[0].Rows[0]["Name"].ToString();
				}
				if(ds.Tables[0].Rows[0]["Been"]!=null && ds.Tables[0].Rows[0]["Been"].ToString()!="")
				{
					this.Been=int.Parse(ds.Tables[0].Rows[0]["Been"].ToString());
				}
				if(ds.Tables[0].Rows[0]["WinCount"]!=null && ds.Tables[0].Rows[0]["WinCount"].ToString()!="")
				{
					this.WinCount=int.Parse(ds.Tables[0].Rows[0]["WinCount"].ToString());
				}
				if(ds.Tables[0].Rows[0]["LoseCount"]!=null && ds.Tables[0].Rows[0]["LoseCount"].ToString()!="")
				{
					this.LoseCount=int.Parse(ds.Tables[0].Rows[0]["LoseCount"].ToString());
				}
				if(ds.Tables[0].Rows[0]["RunCount"]!=null && ds.Tables[0].Rows[0]["RunCount"].ToString()!="")
				{
					this.RunCount=int.Parse(ds.Tables[0].Rows[0]["RunCount"].ToString());
				}
				if(ds.Tables[0].Rows[0]["winBeen"]!=null && ds.Tables[0].Rows[0]["winBeen"].ToString()!="")
				{
					this.winBeen=int.Parse(ds.Tables[0].Rows[0]["winBeen"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Lv"]!=null && ds.Tables[0].Rows[0]["Lv"].ToString()!="")
				{
					this.Lv=int.Parse(ds.Tables[0].Rows[0]["Lv"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Exp"]!=null && ds.Tables[0].Rows[0]["Exp"].ToString()!="")
				{
					this.Exp=int.Parse(ds.Tables[0].Rows[0]["Exp"].ToString());
				}
				if(ds.Tables[0].Rows[0]["AccountId"]!=null && ds.Tables[0].Rows[0]["AccountId"].ToString()!="")
				{
					this.AccountId=int.Parse(ds.Tables[0].Rows[0]["AccountId"].ToString());
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
			strSql.Append(" FROM user ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperMySQL.Query(strSql.ToString());
		}

		#endregion  Method
	}
}

