# PipeLineDotnet

**Project to show:**

1 - How to use Dockerfile to publish an app, how to integrate Postgres database with for the data source of the applition, how we can pull database in docker and run it inside a container so that our API app can connect to it. 

2 - How workflow file works after "push" in the "main" branch, we have configured it in the workflow (main) file

We can run the project in local with a docker Installed in machine, This project will run a StudentAPI and will test its 2 endpoints, get and post

To setup docker/database/network etc, we have to follow this article - https://docs.docker.com/language/dotnet/

**Steps to run**

Open project in Visual Studio 2022 via .sln file

Below are the ways to run it. 

All commands should be executed from the **PipeLineDotnet** or the root folder, if any path mismatches then we can try another path like ./project_folder/file_name

To run the API we need 3 things, Postgres DB, a network , and image to run.

  Create network :

        docker network create postgres-net

As the network(postgres-net) is already created. Below are 2 things to run 1 by 1 to make the app running.

1 run PostgreSQL in a container and attach to the volume and network we created above.

    docker run --rm -d -v postgres-data:/var/lib/postgresql/data --network postgres-net  --name db -e POSTGRES_USER=postgres -e  POSTGRES_PASSWORD=example postgres

2 let’s run our container(name =dotnet-app) on the same network as the database. This allows us to access the database by its container name.

    docker run --rm -d --network postgres-net --name dotnet-app -p 5001:80 clockbox

**Another way to run the app is: Via docker-compose**

We can run all 3 at once. 

=> .Net app, database & adminer

        PS D:\Temp\LocalProjects\PipeLineDotnet> docker compose up --build

    
To get a UI of Postgres DB we can run adminer. To Connect Adminer and populate the database :
Username = postgres, Password = example

    docker run --rm -d --network postgres-net --name db-admin -p 8080:8080 adminer

To test our app, we will need to add some data in the postgres database, a Student table like in our model with 4 attributes, add atleast 2 students data there, then can run our app.

4 - stop containers
    docker stop db-admin dotnet-app db 


For setting up test

    1- D:\Temp\LocalProjects\DockerTest1 dotnet new xunit -n myWebApp.Tests -o tests
    2 - Add command in D:\Temp\LocalProjects\DockerTest1\DockerTest1\tests folder : dotnet add package Testcontainers --version 2.3.0

To test :

Before running the test, make sure we have data in our postgres container.

    PS D:\Temp\LocalProjects\PipeLineDotnet> dotnet test tests

Locally On test we should get such output of testing 3 method. But on github workflow we can test 1 method of API and the postgres data is not available yet there. After adding data to the postgres on server we can run all 3 test cases there as well.

Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     4, Duration: 56 s - myWebApp.Tests.dll (net7.0)


  We can also run the image( name = clockbox ) like this - 

        PS D:\Temp\LocalProjects\DockerTest1> docker build -f DockerTest1/Dockerfile  -t clockbox .


**Github workflow Lerning source -** 
   https://www.youtube.com/watch?v=0lbDMomNt4A 

  https://www.youtube.com/watch?v=PZXzecYL0c8&t=32s

**Test-reports can be found from the Action > Latest run file > open the run > check the test Report**

 
