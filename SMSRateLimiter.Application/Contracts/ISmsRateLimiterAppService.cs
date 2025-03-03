using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Contracts
{
    public interface ISmsRateLimiterAppService
    {
        bool CanSendMessage(string phoneNumber);
        int GetGlobalMessageCount();
        int GetMessageCountForNumber(string phoneNumber);
    }
}
