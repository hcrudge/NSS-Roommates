using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Chore data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    class ChoreRepository : BaseRepository
    {
        /// <summary>
        ///  When new RoomRepository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public ChoreRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        ///  Get a list of all Chores in the database
        /// </summary>

        public List<Chore> GetAll()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection connection = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                connection.Open();

                // We must "use" commands too.
                using(SqlCommand command = connection.CreateCommand())
                {
                    // Here we setup the command with the SQL script we want to execute before we execute it.
                    command.CommandText = "SELECT Id, Name FROM Chore";

                    // Execute the SQL script in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = command.ExecuteReader();

                    // A list to hold the chores we retrieve from the database.
                    List<Chore> chores = new List<Chore>();

                    //Read() will return true if there's more data rows to read
                    while(reader.Read())
                    {
                        //The "ordinal" is the numeric position of the column in the query results.
                        //For our query, "Id" has an ordinal value of 0 and "Name" has an ordinal value of 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        //We use the reader's "GetXXX" methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        //Do the same for the "Name" ordinal value.
                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        //Creating a new chore object using the data from the database.

                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue,
                        };

                        //..add the newly created chore object to our list of chores.
                        chores.Add(chore);
                    }
                    //close the reader
                    reader.Close();

                    //Return the list of chores to whomever called the method.
                    return chores;

                }
            }


        }

        /// <summary>
        ///  Returns a single chore with the given id.
        /// </summary>
        public Chore GetById(int id)
        {
            using(SqlConnection connection = Connection)
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = command.ExecuteReader();

                    Chore chore = null;

                        // No need for a while() loop as we only expect a single row back from the database.
                        if (reader.Read())
                        {
                            chore = new Chore
                            {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            };
                        }

                    reader.Close();

                    return chore;
                }
            }
        }

        /// <summary>
        ///  Add a new chore to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void Insert(Chore chore)
        {
            using (SqlConnection connection = Connection)
            {
                connection.Open();
                using(SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Chore (Name)
                                                    OUTPUT INSERTED.Id
                                                    VALUES (@name)";
                    command.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)command.ExecuteScalar();

                    chore.Id = id;
                }
            }
        }

        public void AssignChore(int roommateId, int choreId)
        {
            using (SqlConnection connection = Connection)
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO RoommateChore(RoommateId,ChoreId)
                                                    OUTPUT INSERTED.Id
                                                    VALUES(@roommateId, @choreId)";
                    command.Parameters.AddWithValue("@roommateId", roommateId);
                    command.Parameters.AddWithValue("@choreId", choreId);
                    int id = (int)command.ExecuteScalar();

                }
            }
        }


    }
}
