# dp-api-dotnet-core-apikey-filter
TODO - I will add more docs here. But I have just updated to use asp.net core 6 and creates the custom JWT token in a different way. Much of the code is from https://jasonwatmore.com/post/2022/02/18/net-6-role-based-authorization-tutorial-with-example-api He also has some very good UI examples to use with this.

This is a REST API that I use as a starter template. I have a ADO.NET wrapper for my data layer. It's a very light and efficient way to access the DB, as I use Cloud Functions a lot and don't want a heavy ORM. I'll probably make another version with Dapper soon.

This uses 2 roles User and Admin with the REST API. A Bearer Token is used set to expire in 7 days.

The API key management APIs are not in place here, just the database table. This just deals with the Auth Attribute for putting an API key in the header x-api-key and passing a user object back to the controler from the middleware. This is for clients that want to use API commands in a typical B2V app

This is just a quick check-in - a lot more comments and documenation/testing could be added.
