# start application
    cd API
    dotnet watch

    cd client
    ng serve

# see db
    >sqlite open database

# update db schema:
    dotnet ef migrations add {message}
    dotnet ef update