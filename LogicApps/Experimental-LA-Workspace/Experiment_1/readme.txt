dotnet clean 
dotnet restore
dotnet build
dotnet run

C:\Git\Experimental\LogicApps\Experimental-LA-Workspace\Experiment_1\messagesent> dotnet run program.cs

dotnet run --project .\MessageSent -- test-queue --file .\payload-test.xml --subject "YourSubject" --message-id "your-id"