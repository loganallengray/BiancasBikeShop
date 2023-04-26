using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using BiancasBikeShop.Models;
using BiancasBikeShop.Utils;
using static System.Collections.Specialized.BitVector32;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Reflection.PortableExecutable;

namespace BiancasBikeShop.Repositories
{
    public class BikeRepository : IBikeRepository
    {
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection("server=localhost\\SQLExpress;database=BiancasBikeShop;integrated security=true;TrustServerCertificate=true");
            }
        }

        public List<Bike> GetAllBikes()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT b.Id, b.Brand, b.Color,
                                o.Id AS OwnerId, o.[Name] AS OwnerName, o.Address,
                                o.Email, o.Telephone,
                                bt.Id AS BikeTypeId, bt.Name AS BikeTypeName,
                                wo.Id AS WorkOrderId, wo.Description AS WorkOrderDescription,
                                wo.DateInitiated AS WorkOrderDateInitiated, wo.DateCompleted AS WorkOrderDateCompleted
                        FROM Bike b
                            LEFT JOIN Owner o ON o.Id = b.OwnerId
                            LEFT JOIN BikeType bt ON bt.Id = b.BikeTypeId
                            LEFT JOIN WorkOrder wo ON wo.BikeId = b.Id;
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Bike> bikes = new List<Bike>();

                        while (reader.Read())
                        {
                            int bikeId = DbUtils.GetInt(reader, "Id");
                            var existingBike = bikes.FirstOrDefault(p => p.Id == bikeId);

                            if (existingBike == null)
                            {
                                existingBike = MakeBike(reader);

                                bikes.Add(existingBike);
                            }

                            if (DbUtils.IsNotDbNull(reader, "WorkOrderId"))
                            {
                                existingBike.WorkOrders.Add(MakeWorkOrder(reader));
                            }
                        }

                        return bikes;
                    }
                }
            }
        }

        public Bike GetBikeById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT b.Id, b.Brand, b.Color,
                                o.Id AS OwnerId, o.[Name] AS OwnerName, o.Address,
                                o.Email, o.Telephone,
                                bt.Id AS BikeTypeId, bt.Name AS BikeTypeName,
                                wo.Id AS WorkOrderId, wo.Description AS WorkOrderDescription,
                                wo.DateInitiated AS WorkOrderDateInitiated, wo.DateCompleted AS WorkOrderDateCompleted
                        FROM Bike b
                            LEFT JOIN Owner o ON o.Id = b.OwnerId
                            LEFT JOIN BikeType bt ON bt.Id = b.BikeTypeId
                            LEFT JOIN WorkOrder wo ON wo.BikeId = b.Id
                        WHERE b.Id = @id;
                    ";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Bike bike = null;

                        while (reader.Read())
                        {
                            if (bike == null)
                            {
                                bike = (MakeBike(reader));
                            }

                            if (DbUtils.IsNotDbNull(reader, "WorkOrderId"))
                            {
                                bike.WorkOrders.Add(MakeWorkOrder(reader));
                            }
                        }

                        return bike;
                    }
                }
            }
        }

        public int GetBikesInShopCount()
        {
            int count = 0;
            // implement code here... 
            return count;
        }

        public Bike MakeBike(SqlDataReader reader)
        {
            return new Bike()
            {
                Id = DbUtils.GetInt(reader, "Id"),
                Brand = DbUtils.GetString(reader, "Brand"),
                Color = DbUtils.GetString(reader, "Color"),
                Owner = new Owner()
                {
                    Id = DbUtils.GetInt(reader, "OwnerId"),
                    Name = DbUtils.GetString(reader, "OwnerName"),
                    Address = DbUtils.GetString(reader, "Address"),
                    Email = DbUtils.GetString(reader, "Email"),
                    Telephone = DbUtils.GetString(reader, "Telephone")
                },
                BikeType = new BikeType()
                {
                    Id = DbUtils.GetInt(reader, "BikeTypeId"),
                    Name = DbUtils.GetString(reader, "BikeTypeName")
                },
                WorkOrders = new List<WorkOrder>()
            };
        }

        public WorkOrder MakeWorkOrder(SqlDataReader reader)
        {
            WorkOrder workOrder = new WorkOrder()
            {
                Id = DbUtils.GetInt(reader, "WorkOrderId"),
                Description = DbUtils.GetString(reader, "WorkOrderDescription"),
                DateInitiated = DbUtils.GetDateTime(reader, "WorkOrderDateInitiated")
            };

            if (DbUtils.IsNotDbNull(reader, "WorkOrderDateCompleted"))
            {
                workOrder.DateCompleted = DbUtils.GetDateTime(reader, "WorkOrderDateCompleted");
            }

            return workOrder;
        }
    }
}
