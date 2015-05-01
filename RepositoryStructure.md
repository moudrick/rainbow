Structure is set in accordance to the following recommendations [here in Rahul's blog](http://community.rainbowportal.net/blogs/rahul_notes/archive/2006/02/10/338.aspx)

> IMPORTANT! To **anonymously** _checkout_ or _export_ use **http** protocol.
> Otherwise if you have been granted with commit rights use **https** protocol to **authorized** _export_, _checkout_, _switch_, _commit_, other repository operations.

The root of the repository is **http://rainbow.googlecode.com/svn/* (**https://rainbow.googlecode.com/svn/*)
Do not checkout or export this url directly, you have to add a **relative path** to a structure unit for best result. Please remember that the relative paths are case sensitive.

> WARNING! There is no ~~http://rainbow.googlecode.com/svn/trunk/~~  as it shown at [standard Google Code page of the Rainbow project](http://code.google.com/p/rainbow/source)~~





We use our own structure instead of initially precreated structure. It is described below more detailed. You can browse this structure by TortoiseSvn Repo-browser (see http://tortoisesvn.tigris.org/). You who has rights to commit please follow this structure and never make commits outside your sandbox.



| | |
|:|:|
|  |  |
| **Relative paths for substructure units** | **Short description** |
|  |  |
| **/wiki/** | standard google code wiki for the whole repository |
|  |  |
| **/sandboxes/** | devepolers' sandboxes with their **personal** trunks/tags/branches and other required personal structure (RepositorySandboxStructure) |
|  |  |
| **/NET\_1\_1/** | latest code of **rainbow portal** for **.Net 1.1** |
|  |  |
| **/NET\_1\_1/branches/** |  |
| **/NET\_1\_1/tags/** |  |
|  |  |
| **/NET\_1\_1/trunk/** | latest trunk with its own substructure |
|  |  |
| **/NET\_1\_1/trunk/ECommerce/** | ECommerce module |
| **/NET\_1\_1/trunk/Extensions/** | extensions necessary |
| **/NET\_1\_1/trunk/Rainbow/** | site root |
|  |  |
|  |  |
| **/NET\_2\_0/** | unfolded comprehensive structure for working on **rainbow** for **.Net 2.0** version |
|  |  |
| **/NET\_2\_0/devint/** | _development & integration_ folder |
|  |  |
| **/NET\_2\_0/devint/branches/** |  |
| **/NET\_2\_0/devint/tags/** |  |
| **/NET\_2\_0/devint/trunk/** | latest _devint_ trunk with its own content (see RootFiles) and substructure (see [NET\_2\_0\_BriefFoldersStructure](NET_2_0_BriefFoldersStructure.md)) |
|  |  |
| **/NET\_2\_0/stage/** | stage environment with its own separate structure |
|  |  |
| **/NET\_2\_0/stage/branches/** |  |
| **/NET\_2\_0/stage/tags/** |  |
| **/NET\_2\_0/stage/trunk/** |  |
|  |  |
|  |  |
| **/NET\_2\_0/prod/** | production environment with its own separate structure |
|  |  |
| **/NET\_2\_0/prod/branches/** |  |
| **/NET\_2\_0/prod/tags/** |  |
| **/NET\_2\_0/prod/trunk/** |  |
|  |  |
|  |  |

























































