using AppliancesShop_DataAccess.Repository.IRepository;
using AppliancesShop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliancesShop_DataAccess.Repository
{
    public class LogRepository : Repository<LogEntry>, ILogRepository
    {
        public LogRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
