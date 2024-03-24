using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Hadasim4._0Ex1.Pages.Clients
{
    public class CreateModel : PageModel
    {
        public ClientInformation clientInformation= new ClientInformation();
        public CovidInformation covidInformation = new CovidInformation();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet() 
        { 
        }

        //set a red border for the input field with an error
        private void SetErrorStyle(string fieldName)
        {
            ViewData[$"{fieldName}ErrorStyle"] = "border-color: red;";
        }

        //set an error message for the input field
        private void SetErrorMessage(string fieldName, string message)
        {
            ViewData[$"{fieldName}ErrorMessage"] = message;
        }

        //executed when we press the submit button
        public void OnPost()
        {
            clientInformation.fullName = Request.Form["fullName"];
            clientInformation.id = Request.Form["id"];
            clientInformation.city = Request.Form["city"];
            clientInformation.street = Request.Form["street"];
            clientInformation.dateOfBirth = Request.Form["dateOfBirth"];
            clientInformation.phone = Request.Form["phone"];
            clientInformation.mobilePhone = Request.Form["mobilePhone"];
    
            covidInformation.client_id = Request.Form["id"];
            covidInformation.vaccination1Date = Request.Form["vaccination1Date"];
            covidInformation.vaccination1Manufacturer = Request.Form["vaccination1Manufacturer"];
            covidInformation.vaccination2Date = Request.Form["vaccination2Date"];
            covidInformation.vaccination2Manufacturer = Request.Form["vaccination2Manufacturer"];
            covidInformation.vaccination3Date = Request.Form["vaccination3Date"];
            covidInformation.vaccination3Manufacturer = Request.Form["vaccination3Manufacturer"];
            covidInformation.vaccination4Date = Request.Form["vaccination4Date"];
            covidInformation.vaccination4Manufacturer = Request.Form["vaccination4Manufacturer"];
            covidInformation.positiveTestDate = Request.Form["positiveTestDate"];
            covidInformation.recoveryDate = Request.Form["recoveryDate"];

            //check inputs
            if (string.IsNullOrWhiteSpace(clientInformation.fullName))
            {
                errorMessage = "Name is required";
                SetErrorStyle("Name");
                SetErrorMessage("Name", errorMessage);
                return;
            }

            if (string.IsNullOrWhiteSpace(clientInformation.id))
            {
                errorMessage = "ID is required";
                SetErrorStyle("id");
                SetErrorMessage("id", errorMessage);
                return;
            }
            else if (clientInformation.id.Length != 9)
            {
                errorMessage = "ID must be exactly 9 digits long";
                SetErrorStyle("id");
                SetErrorMessage("id", errorMessage);
                return;
            }

            if (string.IsNullOrWhiteSpace(clientInformation.city))
            {
                errorMessage = "City is required";
                SetErrorStyle("City");
                SetErrorMessage("City", errorMessage);
                return;
            }

            if (string.IsNullOrWhiteSpace(clientInformation.street))
            {
                errorMessage = "Street is required";
                SetErrorStyle("Street");
                SetErrorMessage("Street", errorMessage);
                return;
            }

            if (string.IsNullOrWhiteSpace(clientInformation.dateOfBirth))
            {
                errorMessage = "Date of birth is required";
                SetErrorStyle("DateOfBirth");
                SetErrorMessage("DateOfBirth", errorMessage);
                return;
            }

            if (!string.IsNullOrEmpty(clientInformation.phone) && clientInformation.phone.Length != 7)
            {
                errorMessage = "Phone must be exactly 7 digits long";
                SetErrorStyle("phone");
                SetErrorMessage("phone", errorMessage);
                return;
            }

            if (string.IsNullOrWhiteSpace(clientInformation.mobilePhone))
            {
                errorMessage = "Mobile phone is required";
                SetErrorStyle("MobilePhone");
                SetErrorMessage("MobilePhone", errorMessage);
                return;
            }
            else if (clientInformation.mobilePhone.Length != 10)
            {
                errorMessage = "Mobile phone must be exactly 10 digits long";
                SetErrorStyle("mobilePhone");
                SetErrorMessage("mobilePhone", errorMessage);
                return;
            }

            if (!string.IsNullOrWhiteSpace(covidInformation.positiveTestDate) && !string.IsNullOrWhiteSpace(covidInformation.recoveryDate))
            {
                DateTime positiveTest, recovery;
                if (DateTime.TryParse(covidInformation.positiveTestDate, out positiveTest) && DateTime.TryParse(covidInformation.recoveryDate, out recovery))
                {
                    if (recovery <= positiveTest)
                    {
                        errorMessage = "Recovery date must be after positive test date.";
                        SetErrorStyle("recoveryDate");
                        SetErrorMessage("recoveryDate", errorMessage);
                        return;
                    }
                }
                else
                {
                    errorMessage = "Invalid date format for positive test date or recovery date.";
                    return;
                }
            }

            //save the new client into the database
            try
            {
                string connectionString = "server=localhost;uid=root;password=Jtmmaethel1;database=CoronaDatabase;sslmode=none";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert into the Client table first to get the client ID
                    string clientInsertSql = "INSERT INTO Clients (FullName, Id, City, Street, DateOfBirth, Phone, MobilePhone) " +
                        "VALUES (@fullName, @id, @city, @street, @dateOfBirth, @phone, @mobilePhone);";
                    using (MySqlCommand command = new MySqlCommand(clientInsertSql, connection))
                    {
                        command.Parameters.AddWithValue("@fullName", clientInformation.fullName);
                        command.Parameters.AddWithValue("@id", clientInformation.id);
                        command.Parameters.AddWithValue("@city", clientInformation.city);
                        command.Parameters.AddWithValue("@street", clientInformation.street);
                        command.Parameters.AddWithValue("@dateOfBirth", clientInformation.dateOfBirth);
                        command.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(clientInformation.phone) ? DBNull.Value : (object)clientInformation.phone);
                        command.Parameters.AddWithValue("@mobilePhone", clientInformation.mobilePhone);

                        command.ExecuteNonQuery();
                    }

                    // Retrieve the client ID
                    string clientId = clientInformation.id;
                    
                    // Insert into the Covid table
                    string sql = "INSERT INTO Covid (Client_Id, Vaccination1Date, Vaccination1Manufacturer, Vaccination2Date, Vaccination2Manufacturer, Vaccination3Date, Vaccination3Manufacturer, Vaccination4Date, Vaccination4Manufacturer, PositiveTestDate, RecoveryDate) " +
                    "VALUES (@client_Id, @vaccination1Date, @vaccination1Manufacturer, @vaccination2Date, @vaccination2Manufacturer, @vaccination3Date, @vaccination3Manufacturer, @vaccination4Date, @vaccination4Manufacturer, @positiveTestDate, @recoveryDate);";
                   
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@client_Id", clientId);
                        command.Parameters.AddWithValue("@vaccination1Date", string.IsNullOrWhiteSpace(covidInformation.vaccination1Date) ? DBNull.Value : (object)DateTime.Parse(covidInformation.vaccination1Date));
                        command.Parameters.AddWithValue("@vaccination1Manufacturer", string.IsNullOrWhiteSpace(covidInformation.vaccination1Manufacturer) ? DBNull.Value : (object)covidInformation.vaccination1Manufacturer);
                        command.Parameters.AddWithValue("@vaccination2Date", string.IsNullOrWhiteSpace(covidInformation.vaccination2Date) ? DBNull.Value : (object)DateTime.Parse(covidInformation.vaccination2Date));
                        command.Parameters.AddWithValue("@vaccination2Manufacturer", string.IsNullOrWhiteSpace(covidInformation.vaccination2Manufacturer) ? DBNull.Value : (object)covidInformation.vaccination2Manufacturer);
                        command.Parameters.AddWithValue("@vaccination3Date", string.IsNullOrWhiteSpace(covidInformation.vaccination3Date) ? DBNull.Value : (object)DateTime.Parse(covidInformation.vaccination3Date));
                        command.Parameters.AddWithValue("@vaccination3Manufacturer", string.IsNullOrWhiteSpace(covidInformation.vaccination3Manufacturer) ? DBNull.Value : (object)covidInformation.vaccination3Manufacturer);
                        command.Parameters.AddWithValue("@vaccination4Date", string.IsNullOrWhiteSpace(covidInformation.vaccination4Date) ? DBNull.Value : (object)DateTime.Parse(covidInformation.vaccination4Date));
                        command.Parameters.AddWithValue("@vaccination4Manufacturer", string.IsNullOrWhiteSpace(covidInformation.vaccination4Manufacturer) ? DBNull.Value : (object)covidInformation.vaccination4Manufacturer);
                        command.Parameters.AddWithValue("@positiveTestDate", string.IsNullOrWhiteSpace(covidInformation.positiveTestDate) ? DBNull.Value : (object)DateTime.Parse(covidInformation.positiveTestDate));
                        command.Parameters.AddWithValue("@recoveryDate", string.IsNullOrWhiteSpace(covidInformation.recoveryDate) ? DBNull.Value : (object)DateTime.Parse(covidInformation.recoveryDate));
                        
                        command.ExecuteNonQuery();
                    }
                    
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // MySQL error code for duplicate entry
                {
                    errorMessage = "ID must be unique.";
                    SetErrorStyle("Id");
                    SetErrorMessage("Id", errorMessage);
                }
                else
                    errorMessage = ex.Message; // Handle other MySQL database errors
                return;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message; // Handle other types of exceptions
                return;
            }

            //clear the field
            clientInformation.fullName = "";
            clientInformation.id = "";
            clientInformation.city = "";
            clientInformation.street = "";
            clientInformation.dateOfBirth = "";
            clientInformation.phone = "";
            clientInformation.mobilePhone = "";
            covidInformation.vaccination1Date = "";
            covidInformation.vaccination1Manufacturer = "";
            covidInformation.vaccination2Date = "";
            covidInformation.vaccination2Manufacturer = "";
            covidInformation.vaccination3Date = "";
            covidInformation.vaccination3Manufacturer = "";
            covidInformation.vaccination4Date = "";
            covidInformation.vaccination4Manufacturer = "";
            covidInformation.positiveTestDate = "";
            covidInformation.recoveryDate = "";

            successMessage = "New client Added Correctly";

            //redirect the user to the list of clients
            Response.Redirect("/Clients/ClientList");
        }
    }
}
