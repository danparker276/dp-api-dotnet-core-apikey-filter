﻿using dp.business.Enums;
using dp.business.Models;
using dp.data.AdoNet.SqlExecution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace dp.data.AdoNet.DataAccessObjects
{
    public class UserDao : BaseDao
    {
        public UserDao(string dpDbConnectionString) : base(dpDbConnectionString)
        {
        }
        public async Task<User> ValidateUser(string email, string password, Role userTypeId)
        {

            SqlQuery proc = new SqlQuery(@" 
                select * from users 
                where email=@email 
                and  password =HashBytes('SHA2_256', @password) and userTypeId=@userTypeId;
                ", 30, System.Data.CommandType.Text);
            proc.AddInputParam("email", SqlDbType.NVarChar, email);
            proc.AddInputParam("password", SqlDbType.NVarChar, password);
            proc.AddInputParam("userTypeId", SqlDbType.Int, (int)userTypeId);
            return await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {
                while (dataReader.Read())
                {
                    return new UserResponse()
                    {
                        UserId = SqlQueryResultParser.GetValue<Int32>(dataReader, "userId"),
                        Role = (Role)SqlQueryResultParser.GetValue<Int32>(dataReader, "userTypeId"),
                        IsActive = SqlQueryResultParser.GetValue<Boolean>(dataReader, "isActive")


                    };
                }
                return null;
            });

        }

        public async Task<int?> CreateUser(UserCreateRequest user)
        {

            SqlQuery proc = new SqlQuery(@" 
	IF (Select count(*) from users where email=@email and usertypeid=@userTypeId ) = 0
	BEGIN
	insert into users (password, email, UserTypeId)
	values (HashBytes('SHA2_256', @password),@email,@userTypeId); select SCOPE_IDENTITY();

	END;
                ", 30, System.Data.CommandType.Text);
            proc.AddInputParam("email", SqlDbType.NVarChar, user.Email);
            proc.AddInputParam("password", SqlDbType.NVarChar, user.Password);
            proc.AddInputParam("userTypeId", SqlDbType.Int, (int)user.Role);
            return await _queryExecutor.ExecuteAsync(proc, sqlReader => GetReturnValue<int?>(sqlReader));


        }

        public async Task<User> GetUserInfo(int userId)
        {
            SqlQuery proc = new SqlQuery(@" 
                select * from users 
                where UserId=@userId;
                ", 30, System.Data.CommandType.Text);
            proc.AddInputParam("userId", SqlDbType.Int, userId);
            return await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {
                while (dataReader.Read())
                {
                    return new User()
                    {
                        UserId = SqlQueryResultParser.GetValue<Int32>(dataReader, "userId"),
                        Role = (Role)SqlQueryResultParser.GetValue<Int32>(dataReader, "userTypeId"),
                        IsActive = SqlQueryResultParser.GetValue<Boolean>(dataReader, "isActive"),
                        Email = SqlQueryResultParser.GetValue<String>(dataReader, "email")
                    };
                }
                return null;
            });
        }
        public async Task<int?> CheckUserAPIKey(string apiKey)
        {

            SqlQuery proc = new SqlQuery(@" select userId from user_api_keys where ApiKeyValue=@apikey; ",
                30, System.Data.CommandType.Text);
            proc.AddInputParam("apiKey", SqlDbType.NVarChar, apiKey);
            return await _queryExecutor.ExecuteAsync(proc, sqlReader => GetReturnValue<int?>(sqlReader));
        }
        public async Task<User> GetUserFromAPIKey(string apiKey)
        {

            SqlQuery proc = new SqlQuery(@" 

                    select u.* from user_api_keys uak
                        inner join users u on u.userId=uak.userId
                        where uak.ApiKeyValue=@apikey;
                ", 30, System.Data.CommandType.Text);
            proc.AddInputParam("apiKey", SqlDbType.NVarChar, apiKey);
            return await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {

                while (dataReader.Read())
                {
                    return (new User()
                    {

                        UserId = SqlQueryResultParser.GetValue<Int32>(dataReader, "userId"),
                        Email = SqlQueryResultParser.GetValue<String>(dataReader, "email"),
                        IsActive = SqlQueryResultParser.GetValue<Boolean>(dataReader, "IsActive"),
                        Role = (Role)SqlQueryResultParser.GetValue<Int32>(dataReader, "UserTypeId")
                    });
                }
                return null;


            });


        }
        public async Task<List<User>> GetUserList()
        {
            SqlQuery proc = new SqlQuery(@" 
                select userId, userTypeId, isActive, email from users;
                ", 30, System.Data.CommandType.Text);
            return await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {
                List<User> users = new List<User>();
                while (dataReader.Read())
                {
                    users.Add(new User()
                    {
                        UserId = SqlQueryResultParser.GetValue<Int32>(dataReader, "userId"),
                        Role = (Role)SqlQueryResultParser.GetValue<Int32>(dataReader, "userTypeId"),
                        IsActive = SqlQueryResultParser.GetValue<Boolean>(dataReader, "isActive"),
                        Email = SqlQueryResultParser.GetValue<String>(dataReader, "email")
                    });
                }
                return users;
            });
        }



    }
}