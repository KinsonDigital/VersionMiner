[<-- Table Of Contents](docs.md)

# Action Class Usage

Since the ```action``` class is just a wrapper over ```@actions/core```, just simply call the same methods as you would if you were using the ```@actions/core``` functions.

Currently implemented functions:

1. Use ```action.getInput()``` to replace ```core.getInput()```
2. Use ```action.setOutput()``` to replace ```core.setOutput()```
3. Use ```action.info()``` to replace ```core.info()```
4. Use ```action.error()``` to replace ```core.error()```
5. Use ```action.warning()``` to replace ```core.warning()```

**Example Environment File:**
``` json
{
    "environment": "",
    "repo-owner": "",
	"repo-name": "",
	"relative-file-path": "",
	"user-name": "",
	"password": ""
}
```

Below are some situations that will treat the action as if it is in a production environment.

* The **env.json** file does not exist.
* The **environment** key does not exist.
* The **environment** key is not a string type
* The **environment** key is empty, null, or undefined

The following **environment** values are acceptable for a development environment.
* dev
* develop

The following **environment** values are acceptable for a production environment.
* prod
* production
* ""
* null
* undefined