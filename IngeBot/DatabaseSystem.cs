using Npgsql;
using System;

namespace IngeBot
{
    public class DatabaseSystem
    {

        private static string connectionKey = "";

        public static void Init(string host, string port, string username, string password, string database)
        {
            connectionKey = $"Host={host};Port={port};Username={username};Password={password};Database={database}";
        }

        public static NpgsqlConnection GetConnection()
        {

            NpgsqlConnection connection = new NpgsqlConnection(connectionKey);
            connection.Open();
            return connection;

        }

        public static NpgsqlDataReader ExecuteSelect(string request)
        {
            NpgsqlConnection connection = GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand(request, connection);
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        public static int ExecuteInsert(string request, params (string, object)[] args)
        {

            using NpgsqlConnection connection = GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand(request, connection);

            foreach (var (name, value) in args)
                cmd.Parameters.AddWithValue(name, value);


            var result = cmd.ExecuteScalar();

            if (result == null) return -1;
            return (int)result;

        }

        public static int ExecuteDelete(string request, int id)
        {
            using NpgsqlConnection connection = GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand(request, connection);

            cmd.Parameters.AddWithValue("@id", id);

            var result = cmd.ExecuteScalar();

            if (result == null) return -1;
            return (int)result;
        }

    }
}
