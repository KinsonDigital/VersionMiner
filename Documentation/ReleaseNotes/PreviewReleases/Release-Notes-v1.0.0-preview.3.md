<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    VersionMiner Preview Release Notes - v1.0.0-preview.3
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#83](https://github.com/KinsonDigital/VersionMiner/issues/83) - Fix a bug where the GitHub console output was incorrect when an invalid branch was detected.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#94](https://github.com/KinsonDigital/VersionMiner/issues/94) - Improved and updated how repository data is pulled.
   >💡This data pulled is the file where the versions are contained.
2. [#93](https://github.com/KinsonDigital/VersionMiner/issues/93) - Changed how workflow step outputs are dealt with.
   >💡This is a required change related to workflow changes that GitHub is enforcing.  Want more info? Go [here](https://github.blog/changelog/2022-10-11-github-actions-deprecating-save-state-and-set-output-commands/).
3. [#89](https://github.com/KinsonDigital/VersionMiner/issues/89) - Set up a [dependabot](https://github.com/dependabot) for managing dependencies.
4. [#88](https://github.com/KinsonDigital/VersionMiner/issues/88) - Updated the workflows for new workflow job step outputs.
   >💡This is a required change related to workflow changes that GitHub is enforcing.
5. [#86](https://github.com/KinsonDigital/VersionMiner/issues/86) - Updated dotnet from version _**v6.0**_ to _**v7.0**_
6. [#82](https://github.com/KinsonDigital/VersionMiner/issues/82) - Changed how internal visibility is exposed to the unit test project.
7. [#75](https://github.com/KinsonDigital/VersionMiner/issues/75) - Replaced a custom-written library used to get GitHub data with [Octokit](https://github.com/octokit).
8. [#73](https://github.com/KinsonDigital/VersionMiner/issues/73) - Removed PR and issue templates.
   >💡This enables the project to use the [templates](https://github.com/KinsonDigital/.github/tree/feature/24-create-org-docs/.github) from the entire organization.
9. [#44](https://github.com/KinsonDigital/VersionMiner/issues/44) - Added automated code coverage to the CI/CD process by using [codecov](https://about.codecov.io/).
10. [#9](https://github.com/KinsonDigital/VersionMiner/issues/9) - Greatly improved the CI/CD release pipeline.
