# ASCII-Server

The project's goal is to develop a web application for accepting images, processing them into their text (only ascii supported right now) represention and saving it with the image and user data

Status: *Under development*

## Using

Main api uses static Shared/Config/CommunicationUrls class to define urls for communication. When changing listening ports of microservices or adding new ones, this is the class to add changes to. Its set up is described later in this file

- ### Defaults

1. Main api uses ports 5000 for http and 7000 for https
2. Processing api uses port 5001 for http
3. Storage api uses port 5002 for http
4. Maximum allowed image size is 100 megabytes

- ### shared

1. Rename ConfigExample folder to Config
2. Change all extensions in ConfigExample/ to .cs
3. SupportedTypes class lists types supportded by ImageSharp library used for processing image data as of version 3.0.1

- ### main-api

1. (optionally) configure listening ports in appsettings.json

- ### processing

1. (optionally) configure listening port in appsettings.json
2. When defining new processors, inherit from PerRowProcessor to process every second row of an image (for monospace fonts with 1/2 aspect ratio) or from Processor for a different implementation  

- ### storage

1. (optionally) configure listening port in appsettings.json

- ### Testing

Testing.html file located in project's base directory can be used for viewing the results
A .json file in repository's root directory is provided with postman requests collection, it can be imported to postman and used for testing
