# Commit conventions #

Please keep the following commit convention.

## Сautions ##

  * Do not commit anything outside your sandbox root.
  * Avoid commiting large files.
  * Avoid commiting the redundant files, regardless of their size. Such as:
    * files that can be built from sources in the repository
    * files that are local for your environment (`*.suo, *.??proj.user` etc, see below)

## Global ignores ##

<sup>How to locally svn:ignore frequently ignored stuff without changing any property in the repository</sup>

No need to explicitely ignore frequently ignored stuff.

You can use global config.
You can find it in **%USERPROFILE%\Application Data\Subversion\config** file  on Windows
systems.

You have to find the section **`[miscellany]`** and uncomment the **`global-ignores`** key

For **ASP.Net** and other **.Net** development it is recommended to ignore the following list of signatures:

```
global-ignores = bin obj _old Bin *.suo *.user *.db Thumbs.db *.pdb *.csproj.user *.pdb
```




