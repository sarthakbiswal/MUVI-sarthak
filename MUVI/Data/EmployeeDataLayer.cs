using MUVI.Model;
using System.Data.SqlClient;

namespace MUVI.Data
{
    public class EmployeeDataLayer
    {
            private readonly string _connectionString;

            public EmployeeDataLayer(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("MyConnectionString");
            }

            public List<Employee> SearchEmployees(int pageLimit, int pageIndex, string searchText)
            {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = @"SELECT * FROM (
                                  SELECT *, ROW_NUMBER() OVER (ORDER BY Id) AS RowNumber
                                  FROM Employees
                                  WHERE FirstName LIKE @SearchText OR LastName LIKE @SearchText OR Email LIKE @SearchText
                                        OR Contact LIKE @SearchText OR Country LIKE @SearchText OR State LIKE @SearchText
                                        OR City LIKE @SearchText OR Pin LIKE @SearchText
                              ) AS PagedEmployees
                              WHERE RowNumber BETWEEN @StartIndex AND @EndIndex";

                    var startIndex = (pageIndex - 1) * pageLimit + 1;
                    var endIndex = pageIndex * pageLimit;

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                        command.Parameters.AddWithValue("@StartIndex", startIndex);
                        command.Parameters.AddWithValue("@EndIndex", endIndex);

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            var employees = new List<Employee>();
                            while (reader.Read())
                            {
                                var employee = new Employee
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Contact = reader["Contact"].ToString(),
                                    Country = reader["Country"].ToString(),
                                    State = reader["State"].ToString(),
                                    City = reader["City"].ToString(),
                                    Pin = reader["Pin"].ToString()
                                };

                                employees.Add(employee);
                            }

                            return employees;
                        }
                    }
                }
            }
            catch (Exception ex)
                {
                    return null;
                }

            }


            public Employee GetEmployeeById(int id)
            {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "SELECT * FROM Employees WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var employee = new Employee
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Contact = reader["Contact"].ToString(),
                                    Country = reader["Country"].ToString(),
                                    State = reader["State"].ToString(),
                                    City = reader["City"].ToString(),
                                    Pin = reader["Pin"].ToString()
                                };

                                return employee;
                            }
                        }
                    }
                }

                return null; // Return null if employee is not found
            }
            catch (Exception ex)
                {
                    return null;
                }

            }


            public void CreateEmployee(Employee employee)
            {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = @"INSERT INTO Employees (FirstName, LastName, Email, Contact, Country, State, City, Pin)
                                  VALUES (@FirstName, @LastName, @Email, @Contact, @Country, @State, @City, @Pin);
                                  SELECT SCOPE_IDENTITY();";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Email", employee.Email);
                        command.Parameters.AddWithValue("@Contact", employee.Contact);
                        command.Parameters.AddWithValue("@Country", employee.Country);
                        command.Parameters.AddWithValue("@State", employee.State);
                        command.Parameters.AddWithValue("@City", employee.City);
                        command.Parameters.AddWithValue("@Pin", employee.Pin);

                        connection.Open();
                        var id = Convert.ToInt32(command.ExecuteScalar());
                        employee.Id = id;
                    }
                }
            }
            catch (Exception ex)
                {

                }
            }

            public void UpdateEmployee(Employee employee)
            {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = @"UPDATE Employees SET
                                  FirstName = @FirstName,
                                  LastName = @LastName,
                                  Email = @Email,
                                  Contact = @Contact,
                                  Country = @Country,
                                  State = @State,
                                  City = @City,
                                  Pin = @Pin
                                  WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", employee.Id);
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Email", employee.Email);
                        command.Parameters.AddWithValue("@Contact", employee.Contact);
                        command.Parameters.AddWithValue("@Country", employee.Country);
                        command.Parameters.AddWithValue("@State", employee.State);
                        command.Parameters.AddWithValue("@City", employee.City);
                        command.Parameters.AddWithValue("@Pin", employee.Pin);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
                {

                }

            }

            public void DeleteEmployee(int id)
            {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "DELETE FROM Employees WHERE Id = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
                {

                }
            }


        }
    }

