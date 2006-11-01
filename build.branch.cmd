@rem "Move required environment variable value assignment to the last position in the group"

@rem webAppTemplate - Default value
@set webAppTemplate="Rainbow2.0"

@rem branch
@set branch="sandboxes/CMR/trunk/"
@set branch="sandboxes/MGF/branches/WebApplicationProject/"
@set branch="sandboxes/MGF/branches/Webparts/"
@set branch="sandboxes/moudrick/branches/MGF_Webparts_371/"
@set branch="sandboxes/moudrick/branches/MGF_351/"

@set branch="sandboxes/MGF/trunk/"
@set webAppTemplate="Rainbow2.0-Trunk"

nant main -logfile:.build.branch.nant.log -D:in.branch="%branch%" -D:in.webAppTemplate="%webAppTemplate%"

