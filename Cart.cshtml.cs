using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace pets.Pages
{
    public class CartModel : PageModel
    {
        private readonly string _connectionString;
        public List<CartItem> CartItems { get; set; }

        public CartModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            CartItems = new List<CartItem>();
        }


        public void OnGet()
        {
            CartItems = new List<CartItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT P.PetID, P.Name, P.Price, C.Quantity, P.ImageURL FROM Pets P JOIN Cart C ON P.PetID = C.PetID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CartItems.Add(new CartItem
                        {
                            PetID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetString(2),
                            Quantity = reader.GetInt32(3),
                            ImageURL = reader.GetString(4)

                        });
                    }
                }
            }
        }

        public IActionResult OnPostUpdate(int petId, int quantity)
        {
            if (quantity <= 0)
            {
                // If quantity is 0 or negative, delete the item from the cart
                return OnPostDelete(petId);
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "UPDATE Cart SET Quantity = @Quantity WHERE PetID = @PetID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@PetID", petId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int petId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "DELETE FROM Cart WHERE PetID = @PetID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PetID", petId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }


    }


    public class CartItem
    {
        public int PetID { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
    }
}
