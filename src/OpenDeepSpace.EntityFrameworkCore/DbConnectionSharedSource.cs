/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/20 21:20:22	
CLR: 4.0.30319.42000	
Description:


=====================================Copyright Create Declare End==================================

=====================================Modify Records Explain Start==================================

Modifier:	
ModificationTime:
Description:


===================================================================================================

Modifier:
ModificationTime:
Description:


=====================================Modify Records Explain End====================================

===================================================================================================*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data.Common;

namespace OpenDeepSpace.EntityFrameworkCore
{
    /// <summary>
    /// DbConnection共享源 用于不同上下文实例 同数据库字符串 共享连接提供DbConnection连接源
    /// </summary>
    public class DbConnectionSharedSource
    {
        //Dbconnection连接字典 key为字符串名称
        private readonly Dictionary<string, DbConnection> dbConnections = new Dictionary<string, DbConnection>();

        

        public DbConnection this[string connectionString]
        {
            get
            {
                if (!dbConnections.TryGetValue(connectionString, out DbConnection? conn))
                {
                    return dbConnections[connectionString] = null;
                }
                else
                {
                    return conn;
                }
            }

        }
    }
}
