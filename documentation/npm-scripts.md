[<-- Table Of Contents](docs.md)

# Project NPM Scripts

The following npm scripts are available for the development and publishing process of the GitHub action.

1. *```lint```* - This will manually run the eslinter and return the results in the console.
   * This is useful when the linting for some reason might not make sense or stop linting when you know that it should be working.  Running this will show errors with the linting configuration to help troubleshoot issues.
2. *```clean:bin```* - Cleans/deletes the **bin** folder.  This folder is the transpiled typescript output when running the **build**, or **rebuild** npm script.  This is what is executed when debugging locally when pressing **F5** in **VSCode**.
3. *```clean:dist```* - Cleans/deletes the **dist** folder. This folder is the **published** version of the GitHub action and is where the GitHub action is actually ran from when being used.
4. *```clean:modules```* - Cleans/deletes the **node_modules** folder.  Sometimes this is required to make sure that things are refreshed properly when having strange issues.
5. *```clean:project```* - Cleans/deletes the **bin** and **node_modules** folder in a single step.
6. *```build```* - Builds the project for testing locally on the machine.
7. *```rebuild```* - Cleans and builds the project in a single step.
8. *```run:app```* - Runs the application for debugging as well as viewing console output.
9. *```publish:app```* - This packages the entire application into a single **index.js** file in the **dist** folder.  This is where the action is run from when other projects utilize the action and must be committed to the repository.