# Dev Challenge 2023

## Prerequisites

Run 
```dotnet restore```

OR - (add manually)

- Microsoft.NET.Test.Sdk
```dotnet add package Microsoft.NET.Test.Sdk```
- MOQ
```dotnet add package Moq```
- NUnit
```dotnet add package NUnit```
- NUnit3TestAdapter
```dotnet add package NUnit3TestAdapter```

## Getting Started

1) Go to `secrets.json` (Right click on the project -> Manage User Secrets)
2) Add the following:
```
{
    "DevChallengeApiSettings:Email": "youremail@mail.com",
    "DevChallengeApiSettings:Token": ""
}
```

## Test

Run
```dotnet test```

* Note - the first test will trigger actual api calls. The second test uses a mock to simulate the calls.

OR - run manually from your IDE

## API

The Dev Challenge API provided by Win Systems is used as an external data source. The API offers various endpoints for retrieving blocks data and performing checks on the blocks.
