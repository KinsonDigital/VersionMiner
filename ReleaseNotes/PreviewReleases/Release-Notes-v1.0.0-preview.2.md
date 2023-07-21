<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Version Miner Preview Release Notes - v1.0.0-preview.2
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#17](https://github.com/KinsonDigital/VersionMiner/issues/17) - Added create issue link to action console output.
    - Clicking this link will take you directly to the **create issue** page for the project.
2. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Added a new input named `trim-start-from-branch`.
    - This new feature will trim the value of the `trim-start-from-branch` input from the name of the `branch-name` input value.  This is useful to trim extra textual artifacts from the branch added by using **GitHub** default environment variables.
3. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Changed the `file-format` input to not use case sensitive values.
    - This means the value `xml` and `XML` are the same.  Previously it was case sensitive and a value like `XML` would of failed the workflow.
4. [#49](https://github.com/KinsonDigital/VersionMiner/issues/49) - Replace built in `GitHubDataService` related code with the new library [GitHubData](https://github.com/KinsonDigital/GitHubData).
5. [#48](https://github.com/KinsonDigital/VersionMiner/issues/48) - Added new action input named `repo-token`.
    - This adds the ability for **VersionMiner** to be able to make authenticated requests so the request rate limits are not reached when the action is executed.  Refer to the [GitHub API Authentication](https://docs.github.com/en/rest/guides/getting-started-with-the-rest-api#authentication) documentation for more information on request rate limits.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Refactored the name of the `InvalidFileTypeException` exception to `InvalidFileFormatException`.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Update nuget package **xunit.runner.visualstudio** from **v2.4.3** to **v2.4.5** in the unit testing project.
2. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Update nuget package **Microsoft.NET.Test.Sdk** from **v17.1.0** to **v17.2.0** in the unit testing project.
3. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Update nuget package **CommandLineParser** from **v2.8.0** to **v2.9.1** in the main project.
4. [#48](https://github.com/KinsonDigital/VersionMiner/issues/48) - Update nuget package **KinsonDigital.GitHubData** from **v1.0.0-preview.4** to **v1.0.0-preview.5**.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#23](https://github.com/KinsonDigital/VersionMiner/issues/23) - Refactored the word "ubuntu" to the value of "Ubuntu".
    - This was just a simple documentation improvement.
2. [#25](https://github.com/KinsonDigital/VersionMiner/issues/25) - Added badges to the project's README file.
    - Added a latest releases badge.
    - Added a unit test status badge.
    - Added a discord server badge.
    - Added a twitter badge.
3. [#26](https://github.com/KinsonDigital/VersionMiner/issues/26) - Created a project item issue template for creating project items for project maintainers.
4. [#27](https://github.com/KinsonDigital/VersionMiner/issues/27) - Created a build and unit testing status check workflows to be executed for pull requests.
5. [#28](https://github.com/KinsonDigital/VersionMiner/issues/28) - Changed the GitHub action marketplace icon from a settings icon to a search icon.
6. [#29](https://github.com/KinsonDigital/VersionMiner/issues/29) - Setup the solution to hold release notes.
    - This will help keep a history of the release notes as well as help publish release notes with the releases.
7. [#31](https://github.com/KinsonDigital/VersionMiner/issues/31) - Setup project to display the **KinsonDigital** contributions button.
8. [#32](https://github.com/KinsonDigital/VersionMiner/issues/32) - Fixed a documentation issue in the project README file where the YAML examples were using the incorrect inputs.
9. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Improved **README** file in various areas.
10. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Added documentation for the new `trim-start-from-branch` input in the **README** file with an example.
11. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Refactor exception messages to show the input names that are in seen in the workflow when being used.
12. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Added 3 live templates for **JetBrains Rider** users to easily create unit test methods.  Live template names below:
    - xu-null-ctor-param
    - xu-create-test-obj-method
    - xu-test-method
13. [#45](https://github.com/KinsonDigital/VersionMiner/issues/45) - Added an integration tests project to the solution named **VersionMinerIntegrationTests**.
    - ⚠️Currently this test project make HTTP requests to a test repository that lives in the KinsonDigital organization named [ActionTestRepo](https://github.com/KinsonDigital/ActionTestRepo).  This repository is for the sole purpose of testing out various organizational GitHub actions.  These integration tests do not authenticate with login credentials or a PAT, which results in a request/hour limit of 60.  This is currently being worked on and will be available on a future preview release.  Refer to [Issue #47](https://github.com/KinsonDigital/VersionMiner/issues/47) for more info.
14. [#55](https://github.com/KinsonDigital/VersionMiner/issues/55) - Updated and added issue and PR templates to project.
15. [#53](https://github.com/KinsonDigital/VersionMiner/issues/53) - Added code of conduct to project.
16. [#58](https://github.com/KinsonDigital/VersionMiner/issues/58) - Added branching documentation to the project.
17. [#61](https://github.com/KinsonDigital/VersionMiner/issues/61) - Fix issue with release todo issue template.
    - This involved removing illegal characters from the template that was causing it to not function properly when create a new GitHub issue.
