# Panda Bank

...

visual studio 2019
.Net Core 2.2
Microsoft SQL Server Management Studio 18

### How to run

by visual studio 2019

1. Check file [appsettings.json] PandaBankDbConnection set connectionstring of your.
In 2 Project [PandaBank.Account] and [PandaBank.User]

2. Change Start Up Project to User
3. Change Default Project  too  User.DAL And type [ update-database ] in  Package Manager Console  window

```
update-database
```

4. And repeat with Account project

```
update-database
```

5. And set it to run multiple project

- PandaBank.Server
- PandaBank.User
- PandaBank.Account



### Running the tests

1. In root folder have [PandaBank.postman_collection.json] You can import it to Postman to run test.
2. Test by xUnitTest [PandaBank.UnitTest] 


 
