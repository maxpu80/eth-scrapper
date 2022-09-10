# Actor state bug when used inside docker-compose

https://github.com/dapr/dapr/issues/4891

Steps to reproduce

+ `dapr init` if still not
+ `docker-compose up`
+ `curl http://localhost:3500/v1.0/actors/scrapper-dispatcher/v5/method/State` or use `dotnet\Eth\ScrapperDispatcherActor\test.http` file

"errorCode":"ERR_ACTOR_RUNTIME_NOT_FOUND"


At the same time it works when service started using dapr cli see how to start `dotnet\Eth\ScrapperDispatcherActor\readme.md` run to check `curl http://localhost:3500/v1.0/actors/scrapper-dispatcher/v5/method/State`