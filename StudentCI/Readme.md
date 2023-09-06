1 - Build docker 

D:\Temp\LocalProjects\PipeLineDotnet\StudentCI> docker build -f StudentCI/Dockerfile  -t dockerimagestudentci .

2 - run the docker file

D:\Temp\LocalProjects\PipeLineDotnet\StudentCI\StudentCI> docker run --rm -d --network postgres-net --name studentci-container -p 5001:80 dockerimagestudentci

3 - stop the container

PS D:\Temp\LocalProjects\PipeLineDotnet\StudentCI> docker stop studentci-container


4 - Before running test cases -
change - 
_appContainer = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("image_name")
