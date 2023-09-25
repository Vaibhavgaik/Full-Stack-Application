using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Full_Stack_Application.Models; // Import the Product model

namespace Full_Stack_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string query = "SELECT * FROM Products";
                List<Product> products = new List<Product>(); // Create a list to store Product objects
                string sqlDataSource = _configuration.GetConnectionString("ProductCon");

                using (MySqlConnection myConnection = new MySqlConnection(sqlDataSource))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(query, myConnection))
                    {
                        myConnection.Open();

                        using (MySqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                Product product = new Product
                                {
                                    ID = Convert.ToInt32(myReader["ID"]),
                                    ProductName = myReader["ProductName"].ToString(),
                                    Category = myReader["Category"].ToString(),
                                    Price = Convert.ToDecimal(myReader["Price"]),
                                    StockQuantity = Convert.ToInt32(myReader["StockQuantity"]),
                                    Supplier = myReader["Supplier"].ToString()
                                };

                                products.Add(product); // Add each product to the list
                            }
                        }

                        myConnection.Close();
                    }
                }

                return Ok(products); // Serialize the list of products
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Post(Product product)
        {
            try
            {
                string query = @"
                    INSERT INTO Products (ProductName, Category, Price, StockQuantity, Supplier) 
                    VALUES (@ProductName, @Category, @Price, @StockQuantity, @Supplier);
                ";

                string sqlDataSource = _configuration.GetConnectionString("ProductCon");

                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                    {
                        myCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                        myCommand.Parameters.AddWithValue("@Category", product.Category);
                        myCommand.Parameters.AddWithValue("@Price", product.Price);
                        myCommand.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                        myCommand.Parameters.AddWithValue("@Supplier", product.Supplier);

                        myCommand.ExecuteNonQuery();

                        mycon.Close();
                    }
                }

                return Ok("Product added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult Put(Product product)
        {
            try
            {
                string query = @"
                    UPDATE Products 
                    SET ProductName = @ProductName, Category = @Category, 
                        Price = @Price, StockQuantity = @StockQuantity, Supplier = @Supplier 
                    WHERE ID = @ID;
                ";

                string sqlDataSource = _configuration.GetConnectionString("ProductCon");

                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                    {
                        myCommand.Parameters.AddWithValue("@ID", product.ID);
                        myCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                        myCommand.Parameters.AddWithValue("@Category", product.Category);
                        myCommand.Parameters.AddWithValue("@Price", product.Price);
                        myCommand.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                        myCommand.Parameters.AddWithValue("@Supplier", product.Supplier);

                        myCommand.ExecuteNonQuery();

                        mycon.Close();
                    }
                }

                return Ok("Product updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                string query = "DELETE FROM Products WHERE ID = @ID";

                string sqlDataSource = _configuration.GetConnectionString("ProductCon");

                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                    {
                        myCommand.Parameters.AddWithValue("@ID", id);

                        myCommand.ExecuteNonQuery();

                        mycon.Close();
                    }
                }

                return Ok("Product deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
