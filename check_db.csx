using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Npgsql;

var config = JsonNode.Parse(File.ReadAllText(""d:/FPT/PRM393/BE/CourtManager.APIs/appsettings.json""));
var connString = config[""ConnectionStrings""][""DefaultConnection""].ToString();

using var conn = new NpgsqlConnection(connString);
conn.Open();

using var cmd = new NpgsqlCommand(""SELECT \""BookingItemId\"", \""BookingId\"", \""IsDeleted\"" FROM \""BookingItems\"" WHERE \""BookingId\"" = '699c0e0c-f5fe-484e-96ea-059576330ac8'"", conn);
using var reader = cmd.ExecuteReader();
var hasRows = false;
while (reader.Read())
{
    hasRows = true;
    Console.WriteLine($""BookingItemId: {reader[0]}, BookingId: {reader[1]}, IsDeleted: {reader[2]}"");
}
if (!hasRows) Console.WriteLine(""No BookingItems found for this BookingId."");
