# PipeLineDotnet

**Project demonstrates:**

1 - How to use Dockerfile to publish an app, how to integrate Postgres database for the data source of the applition, how we can pull database in docker and run it inside a container so that our API app can connect to it. 

2 - How to configure CI/CD for the application, how workflow file works after "push" in the "main" branch, we have configured it  in the workflow (postgresrun.yml) file.

**What happens when github workflow runs:**

- It creates a network

- It creates the PostgresData

- Our code creates checks of DB is present, if not, we create it

- Then we add 2 tables in our DB

- Then we run our application

- Then we run our test case based on that DB. This project will run a ProductAPI, BirdAPI and will test its 2 endpoints, get and post.

Locally, We can run the project in local with a docker Installed in machine, 

To setup docker/database/network etc, we have to follow this article - https://docs.docker.com/language/dotnet/

--------------------------------------------------------------------------------------------------------------------------------------------------------------------

     
**Steps to run**

Open project in Visual Studio 2022 via .sln file

Below are the ways to run it. 

All commands should be executed from the **PipeLineDotnet** or the root folder, if any path mismatches then we can try another path like ./project_folder/file_name

To run the API we need 3 things, Postgres DB, a network , and image to run.

  Create network :

    docker network create postgres-net

After the network(postgres-net) is already created. Below are 2 things to run 1 by 1 to make the app running.

0-   We create the image( name = clockbox ) like this, note that you - 

    docker build -f Dockerfile  -t clockbox .


1- **Remember to run PostgreSQL before running the API app** in a container and attach to the volume and network we created above.

    docker run --rm -d -v postgres-data:/var/lib/postgresql/data --network postgres-net  --name db -e POSTGRES_USER=postgres -e  POSTGRES_PASSWORD=example postgres

2- let’s run our container(name =dotnet-app) on the same network as the database. This allows us to access the database by its container name.

    docker run --rm -d --network postgres-net --name dotnet-app -p 5001:80 clockbox

**To test** : We can hit url of Student API and Bird API using postman. Then we can add some data via /Add post call.

url (POST)- http://localhost:5001/product/Add

data - 
  { "Id":"23", "name": "Computer", "category": "IT", "price": 9.99}

--------------------------------------------------------------------------------------------------------------------------------------------------------------------


To get a UI of Postgres DB we can run adminer. To Connect Adminer and populate the database :
Username = postgres, Password = example

    docker run --rm -d --network postgres-net --name db-admin -p 8080:8080 adminer
    
--------------------------------------------------------------------------------------------------------------------------------------------------------------------

**Another way to run the app is: Via docker-compose**

We can run all 3 at once. 

=> .Net app, database & adminer

        PS D:\Temp\LocalProjects\PipeLineDotnet> docker compose up --build


--------------------------------------------------------------------------------------------------------------------------------------------------------------------

To test our app via test cases, we will need to have a DB. In our test case we are creating student data and getting it.

For setting up test cases, following the same article.

    1- D:\Temp\LocalProjects\DockerTest1 dotnet new xunit -n myWebApp.Tests -o tests
    2 - Add command in D:\Temp\LocalProjects\DockerTest1\DockerTest1\tests folder : dotnet add package Testcontainers --version 2.3.0


To run test cases

Before running the test, make sure we have data in our postgres container.

    PS D:\Temp\LocalProjects\PipeLineDotnet> dotnet test tests

Locally on your machine after setting up everything test we should get such output of testing 4 method. On github workflow also, we can test all methods of API and the postgres data is not available yet there. 


Note: For test cases to run on server, we are calling get and post calls, to pass a Get test case we should have the data present in the database there, we are seeding the on project startup.

Passed!  - Failed:     0, Passed:     4, Skipped:     0, Total:     4, Duration: 56 s - myWebApp.Tests.dll (net7.0)

![image](https://github.com/qwertycod/PipeLineDotnet/assets/112320985/9b9732ff-d714-46d9-b37a-85a4ddb33d44)


--------------------------------------------------------------------------------------------------------------------------------------------------------------------

**To connect the API app with postgres without hosting the app on docker hub**, means API app is running locally and postgres DB running on docker we can make configuration change in program file connection string like this:

**Source -** https://www.code4it.dev/blog/postgres-crud-operations-npgsql/

1 - stop the already running postgres DB on docker. 
2 - Run a new postres on our another port like:

      docker run --name myPostgresDb -p 5455:5432 -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=example -e POSTGRES_DB=my_db -d postgres
3-  connection string change -

    CONNECTION_STRING = "Host=localhost:5455;" + "Username=postgres;" + "Password=example;" + "Database=my_db";

4 - Run the API app via Run or F5, use the url thats created on running in postman and test the APP.

--------------------------------------------------------------------------------------------------------------------------------------------------------------------

**Github workflow Lerning source -** 
   https://www.youtube.com/watch?v=0lbDMomNt4A 

  https://www.youtube.com/watch?v=PZXzecYL0c8&t=32s

--------------------------------------------------------------------------------------------------------------------------------------------------------------------


**Test-reports on Github can be found from the Action > Latest run file > open the run > check the test Report**

 
