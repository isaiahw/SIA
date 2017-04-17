using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Xamarin.Forms;

using SIA.iOS;

[assembly: Dependency(typeof(GetNavData))]
namespace SIA.iOS
{
    public class GetNavData : IGetNavData
    {
        private string connStr;
        private string data = "";
        private List<string> dataList;

        public GetNavData() { }

        public string getQueryJSON(string sql)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {

                SqlCommand command = new SqlCommand(sql, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //must convert to JSON format
                        data = reader[0].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    data = ex.Message;
                }
            }
            return data;
        }

        public string getQuerySingleRow(string sql)
        {
            

            using (SqlConnection connection = new SqlConnection(connStr))
            {

                SqlCommand command = new SqlCommand(sql, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        data=reader[0].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    data = ex.Message;
                }
            }
            return data;
        }

        List<string> IGetNavData.getQueryMultipleRows(string sql)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {

                SqlCommand command = new SqlCommand(sql, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        dataList.Add(reader[0].ToString());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    dataList.Add(ex.Message);
                }
            }
            return dataList;
        }
    }
}