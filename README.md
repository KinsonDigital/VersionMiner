<h1 align="center">

**🪨⛏️**

**VersionMiner**

</h1>

<div align="center">

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/kinsondigital/VersionMiner/build-status-check.yml?style=flat&label=%F0%9F%94%A7Build&color=2f8840)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/kinsondigital/VersionMiner/unit-test-status-check.yml?style=flat&label=%F0%9F%A7%AATests&color=2f8840)

[![Codecov](https://img.shields.io/codecov/c/github/KinsonDigital/VersionMiner?color=2F8840&label=Code%20Coverage&logo=codecov)](https://app.codecov.io/gh/KinsonDigital/VersionMiner/tree/release%2Fv1.0.0)

[![Good First GitHub Issues](https://img.shields.io/github/issues/kinsondigital/VersionMiner/good%20first%20issue?color=7057ff&label=Good%20First%20Issues)](https://github.com/KinsonDigital/VersionMiner/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22)
[![Discord](https://img.shields.io/discord/481597721199902720?color=%23575CCB&label=chat%20on%20discord&logo=discord&logoColor=white)](https://discord.gg/qewu6fNgv7)
</div>

<br>

<div align="center">

## **🤷🏼‍♂️ What is it? 🤷🏼‍♂️**
</div>


This **GitHub Action** makes it easy to pull versions from XML files.
It can be used in your workflows for other uses such as version validation, version tag management, and more!!

<details closed><summary><b>TLDR</b> - Additional Info</summary>

In a nutshell, _**VersionMiner**_ pulls versions out of XML data files for use in workflows.
Just tell the action which repo, branch, and file contains the version, and it will search through the file for the `version-keys` and pull out the value of that key. This value is used as the value of the action's output,
which has the name `version`, so you can use it in the rest of your workflow.
</details>

<details closed><summary><b>TLDR</b> - Use Cases</summary>

- Create tags automatically with the version, during the release process.
- Validate the version syntax to help enforce version syntax.
  - Example: Semantic version vs. a date-based version.
- Manage release note file names by having the version embedded in the file name.
- Use the version in the title of a GitHub release.
- Release announcements.
  - Example: Use the version in a release announcement on Twitter.
- Use status check workflows to verify versions before a pull request can be completed.
</details>

<br/>

>**Note** VersionMiner is built using C#/NET and runs in a docker container.  If the job step for running this action is contained in a job that runs on **Windows**, you will need to move the step to a job that runs on **Ubuntu**.  You can split up your jobs to fulfill the `runs-on` requirements of the GitHub action. This can be accomplished by moving the step into its job.  You can then route the action step outputs to the job outputs and use them throughout the rest of your workflow.  
For more information on step and job outputs, refer to the GitHub documentation links below:
>- [Defining outputs for jobs](https://docs.github.com/en/actions/using-jobs/defining-outputs-for-jobs)
>- [Setting a step action output parameter](https://docs.github.com/en/actions/using-workflows/workflow-commands-for-github-actions#setting-an-output-parameter)


<div id="output-example" align="center"><h2 style="font-weight:bold">🪧 Example 🪧</h2></div>

```yml
name: Get Version Example

jobs:
  get_version_job:
    runs-on: ubuntu-latest # Cannot use windows
    steps:
    - uses: actions/3

    - name: Get Version From C# Project File
      id: get-version
      uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
      with:
      repo-owner: JohnDoe
      repo-name: MyRepo
      repo-token: ${{ secrets.GITHUB_TOKEN }}
      branch-name: main
      file-format: xml # Not case sensitive
      file-path: "MyProject/MyProject.csproj"
      version-keys: Version

    - name: Print Version From File
      id: print-output
      run: echo "${{ steps.get-version.outputs.version }}"
```

If the XML file had the contents below, the workflow above would print the value ***1.2.3*** to the GitHub console.

```xml
<!--Quick Example - C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <LangVersion>11.0</LangVersion>
    <Version>1.2.3</Version> <!--The version that would be discovered-->
    <FileVersion>0.1.0</FileVersion>
  </PropertyGroup>
</Project>
```
---

<div align="center">

## **➡️ Action Inputs ⬅️**
</div>

| Input Name | Description | Required | Default Value |
|:---|:---|:---:|:---:|
| `repo-owner` | The owner of the repository. This is _**NOT**_ case-sensitive. | yes | N/A |
| `repo-name` | The name of the repository. This is _**NOT**_ case-sensitive. | yes | N/A |
| `repo-token` | The repository or PAT token to use for authorized requests. | yes | empty |
| `branch-name` | The name of the branch where the file lives. This _**IS**_ case sensitive. | yes | N/A |
| `file-format` | A non-case-sensitive value representing the data format of the file that contains the version. Currently, the only supported value is `xml` for a file format. | yes | N/A |
| `file-path` | The path to the file relative to the root of the repository. | yes | N/A |
| `version-keys` | A comma-delimited list of keys that hold the version value. Spaces around commas are ignored.  Values must be wrapped with single or double quotes to be processed properly if more than one key exists.  The search for keys will stop once the first occurrence of a key that contains a value is found.  | yes | N/A |
| `case-sensitive-keys` | If true, key searching will be case-sensitive. | no | `true` |
| `trim-start-from-branch` | Will trim the given value from the beginning of the `branch-name` input. | no | empty |
| `fail-on-key-value-mismatch` | If true, the action will fail, if all of the key values listed in the `version-keys` input do not match.  Other failure inputs will not affect this input. | no | `false` |
| `fail-when-version-not-found` | If true, the action will fail, if no version exists.   Other failure inputs will not affect this input. | no | `true` |


<div align="center">

## **⬅️ Action Output ➡️**
</div>

The action output is a single `string` value with the name _**version**_. Click <a href="#output-example">here</a> to see an example of how to use the output of the action.

---

<div align="center" style="font-weight:bold">

## **🪧 More Examples 🪧**
</div>

Searches for a version but does not fail the workflow if no version is found:

```yml
#Example 1 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        repo-token: ${{ secrets.GITHUB_TOKEN }}
        branch-name: main
        file-format: xml # Not case sensitive
        file-path: "${{ github.workspace }}/MyProject/MyProject.csproj"
        version-keys: Version
        fail-when-version-not-found: false
```
```xml
<!--Example 1 - C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <LangVersion>11.0</LangVersion>
    <Version></Version> <!--No value.  Does not fail workflow.-->
  </PropertyGroup>
</Project>
```

<div align="left">
Searches multiple keys for the version. The job fails if no version is found in the keys:

```yml
#Example 2 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        repo-token: ${{ secrets.GITHUB_TOKEN }}
        branch-name: main
        file-format: xml # Not case sensitive
        file-path: "MyProject/MyProject.csproj"
        version-keys: "Version,FileVersion"
```
```xml
<!--Example 2 - C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <LangVersion>11.0</LangVersion>
    <Version></Version> <!--No value. Search continues for the FileVersion key-->
    <FileVersion>4.5.6</FileVersion> <!--Key with a value exists so this value is returned-->
  </PropertyGroup>
</Project>
```

<div align="left">
Searches for a key without using case-sensitivity:

```yml
#Example 3 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        repo-token: ${{ secrets.GITHUB_TOKEN }}
        branch-name: main
        file-format: xml # Not case sensitive
        file-path: "MyProject/MyProject.csproj"
        version-keys: VeRSion # Different casing as the XML key below.
        case-sensitive-keys: false # Not required and has a default value of true.
```
```xml
<!--Example 3 - C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <LangVersion>11.0</LangVersion>
    <version>1.2.3</version> <!--Casing does not match but key is still used.-->
  </PropertyGroup>
</Project>
```

<div align="left">

Trims the value `refs/heads/` from the beginning of the branch.

> **Note** Click [here](https://docs.github.com/en/actions/learn-github-actions/environment-variables#default-environment-variables) to get more information about the default variable `github.ref` used in the example below:

```yml
#Example 4 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        repo-token: ${{ secrets.GITHUB_TOKEN }}
        branch-name: ${{ github.ref }} # If the branch was 'my-branch', this value could be 'refs/heads/my-branch'
        file-format: xml # Not case sensitive
        file-path: "MyProject/MyProject.csproj"
        version-keys: version
        trim-start-from-branch: "refs/heads/"
```

---

<h2 style="font-weight:bold;" align="center">🙏🏼 Contributing 🙏🏼</h2>

Interested in contributing? If so, click [here](https://github.com/KinsonDigital/.github/blob/main/docs/CONTRIBUTING.md) to learn how to contribute your time or [here](https://github.com/sponsors/KinsonDigital) if you are interested in contributing your funds via one-time or recurring donation.

<div align="center">

## **🔧 Maintainers 🔧**
</div>

![x-logo-dark-mode](https://raw.githubusercontent.com/KinsonDigital/.github/main/Images/x-logo-16x16-dark-mode.svg#gh-dark-mode-only)
![x-logo-light-mode](https://raw.githubusercontent.com/KinsonDigital/.github/main/Images/x-logo-16x16-light-mode.svg#gh-light-mode-only)
[Calvin Wilkinson](https://twitter.com/KDCoder) (KinsonDigital GitHub Organization - Owner)


![x-logo-dark-mode](https://raw.githubusercontent.com/KinsonDigital/.github/main/Images/x-logo-16x16-dark-mode.svg#gh-dark-mode-only)
![x-logo-light-mode](https://raw.githubusercontent.com/KinsonDigital/.github/main/Images/x-logo-16x16-light-mode.svg#gh-light-mode-only)
[Kristen Wilkinson](https://twitter.com/kswilky) (KinsonDigital GitHub Organization - Project Management, Documentation, Tester)
 
<br>

<h2 style="font-weight:bold;" align="center">🚔 Licensing And Governance 🚔</h2>

<div align="center">

[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg?style=flat)](https://github.com/KinsonDigital/.github/blob/main/docs/code_of_conduct.md)
[![GitHub](https://img.shields.io/github/license/kinsondigital/versionminer)](https://github.com/KinsonDigital/VersionMiner/blob/main/LICENSE.md)
</div>

This software is distributed under the very permissive MIT license and all dependencies are distributed under MIT-compatible licenses.
This project has adopted the code of conduct defined by the **Contributor Covenant** to clarify expected behavior in our community.
