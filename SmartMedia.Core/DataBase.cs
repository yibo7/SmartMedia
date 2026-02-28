using LiteDB;
using XS.Data2.LiteDBBase;

namespace SmartMedia.Core;



#region Sqlite 通用基类代码

//public class DataBase<T> : DbBase<T> where T : class, new()
//{


//    protected override IDbConnection GetConn => Connections.GetConnectionSqlite(@"Filename=data.db;");
//    //protected override IDbConnection GetConn => Connections.GetConnectionMysql(@"Server=127.0.0.1;Database=atq_service;User ID=root;Password=MySql2015;charset=utf8;");
//    //protected override IDbConnection GetConn => Connections.GetConnectionMysql(@"Server=43.139.156.148;Port=1792;Database=atq_service;User ID=cqs263;Password=369913836@Mysql;charset=utf8;");
//    private static DataBase<T> _instance;
//    private static readonly object syslock = new object();
//    /// <summary>
//    /// 生成此类的单例实例
//    /// </summary>
//    /// <returns></returns>
//    public static DataBase<T> GetInstance()
//    {
//        if (_instance == null)
//        {
//            lock (syslock)
//            {
//                if (_instance == null)
//                {
//                    _instance = new DataBase<T>();
//                }
//            }
//        }
//        return _instance;
//    }
//}

#endregion

#region LiteDb 通用基类代码  
public class LiteDbInt<T> : LiteBaseInt<T> where T : LiteModelBaseInt
{
    protected override LiteDatabase GetDb => new LiteDatabase("Filename=LiteData.db;Connection=shared");

}

#endregion
