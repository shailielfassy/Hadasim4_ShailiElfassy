using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Hadasim4._0Ex1.Pages.Clients
{
    public class EditModel : PageModel
    {
        public ClientInformation clientInformation = new ClientInformation();
        public CovidInformation covidInformation = new CovidInformation();
        public string errorMessage = "";
        public string successMessage = "";
        string base64Image = "";
        public Image image = new Image();

        public IFormFile UploadedImage { get; set; }

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

        public void OnGet()
        {
            string id = Request.Query["id"];
            try
            {
                string connectionString = "server=localhost;uid=root;password=Jtmmaethel1;database=CoronaDatabase;sslmode=none";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); //show information of all the tables
                    string sql = "SELECT * FROM clients " +
                    "JOIN covid ON clients.id = covid.client_id " +
                    "LEFT JOIN images ON clients.id = images.client_id " +
                    "WHERE clients.id = @id;";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                clientInformation.fullName = reader.GetString("FullName");
                                clientInformation.id = " " + reader.GetUInt32("Id");
                                clientInformation.city = reader.GetString("City");
                                clientInformation.street = reader.GetString("Street");
                                clientInformation.dateOfBirth = reader.GetDateTime("DateOfBirth").ToString("yyyy-MM-dd");
                                clientInformation.phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone");
                                clientInformation.mobilePhone = reader.GetString("MobilePhone");
                                
                                covidInformation.client_id = " " + reader.GetUInt32("id");
                                covidInformation.vaccination1Date = reader.IsDBNull(reader.GetOrdinal("Vaccination1Date")) ? null : reader.GetDateTime("Vaccination1Date").ToString("yyyy-MM-dd");
                                covidInformation.vaccination1Manufacturer = reader.IsDBNull(reader.GetOrdinal("Vaccination1Manufacturer")) ? null : reader.GetString("Vaccination1Manufacturer");
                                covidInformation.vaccination2Date = reader.IsDBNull(reader.GetOrdinal("Vaccination2Date")) ? null : reader.GetDateTime("Vaccination2Date").ToString("yyyy-MM-dd");
                                covidInformation.vaccination2Manufacturer = reader.IsDBNull(reader.GetOrdinal("Vaccination2Manufacturer")) ? null : reader.GetString("Vaccination2Manufacturer");
                                covidInformation.vaccination3Date = reader.IsDBNull(reader.GetOrdinal("Vaccination3Date")) ? null : reader.GetDateTime("Vaccination3Date").ToString("yyyy-MM-dd");
                                covidInformation.vaccination3Manufacturer = reader.IsDBNull(reader.GetOrdinal("Vaccination3Manufacturer")) ? null : reader.GetString("Vaccination3Manufacturer");
                                covidInformation.vaccination4Date = reader.IsDBNull(reader.GetOrdinal("Vaccination4Date")) ? null : reader.GetDateTime("Vaccination4Date").ToString("yyyy-MM-dd");
                                covidInformation.vaccination4Manufacturer = reader.IsDBNull(reader.GetOrdinal("Vaccination4Manufacturer")) ? null : reader.GetString("Vaccination4Manufacturer");
                                covidInformation.positiveTestDate = reader.IsDBNull(reader.GetOrdinal("PositiveTestDate")) ? null : reader.GetDateTime("PositiveTestDate").ToString("yyyy-MM-dd");
                                covidInformation.recoveryDate = reader.IsDBNull(reader.GetOrdinal("RecoveryDate")) ? null : reader.GetDateTime("RecoveryDate").ToString("yyyy-MM-dd");

                                image.client_id = " " + reader.GetUInt32("id");
                                byte[] temp = (byte[])reader["ImageData"];
                                image.img = Encoding.UTF8.GetString(temp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        //check order of vaccinations
        private bool IsValidVaccinationOrder(string date1, string date2)
        {
            if (string.IsNullOrWhiteSpace(date1) || string.IsNullOrWhiteSpace(date2))
                return true;
            if (DateTime.TryParse(date1, out DateTime dt1) && DateTime.TryParse(date2, out DateTime dt2))
                return dt2 > dt1;
            return false;
        }

        //when we submit
        public async Task<IActionResult> OnPost()
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

            image.client_id= Request.Form["id"];
            UploadedImage = Request.Form.Files["profilePicture"];

            // Check if an image was uploaded
            if (UploadedImage != null)
            {
                byte[] imageDataBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await UploadedImage.CopyToAsync(memoryStream);
                    imageDataBytes = memoryStream.ToArray();
                    base64Image = Convert.ToBase64String(imageDataBytes);
                }
                image.img = base64Image;
            }

            //check inputs
            if (string.IsNullOrWhiteSpace(clientInformation.fullName))
            {
                errorMessage = "Name is required";
                SetErrorStyle("Name");
                SetErrorMessage("Name", errorMessage);
                return Page();
            }

            if (string.IsNullOrWhiteSpace(clientInformation.city))
            {
                errorMessage = "City is required";
                SetErrorStyle("City");
                SetErrorMessage("City", errorMessage);
                return Page();
            }

            if (string.IsNullOrWhiteSpace(clientInformation.street))
            {
                errorMessage = "Street is required";
                SetErrorStyle("Street");
                SetErrorMessage("Street", errorMessage);
                return Page();
            }

            if (string.IsNullOrWhiteSpace(clientInformation.dateOfBirth))
            {
                errorMessage = "Date of birth is required";
                SetErrorStyle("DateOfBirth");
                SetErrorMessage("DateOfBirth", errorMessage);
                return Page();
            }

            if (!string.IsNullOrEmpty(clientInformation.phone) && clientInformation.phone.Length != 7)
            {
                errorMessage = "Phone must be exactly 7 digits long";
                SetErrorStyle("phone");
                SetErrorMessage("phone", errorMessage);
                return Page();
            }

            if (string.IsNullOrWhiteSpace(clientInformation.mobilePhone))
            {
                errorMessage = "Mobile phone is required";
                SetErrorStyle("MobilePhone");
                SetErrorMessage("MobilePhone", errorMessage);
                return Page();
            }
            else if (clientInformation.mobilePhone.Length != 10)
            {
                errorMessage = "Mobile phone must be exactly 10 digits long";
                SetErrorStyle("mobilePhone");
                SetErrorMessage("mobilePhone", errorMessage);
                return Page();
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
                        return Page();
                    }
                }
                else
                {
                    errorMessage = "Invalid date format for positive test date or recovery date.";
                    return Page();
                }
            }

            // Validate vaccination dates
            if (!IsValidVaccinationOrder(covidInformation.vaccination1Date, covidInformation.vaccination2Date))
            {
                errorMessage = "Second vaccination date must be after the first vaccination date.";
                SetErrorStyle("vaccination2Date");
                SetErrorMessage("vaccination2Date", errorMessage);
                return Page();
            }

            if (!IsValidVaccinationOrder(covidInformation.vaccination2Date, covidInformation.vaccination3Date))
            {
                errorMessage = "Third vaccination date must be after the second vaccination date.";
                SetErrorStyle("vaccination3Date");
                SetErrorMessage("vaccination3Date", errorMessage);
                return Page();
            }

            if (!IsValidVaccinationOrder(covidInformation.vaccination3Date, covidInformation.vaccination4Date))
            {
                errorMessage = "Fourth vaccination date must be after the third vaccination date.";
                SetErrorStyle("vaccination4Date");
                SetErrorMessage("vaccination4Date", errorMessage);
                return Page();
            }

            try
            {
                string connectionString = "server=localhost;uid=root;password=Jtmmaethel1;database=CoronaDatabase;sslmode=none";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Update client information
                    string updateClientSql = "UPDATE Clients " +
                        "SET FullName=@fullName, Id=@id, City=@city, Street=@street, DateOfBirth=@dateOfBirth, Phone=@phone, MobilePhone=@mobilePhone " +
                        "WHERE Id=@id;";

                    using (MySqlCommand updateClientCommand = new MySqlCommand(updateClientSql, connection))
                    {
                        updateClientCommand.Parameters.AddWithValue("@fullName", clientInformation.fullName);
                        updateClientCommand.Parameters.AddWithValue("@id", clientInformation.id);
                        updateClientCommand.Parameters.AddWithValue("@city", clientInformation.city);
                        updateClientCommand.Parameters.AddWithValue("@street", clientInformation.street);
                        updateClientCommand.Parameters.AddWithValue("@dateOfBirth", clientInformation.dateOfBirth);
                        updateClientCommand.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(clientInformation.phone) ? DBNull.Value : (object)clientInformation.phone);
                        updateClientCommand.Parameters.AddWithValue("@mobilePhone", clientInformation.mobilePhone);

                        updateClientCommand.ExecuteNonQuery();
                    }

                    // Update Covid information
                    string updateCovidSql = "UPDATE Covid " +
                        "SET Vaccination1Date=@vaccination1Date, Vaccination1Manufacturer=@vaccination1Manufacturer, " +
                        "vaccination2Date=@vaccination2Date, vaccination2Manufacturer=@vaccination2Manufacturer, " +
                        "vaccination3Date=@vaccination3Date, vaccination3Manufacturer=@vaccination3Manufacturer, " +
                        "vaccination4Date=@vaccination4Date, vaccination4Manufacturer=@vaccination4Manufacturer, " +
                        "PositiveTestDate=@positiveTestDate, RecoveryDate=@recoveryDate " +
                        "WHERE client_id=@client_id;";

                    DateTime? ParseDate(string input)
                    {
                        if (DateTime.TryParse(input, out DateTime date))
                        {
                            return date;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    using (MySqlCommand updateCovidCommand = new MySqlCommand(updateCovidSql, connection))
                    {
                        updateCovidCommand.Parameters.AddWithValue("@client_id", clientInformation.id);
                        updateCovidCommand.Parameters.AddWithValue("@vaccination1Date", string.IsNullOrWhiteSpace(covidInformation.vaccination1Date) ? DBNull.Value : (object)ParseDate(covidInformation.vaccination1Date));
                        updateCovidCommand.Parameters.AddWithValue("@vaccination1Manufacturer", string.IsNullOrWhiteSpace(covidInformation.vaccination1Manufacturer) ? DBNull.Value : (object)covidInformation.vaccination1Manufacturer);
                        updateCovidCommand.Parameters.AddWithValue("@vaccination2Date", string.IsNullOrWhiteSpace(covidInformation.vaccination2Date) ? DBNull.Value : (object)ParseDate(covidInformation.vaccination2Date));
                        updateCovidCommand.Parameters.AddWithValue("@vaccination2Manufacturer", string.IsNullOrWhiteSpace(covidInformation.vaccination2Manufacturer) ? DBNull.Value : (object)covidInformation.vaccination2Manufacturer);
                        updateCovidCommand.Parameters.AddWithValue("@vaccination3Date", string.IsNullOrWhiteSpace(covidInformation.vaccination3Date) ? DBNull.Value : (object)ParseDate(covidInformation.vaccination3Date));
                        updateCovidCommand.Parameters.AddWithValue("@vaccination3Manufacturer", string.IsNullOrWhiteSpace(covidInformation.vaccination3Manufacturer) ? DBNull.Value : (object)covidInformation.vaccination3Manufacturer);
                        updateCovidCommand.Parameters.AddWithValue("@vaccination4Date", string.IsNullOrWhiteSpace(covidInformation.vaccination4Date) ? DBNull.Value : (object)ParseDate(covidInformation.vaccination4Date));
                        updateCovidCommand.Parameters.AddWithValue("@vaccination4Manufacturer", string.IsNullOrWhiteSpace(covidInformation.vaccination4Manufacturer) ? DBNull.Value : (object)covidInformation.vaccination4Manufacturer);
                        updateCovidCommand.Parameters.AddWithValue("@positiveTestDate", string.IsNullOrWhiteSpace(covidInformation.positiveTestDate) ? DBNull.Value : (object)ParseDate(covidInformation.positiveTestDate));
                        updateCovidCommand.Parameters.AddWithValue("@recoveryDate", string.IsNullOrWhiteSpace(covidInformation.recoveryDate) ? DBNull.Value : (object)ParseDate(covidInformation.recoveryDate));

                        updateCovidCommand.ExecuteNonQuery();
                    }

                    // Insert/Update image if loaded
                    if (UploadedImage != null && UploadedImage.Length > 0)
                    {
                        // Check if an image record already exists for the client
                        string checkImageSql = "SELECT COUNT(*) FROM Images WHERE Client_Id = @clientId";
                        using (MySqlCommand checkImageCommand = new MySqlCommand(checkImageSql, connection))
                        {
                            checkImageCommand.Parameters.AddWithValue("@clientId", clientInformation.id);
                            int imageCount = Convert.ToInt32(checkImageCommand.ExecuteScalar());

                            if (imageCount > 0)
                            {
                                // If an image record exists, update the existing record with the new image data
                                string updateImageSql = "UPDATE Images SET ImageData = @imageData WHERE Client_Id = @clientId";
                                using (MySqlCommand updateImageCommand = new MySqlCommand(updateImageSql, connection))
                                {
                                    updateImageCommand.Parameters.AddWithValue("@clientId", clientInformation.id);
                                    updateImageCommand.Parameters.AddWithValue("@imageData", base64Image);
                                    updateImageCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // If no image record exists, insert a new record
                                string insertImageSql = "INSERT INTO Images (Client_Id, ImageData) VALUES (@clientId, @imageData)";
                                using (MySqlCommand insertImageCommand = new MySqlCommand(insertImageSql, connection))
                                {
                                    insertImageCommand.Parameters.AddWithValue("@clientId", clientInformation.id);
                                    insertImageCommand.Parameters.AddWithValue("@imageData", base64Image);
                                    insertImageCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    string s = Request.Form["removeImageFlag"];
                    if (s =="true") //if remove button was pressed then remove the image
                    {
                        string id = Request.Query["id"];

                        string deleteimageSql = "DELETE FROM images WHERE client_id = @id;";

                        using (MySqlCommand deleteClientCommand = new MySqlCommand(deleteimageSql, connection))
                        {
                            deleteClientCommand.Parameters.AddWithValue("@id", id);
                            deleteClientCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message; // Handle other types of exceptions
                return Page();
            }

            //redirect the user to the list of clients
            return RedirectToPage("/Clients/ClientList");
        }
    }
}