using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot.Models
{
    public abstract class Model<T> where T : Model<T>
    {

        [ColumnAttribut("id")]
        public int id;

        public abstract string GetTableName();
        public abstract (string column, object value)[] GetColumn();



        protected Model()
        {

        }


        public bool Save()
        {

            /// INSERT
            if (id == -1)
            {

                (string column, object value)[] columns = GetColumn();
                string keys = string.Join(",", columns.Select(v => v.column));
                string maps = string.Join(",", columns.Select(v => "@" + v.column));
                (string, object)[] values = columns.Select(v => ("@" + v.column, v.value)).ToArray();

                id = DatabaseSystem.ExecuteInsert(
                    $"INSERT INTO {GetTableName()} ({keys}) VALUES ({maps}) RETURNING id",
                    values
                );

                return id != -1;
            }

            /// UPDATE
            else
            {

                (string column, object value)[] columns = GetColumn();
                string updates = string.Join(",", columns.Select(v => $"{v.column} = @{v.column}"));
                List<(string, object)> values = columns.Select(v => ("@" + v.column, v.value)).ToList();
                values.Add(("@id", id));

                id = DatabaseSystem.ExecuteInsert(
                    $"UPDATE {GetTableName()} SET {updates} WHERE id = @id RETURNING id",
                    values.ToArray()
                );

                return id != -1;
            }

        }

        public static T? FindById(int id)
        {
            T? instance = (T?)Activator.CreateInstance(typeof(T), nonPublic: true);
            if (instance == null) return default;

            NpgsqlDataReader reader = DatabaseSystem.ExecuteSelect($"SELECT * FROM {instance.GetTableName()} WHERE id = '{id}';");
            if (reader == null || !reader.Read()) return default;

            foreach (var prop in typeof(T).GetFields())
            {
                ColumnAttribut? col = prop.GetCustomAttribute<ColumnAttribut>();
                if (col != null) prop.SetValue(instance, reader[col.Name]);

            }

            return instance;
        }

        public static T? FindWhere(params object[] args)
        {
            T? instance = (T?)Activator.CreateInstance(typeof(T), nonPublic: true);
            if (instance == null) return default;

            string where = string.Join(" ", args);

            Console.WriteLine(where);

            NpgsqlDataReader reader = DatabaseSystem.ExecuteSelect($"SELECT * FROM {instance.GetTableName()} WHERE {where};");
            if (reader == null || !reader.Read()) return default;

            foreach (var prop in typeof(T).GetFields())
            {
                ColumnAttribut? col = prop.GetCustomAttribute<ColumnAttribut>();
                if (col != null) prop.SetValue(instance, reader[col.Name]);

            }

            return instance;
        }

    }
}
