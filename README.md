# FetchRewards_API
Fetch Rewards WebAPI Assignment

# Notes 

To Run API:  
  >From Solution Directory:  
    docker build -f ./Dockerfile -t api .  
    docker run -d -p hostMachinePort:8080 --name api_container api  
    where hostMachinePort is desired http port on host machine 

To Run API with Integration Tests:  
  >From Solution Directory:  
    docker-compose -f ./docker-compose-integration-tests.yml build 
    docker-compose -f ./docker-compose-integration-tests.yml up

  >Please Note:  
    docker-compose file will bind target container port to host machine port 8080 by default.  
    If this value is changed, integration tests will fail unless value in  
    ./API.IntegrationTest.Console/appsettings.integration.test.json["EndpointSettings:Port"]  
    is updated accordingly.

API.Test includes unit tests of components
