# Author

Jonathan Chavez

# .NET Challenge for Jobsity

This is a simple browser-based chat using .NET that allow users to talk in real time and, at the same time, to get stock quotes from a financial service using commands.

# Implemented Requirements

* New users can create their own accounts.
* Existing users can log in.
* Users can chat with any logged user in real time.
* There is a bot who is listening to certain commands like: /help /stock=code
* When /stock command is invoked by any user, the bot will get a quote from the specified stock code.
* /stock=code commands are not saved in db.
* When a user joins the chat, last 50 messages are retrieved and the bot automatically say hi.

# Development Plataform and components

* .NET Core 3.1
* PostgreSQL 12.2 (with Entity Framework Core)
* SignalR (as a message broker for the chat hub)
* MSTest (TDD approach was used to implement the stock service and the bot functionality)
* Heroku as PaaS (I ran out of AWS free tier credits so I used Heroku's free PaaS: 1 Web Dyno + PostgreSQL)
* Docker for containerization (Dockerfile)
* GitHub Actions for Continuous Deployment (.github/workflows/main.yml)

# Online DEMO

https://financialchatroom.herokuapp.com/

PD1: Due to some free tier limitations on Heroku, the *first call* can take up to 10 seconds since the dyno sleeps during inactivity time and needs to wake up.
PD2: Password strength has been reduced on purpose to facilitate account creation.

# Local testing

For security reasons, the database's connection string is saved into the user's secret store. So you need to execute the following command to store it:

dotnet user-secrets set "DATABASE_URL" "postgres://user:password@server:5432/dbname"
