using System.Data;

namespace Domain.Entities.UserBroker
{
    public class UserBrokerConfigurationDataTable
    {
        public static DataTable ToDataTable(IEnumerable<UserBrokerConfiguration> configurations)
        {
            var table = new DataTable();
            table.Columns.Add("ConfigKey", typeof(string));
            table.Columns.Add("ConfigValue", typeof(string));

            foreach (var config in configurations)
            {
                var row = table.NewRow();
                row["ConfigKey"] = config.ConfigKey;
                row["ConfigValue"] = config.ConfigValue;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
