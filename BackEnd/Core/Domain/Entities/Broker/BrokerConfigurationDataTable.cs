using System.Data;

namespace Domain.Entities.Broker
{
    public class BrokerConfigurationDataTable
    {
        public static DataTable ToDataTable(IEnumerable<BrokerConfiguration> configurations)
        {
            var table = new DataTable();
            table.Columns.Add("BrokerId", typeof(Guid));
            table.Columns.Add("ConfigKey", typeof(string));

            foreach (var config in configurations)
            {
                var row = table.NewRow();
                row["BrokerId"] = config.BrokerId;
                row["ConfigKey"] = config.ConfigKey;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
