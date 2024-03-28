using Hadasim4._0Ex1.Pages.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hadasim4._0Ex1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public List<ActivePatientData> ActivePatientsData { get; set; } = new List<ActivePatientData>();

        //property to store the amount of clients with no vaccinations
        public int ClientsWithNoVaccinations { get; set; }

        //property to store the amount of clients
        public int SumClients { get; set; }

        public string errorMessage = "";

        //showing covid data summary at the home page:
        //it includes bonuses 
        public void OnGet()
        {
            try
            {
                string connectionString = "server=localhost;uid=root;password=Jtmmaethel1;database=CoronaDatabase;sslmode=none";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    //Retrieve the number of active patients for each date within the last 30 days
                    string sql = "SELECT date_column, COUNT(*) AS active_patients FROM( SELECT client_id, positiveTestDate AS date_column FROM Covid " +
                    "WHERE positiveTestDate IS NOT NULL AND positiveTestDate >= DATE_SUB(CURDATE(), INTERVAL 30 DAY) AND recoveryDate IS NULL UNION ALL SELECT client_id, recoveryDate AS date_column FROM Covid " +
                    "WHERE recoveryDate IS NOT NULL AND recoveryDate >= DATE_SUB(CURDATE(), INTERVAL 30 DAY)) AS subquery GROUP BY date_column ORDER BY date_column;";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ActivePatientData activePatient = new ActivePatientData
                                {
                                    DateColumn = reader.GetDateTime("date_column").ToString("yyyy-MM-dd"),
                                    ActivePatients = " " + reader.GetInt32("active_patients")
                                };
                                ActivePatientsData.Add(activePatient);
                            }
                        }
                    }

                    //Retrieve the number of unvaccinated patients
                    string NoVaccinsql = "SELECT COUNT(*) FROM Covid WHERE vaccination1Date IS NULL AND vaccination2Date IS NULL AND vaccination3Date IS NULL AND vaccination4Date IS NULL;";
                    using (MySqlCommand command = new MySqlCommand(NoVaccinsql, connection))
                    {
                        ClientsWithNoVaccinations = Convert.ToInt32(command.ExecuteScalar());
                    }

                    //Retrieve the number of unvaccinated patients
                    string NmClientsql = "SELECT COUNT(*) FROM Clients;";
                    using (MySqlCommand command = new MySqlCommand(NmClientsql, connection))
                    {
                        SumClients = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (MySqlException ex)
            {
                errorMessage = ex.Message;
            }
        }
    }

    public class ActivePatientData
    {
        public string DateColumn;
        public string ActivePatients;
    }
}
