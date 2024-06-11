using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft .Data.SqlClient;


namespace pets.Pages
{
    public class ShoppingModel : PageModel
    {
        private readonly string _connectionString;

        public ShoppingModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IList<Pet> Pets { get; set; } = new List<Pet>();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public void OnGet()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT PetID, Name,Price, Description, ImageURL FROM Pets";

                if (!string.IsNullOrEmpty(SearchString))
                {
                    sql += " WHERE Name LIKE @SearchString OR Description LIKE @SearchString";
                }

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (!string.IsNullOrEmpty(SearchString))
                    {
                        command.Parameters.AddWithValue("@SearchString", "%" + SearchString + "%");
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pets.Add(new Pet
                            {
                                PetID = reader.GetInt32( 0),
                                Name = reader.GetString(1),
                                Price = reader.GetString(2),
                                Description = reader.GetString(3),
                                ImageURL = reader.GetString(4)
                            });
                        }
                    }
                }
            }
        }
        public IActionResult OnPost(int petId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Insert the pet into the cart table
                string sql = "INSERT INTO Cart (PetID, Quantity) VALUES (@PetID, 1)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PetID", petId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }
    }

    public class Pet
    {
        public int PetID { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
    }
}
