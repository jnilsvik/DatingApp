# start application
    dotnet watch

# see db
    >sqlite open database

# update db schema:
    dotnet ef migrations add {message}
    dotnet ef update