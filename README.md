# dp-api-dotnet-core-apikey-filter
This is adding an Attribute for an ApiKey to be used for certain API calls using MVC filters as a middleware.

Note there are notes on the rest of the API setup at https://github.com/danparker276/dp-api-dotnet-core-jwt-users the other project
This just shows the case where your users API access so they don't have to log in every time. They'll get as many API keys as you want.

The API key management APIs are not in place here, just the database table. This just deals with the Auth Attribute for putting an API key in the header x-api-key and passing a user object back to the controler from the middleware.

This is just a quick check-in
