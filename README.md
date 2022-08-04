# RapidPayAPI (Challenge)
## Goal

RapidPay as a payment provider needs YOU to develop its new Authorization system and is willing to
pay accordingly!
You’ll need to develop a RESTful API that uses basic authentication.
The API will be written in C#, the data can be stored in the memory or in a database. The API will include
two modules:
- Card management module
- Payment Fees module
- Bonus

## Card management module Features 
The card management module includes three API endpoints:
- Create card (card format is 15 digits)
- Pay (using the created card, and update balance)
- Get card balance

## Payment Fees Module Features
- Every hour, the Universal Fees Exchange (UFE) randomly selects a decimal between 0 and 2.
- The new fee price is the last fee amount multiplied by the recent random decimal.
- You should develop a Singleton to simulate the UFE service and the fee should be applied to every payment

## Bonus Features

- Improve your API performance and throughput using multithreading.
- Generally, using basic authentication is not a good solution. Improve the authentication so we
can make our Authorization system secure.
- Make the shared resources thread safe using a design pattern in case you are storing the data in
the memory. In case you are using a database to persist the cards and transaction improve the
database design and the usage of the ORM framework.


## Installation

RapidPayAPI works with 2 different environments and data stores. 


For development environment(SQL server In Memory)...

```sh
dotnet run
```



# For production environment(SQL Server Database):
- First you need to change the following code:

 ```csharp
 if(builder.Environment.IsProduction())
 {
    Console.WriteLine("-->using  Sql Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("RapidPayConn")))
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
 }
 else
 {
    Console.WriteLine("-->using in Mem Db" );
     builder.Services.AddDbContext<AppDbContext>(
                 opt=>opt.UseInMemoryDatabase("InMem"))
                  .AddIdentityCore<User>()
                  .AddRoles<IdentityRole>()
                 .AddEntityFrameworkStores<AppDbContext>();
 }
```
to :
 ```csharp
    Console.WriteLine("-->using  Sql Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("RapidPayConn")))
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
```
- Comment the following lines:
 ```csharp
//Seed BankAccountData
// PrepDb.PrepPopulation(app, app.Environment.IsProduction());
// //Seed Identity Data In Production Environment
// await SeedIdentityData();
```
- In order to setup your production database, you can use dotnet ef cli for add a migration and update your database.

```sh
dotnet ef migrations add InitialMigration
dotnet ef database update
```

- Revert the code changes and Finally 
```sh
dotnet run --launch-profile "RapidPayAPI-Production"
```
## Authentication Endpoint

| Verb | URI | Operation | Description |
| ------ | ------ | ------ | ------ |
| POST | /api/Authentication/login | Authentication and Create | Create a user token |

## RapidPay Endpoints
| Verb | URI | Operation | Description |
| ------ | ------ | ------ | ------ |
| GET | /api/RapidPay/GetAllBankAccounts | Read | Read all bank accounts |
| GET | /api/RapidPay/GetAllBanks | Read | Read all banks |
| GET | /api/RapidPay/GetBankAccountById/{bankaccountid} | Read | Read a single bank account(by id) |
| GET | /api/RapidPay/GetBalance/{bankaccountid} | Read | Read the balance of a single bank account(by id) |
| POST | /api/RapidPay/CreateBankAccount | Create | Create a new bank account |
| PUT | /api/RapidPay/Pay | Update | Update the balance of a single bank account(by card number,payment type and amount ) |





