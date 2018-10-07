using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace BlogEngine.Core.Data.Contracts
{
    public interface IMembershipProvider
    {
        MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords, string process);
    }
}
