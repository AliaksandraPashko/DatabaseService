using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;
using Newtonsoft.Json;
using System.Web.ModelBinding;

namespace DatabaseWebService
{
    [AspNetCompatibilityRequirements(RequirementsMode =AspNetCompatibilityRequirementsMode.Required)]
    public class Database : IDatabase
    {
        SqlConnection connection;
        SqlCommand command;

        string connectionString = ConfigurationManager.ConnectionStrings["RailwayDbConnectionString"].ConnectionString;

        public List<Destination> GetDestination()
        {
            SqlDataReader reader;
            connection = new SqlConnection(connectionString);
            connection.Open();
            command = new SqlCommand("Select id, route_from, route_to from Destination", connection);
            reader = command.ExecuteReader();
          
            List<Destination> list = new List<Destination>();
            SqlDataReader rdr;
            while (reader.Read())
            {
                string id = reader[0].ToString();
                Destination destination = new Destination();
                destination.route_from = reader[1].ToString();
                destination.route_to = reader[2].ToString();

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand("Select free from Date where idDestination =" + id, sqlConnection);

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr[0].ToString() == "True")
                    {
                        list.Add(destination);
                        rdr.Close();
                        break;
                    }
                }   
                sqlConnection.Close();
            }
            
            connection.Close();
            return list;
        }

        public Dictionary<string, List<string>> GetDateOfDestination(string from, string to)
        {
            List<string> list = new List<string>();

            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlDataReader reader;
            string id;
            SqlCommand command = new SqlCommand("Select * from Destination where route_from = @from and route_to = @to", connection);
            command.Parameters.Add(new SqlParameter("@from", from));
            command.Parameters.Add(new SqlParameter("@to", to));
            reader = command.ExecuteReader();
            reader.Read();
           
            id = reader[0].ToString();
            reader.Close();
            command = new SqlCommand("Select date,free from Date where IdDestination = " + id, connection);

            reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader[1].ToString() == "True")
                {
                    string info = reader[0].ToString();
                    list.Add(info);
                }
            }
           
            connection.Close();

            Dictionary<string, List<string>> dictionary = new Dictionary<string,List<string>>();

            foreach (string s in list)
            {
                string[] vals = s.Split(' ');
                string date = vals[0];
                string time = vals[1];
                if (!dictionary.ContainsKey(date))
                {
                    List<string> times = new List<string>();
                    times.Add(time);
                    dictionary.Add(date,times);
                }
                else
                {
                    List<String> times = dictionary[date];
                    times.Add(time);
                    dictionary[date] = times;
                }
            }

            return dictionary;
        }

        public Response GetConfirmation(string from, string to, string datetime)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            datetime = datetime.Replace('-', ':');
           
            SqlDataReader reader;
            string id;
            SqlCommand command = new SqlCommand("Select * from Destination where route_from = @from and route_to = @to", connection);
            command.Parameters.Add(new SqlParameter("@from", from));
            command.Parameters.Add(new SqlParameter("@to", to));
            reader = command.ExecuteReader();
            reader.Read();

            id = reader[0].ToString();
            reader.Close();

            bool isParsed = DateTime.TryParse(datetime, out var dateTime);

            command = new SqlCommand("Select id from Date where IdDestination = @id and date = @datetime", connection);
            command.Parameters.Add(new SqlParameter("@id", id));
            command.Parameters.Add(new SqlParameter("@datetime", dateTime));
            
            reader = command.ExecuteReader();
            reader.Read();
            var secondId = reader[0];
            reader.Close();

            SqlTransaction tr = connection.BeginTransaction();
            command = new SqlCommand("Update Date Set free = @flag where id = @id", connection, tr);
            command.Parameters.Add(new SqlParameter("@flag", false));
            command.Parameters.Add(new SqlParameter("@id", secondId));

            Response response = new Response();
            if (command.ExecuteNonQuery() != 0)
            {
                response.responseCode = 200;
                response.responseMessage = "Success";
                tr.Commit();
            }
            else
            {
                response.responseCode = 404;
                response.responseMessage = "Error";
                tr.Rollback();
            }

            connection.Close();
            return response;
        }

    }
}
