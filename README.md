<h1 align="center">

**Version Miner Action🪨⛏️**
</h1>

<div align="center">
<h3>GitHub Action for pulling out versions from XML files.</h3>

<div hidden>TODO: ADD BADGES HERE</div>

</div>


<div align="center">

## **What is it?**
</div>


This is a **GitHub Action** to make it easy to pull versions from XML files.
This can be used in your workflows for other uses such as version validation, version tag management, and more!!

<div align="center"><h3 style="font-weight:bold">⚠️Quick Note⚠️</h3></div>

This GitHub action is built using C#/NET and runs in a docker container.  This means that the action can only be run on Linux.  Running in ***Windows*** is not supported.  If you need to use steps on ***Windows*** AND ***Ubuntu***, then you can split up your workflow so that this action is in an isolated job that runs on ***Ubuntu***, while the rest of the workflow can be executed in ***Windows***.

<div align="center"><h3 style="font-weight:bold">Usage Examples</h3></div>

- Create tags automatically with the version during the release process.
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
      uses: KinsonDigital/VersionMiner@v1.0.0-preview.1
      with:
      repo-owner: JohnDoe
      repo-name: MyRepo
      branch-name: master
      file-type: xml
      file-path: MyProject/MyProject.csproj
      version-keys: Version

    - name: Print Version From File
      id: print-output
      run: echo "${{ steps.get-version.outputs.version }}"
```

So if the C# project file had the contents below, the workflow above would print the value ***1.2.3*** to the GitHub console.

``` xml
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
| `repo-owner` | The owner of the repository | yes | N/A |
| `repo-name` | The name of the repository | yes | N/A |
| `branch-name` | The name of the branch where the file lives. | yes | N/A |
| `file-format` | The type of file that contains the version. Currently, the only supported value is `xml`. | yes | N/A |
| `file-path` | The path to the file relative to the root of the repository. | yes | N/A |
| `version-keys` | The key(s) that can hold the version in the file. | yes | N/A |
| `case-sensitive-keys` | If true, key searching will be case-sensitive. | no | `true` |
| `fail-on-key-value-mismatch` | If true, the action will fail, if all of the key values listed in the `version-keys` input do not match.  Other failure inputs will not affect this input. | no | `false` |
| `fail-when-version-not-found` | If true, the action will fail, if no version exists.   Other failure inputs will not affect this input. | no | `true` |

---

<div align="center" style="font-weight:bold">

## **Examples**
</div>

<div align="center">

### **Example 1 - (Pass If Version Not Found)**
</div>

Requirements:
- Search for a version but do not fail the workflow if no version is found

⚠️The action input `fail-when-version-not-found` is not required and has a default value of `true`.  If you do not want the action to fail when the version is not found, you must explicitly use the input.

``` yml
#Example 1 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.1
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        branch-name: master
        file-type: xml
        file-path: MyProject/MyProject.csproj
        version-keys: Version
        fail-when-version-not-found: false
```
``` xml
<!--Example 1 C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>10.0</LangVersion>
    <Version></Version>👈🏼No value.  Does not fail workflow.
</Project>
```

<div align="center">

### **Example 2 - (Multiple Key Searching)**
</div>

Requirements:
- Search multiple keys for the version.
- Fail the workflow if no version are found in the keys.

Result:  
- The example below will use the value of ***4.5.6*** as the action output.

⚠️Since the `fail-when-version-not-found` input is not explicitly used in the YAML, the default value of `true` will be used and the workflow will fail if the version was not found.

``` yml
#Example 2 Workflow
- name: Get Version From C# Project File
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.1
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        branch-name: master
        file-type: xml
        file-path: MyProject/MyProject.csproj
        version-keys: Version,FileVersion
```
``` xml
<!--Example 2 C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>10.0</LangVersion>
    <Version></Version>👈🏼No value.  Search continues using the FileVersion key
    <FileVersion>4.5.6</FileVersion>👈🏼Key exists so this value is returned
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
    uses: KinsonDigital/VersionMiner@v1.0.0-preview.1
    with:
        repo-owner: JohnDoe
        repo-name: MyRepo
        branch-name: master
        file-type: xml
        file-path: MyProject/MyProject.csproj
        version-keys: VeRSion 👈🏼 # Different casing as XML key below
```
``` xml
<!--Example 3 C# Project File-->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>10.0</LangVersion>
    <version>1.2.3</version> 👈🏼 <!--Spelling matches "VeRSion" but still is found as a version key.-->
</Project>
```

---

<div align="center">

## **Other Info**
</div>

<div align="left">

### License
- [MIT License - VersionMiner](https://github.com/KinsonDigital/VersionMiner/blob/preview/v1.0.0-preview.1/LICENSE)
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
