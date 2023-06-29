/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/29 12:54:53	
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

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.EntityFrameworkCore
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        public ConnectionStringResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IConfiguration configuration { get; set; }

        

        public string Resolve()
        {
            return configuration.GetConnectionString("ods");
        }
    }
}
