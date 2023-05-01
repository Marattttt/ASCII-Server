# ASCII-Server

The project's goal is to develop a web application for accepting images, processing them into their ASCII represention, optionally saving it, and giving the text back to user

Status: *Under development*

## Using

Note: main api uses static CommunicationUrls class to define urls for communication. When changing listening ports of microservices or adding new ones, this is the class to add changes to. Its set up is described later in this part

- ### Defaults

1. Main api uses ports 5000 for http and 7000 for https
2. Processing api uses port 5001 for http
3. Maximum allowed image size is 5 megabytes

- ### main-api

1. Rename ConfigExample folder to Config
2. Change Config/FileConfig.txt extension to .cs
3. Edit Config/FileConfig.cs to match your settings
4. Change Config/CommunicationUrls.txt to .cs and make sure it is up to date with other microservices
5. Edit MaxFileSizeBytes constant in ImagesService.cs to match your needs
6. (optionally) configure listening ports in appsettings.json

- ### processing

1. (optionally) configure listening port in appsettings.json

- ### Testing

Testing.html file located in project's base directory can be used for testing and viewing the results
