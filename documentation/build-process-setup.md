[<-- Table Of Contents](docs.md)

# Build Process & Setup

**Warning - Change the build scripts at your own risk**

The build process is more of a custom setup. The purpose of this is to smooth out and improve the build and publish process with consistent console output as well as better problem reporting.  This also gives more control over the build process itself.  Below are explanations of each build script in the **build-process** folder.

1. Why is the extension of the build process scripts **.mjs** instead of **.js**?
   * This has to do with the conflict between the **nodemon** package and use of **ES6** module syntax being used in the scripts.  When executing javascript files using node and using **ES6** module syntax, the **package.json** file must contain the value **type** key with value below
        ``` json
        {
            "name": "vget",
            "version": "1.0.0",
            "description": "vget",
            "type": "module"
        }
        ```
    This tells node that the javascript file is using the **ES6** module syntax.  The problem is that the **nodemon** package is utilizing the **tsconfig.json** file must be setup to use the **commonjs** format.  If you were to use the file extension of **.js** for the scripts, you have to set the **package.json** to use **module** as the value of the **type** key.  Doing this then breaks **nodemon**.  To prevent this, node allows you to use the extension **.mjs** as a file extension instead which tells node that the file is using the **ES6** module syntax.  This means that the **package.json** file does not have to contain the **type** key/value pair!!  This then allows the use of **nodemon** and the build scripts without conflict.
2. **Clean (clean.mjs)**
   * This script cleans (deletes) certain directories to clean up the environment for running, publishing and testing.  This is same idea as cleaning your solution in **Visual Studio**.
   * This script takes 3 available commands below:
     * *```dist```* - Cleans/deletes the **dist** folder
     * *```bin```* - Cleans/deletes the **bin** folder
     * *```modules```* - Cleans/deletes the **node_modules** folder
3. **Build (build-dev.mjs)**
   * This script builds the application for local testing in a graceful manner.  This is done by first checking if the **node_modules** folder exists.  If it does not then it pulls down the packages first then builds(transpiles) the typescript code to executable javascript to a **bin** folder.  Hitting the F5 key in **VSCode** will then execute this code for testing purposes.  This process builds debug maps so breakpoints can be used in **VSCode** while debugging.
4. **Publish (publish.mjs)**
   * This script properly prepares and builds the GitHub action for use. When executed, it will preform a clean by deleting the **dist** and **node_modules**, pulling down the packages, and then building the GitHub action by transpiling all of the typescript code into a single **index.js** file and pushing it into the **dist** folder.  This is the actual code that is executed when the github action is ran.