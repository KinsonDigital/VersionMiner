[<-- Table Of Contents](docs.md)

# Project TypeScript Linting

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