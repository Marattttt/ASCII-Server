# ASCII-Server

The project's goal is to develop a web application for accepting images, processing them into their ASCII represention, optionally saving it, and giving the text back to user

Status: *Under development*

## Using

- ### Defaults

1. Main api uses ports 5000 for http and 7000 for https
2. Processing api uses port 5001 for http

#### *Both are specified in appsettings.json of each project, but the processing address is hardcoded into CommunicationService of main api, which needs changing*

- ### Testing

Testing.html file located in project's base directory can be used for testing and outputting the results

- ### main-api

1. Rename ConfigExample folder to Config
2. Change Config/FileConfig.txt extension to .cs
3. Edit Config/FileSettings.json to fit your needs
