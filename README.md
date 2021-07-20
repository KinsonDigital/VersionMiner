
## <span style="color:blue">Environment Variables</span>

The environment variables is what is used to not only signify what the action is in a **development** or **production** environment, but also to hold values for the action inputs.  These values are purely for testing purposes.  Using this system prevents the developer from having to constantly deploy the action code changes and then have to test the action using another project.  This development loop is slow, so this helps save development and testing time.

```
{
    "environment": "",
    "repoOwner": "",
	"repoName": "",
	"relativeFilePath": "",
	"userName": "",
	"password": ""
}
```

## TypeScript Linting

The typescript linting that is setup for this project is very explicit.  The reason for this is to keep the code base clean and to avoid any pitfalls of the language.  If any unique situations come up that violate the linting rules, they can be disabled/enabled with the comments below.  

Disables a rule for a file.
```js
/* eslint-disable <typescript-rule-name> */
```

Enables a rule for a file.
```js
/* eslint-enable <typescript-rule-name> */
```

If code is **between** an **enable** and **disable** comment, the linting rule will be ignored for any code in-between the comments.

**ESLinting References:**
1. https://eslint.org/docs/rules/
2. https://github.com/typescript-eslint/typescript-eslint/tree/master/packages/eslint-plugin
