﻿
namespace nuComponents.DataTypes.Shared.SqlDataSource
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using umbraco.DataLayer;
    using nuComponents.DataTypes.Shared.Picker;
    using System.Text.RegularExpressions;
    using nuComponents.DataTypes.Interfaces;

    public class SqlDataSource : IPickerDataSource
    {
        public string SqlExpression { get; set; }

        public string ConnectionString { get; set; }

        public IEnumerable<PickerEditorOption> GetEditorOptions(int contextId)
        {
            List<PickerEditorOption> pickerEditorOptions = new List<PickerEditorOption>();

            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[this.ConnectionString];            
            if (connectionStringSettings != null)
            {
                using (ISqlHelper sqlHelper = DataLayerHelper.CreateSqlHelper(connectionStringSettings.ConnectionString))
                {
                    string sql = Regex.Replace(this.SqlExpression, "\n|\r", string.Empty);
                    List<IParameter> parameters = new List<IParameter>();

                    if (sql.Contains("@contextId"))
                    {
                        parameters.Add(sqlHelper.CreateParameter("@contextId", contextId));
                    }

                    using(IRecordsReader recordsReader = sqlHelper.ExecuteReader(sql, parameters.ToArray()))
                    {
                        if(recordsReader != null)
                        {
                            while(recordsReader.Read())
                            {
                                pickerEditorOptions.Add(
                                    new PickerEditorOption()
                                    {
                                        Key = recordsReader.GetObject("Key").ToString(),
                                        Label = recordsReader.GetObject("Label").ToString()
                                    }
                                );
                            }
                        }
                    }
                }
            }

            return pickerEditorOptions;
        }
    }
}
