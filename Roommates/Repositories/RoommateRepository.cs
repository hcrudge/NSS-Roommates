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
        ///  This class is responsible for interacting with Roommate data.
        ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
        /// </summary>
    class RoommateRepository : BaseRepository
    { 
        
        public RoommateRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        ///  Get a list of all Roommates in the database
        /// </summary>
        
        public List<Roommate> GetAll()
        {
            using (SqlConnection connection = Connection)
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id, FirstName, LastName, RentPortion, MoveInDate, RoomId FROM Roommate";

                    SqlDataReader reader = command.ExecuteReader();

                    List<Roommate> mates = new List<Roommate>();

                    while(reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        int fNameColumnPosition = reader.GetOrdinal("FirstName");
                        string fNameValue = reader.GetString(fNameColumnPosition);

                        int lNameColumnPosition = reader.GetOrdinal("LastName");
                        string lNameValue = reader.GetString(lNameColumnPosition);

                        int rentColumnPosition = reader.GetOrdinal("RentPortion");
                        int rentValue = reader.GetInt32(rentColumnPosition);

                        int moveInColumnPosition = reader.GetOrdinal("MoveInDate");
                        DateTime moveInValue = reader.GetDateTime(moveInColumnPosition);

                        Roommate mate = new Roommate
                        {
                            Id = idValue,
                            FirstName = fNameValue,
                            LastName = lNameValue,
                            RentPortion = rentValue,
                            MoveInDate = moveInValue,
                       
                        };

                        mates.Add(mate);
                    }

                    reader.Close();

                    return mates;


                }
            }
        }



















    }
}
