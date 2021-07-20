[<-- Table Of Contents](docs.md)

# Environment Variables

The environment variables is what is used to not only signify what the action is in a **development** or **production** environment, but also to hold values for the action inputs.  These values are purely for testing purposes.  Using this system prevents the developer from having to constantly deploy the action code changes and then have to test the action using another project.  This development loop is slow, so this helps save development and testing time.

The environment variables for testing must reside in a file with the name **env.json** and follow the simple key/value pair setup like below.  This file is ignored by GIT so it does not get checked into the repository.  This is to prevent any input values such as tokens being added to the repo.

What does t mean to be in a **development** vs a **production** environment?  Code that accesses the [@actions/core](https://github.com/actions/toolkit/tree/main/packages/core) cannot be used in a regular local environment.  Code that gets input, sets output, and other GitHub action specific code only work in the GitHub workflow environment.  The ```action``` class in combination of the ```environment``` class wrap these operations to allow the ability to operate differently depending on which **environment** the action is running in.

If the action is in the **development** environment, (meaning the **environment** value in the **env.json** has the value of **dev** or **develop**), then the ```action``` class will return the value from the **env.json** file.  If it is in **production**, then it will get the actual input of the action described in the **action.yml** file.

Since the **env.json** file is not committed to the repository and is not included as part of the **publish** process, by default the action is simply considered as **production**.  No special environment setup is required for the action to run properly as a published usable action.

**Examples of action code that only works in workflow environment:**
 ``` js
 core.getInput();
 core.setOutput();
 core.info();
 core.error();
 core.warning();
 ```