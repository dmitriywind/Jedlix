using Jedlix.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jedlix.Repositories.Contracts
{
    public interface ICustomerChargingPreferencesRepo
    {
        Task AddCustomerChargingPreferences(IEnumerable<CustomerChargingPreference> customerChargingPreferences);
        Task<IEnumerable<CustomerChargingPreference>> GetCustomerChargingPreferences();
    }
}
