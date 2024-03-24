using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace Hadasim4._0Ex1.Pages.Clients
{
    public class IndexModel : PageModel
    {
        //public so we can read it from the page
        public List<ClientInformation> listClients = new List<ClientInformation>();
        
        //executed when we access the page using http get method
        public void OnGet()
        {
            try
            {
                string connectionString = "server=localhost;uid=root;password=Jtmmaethel1;database=CoronaDatabase;sslmode=none";

                using (MySqlConnection connection = new (connectionString))
                {
                    connection.Open();
                    //create the sql query that allows us to read the data from the clients table
                    string sql = "SELECT * FROM Clients";
                    //create the sql commande
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //read the data from the table and save it into clientInformation object
                            while (reader.Read())
                            {
                                ClientInformation clientInformation = new ClientInformation();
                                clientInformation.fullName = reader.GetString(0);
                                clientInformation.id = " " + reader.GetInt32(1);
                                clientInformation.city = reader.GetString(2);
                                clientInformation.street = reader.GetString(3);
                                clientInformation.dateOfBirth = reader.GetDateTime(4).ToString("yyyy-MM-dd");
                                clientInformation.phone = reader.IsDBNull(5) ? null : reader.GetString(5);
                                clientInformation.mobilePhone = reader.GetString(6);
                               
                                listClients.Add(clientInformation);
                            }

                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }

    //store the data of only one client from the database
    public class ClientInformation
    {
        public string fullName;
        public string id;
        public string city;
        public string street;
        public string dateOfBirth;
        public string phone;
        public string mobilePhone;
    }

    public class CovidInformation
    {
        public string client_id; 
        public string vaccination1Date;
        public string vaccination1Manufacturer;
        public string vaccination2Date;
        public string vaccination2Manufacturer;
        public string vaccination3Date;
        public string vaccination3Manufacturer;
        public string vaccination4Date;
        public string vaccination4Manufacturer;
        public string positiveTestDate;
        public string recoveryDate;
    }
}