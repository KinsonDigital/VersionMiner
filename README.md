<h1 align="center">

**Version Miner Action🪨⛏️**
</h1>

<div align="center">
  <h3>GitHub Action for pulling out versions from files.</h3>
</div>

<div align="center">

![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/KinsonDigital/VersionMiner?color=%23318A42&include_prereleases&label=Latest%20Release&logo=github)
</div>

<div align="center">

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/kinsondigital/VersionMiner/%E2%9C%94Unit%20Testing%20Status%20Check?color=%23238636&label=Unit%20Tests)
![GitHub issues by-label](https://img.shields.io/github/issues/kinsondigital/VersionMiner/good%20first%20issue?color=%23238636&label=Good%20First%20Issues)
</div>

<div align="center">

![Discord](https://img.shields.io/discord/481597721199902720?color=%23575CCB&label=discord&logo=discord&logoColor=white)
![Twitter URL](https://img.shields.io/twitter/url?color=%235c5c5c&label=Follow%20%40KDCoder&logo=twitter&url=https%3A%2F%2Ftwitter.com%2FKDCoder)
</div>


<div align="center">

## **What is it?**
</div>


This is a **GitHub Action** to make it easy to pull versions from XML files.
This can be used in your workflows for other uses such as version validation, version tag management, and more!!


<div align="center"><h3 style="font-weight:bold">📒Quick Note📒</h3></div>

This GitHub action is built using C#/NET and runs in a docker container.  This means that the action can only be run on Linux.  Running in ***Windows*** is not supported.  If you need to use steps on ***Windows*** AND ***Ubuntu***, then you can split up your workflow so that this action is in an isolated job that runs on ***Ubuntu***, while the rest of the workflow can be executed in ***Windows***.

<div align="center"><h3 style="font-weight:bold">Usage Examples</h3></div>

- Create tags automatically with the version, during the release process.
- Validate the version syntax to help enforce version syntax.
  - Example: Semantic version vs. a date-based version.
- Manage release note file names by having the version embedded in the file name.
- Use the version in the title of a GitHub release.
- Release announcements.
  - Example: Use the version in a release announcement on Twitter.
- Use status check workflows to verify versions before a pull request can be completed.
- Whatever your imagination comes up with!!

---

<div align="center"><h2 style="font-weight:bold">What does it do?</h2></div>

In a nutshell, this pulls versions out of XML data files for use in workflows.
Just tell the action which repo, branch, and file contains the version, and it will search through the file for the `version-keys` and pull out the value of that key. This value is used as the value of the action's output,
which has the name `version`, so you can use it in the rest of your workflow.

The `version-keys` input is just a comma delimited list of XML keys to search for in the XML file.  
Example:  
If the value of the `version-keys` input was ***"Version,FileVersion"***, then it would search
the XML for any XML elements that match the name ***"Version"*** or ***"FileVersion"***.  The first element that has a value will be the value returned.  So if the XML element ***"Version"*** had a value of ***1.2.3***, then it would simply return the value of the ***"Version"*** element and stop looking for values in any other XML elements.

---

<div align="center"><h3 style="font-weight:bold">Quick Example</h3></div>

```yaml
name: Get Version Example

jobs:
  Get_Version_Job:
    runs-on: ubuntu-latest # Cannot use windows
    steps:
    - uses: actions/checkout@v2

    - name: Get Version From C# Project File
      id: get-version
      uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
      with:
      repo-owner: JohnDoe
      repo-name: MyRepo
      branch-name: master
      file-format: xml # Not case sensitive
      file-path: "MyProject/MyProject.csproj"
      version-keys: Version

    - name: Print Version From File
      id: print-output
      run: echo "${{ steps.get-version.outputs.version }}"
```

So if the XML file had the contents below, the workflow above would print the value ***1.2.3*** to the GitHub console.

``` xml
<!--Quick Example - C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>10.0</LangVersion>
    <Version>1.2.3</Version>
    <FileVersion>0.1.0</FileVersion>
</Project>
```

<div align="left">
<a href="#examples">More Examples Below!! 👇🏼</a>
</div>

---

<div align="center">

## **Action Inputs**
</div>

| Input Name | Description | Required | Default Value |
|---|:----|:---:|---|
| `repo-owner` | The owner of the repository. | yes | N/A |
| `repo-name` | The name of the repository. | yes | N/A |
| `branch-name` | The name of the branch where the file lives. | yes | N/A |
| `file-format` | The data format of the file that contains the version. Currently, the only supported value is `xml` for a file format.  Not case sensitive. | yes | N/A |
| `file-path` | The path to the file relative to the root of the repository. | yes | N/A |
| `version-keys` | A comma delimited list of keys possibly hold the version value. Spaces around commas are ignored.  Must be wrapped with single or double quotes to be processed properly if more than one key exists. | yes | N/A |
| `case-sensitive-keys` | If true, key searching will be case-sensitive. | no | `true` |
| `trim-start-from-branch` | Will trim the given value from the beginning of the `branch-name` input. | no | empty |
| `fail-on-key-value-mismatch` | If true, the action will fail, if all of the key values listed in the `version-keys` input do not match.  Other failure inputs will not affect this input. | no | `false` |
| `fail-when-version-not-found` | If true, the action will fail, if no version exists.   Other failure inputs will not affect this input. | no | `true` |

---

<div align="center" style="font-weight:bold">

## **Examples**
</div>

<div align="center">

### **Example 1 - (Pass When Version Is Not Found)**
</div>

Requirements:
- Search for a version but do not fail the workflow if no version is found

📒Quick Note: The action input `fail-when-version-not-found` is not required and has a default value of `true`.  If you do not want the action to fail when the version is not found, you must explicitly use the input with a value of `false`.

``` yml
#Example 1 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        branch-name: master
        file-format: xml # Not case sensitive
        file-path: "MyProject/MyProject.csproj"
        version-keys: Version
        fail-when-version-not-found: false
```
``` xml
<!--Example 1 - C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>10.0</LangVersion>
    <Version></Version> <!--No value.  Does not fail workflow.-->
</Project>
```

<div align="center">

### **Example 2 - (Multiple Key Searching)**
</div>

Requirements:
- Searches multiple keys for the version.
- The job fails if no version is found in the keys.

Result:  
- The example below will use the value of ***4.5.6*** as the action output.

📒Quick Note: Since the `fail-when-version-not-found` input is not explicitly used in the YAML, the default value of `true` will be used and the job will fail if the version was not found.

``` yml
#Example 2 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        branch-name: master
        file-format: xml # Not case sensitive
        file-path: "MyProject/MyProject.csproj"
        version-keys: "Version,FileVersion"
```
``` xml
<!--Example 2 - C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>10.0</LangVersion>
    <Version></Version> <!--No value. Search continues using the FileVersion key-->
    <FileVersion>4.5.6</FileVersion> <!--Key exists so this value is returned-->
</Project>
```

<div align="center">

### **Example 3 - (Key Case Sensitivity)**
</div>

Requirements:
- Search for a key without using case-sensitivity

Result:  
- The example below will use the value of ***1.2.3*** as the action output.

``` yml
#Example 3 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        branch-name: master
        file-format: xml # Not case sensitive
        file-path: "MyProject/MyProject.csproj"
        version-keys: VeRSion # Different casing as the XML key below.
        case-sensitive-keys: false # Not required and has a default value of true.
```
``` xml
<!--Example 3 - C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>10.0</LangVersion>
    <version>1.2.3</version> <!--Spelling matches "VeRSion" but is still discovered.-->
</Project>
```

<div align="center">

### **Example 4 - (Branch Trimming)**
</div>

Requirements:
- Need to trim the value 'refs/heads/' from the beginning of the branch.

Result:  
- The example below will use the value of ***1.2.3*** as the action output.
- Click [here](https://docs.github.com/en/actions/learn-github-actions/environment-variables#default-environment-variables) to get more information about the default variable used in the example below.

``` yml
#Example 4 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.2
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        branch-name: ${{ github.ref }} # If the branch was 'my-branch', this value could be 'refs/heads/my-branch'
        file-format: xml # Not case sensitive
        file-path: "MyProject/MyProject.csproj"
        version-keys: version
        trim-start-from-branch: "refs/heads/"
```

---

<div align="center">

## **Other Info**
</div>

<div align="left">

### License
- [MIT License - VersionMiner](https://github.com/KinsonDigital/VersionMiner/blob/preview/v1.0.0-preview.2/LICENSE)
</div>

<div align="left">

### Maintainer
</div>

- [Calvin Wilkinson](https://github.com/CalvinWilkinson) (Owner and main contributor of the GitHub organization [KinsonDigital](https://github.com/KinsonDigital))
  - [Version Miner](https://github.com/KinsonDigital/VersionMiner) is used in various projects for this organization with great success.
- Click [here](https://github.com/KinsonDigital/VersionMiner/issues/new/choose) to report any issues for this GitHub action!!

<div align="right">
<a href="#what-is-it">Back to the top!👆🏼</a>
</div>
