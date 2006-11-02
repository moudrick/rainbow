//<script language="Javascript" type="text/javascript">
/*
This script is part of the EasyListBox server control.
Purchase and licensing information can be found at EasyListBox.com.
*/
var ELB_listState = new Array();
var ELB_listMode = new Array();
var ELB_BackColorRoll = new Array();
var ELB_ForeColorRoll = new Array();
var ELB_ForeColor = new Array();
var ELB_BackColor = new Array();
var ELB_ParentLists = new Array();
var ELB_ChildLists = new Array();
var ELB_ChildListsItems = new Array();
var ELB_ItemBuffer = new Array();
var ELB_PrimeRows = new Array();

var ELB_Backer = new Array();

var elbTip = false;
var sCurrentDrag = "";
var iCurrentDrag = -1;
var aCurrentDrag = new Array();
var sSearch;
var sSpeedSearch = "";
var sSpeedSearchID = "";
var currentKeyDown = 0;
var binIE = document.all;

var sNoSelect = "asdfqweupodfnsfencv";

var sToolTipPrompt;

function getObj(objName) {
    var theObj = document.getElementById(objName);
    return theObj;
    }

function ELB_focus(ctlID, e) {
    if(sSpeedSearchID != ctlID) { sSpeedSearchID = ctlID; sSpeedSearch = ""; elbTip.innerText = sSpeedSearch; }
    var oHead = getObj(ctlID + "_listHead");
    if(oHead.style.display == "none") {
        getObj(ctlID + "_dropList").focus();
        }
    else if(getObj(ctlID + "_comboText")) {
        getObj(ctlID + "_comboText").select();
        }
    else {
		if(binIE) { getObj(ctlID + "_listHeadText").focus(); }
		else { getObj(ctlID + "_listHeadFocus").focus(); }
        }
    }

function ELB_GetRowHTML(ctlID) {
	var oList = getObj(ctlID + "_dropList");
	var sContent = oList.innerHTML;
	if(binIE) { sContent = sContent.substring(sContent.indexOf("<TR"), sContent.indexOf("</TR>") + 5).replace("display: none;", ""); }
	else { sContent = sContent.substring(sContent.indexOf("<tr"), sContent.indexOf("</tr>") + 5).replace("display: none;", ""); }
	return sContent;
	}

function ELB_init(ctlID, postBackVal, selMode, rollBackColor, rollForeColor, childLists, parentList, swapTarget) {

    ELB_listState[ctlID] = 0;
    ELB_listMode[ctlID] = selMode;
    ELB_BackColorRoll[ctlID] = rollBackColor;
    ELB_ForeColorRoll[ctlID] = rollForeColor;
    ELB_ItemBuffer[ctlID] = new Array();

    var oListTable = getObj(ctlID + "_dropListTable");
	ELB_PrimeRows[ctlID] = ELB_GetRowHTML(ctlID).split(">&nbsp;").join("> ").split("style=\"").join("style=\"padding-right:3px;");

    var oList = getObj(ctlID + "_dropList");
    var oHead = getObj(ctlID + "_listHead");
	var binUseAjax = (getObj(ctlID).getAttribute("useajax")=="true") ? true : false;
    if(binUseAjax) { getObj(ctlID + "_noResults").style.display = "inline"; }

	if(getObj(ctlID).getAttribute("sbw")) { ELB_FillList(ctlID); oListTable = getObj(ctlID + "_dropListTable"); }

    var binClipHead = oHead.clipHead;
    var selIndex = parseInt(getObj(ctlID + "_SelectedIndex").value);
    var displayMode = oHead.getAttribute("displayMode");
    sToolTipPrompt = oHead.getAttribute("ToolTipPrompt");

    var iCell = 0;

    while(oListTable.rows[0].cells[iCell]) {
        if(binIE && (displayMode == "listbox" || selMode == 1) ) {
            //oList.onmouseover = "";
            //oListTable.rows[0].cells[iCell].onblur = "";
            oListTable.rows[0].cells[iCell].onclick = "";
            }
        else if(binIE) {
            oListTable.rows[0].cells[iCell].onclick = "";
            }
        iCell++;
        }
    //if(binIE && (displayMode == "listbox" || selMode == 1)) { getObj(ctlID + "_dropList").onblur="";getObj(ctlID + "_listHead").onblur="" }

    ELB_ForeColor[ctlID] = oHead.style.color;
    ELB_BackColor[ctlID] = oHead.style.background;
    var hdRow = oListTable.rows[0];

    hdRow.style.display = "none";
    hdRow.removeAttribute("selected");

	if(getObj(ctlID + "_BackerFrame")) {
		ELB_Backer[ctlID] = getObj(ctlID + "_BackerFrame");

		var tempList;
		var tempHold;
		tempList = oList.outerHTML;
		tempHold = getObj(ctlID + "_placeHolder");
		oList.outerHTML = "";
		tempHold.outerHTML = tempList;
		oList = getObj(ctlID + "_dropList");
		oListTable = getObj(ctlID + "_dropListTable");
		}
    if((parentList != "") && (getObj(parentList))) {
        var theParent = getObj(parentList);
        ELB_ParentLists[ctlID] = parentList;
		ELB_FillChildList(ctlID, theParent.value, getObj(ctlID).value);
        }
    if(oHead.style.borderStyle == "solid") {
        oList.style.scrollbarArrowColor = oHead.getAttribute("buttonForeground");
        oList.style.scrollbar3dLightColor = oHead.getAttribute("buttonForeground");
        oList.style.scrollbarShadowColor = oHead.getAttribute("buttonForeground");

        oList.style.scrollbarFaceColor = oHead.getAttribute("buttonBackground");
        oList.style.scrollbarHighlightColor = oHead.getAttribute("buttonBackground");
        oList.style.scrollbarDarkShadowColor = oHead.getAttribute("buttonBackground");
        oList.style.scrollbarTrackColor = oHead.getAttribute("buttonBackground");
        }
    if(swapTarget) { getObj(ctlID + "_dropListTable").setAttribute("swapTarget", "true"); ELB_RollUp(ctlID); }
    else if(selMode == 1) { // Multiple
        getObj(ctlID + "_listHeadText").onKeyUp = "";
        ELB_MultiSetup(ctlID);
        }
    else { // Single
        if(binIE) { oHead.style.height = "10px"; }
        oList.style.width = oHead.offsetWidth;
        if(selIndex > -1) {
            if(parentList != "") { selIndex = parseInt(getObj(ctlID + "_SelectedIndex").value); }
			oListTable = getObj(ctlID + "_dropListTable");
			var selRow = getObj(ctlID + "_Row" + selIndex);

			if(selRow) {
				selRow.setAttribute("selected", "true");
	            ELB_Rollover(selRow);
	            getObj(ctlID + "_SelectedText").value = binIE? selRow.cells[0].innerText : ELB_filterHTML(selRow.cells[0].childNodes[0].innerHTML);
	            oListTable.title = binIE? (sToolTipPrompt + "  " + selRow.cells[0].innerText) : (sToolTipPrompt + "  " + ELB_filterHTML(selRow.cells[0].childNodes[0].innerHTML));
				oHead.title = oListTable.title;
				}
			else if(!binUseAjax) {
				ELB_ClearSelection(ctlID);
				}
            }
        }

    if(displayMode == "listbox") {

        if(oHead.style.position == "absolute") {
            oList.style.top = oHead.style.top;
            oList.style.left = oHead.style.left;
            }
        else { oList.style.position = "relative"; }

        if(binIE) { oList.style.display = "inline"; }
        oList.style.visibility = "visible";
        oList.style.border = oHead.style.border;
        oList.tabIndex = getObj(ctlID + "_listHeadText").tabIndex;
        oList.hideFocus = false;
        getObj(ctlID + "_listHeadText").tabIndex = -1;
        oList.style.width = oHead.style.width;
        oHead.style.display = "none";

        if(selMode == 0) {
            ELB_ListBoxSetup(ctlID, swapTarget);
            }
        }

    else if(displayMode == "combo") {
        oList.onmouseover = "";
        if(selIndex != -1 && getObj(ctlID).value != "" && !binUseAjax) {
            getObj(ctlID + "_comboText").value = ELB_filterHTML(getObj(ctlID + "_dropListTable").rows[selIndex].cells[0].innerHTML);
            getObj(ctlID + "_listHead").title = sToolTipPrompt + "  " + getObj(ctlID + "_comboText").value;
            getObj(ctlID + "_dropListTable").title = sToolTipPrompt + "  " + getObj(ctlID + "_comboText").value;
            }
		else if(binUseAjax && getObj(ctlID).value != "") {
			ELB_SendRequest(ctlID, getObj(ctlID).value, "combo");
			}
		else {
		getObj(ctlID + "_comboText").value = getObj(ctlID).value; }
        getObj(ctlID + "_listHeadText").onKeyUp = "";
        getObj(ctlID + "_listHeadText").tabIndex = -1;
        }
    else {
        oList.onmouseover = "";
        }

    if(childLists) {
        ELB_ChildLists[ctlID] = childLists;
        ELB_SetChildren(childLists, getObj(ctlID).value);
        }
	if(postBackVal) { getObj(ctlID).setAttribute("autopostback", postBackVal); }

    if(swapTarget && displayMode == "dropdown" && getObj(ctlID + "_clipText").innerHTML.indexOf("|") >= 0) { getObj(ctlID + "_listHeadText").innerHTML = ""; }
    }

function ELB_ListBoxSetup(ctlID, swapTarget) {

    var elbTemp = getObj(ctlID);
    var selIndex = getObj(ctlID + "_SelectedIndex");
    var tblTemp = getObj(ctlID + "_dropListTable");
    var oHead = getObj(ctlID + "_listHead");

    for(var i=1;i<tblTemp.rows.length;i++) {
        var rowTemp = tblTemp.rows[i];

        if(elbTemp.value.indexOf(rowTemp.selectvalue) >= 0 && !swapTarget) {
            binSelected = true;
            rowTemp.setAttribute("selected", "true");
            if(oHead.showCheckBoxes != "true") { rowTemp.style.background = ELB_BackColorRoll[ctlID]; }
            }
        for(var j=0;j<rowTemp.cells.length;j++) {
            var celTemp = rowTemp.cells[j];
            //celTemp.onclick = celTemp.onblur;
            //celTemp.onblur = "";
            ELB_Rollout(rowTemp);
            if(rowTemp.getAttribute("selected") == "true" && !swapTarget) { celTemp.style.color = ELB_ForeColorRoll[ctlID] }

            }
        }
    }

function ELB_MultiSetup(ctlID) {
    var binSelected = false;
    var elbTemp = getObj(ctlID);
    var oHead = getObj(ctlID + "_listHead");
    var sTitle = "";

    var tblTemp = getObj(ctlID + "_dropListTable");
    for(var i=1;i<tblTemp.rows.length;i++) {
        var rowTemp = tblTemp.rows[i];
        if(elbTemp.value.indexOf("," + rowTemp.getAttribute("selectvalue") + ",") >= 0 || elbTemp.value.indexOf(rowTemp.getAttribute("selectvalue") + ",") == 0) {
            binSelected = true;
            rowTemp.setAttribute("selected", "true");
            rowTemp.onmouseup = tblTemp.parentNode.onmouseup;
            if(oHead.getAttribute("showCheckBoxes") != "true") { rowTemp.style.background = ELB_BackColorRoll[ctlID]; }
            else { rowTemp.cells[0].childNodes[0].checked = true; }
            }
        for(var j=0;j<rowTemp.cells.length;j++) {
            var celTemp = rowTemp.cells[j];
            celTemp.unselectable = "yes";
            //celTemp.onclick = celTemp.onblur;
            //celTemp.onblur = "";
            if(rowTemp.getAttribute("selected") == "true") {
            if(oHead.getAttribute("showCheckBoxes") != "true") { celTemp.style.color = ELB_ForeColorRoll[ctlID]; }
            else {
				if(binIE) { rowTemp.cells[0].childNodes[0].checked = true; }
				else { rowTemp.cells[0].childNodes[0].childNodes[0].checked = true; }
				}
                if(j==0) {
                    if(binIE) { sTitle += "<br />" + celTemp.innerText; }
					else { sTitle += "<br />" + ELB_filterHTML(celTemp.childNodes[0].innerHTML); }
                    }
                }
            }
        }
    if(!binSelected) {
        elbTemp.value = "";
        getObj(ctlID + "_SelectedIndex").value = "-1";
        getObj(ctlID + "_SelectedText").value = "";
        }
    if(sTitle != "" && getObj(ctlID + "_dropListTable").getAttribute("swapTarget") != "true") {
        ELB_displaySelected(ctlID, sTitle);
        }
    }

function ELB_FilterCombo(ctlID, e, prevSel) {
    var comboText = getObj(ctlID + "_comboText");
    var oList = getObj(ctlID + "_dropListTable");
    var sFilter = comboText.value;
    var newText = "";
    var selIndex = parseInt(getObj(ctlID + "_SelectedIndex").value);
    var maxIndex = oList.rows.length - 1;
    var moveIndex = 0;
	var theAlt;

	var binUseAjax = (getObj(ctlID).getAttribute("useajax") == "true") ? true : false;
	var binBypassEvent = false;
	//binBypassEvent = (e==null) ? true : false;

	var theKey;
	if(!binIE && theKey == 9) { ELB_retractList(ctlID); return; }
	if(!binBypassEvent) {
		theKey = binIE ? window.event.keyCode : e.which;
		theAlt = binIE ? window.event.altKey : e.keyCode==18;
	    currentKeyDown--;
		}

    if(theKey == 38 || theKey == 40) { // Up or down arrow
        moveIndex = -39 + theKey;
		selIndex += moveIndex;

        if(selIndex >= maxIndex) { selIndex = maxIndex; }
        else if(selIndex < 1) { selIndex = 1; }

        ELB_SelectItem(ctlID, oList.rows[selIndex].id, e);

        if(binIE) { window.event.cancelBubble = true; }
		else { e.stopPropagation(); }
		ELB_expandList(ctlID);
        return;
        }
	if(binUseAjax && prevSel != "elbpickreturnedvalue") { binBypassEvent = true; ELB_SendRequest(ctlID, sFilter, "combo", prevSel); return; }

	if(binBypassEvent)  {  }
    else if(theAlt && theKey == 40) { // Alt + down arrow
        ELB_expandList(ctlID);
        return;
        }
    else if(theKey == 36 || theKey == 16) {
        return;
        }
    else if(theKey == 37) {
        return;
        }
    else if(theKey == 27) {
        ELB_ClearSelection(ctlID);
		while(oList.scrollTop != 0) {
			oList.doScroll("scrollBarPageUp");
			}
        return;
        }
    else if(theKey == 18) { // Letting up the Alt key
        if(binIE) { window.event.cancelBubble = true; }
		else { e.stopPropagation(); }
        return;
        }
    else if(sFilter == "" ) { // Clear it out if the field is empty
        if(getObj(ctlID).value != "") {
            ELB_ClearSelection(ctlID);
			while(oList.scrollTop != 0) {
				oList.doScroll("scrollBarPageUp");
				}
            }
        return;
        }
    else if(theKey == 8) { // let the user backspace
        if(parseInt(getObj(ctlID + "_SelectedIndex").value) > -1) {
            sFilter = sFilter.substring(0, sFilter.length - 1);
            }
        if(sFilter.length == 0) {
            comboText.value = "";
            if(getObj(ctlID).value != "") {
                ELB_ClearSelection(ctlID);
                }
			while(oList.scrollTop != 0) {
				oList.doScroll("scrollBarPageUp");
				}
            return };
        }

	if(binIE) { // Use the Enumerator object for IE
		var tmpEnum = new Enumerator(oList.rows);
		var theRow;
		var theText;

		for (;!tmpEnum.atEnd();tmpEnum.moveNext()) { // And if it's an alphanumeric...
	        if(currentKeyDown > 0) { return; }
	        var rowTemp = tmpEnum.item();

	        if(rowTemp.cells[0].innerText.toLowerCase().indexOf(sFilter.toLowerCase()) == 0) {
	            ELB_SelectItem(ctlID, rowTemp.id);

	            newText = comboText.createTextRange();
	            newText.moveStart("character", sFilter.length);
	            newText.select();
	            ELB_expandList(ctlID);
	            return;
	            }
	      	}
		}
	else { // old-school table running for Firefox
	    for(var i=1;i<oList.rows.length;i++) { // And if it's an alphanumeric...
	        if(currentKeyDown > 0) { return; }
	        var rowTemp = oList.rows[i];

	        if(ELB_filterHTML(rowTemp.cells[0].childNodes[0].innerHTML).toLowerCase().indexOf(sFilter.toLowerCase()) == 0) {
	            ELB_SelectItem(ctlID, rowTemp.id, e);

				if(comboText.setSelectionRange) {
					comboText.setSelectionRange(sFilter.length, comboText.value.length);
					}
	            return;
	            }
			}
		}

    ELB_ClearSelection(ctlID);
	while(binIE && oList.scrollTop != 0) {
		oList.doScroll("scrollBarPageUp");
		}
    comboText.value = sFilter;
    comboText.focus();

    getObj(ctlID + "_SelectedIndex").value = -1;
    getObj(ctlID).value = sFilter;
    getObj(ctlID).text = sFilter;
    getObj(ctlID + "_SelectedText").value = sFilter;
    }

function ELB_OnKeyDown(ctlID, e) {
	var theKey = binIE? window.event.keyCode : e.which;

	if(getObj(ctlID + "_listHead").getAttribute("displayMode") == "combo") {
        if(binIE) {
			if(!window.event.repeat) { currentKeyDown++; }
	        window.event.cancelBubble = true;
			}
		else {
			e.stopPropagation();
			if(theKey == 9) { ELB_retractList(ctlID); return; }
			}
        return;
        }

    var selIndex = parseInt(getObj(ctlID + "_SelectedIndex").value);
	var oList = getObj(ctlID + "_dropListTable");
    var maxIndex = oList.rows.length - 1;
	if(maxIndex < 0) { maxIndex = 0; }
    var moveIndex = 0;
	var binBypassEvent = false;

	if(!binBypassEvent) {
		theKey = binIE ? window.event.keyCode : e.which;
		theAlt = binIE ? window.event.altKey : e.keyCode==18;
	    currentKeyDown--;
		}
    if(getObj(ctlID + "_dropList").style.visibility == "visible" &&
	        theKey == 13 &&
			getObj(ctlID + "_listHead").getAttribute("displayMode") != "listbox") { // Pressing Enter on the list

		ELB_retractList(ctlID);
        if(binIE) { window.event.returnValue = false; } else { e.preventDefault(); }
        return;
        }
//    else if(theKey == 38 || theKey == 40) { ELB_FilterSelect(ctlID) }
    else if(theKey != 17 && ELB_listMode[ctlID] != 1) {
        ELB_FilterSelect(ctlID, e);
        try {
            theKey = 123;
            }
        catch(e) {
            }
        finally {
			if(binIE) {
	            window.event.returnValue = false;
	            window.event.cancelBubble = true;
				}
			else {
				e.preventDefault();
				e.stopPropagation();
				}
            }
        return;
        }
    }

function ELB_FilterSelect(ctlID, e) {
    elbTip = document.getElementById(ctlID + "_spanTip");
	if(binIE) {
		if(!elbTip) {
			elbTip = document.createElement('<span id="' + ctlID + '_spanTip" style="position:absolute;visibility:hidden;background:lightyellow; border:1px solid gray;padding:2px;font-size:8pt;font-family:Verdana;z-index:999;height:10px;filter:progid:DXImageTransform.Microsoft.Alpha(opacity=70);" >');
			var oHead = getObj(ctlID + "_listHead");
			oHead.parentNode.appendChild(elbTip);
		 	}
		}
	else {
		//if(!elbTip) {
		//	elbTip = document.createElement('<span id="' + ctlID + '_spanTip" style="position:absolute;visibility:hidden;background:lightyellow; border:1px solid gray;padding:2px;font-size:8pt;font-family:Verdana;z-index:999;height:10px;" >');
		//	var oHead = getObj(ctlID + "_listHead");
			//oHead.parentNode.appendChild(elbTip);
		// 	}
		}

    var tblList = getObj(ctlID + "_dropListTable");
    var selIndex = parseInt(getObj(ctlID + "_SelectedIndex").value);
    var maxIndex = tblList.rows.length - 1;
    var moveIndex = 0;
	var theKey = binIE? window.event.keyCode : e.which;
    var binFilter = false;

    if(theKey == 8) {
        if(theKey == 8 && sSpeedSearch != "") {
            sSpeedSearch = sSpeedSearch.substring(0, sSpeedSearch.length - 1);
            if(elbTip) { elbTip.innerHTML = sSpeedSearch; }
            if(sSpeedSearch == "") {
				ELB_ClearSelection(ctlID);
                if(elbTip) { elbTip.style.visibility = "hidden"; }
				var oList = getObj(ctlID + "_dropList");
				while(binIE && oList.scrollTop != 0) {
					oList.doScroll("scrollBarPageUp");
					}
				}
            }
        binFilter = true;
        }
    else if((theKey >= 48 && theKey <= 57) || (theKey >= 65 && theKey <= 90) || (theKey == 32) ||
     (theKey >= 96 && theKey <= 105)) { // Alphanumeric key (including keypad)
        sSpeedSearch += String.fromCharCode(theKey).toLowerCase();
        if(elbTip) {
            elbTip.innerHTML = sSpeedSearch;
            if(elbTip.style.visibility == "hidden" && getObj(ctlID).getAttribute("showSpeedSearch") == "true") {
                ELB_showSearch(ctlID);
                }
            }
        binFilter = true;
        }

    ELB_focus(ctlID);

    if(binFilter && sSpeedSearch != "") {

    var tmpArray;
    var theText;
    var theIndex = 1;

    if(ELB_ChildListsItems[ctlID]) {
        tmpArray = ELB_ChildListsItems[ctlID];
        var tempIndex = parseInt(getObj(ctlID + "_SelectedIndex").value);
        if(tempIndex > 1) { theIndex = tempIndex; }
        if(sSpeedSearch != "" && theKey == 8) {
            for(var i=theIndex;i>0;i--) {
                theText = ELB_filterHTML(tmpArray[i][2].toLowerCase());
                while(theText.indexOf(" ") == 0) { theText = theText.substring(1, theText.length); }
                try {
					//window.status = "Trimmed text to filter: |" + theText + "|\nFilter value: " + sSpeedSearch;
                    if(theText.indexOf(sSpeedSearch) == 0) {
                        ELB_SelectItem(ctlID, tblList.rows[i].id);
                        ELB_expandList(ctlID);
                        return;
                        }
                    }
                catch(e) {
                    alert("Error filtering list at index " + i + "; item " + tmpArray[i]);
                    }
                }
            }
        else {
            for(var i=theIndex;i<tmpArray.length;i++) {
                theText = ELB_filterHTML(tmpArray[i][2].toLowerCase());
                while(theText.indexOf(" ") == 0) { theText = theText.substring(1, theText.length); }

                try {
                    if(theText.indexOf(sSpeedSearch) == 0) {
                         ELB_SelectItem(ctlID, tblList.rows[i].id);
                         ELB_expandList(ctlID);
                        return;
                        }
                    }
                catch(e) {
                    alert("Error filtering list at index " + i + "; item " + tmpArray[i]);
                    }
                }
            }
        }
        if(binIE) { window.event.keyCode = 123; }
        return;
        }

    if(binIE) { if(window.event.altKey && theKey == 40) { ELB_expandList(ctlID); } } // Alt + down arrow
    else if(theKey == 38) { moveIndex -= 1; if(binIE) { window.event.keyCode = 123; window.event.returnValue = false; } else { e.preventDefault(); } } // Up arrow
    else if(theKey == 40) { moveIndex += 1; if(binIE) { window.event.keyCode = 123; window.event.returnValue = false; } else { e.preventDefault(); } } // Down arrow
    if(theKey == 38 || theKey == 40) { // Up or down arrow
		sSpeedSearch = "";
        if(elbTip) { elbTip.style.visibility = "hidden"; }

        moveIndex = -39 + theKey;
		selIndex += moveIndex;

        if(selIndex >= maxIndex) { selIndex = maxIndex; }
        else if(selIndex < 1) { selIndex = 1; }

		ELB_SelectItem(ctlID, tblList.rows[selIndex].id, e);

        if(binIE) { window.event.cancelBubble = true; }
		else { e.stopPropagation(); }
		ELB_expandList(ctlID);
        return;
        }

    if(theKey == 9) { return }
    else if(theKey !== 35 && theKey !== 36) { return }

    selIndex = parseInt(selIndex) + parseInt(moveIndex);
    if(selIndex == 0) { selIndex = 1; }

    if(selIndex > maxIndex) {
        selIndex = maxIndex;
        sSpeedSearch = "";
        return;
        }
    else if(selIndex < 1) {
        selIndex = 1;
        sSpeedSearch = "";
        return;
        }
    else if(moveIndex != 0) {
        sSpeedSearch = "";
        if(theKey == 36) { // Home button
			ELB_SelectItem(ctlID, tblList.rows[1].id);
			if(binIE) { window.event.returnValue = false; }
			else { e.preventDefault(); }
			}
        else if(theKey == 35) { // End button
			ELB_SelectItem(ctlID,
			tblList.rows[maxIndex].id);
			if(binIE) { window.event.returnValue = false; }
			else { e.preventDefault(); }
			}
        else {ELB_SelectItem(ctlID, tblList.rows[selIndex].id) } // Up or down movement
        }
    else {
        }

    if(tblList.parentNode.style.visibility == "visible") {
        tblList.rows[selIndex].cells[0].focus();
        }

    if(getObj(ctlID + "_listHead").getAttribute("displayMode") == "dropdown") {
        ELB_FocusHead(ctlID);
		//getObj(ctlID + "_listHeadText").focus();
        }
    else if(getObj(ctlID + "_listHead").getAttribute("displayMode") == "combo") {
            if(tblList.parentNode.style.visibility == "visible") { }
            else { getObj(ctlID + "_comboText").select() }
        }
    }

function ELB_showSearch(ctlID) {
    var oHead;
    var binIsHead;

    if(getObj(ctlID + "_listHead").style.display != "none") {
        oHead = getObj(ctlID + "_listHead");
        }
    else {
        oHead = getObj(ctlID + "_dropList");
        }

    var topCorrection = 0;
    var oParent = oHead.parentNode;

    if(oParent.tagName == "TD") {
        if(oParent.vAlign == "bottom") {
            if(oParent.style.borderTopWidth) {
                 topCorrection += parseInt(oParent.style.borderTopWidth)*2 + parseInt(oHead.style.borderWidth);
                 }
            }
        if(oParent.vAlign == "middle" || oParent.vAlign == "") {
            if(oParent.style.borderTopWidth) {
                topCorrection += parseInt(oParent.style.borderTopWidth) + parseInt(oHead.style.borderWidth);
                }
            }
        }

    elbTip.style.left = oHead.offsetLeft;
    elbTip.style.pixelTop = oHead.offsetTop + oHead.offsetHeight; // + topCorrection;
    elbTip.style.zIndex = oHead.style.zIndex + 100;

    var listBottom = elbTip.style.pixelTop + 40;
    var screenBottom = parseInt(document.body.clientHeight) + parseInt(document.body.scrollTop);
    var listTop = -(parseInt(oHead.offsetTop) - 40);

    if((listBottom > screenBottom) && (listTop < (listBottom - screenBottom))) {
        elbTip.style.top = parseInt(oHead.offsetTop) - 40 + topCorrection;
        }

    elbTip.style.visibility = "visible";
    }

function ELB_FilterMultiSelect(ctlID) {
    sSearch = ctlID;
	var theKey = binIE ? window.event.keyCode : 0;
    if(theKey == 9) { return } // Just tabbed in; let it go

    var oSearch = getObj(ctlID + "_searchBox");
    var oTable = getObj(ctlID + "_dropListTable");
    var rowTemp;
    var sText = oSearch.value.toLowerCase();

    if(sText != "") {
        for(var i=1;i<oTable.rows.length;i++) {
            rowTemp = oTable.rows[i];
			sItemText = ELB_filterHTML(rowTemp.cells[0].innerHTML);
            if((sItemText.toLowerCase().indexOf(sText) > -1) && !rowTemp.getAttribute("selected")) {
                ELB_SelectItem(ctlID, rowTemp.id);
                }
            }
        }
    sSearch = "";
    }

function ELB_SendRequest(ctlID, filterText, filterType, prevSel) {
	if(filterText == "" || filterType == undefined) {
		return;
		}
	var elbRequest;
	var sAbsPath = window.location.href;
    if(ELB_listMode[ctlID] == 1 && filterText.length > 2) { filterText = filterText.substring(1, filterText.length - 1); }

	sAbsPath = sAbsPath.toLowerCase().replace("http://", "");
	sAbsPath = "http://" + sAbsPath.substring(0, sAbsPath.indexOf("/"));
	var sRequestPath = sAbsPath + getObj(ctlID).getAttribute("ajaxpath");
	var oF = getObj(ctlID);
	sRequestPath += "?ctlid=" + ctlID + "&hashCode=" + oF.getAttribute("cacheKey") + "&chunkSize=" + oF.getAttribute("chunksize");
	sRequestPath += "&filterType=" + filterType + "&filterValue=" + filterText + "&selMode=" + ELB_listMode[ctlID];

	if (window.XMLHttpRequest) {
		//alert("Firefox: Sending request to \n" + sRequestPath); // Debugging
		elbRequest = new XMLHttpRequest();
		elbRequest.overrideMimeType('text/html');
		}
	else if (window.ActiveXObject) {
		//alert("IE: Sending request to \n" + sRequestPath); // Debugging
		elbRequest = new ActiveXObject("Microsoft.XMLHTTP");
		}
	elbRequest.onreadystatechange = function() {
		if (elbRequest.readyState==4) {
            var returnText;
            //if(elbRequest.ResponseText == null) { alert("Debugging:  response undefined"); }
            //if(!binIE && elbRequest.responseText != null) { alert("Response XML: " + elbRequest.responseText); }
            if(binIE) { returnText = elbRequest.ResponseText; }
            else { returnText = elbRequest.responseText; }
			if(returnText.indexOf("noresults") >= 0) {
				ELB_ClearList(ctlID);
				getObj(ctlID + "_noResults").style.display="inline";
				return;
				}
            getObj(ctlID + "_noResults").style.display="none";
			var tempItems = returnText.split("<elbR>");
			//alert(returnText); // Debugging
			for(var i=0;i<tempItems.length;i++) {
				tempItems[i] = tempItems[i].split("<elbC>");
				}
			ELB_ClearList(ctlID);
			ELB_ChildListsItems[ctlID] = tempItems;
			ELB_FillList(ctlID);

			if(filterType == "combo") {
				getObj(ctlID + "_comboText").value = filterText;
				//ELB_FilterCombo(ctlID, null, "elbpickreturnedvalue");
				ELB_expandList(ctlID);
				getObj(ctlID + "_comboText").focus();
				}
			else if(filterType == "related" && prevSel) {

				var selMode = ELB_listMode[ctlID];
				if(selMode == 0) { ELB_SelectValue(ctlID, prevSel); }
				else if(selMode == 1) {
					var allSelected = prevSel.split(",");
					for(var i=0;i<allSelected.length;i++) {
						if(allSelected[i] != "") { ELB_SelectValue(ctlID, allSelected[i]); }
						}
					}
				}
			}
		}
	elbRequest.open('GET', sRequestPath, true);
	elbRequest.send(null);
	}

function ELB_doResize(ctlID) {
    if(ctlID == "[object]" || !ctlID) {
        ctlID = window.event.srcElement.id;
        ctlID = ctlID.substring(0, ctlID.indexOf("_"));
        }

    var oHead = getObj(ctlID + "_listHead");
    var oList = getObj(ctlID + "_dropList");
    var oHeadText = getObj(ctlID + "_listHeadText");
    var oClipText;
    if(oHead.getAttribute("clipHead") == "true") { oClipText = oHeadText.getElementsByTagName("SPAN")[0]; }

    //window.status = "Resizing... list head width = " + oHead.style.pixelWidth + ";  clip text width = " + oClipText.style.pixelWidth +
    //    ";  SrcElement = " + window.event.srcElement.id;

    oClipText.style.pixelWidth = oHead.style.pixelWidth - 32;

    if(oList.style.visibility == "visible") { ELB_expandList(ctlID); }
    else { oList.style.pixelWidth = oHead.style.pixelWidth; }
    }

function ELB_SwapItem(sourceID, targetID) {
    var tblSource = getObj(sourceID + "_dropListTable");
    var tblTarget = getObj(targetID + "_dropListTable");
    var rowTempOld;
    var rowTempNew;
    var celTempNew;
    var celTempOld;

    ELB_ClearSelection(targetID);

    for(var i=1;i<tblSource.rows.length;i++) {
        rowTempOld = tblSource.rows[i];
        rowTempOld.id = sourceID + "_Row" + i;
        if(rowTempOld.getAttribute("selected")) {
/*
	0 = value
	1 = text
	2 = text2
	3 = text3
	4 = filter
*/
		var aValues = new Array();
		aValues[0] = rowTempOld.getAttribute("selectvalue");
		if(binIE) {
			aValues[1] = rowTempOld.cells[0].innerHTML;
			if(rowTempOld.cells.length > 1) { aValues[2] = rowTempOld.cells[1].innerHTML; }
			if(rowTempOld.cells.length > 2) { aValues[3] = rowTempOld.cells[2].innerHTML; }
			}
		else {
			aValues[1] = rowTempOld.cells[0].childNodes[0].innerHTML;
			if(rowTempOld.cells.length > 1) { aValues[2] = rowTempOld.cells[1].childNodes[0].innerHTML; }
			if(rowTempOld.cells.length > 2) { aValues[3] = rowTempOld.cells[2].childNodes[0].innerHTML; }
			}
		ELB_AddItem(targetID, aValues, true);
		ELB_CommitAdd(targetID);

	        tblSource.deleteRow(i);

		ELB_SelectValue(targetID, aValues[0]);
	        if(tblSource.getAttribute("swapTarget")) { ELB_RollUp(sourceID); }
	        if(tblTarget.getAttribute("swapTarget")) { ELB_RollUp(targetID); }
	        i--;
	        }
        }
	ELB_ClearSelection(sourceID);
    	if(getObj(sourceID + "_listHead").getAttribute("displayMode") != "listbox") { ELB_retractList(sourceID); }
    }

function ELB_MoveItem(ctlID, moveIndex) {
    var tblTemp = getObj(ctlID + "_dropListTable");
    var rowTemp;
    var cloneFrom;
    var cloneTo;
    if(moveIndex < 0) {
        for(var iRow=1;iRow<tblTemp.rows.length;iRow++) {
            rowTemp = tblTemp.rows[iRow];
            if(rowTemp.getAttribute("selected") && (iRow + moveIndex > 0) && (iRow + moveIndex < tblTemp.rows.length)) {
                //tblTemp.moveRow(iRow, (iRow + moveIndex));
                cloneFrom = rowTemp.cloneNode(true);
                cloneTo = tblTemp.rows[iRow + moveIndex].cloneNode(true);
                //alert("Replacing \n" + tblTemp.rows[iRow + moveIndex].innerHTML + "\nwith\n" + cloneFrom.innerHTML);
                tblTemp.tBodies[0].replaceChild(cloneFrom, tblTemp.rows[iRow + moveIndex]);
                //alert("Replacing \n" + rowTemp.innerHTML + "\nwith\n" + cloneTo.innerHTML);
                tblTemp.tBodies[0].replaceChild(cloneTo, tblTemp.rows[iRow]);
                }
            }
        }
    else {
        for(var iRow=tblTemp.rows.length-1;iRow>0;iRow--) {
            rowTemp = tblTemp.rows[iRow];
            if(rowTemp.getAttribute("selected") && (iRow + moveIndex > 0) && (iRow + moveIndex < tblTemp.rows.length)) {
                //tblTemp.moveRow(iRow, (iRow + moveIndex));
                cloneFrom = rowTemp.cloneNode(true);
                cloneTo = tblTemp.rows[iRow + moveIndex].cloneNode(true);
                tblTemp.tBodies[0].replaceChild(cloneFrom, tblTemp.rows[iRow + moveIndex]);
                tblTemp.tBodies[0].replaceChild(cloneTo, tblTemp.rows[iRow]);
                }
            }
        }

    if(tblTemp.getAttribute("swapTarget")) { ELB_RollUp(ctlID); }
    }

function ELB_RollUp(ctlID) {
    var sValue = "";
    var sText = "";
    var sText2 = "";
    var sText3 = "";
    var tblTemp = getObj(ctlID + "_dropListTable");
    var rowTemp;

    for(var iRow=1;iRow<tblTemp.rows.length;iRow++) {
        rowTemp = tblTemp.rows[iRow];
        rowTemp.id = ctlID + "_Row" + iRow;
        sText += ELB_filterHTML(rowTemp.cells[0].innerHTML);
        if(rowTemp.cells.length > 1) { sText2 += ELB_filterHTML(rowTemp.cells[1].innerHTML); }
        if(rowTemp.cells.length > 2) { sText3 += ELB_filterHTML(rowTemp.cells[2].innerHTML); }

        sValue += rowTemp.getAttribute("selectvalue");
        if(sValue != "" && iRow != (tblTemp.rows.length - 1)) {
            sValue += "|";
            sText += "|";
            if(rowTemp.cells.length > 1) { sText2 += "|"; }
            if(rowTemp.cells.length > 2) { sText3 += "|"; }
            }
        }
    getObj(ctlID).value = sValue;
    getObj(ctlID + "_SelectedText").value = sText;
    getObj(ctlID + "_SelectedText2").value = sText2;
    getObj(ctlID + "_SelectedText3").value = sText3;
    }

function ELB_AddItem(ctlID, arrValues, binQueue) {
    var tblTarget = getObj(ctlID + "_dropListTable");
	var allHTML;
	var iBufferLength = ELB_ItemBuffer[ctlID].length;

	if(!binQueue) { allHTML = tblTarget.outerHTML.replace("</TABLE>", "").replace("</TBODY>", ""); }
	var rowTemp = ELB_PrimeRows[ctlID];

	rowTemp = rowTemp.split("_Row0").join("_Row" + (tblTarget.rows.length + ELB_ItemBuffer[ctlID].length));
	rowTemp = rowTemp.replace("DISPLAY: none", "");
	rowTemp = rowTemp.split(".nullvalue").join(arrValues[0]);
	rowTemp = rowTemp.replace("selected=\"false\"", "");
	rowTemp = rowTemp.replace("ELB_checkActive('" + ctlID + "');", "");
	if(arrValues[4]) {	rowTemp = rowTemp.replace(".nullfilter", arrValues[4]); }
	if(arrValues[3]) {	rowTemp = rowTemp.replace(".nulltext3", arrValues[3]); }
	if(arrValues[2]) {	rowTemp = rowTemp.replace(".nulltext2", arrValues[2]); }
	if(arrValues[1]) { rowTemp = rowTemp.split(".nulltext").join(arrValues[1]); }
	else { rowTemp = rowTemp.replace(".nulltext", arrValues[0]); }

	//alert("New HTML to add: \n \n " + rowTemp + "\n\nQueueing = " + binQueue); // Debugging
	if(binQueue) {
		var sBody = binIE ? "TBODY" : "tbody";
		ELB_ItemBuffer[ctlID][iBufferLength] = rowTemp.replace("</" + sBody + ">", "").replace("<" + sBody + ">", "");
		}
	else {
		allHTML += rowTemp;
		tblTarget.outerHTML = allHTML.replace("</TBODY>\r\n<TBODY>", "").replace("</TBODY>\r\n<TBODY>") + "</TABLE>";
	    if(tblTarget.getAttribute("swapTarget")) { ELB_RollUp(ctlID); }
		}
    }

function ELB_CommitAdd(ctlID) {
	if(ELB_ItemBuffer[ctlID].length < 1) { return false; }
	else {
		var tblList = getObj(ctlID + "_dropListTable");
		var oList = getObj(ctlID + "_dropList");
		var allHTML;
		var sBody = binIE ? "TBODY" : "tbody";
		var sTable = binIE ? "TABLE" : "table";
		allHTML = oList.innerHTML.replace("</" + sTable + ">", "").replace("</" + sBody + ">", "");

		oList.innerHTML = allHTML + ELB_ItemBuffer[ctlID].join("") + "</table>";
		ELB_ItemBuffer[ctlID] = new Array();
	    if(tblList.getAttribute("swapTarget")) { ELB_RollUp(ctlID); }
		}
	try {
		if(getObj(ctlID + "_listHead").getAttribute("displayMode").toLowerCase() == "listbox") {
			var oTemp = getObj(ctlID + "_dropList");
			if(oTemp.style.visibility == "hidden") { return; }
			oTemp.focus();
			oTemp.blur();
			}
		else {
			var oTemp = getObj(ctlID + "_listHead");
			if(oTemp.style.visibility == "hidden") { return; }
			oTemp.focus();
			oTemp.blur();
			}
		}
	catch (ex) {		}
	}

function ELB_RemoveItem(ctlID, rowIndex) {
	rowIndex = parseInt(rowIndex);
    var tblList = getObj(ctlID + "_dropListTable");
    tblList.deleteRow(rowIndex);
    if(tblList.swapTarget) { ELB_RollUp(ctlID); }
    }

function ELB_ClearSelection(ctlID, e) {
    var oHead = getObj(ctlID + "_listHead");
    var oList = getObj(ctlID + "_dropListTable");

    if(oList.title.length > 0) { oList.title = ""; }
    if(oHead.title.length > 0) { oHead.title = ""; }

    if(ELB_ChildLists[ctlID]) { // Clear out the children
        ELB_SetChildren(ELB_ChildLists[ctlID], "aybabtuaybabtuaybabtu");
        }
    if(oList.getAttribute("swapTarget") == "true") {
        for(var i=0;i<oList.rows.length;i++) {
            if(oList.rows[i].getAttribute("selected") == "true") {
                oList.rows[i].removeAttribute("selected");
                ELB_Rollout(oList.rows[i], e);
                }
            }
	if(getObj(ctlID + "_clipText")) { getObj(ctlID + "_clipText").innerHTML = ""; }
        else { getObj(ctlID + "_listHeadText").innerHTML = ""; }
        }
    else if(ELB_listMode[ctlID] == 1) {
        for(var i=0;i<oList.rows.length;i++) {
            if(oList.rows[i].getAttribute("selected") == "true") {
                ELB_SelectItem(ctlID, oList.rows[i].id);
                ELB_Rollout(oList.rows[i], e);
                }
            }
        }
    else {
        var selIndex = parseInt(getObj(ctlID + "_SelectedIndex").value);
        var displayMode = getObj(ctlID + "_listHead").getAttribute("displayMode");
        if(displayMode == "combo") { getObj(ctlID + "_comboText").value = "" }
        else if(oHead.getAttribute("clipHead") == "true") {
			if(binIE) { getObj(ctlID + "_clipText").innerText = ""; }
			else { getObj(ctlID + "_clipText").innerHTML = ""; }
			}
        else {
			if(binIE) { getObj(ctlID + "_listHeadText").innerText = ""; }
			else { getObj(ctlID + "_listHeadText").innerHTML = ""; }
			}
        if(selIndex > -1) {
            var tempRow = oList.rows[selIndex];
			if(tempRow) {
	            tempRow.removeAttribute("selected");
	            ELB_Rollout(tempRow, e);
				}
            }
        getObj(ctlID).value = "";
        getObj(ctlID + "_SelectedIndex").value = -1;
        getObj(ctlID + "_SelectedText").value = "";
        getObj(ctlID + "_SelectedText2").value = "";
        getObj(ctlID + "_SelectedText3").value = "";
        }
    if((oHead.getAttribute("promptText") != "") && (oHead.getAttribute("displayMode") != "combo")) {
	if(getObj(ctlID + "_clipText")) { getObj(ctlID + "_clipText").innerHTML = oHead.getAttribute("promptText"); }
        else { getObj(ctlID + "_listHeadText").innerHTML = oHead.getAttribute("promptText"); }
	//if(binIE) { getObj(ctlID + "_listHeadText").innerText = oHead.promptText; }
	//else { getObj(ctlID + "_listHeadText").innerHTML = oHead.getAttribute("promptText"); }
	}
    else if(oHead.getAttribute("displayMode") != "combo") {
	if(getObj(ctlID + "_clipText")) { getObj(ctlID + "_clipText").innerHTML = ""; }
        else { getObj(ctlID + "_listHeadText").innerHTML = ""; }
	}
    }

function ELB_SelectAll(ctlID) {
    var oList = getObj(ctlID + "_dropListTable");
    if(ELB_listMode[ctlID] == 1) {
        for(var i=1;i<oList.rows.length;i++) {
            if(!oList.rows[i].getAttribute("selected")) {
                ELB_SelectItem(ctlID, oList.rows[i].id);
                ELB_Rollover(oList.rows[i]);
                }
            }
        }
    }

function ELB_FocusHead(ctlID) {
    var oHeadText = getObj(ctlID + "_listHeadText");
    var oHead = getObj(ctlID + "_listHead");
    currentKeyDown = 0;

    if(ctlID != sSpeedSearchID) { sSpeedSearchID = ctlID; sSpeedSearch = ""; }

    if(oHead.displayMode == "combo") {
        currentKeyDown = 0;
		getObj(ctlID + "_comboText").select();
        }
    else {
        oHeadText.style.color = ELB_ForeColorRoll[ctlID];
        oHeadText.parentNode.parentNode.style.background = ELB_BackColorRoll[ctlID];
        }
    }

function ELB_HideList(ctlID, binCollapse) {
    var sDisplayMode = getObj(ctlID + "_listHead").displayMode;
    var oList;
    if(sDisplayMode == "listbox") {
        oList = getObj(ctlID + "_dropList");
        }
    else {
        oList = getObj(ctlID + "_listHead");
        }
    if(binCollapse) {
        oList.style.display = "none";
        }
    else {
        oList.style.visibility = "hidden";
        }
    }

function ELB_ShowList(ctlID) {
    var sDisplayMode = getObj(ctlID + "_listHead").displayMode;
    if(sDisplayMode == "listbox") {
        getObj(ctlID + "_dropList").style.visibility = "visible";
        getObj(ctlID + "_dropList").style.display = "block";
        }
    else {
        getObj(ctlID + "_listHead").style.visibility = "visible";
        getObj(ctlID + "_listHead").style.display = "block";
        }
    }

function ELB_GetValue(ctlID) {
    return getObj(ctlID).value;
    }

function ELB_GetText(ctlID) {
	return getObj(ctlID + "_SelectedText").value;
	}

function ELB_SelectValue(ctlID, sVal) {
    var tblList = getObj(ctlID + "_dropListTable");
    var rowName;
    var rowVal;
	var i=0;
	var theRow;

   	if(binIE) { // Use the Enumerator object for IE
		var tmpEnum = new Enumerator(tblList.rows);
		for (;!tmpEnum.atEnd();tmpEnum.moveNext()) {
	       theRow = tmpEnum.item();
	       rowName = ctlID + "_Row" + i;
	       rowVal = theRow.selectvalue;
	       if(rowVal == sVal) {
	           ELB_SelectItem(ctlID, rowName);
	           return true;
	           }
	       i++;
	       }
		}
	else { // old-school table running for Firefox
	    for(var i=1;i<tblList.rows.length;i++) { // And if it's an alphanumeric...
	        if(currentKeyDown > 0) { return; }
	        theRow = tblList.rows[i];
			//alert("Checking row " + i + " for value " + sVal + " for " + ctlID); // Debugging
	        if(theRow.getAttribute("selectvalue") == sVal) {
	            ELB_SelectItem(ctlID, theRow.id);
	            return;
	            }
			}
		}

    return false;
    }

function ELB_SelectText(ctlID, sText) {
    var tblList = getObj(ctlID + "_dropListTable");
    var rowName;
    var rowText;
    for(var i=1; i<tblList.rows.length;i++) {
        rowName = ctlID + "_Row" + i;
        rowText = tblList.rows[i].cells[0].innerText;
        if(rowText == sText) {
            ELB_SelectItem(ctlID, rowName);
            return true;
            }
        }
        return false;
    }

function ELB_SelectItem(ctlID, rowID, e) { // Select an item for posting or just keeping around
    if(rowID.indexOf("displayField") > 0) { return; }
	if(ctlID.replace(" ", "") == "") { return; }

    var noChange = getObj(ctlID).noChange;
	var theKey = "";
	if(window.event) { theKey = window.event.keyCode; }
	else if(e) { theKey = e.which; }
    var binChange = false;
    var itemRow = getObj(rowID);
	var binUseAjax = (getObj(ctlID).getAttribute("useajax")=="true") ? true : false;

    if(rowID.indexOf("_sC") > 0) { itemRow = itemRow.parentNode; } // in case of checkboxes

    rowID = itemRow.id;

    var selectIndex = parseInt(rowID.replace(ctlID + "_Row", ""));
    var changeProc = getObj(ctlID).myChange;
    var itemValue;

	itemValue = itemRow.getAttribute("selectvalue");
    if(itemValue.indexOf(sNoSelect) >= 0) { return; }

    var itemName;
    var selText;
    var oHead = getObj(ctlID + "_listHead");
    var displayMode = oHead.getAttribute("displayMode");
	sToolTipPrompt = oHead.getAttribute("ToolTipPrompt");
    var tblTemp = getObj(ctlID + "_dropListTable");

    var currentVal = getObj(ctlID);
    var currentText = getObj(ctlID + "_SelectedText");
    var currentText2 = getObj(ctlID + "_SelectedText2");
    var currentText3 = getObj(ctlID + "_SelectedText3");

    if(displayMode == "combo") { // No formatting for combo box
		if(binIE) { itemName = getObj(rowID).childNodes[0].innerText; selText = itemName; }
		else { itemName = ELB_filterHTML(itemRow.cells[0].childNodes[0].innerHTML); selText = itemName; }
		}
    else if(binIE) { itemName = itemRow.childNodes[0].innerHTML; selText = itemRow.childNodes[0].innerText; }
	else { itemName = itemRow.cells[0].childNodes[0].innerHTML; selText = ELB_filterHTML(itemName); }

    if(ELB_listMode[ctlID] == 1) { // Multiple
        var currentSelIndex = getObj(ctlID + "_SelectedIndex");

        if(!itemRow.getAttribute("selected")) { // Add selected value
            if(currentVal.value != "") {
                currentSelIndex.value = currentSelIndex.value + selectIndex + ",";
                currentVal.value = currentVal.value + itemValue + ",";
                currentText.value = currentText.value + selText + ",";
                if(itemRow.cells[1]) { currentText2.value = binIE ? currentText2.value + itemRow.cells[1].innerText + "," : currentText2.value + ELB_filterHTML(itemRow.cells[1].childNodes[0].innerHTML) + ","; }
                if(itemRow.cells[2]) { currentText3.value = binIE ? currentText3.value + itemRow.cells[2].innerText + "," : currentText2.value + ELB_filterHTML(itemRow.cells[2].childNodes[0].innerHTML) + ","; }
                }
            else {
                currentSelIndex.value = "," + selectIndex + ",";
                currentVal.value = "," + itemValue + ",";
                currentText.value = "," + selText + ",";
                if(itemRow.cells[1]) { currentText2.value = binIE ? "," + itemRow.cells[1].innerText + "," : "," + ELB_filterHTML(itemRow.cells[1].childNodes[0].innerHTML) + ","; }
                if(itemRow.cells[2]) { currentText3.value = binIE ? "," + itemRow.cells[2].innerText + "," : "," + ELB_filterHTML(itemRow.cells[2].childNodes[0].innerHTML) + ","; }
                }
            itemRow.setAttribute("selected", "true");
            if(oHead.getAttribute("showCheckBoxes") != "true") { ELB_Rollover(itemRow, true); }
            }
        else { // Remove selected value
            currentVal.value = currentVal.value.replace("," + itemValue, "");
            currentSelIndex.value = currentSelIndex.value.replace("," + selectIndex, "");
            currentText.value = currentText.value.replace("," + selText, "");
            if(itemRow.cells[1]) { currentText2.value = binIE ? itemRow.cells[1].innerText : ELB_filterHTML(itemRow.cells[1].childNodes[0].innerHTML); }
            if(itemRow.cells[2]) { currentText3.value = binIE ? itemRow.cells[2].innerText : ELB_filterHTML(itemRow.cells[2].childNodes[0].innerHTML); }
            itemRow.removeAttribute("selected");
            ELB_Rollout(itemRow, e);
            }

        var sTitle = "";
        var cRows = tblTemp.rows;
        var i = 0;
        var binSelected = false;
        for(i=1;i<cRows.length;i++) {
            if(cRows[i].getAttribute("selected")) {
                binSelected = true;
                sTitle = binIE ? (sTitle + "<br />" + cRows[i].cells[0].innerText) : (sTitle + "<br />" + ELB_filterHTML(cRows[i].cells[0].childNodes[0].innerHTML));
                }
            }
        ELB_displaySelected(ctlID, sTitle);
        if(binSelected == false) {
            currentSelIndex.value = -1;
            currentVal.value = "";
            currentText.value = "";
            }

        if (window.event != null){
            var oEvent = document.createEventObject(window.event);
            if(oEvent.ctrlKey) {
                document.selection.empty();
                }
            }
        if(sSearch == ctlID) { getObj(ctlID + "_searchBox").focus() }
        else if(binIE && getObj(ctlID + "_dropList").style.visibility != "hidden") { getObj(ctlID + "_dropList").focus() }
		binChange = true;
        }
    else { // Single, dropdown format
        if(rowID == "nothing") {
            getObj(ctlID).value = "";
            getObj(ctlID + "_SelectedIndex").value = -1;
            getObj(ctlID + "_SelectedText").value = "";
            return;
            }
		var prevIndex = parseInt(getObj(ctlID + "_SelectedIndex").value);
		//if(binUseAjax && displayMode == "combo") { prevIndex = -1; }
		if(prevIndex > -1) {
        	var prevRow = getObj(ctlID + "_dropListTable").rows[parseInt(getObj(ctlID + "_SelectedIndex").value)];
			if(prevRow) { prevRow.removeAttribute("selected"); }
            ELB_Rollout(prevRow, e);
            }

        itemRow.setAttribute("selected", "true");
        ELB_Rollover(itemRow, true);

        getObj(ctlID + "_SelectedIndex").value = selectIndex;
        getObj(ctlID).selectedIndex = selectIndex;

        if(itemValue != getObj(ctlID).value) {
            // Check for new value to trigger the onChange event
            binChange = true;
            }
        if(itemValue != "") {
            getObj(ctlID).value = itemValue;
            getObj(ctlID).text = selText;
            getObj(ctlID + "_SelectedText").value = selText;
            if(itemRow.cells[1]) { currentText2.value = itemRow.cells[1].innerText; }
            if(itemRow.cells[2]) { currentText3.value = itemRow.cells[2].innerText; }
            if(!noChange) {
                if(displayMode == "combo") {
                    getObj(ctlID + "_comboText").value = itemName;
                    }
                else {
                    if(oHead.clipHead == "true") {
                        getObj(ctlID + "_listHeadText").childNodes[0].innerHTML = itemName;
                        }
                    else {
                        getObj(ctlID + "_listHeadText").innerHTML = itemName;
                        }
                    }
                }
            }
        if(displayMode != "listbox" && theKey != 38 && theKey != 40) {
            // Only retract the list and focus the head if we didn't just do an up or down arrow (and if it's a dropdown/combo).
            ELB_retractList(ctlID);
            if(displayMode != "combo") {
                if(binIE) { getObj(ctlID + "_listHeadText").focus(); }
                }
            else { selText = itemName; }
            }
        else if(displayMode == "listbox" && binIE) {
            if(getObj(ctlID + "_dropList").style.visibility == "visible") { getObj(ctlID + "_dropList").focus();}
            }
        getObj(ctlID + "_dropListTable").title = sToolTipPrompt + "  " + ELB_filterHTML(selText);
        oHead.title = sToolTipPrompt + "  " + ELB_filterHTML(selText);
        if(displayMode == "combo") { getObj(ctlID + "_comboText").title = sToolTipPrompt + "  " + ELB_filterHTML(selText); }
        }
    if(binChange) {
        eval(getObj(ctlID).myChange);
        }
	var sOldValue = getObj(ctlID + "_old").value;
    if(binIE && getObj(ctlID).getAttribute("autopostback") && document.activeElement.id != (ctlID + "_comboText") && getObj(ctlID).value != sOldValue) {
        if(getObj(ctlID).getAttribute("autopostback") == true) {
			__doPostBack(getObj(ctlID).getAttribute("postbackname"), '');
			}
        }
    else if(getObj(ctlID).getAttribute("autopostback") ) {
        if(getObj(ctlID).getAttribute("autopostback") == "true") {
			__doPostBack(getObj(ctlID).getAttribute("postbackname"), '');
			}
        }

    if(oHead.getAttribute("showCheckBoxes") == "true") {
        if(itemRow.getAttribute("selected")) {
            if(binIE) { itemRow.cells[0].childNodes[0].checked = true; }
			else { itemRow.cells[0].childNodes[0].getElementsByTagName("INPUT")[0].checked = true; }
            }
        else {
            if(binIE) { itemRow.cells[0].childNodes[0].checked = false; }
			else { itemRow.cells[0].childNodes[0].getElementsByTagName("INPUT")[0].checked = false; }
            }
        }

    //if(ELB_ChildLists[ctlID]) { ELB_SetChildren(ELB_ChildLists[ctlID], itemValue); }
	if(ELB_ChildLists[ctlID]) { ELB_SetChildren(ELB_ChildLists[ctlID], currentVal.value); }

    if(tblTemp.swapTarget) { ELB_RollUp(ctlID); }

    if(getObj(ctlID + "_listHead").getAttribute("displayMode") == "combo" && (theKey == 38 || theKey == 40) ) { getObj(ctlID + "_comboText").select(); }

    if(binIE) {
        itemRow.blur();
        getObj(ctlID + "_dropListTable").blur();
        getObj(ctlID + "_dropList").blur();
        //document.body.focus();
        }

    if(noChange) {
        getObj(ctlID + "_SelectedIndex").value = 0;
        getObj(ctlID).selectedIndex = 0;
        getObj(ctlID).value = "";
        getObj(ctlID + "_SelectedText").value = "";

        getObj(ctlID).text = "";
        itemRow.removeAttribute("selected");
        ELB_Rollout(itemRow, e);
        }
    }

function ELB_SetChildren(arrChildren, filterVal) {
    for(var iChild = 0;iChild < arrChildren.length;iChild++) {
        if(arrChildren[iChild] != "" && getObj(arrChildren[iChild])) {
            ELB_FillChildList(arrChildren[iChild], filterVal);
            }
        }
    }

function ELB_ClearList(ctlID) {
    var tblList = getObj(ctlID + "_dropListTable");
    if(tblList.rows.length > 1) { ELB_ClearSelection(ctlID); }
    var iChildRows = 0;
    var iCount = tblList.rows.length;
    for(iChildRows=iCount-1;iChildRows>0;iChildRows--) { tblList.deleteRow(iChildRows); }
    //if(getObj(ctlID + "_noResults")) { getObj(ctlID + "_noResults").style.display = "inline"; }
	}

function ELB_FillChildList(ctlID, filterVal, prevSel) {
	var binUseAjax = (getObj(ctlID).getAttribute("useajax") == "true") ? true : false;
	if(binUseAjax) { ELB_SendRequest(ctlID, filterVal, "related", prevSel); return; }

    var arrListItems = ELB_ChildListsItems[ctlID];
    var tblList = getObj(ctlID + "_dropListTable");
    var selMode = ELB_listMode[ctlID];
    var guideRow = tblList.rows[0];
    var guideCell = guideRow.cells[0];

    var arrItem;
    var rowTemp;
    var celTemp;

    var iChildRows = 0;
    var iSourceRows = 0;
    var iNewRowIndex = 0;
    var iCount = tblList.rows.length;

    var parentList = ELB_ParentLists[ctlID];
	var binMulti = (ELB_listMode[parentList] == 1) ? true : false;

    if(tblList.rows.length > 1) { ELB_ClearSelection(ctlID); }

    for(iChildRows=iCount-1;iChildRows>0;iChildRows--) { tblList.deleteRow(iChildRows); }

    if(getObj(ctlID + "_listHead").getAttribute("displayMode") == "dropdown") { tblList.rows[0].style.display = "block"; }

    if(!binMulti) { filterVal = "," + filterVal + ","; }

    for(iSourceRows=1;iSourceRows<arrListItems.length;iSourceRows++) {
        arrItem = arrListItems[iSourceRows];

		//alert(" on row " + iSource + " for " + ctlID + " with selected value " + getObj(ctlID) + "\nfilterVal: " + filterVal);  // Debugging

        //if(arrItem[0] == filterVal) {
		if(filterVal.indexOf("," + arrItem[0] + ",") >= 0) {
            iNewRowIndex++;
            if(selMode == 0 && arrItem[1] == getObj(ctlID).value) { getObj(ctlID + "_SelectedIndex").value = iNewRowIndex; }
			if(binIE || !binIE) { ELB_AddItem(ctlID, new Array(arrItem[1], arrItem[2], arrItem[3], arrItem[4]), true); }
            }

        }
	ELB_CommitAdd(ctlID);
    if(getObj(ctlID + "_noResults")) { getObj(ctlID + "_noResults").style.display = "none"; }
    if(tblList.rows[0]) { tblList.rows[0].style.display = "none"; }
    getObj(ctlID + "_dropListTable").rows[0].style.display = "none";
	if(prevSel) { ELB_SelectValue(ctlID, prevSel); }
    }

function ELB_FillList(ctlID) {
	var itemsArray;
	var rowArray = new Array();
	var shiftedArray = new Array();

	if(ELB_ChildListsItems[ctlID]) {
		itemsArray = ELB_ChildListsItems[ctlID];
		}
	else { return; }

	for(var i=1;i<itemsArray.length;i++) {
		rowArray = itemsArray[i];
		shiftedArray[0] = rowArray[1];
		shiftedArray[1] = rowArray[2];
		shiftedArray[2] = rowArray[3];
		shiftedArray[3] = rowArray[4];
		shiftedArray[4] = rowArray[0];
		ELB_AddItem(ctlID, shiftedArray, true);
		}
	ELB_CommitAdd(ctlID);
	if(binIE) { getObj(ctlID + "_dropList").click(); document.body.click(); }
	}

function ELB_filterHTML(sText) {
	//sText = sText.replace(/<\/?\s*img[^>]*>/gi, "" );
	sText = sText.replace(/<\/?\s*[^>]*>/gi, "" );
	sText = sText.replace(/&nbsp;/, " ");
    sText = sText.replace(String.fromCharCode(160), "");
	return sText;
	}

function ELB_displaySelected(ctlID, strSelections) {
    var selectedList = strSelections;

    if(getObj(ctlID + "_displayField") && getObj(ctlID)) {
        var myDisplay = getObj(ctlID + "_displayField");
        //if(strSelections) {
        //    if(selectedList.indexOf("<br />") == 0) { selectedList = selectedList.substr(6) }
        //    myDisplay.innerHTML = selectedList; //.split("<br />").join("\n");
        //    }
        //else {
            selectedList = "";
            var myRows = getObj(ctlID + "_dropListTable").rows;
            for(var iRow=1;iRow<myRows.length;iRow++) {
                if(myRows[iRow].getAttribute("selected") && selectedList == "") {
                    selectedList = "<div onclick=\"ELB_SelectValue('" + ctlID + "', '" +
                     myRows[iRow].getAttribute("selectvalue") + "')\" style='cursor:hand;border-width:0px;' class='" + myDisplay.className + "'>" +
                     ELB_filterHTML(myRows[iRow].cells[0].innerHTML) + "</div>"
                    }
                else if(myRows[iRow].getAttribute("selected")) {
                     selectedList += "<div onclick=\"ELB_SelectValue('" + ctlID + "', '" +
                     myRows[iRow].getAttribute("selectvalue") + "')\" style='cursor:hand;border-width:0px;' class='" + myDisplay.className + "'>" +
                     ELB_filterHTML(myRows[iRow].cells[0].innerHTML) + "</div>" }
                }
            myDisplay.innerHTML = selectedList;
        //    }
        }
    else if(getObj(ctlID)) {
        var oHead = getObj(ctlID + "_listHead");
		sToolTipPrompt = oHead.getAttribute("ToolTipPrompt");
        getObj(ctlID + "_dropListTable").title = sToolTipPrompt + "\n" + ELB_filterHTML(selectedList.split("<br />").join("\n"));
        oHead.title = getObj(ctlID + "_dropListTable").title;
        if(oHead.getAttribute("displayMode") == "dropdown" && ELB_listMode[ctlID] == 1)  {
            var sNewText = getObj(ctlID + "_dropListTable").title.replace(sToolTipPrompt + "\n\n", "").split("\n").join(",");
            if(sNewText.replace(sToolTipPrompt + ",", "").length < 3) { sNewText = ""; }
            if(oHead.getAttribute("clipHead") == "true") {
                getObj(ctlID + "_clipText").innerHTML = sNewText;
                }
            else {
                getObj(ctlID + "_listHeadText").innerHTML = sNewText;
                }
            }
        }
    }

function ELB_checkActiveMoz(ctlID, e) {
    var comboText = getObj(ctlID + "_comboText");
    if(comboText.getAttribute("limitToList") == "true" && comboText.value != "" &&
				getObj(ctlID).getAttribute("useajax") == "false" &&
        parseInt(getObj(ctlID + "_SelectedIndex").value) == -1) {
        e.stopPropagation();
        e.preventDefault();
        alert(comboText.getAttribute("restrictionMessage"));
        ELB_ClearSelection(ctlID);
        return;
        }
    else if(comboText.value != "" && parseInt(getObj(ctlID + "_SelectedIndex").value) == -1) {
        getObj(ctlID).value = comboText.value;
        if(getObj(ctlID).autopostback == true && (getObj(ctlID).value != getObj(ctlID + "_old").value)) {
            __doPostBack(getObj(ctlID).getAttribute("postbackname"), '');
            }
        }
    else {
        if(getObj(ctlID).autopostback == true && (getObj(ctlID).value != getObj(ctlID + "_old").value)) {
            __doPostBack(getObj(ctlID).getAttribute("postbackname"), '');
            }
        }
    comboText.style.color = ELB_ForeColor[ctlID]
    }

function ELB_checkActive(ctlID, e) {

    if(!ctlID) {
        ctlID = window.event.srcElement.id;
        ctlID = ctlID.substring(0, ctlID.indexOf("_"));
        }

    var theID = document.activeElement.id;
    var parentID = document.activeElement.parentNode.id;
    var displayMode = getObj(ctlID + "_listHead").getAttribute("displayMode");
    var tblTemp = getObj(ctlID + "_dropListTable");
    if(ELB_listMode[ctlID] == 1) { document.selection.empty() }

    theID = "  element: " + theID + "   parent: " + parentID;

    if(theID.indexOf(sSpeedSearchID) < 0) { sSpeedSearch = ""; }

    if(theID.indexOf(ctlID) == -1 && theID.indexOf(ctlID + "_listHead") == -1) {
        var oHeadText = getObj(ctlID + "_listHeadText");
        var oHead = getObj(ctlID + "_listHead");
        var comboText = getObj(ctlID + "_comboText");

        if(oHead.getAttribute("displayMode") == "combo") {
            if(comboText.getAttribute("limitToList") == "true" && comboText.value != "" &&
				getObj(ctlID).getAttribute("useajax") == "false" &&
                parseInt(getObj(ctlID + "_SelectedIndex").value) == -1) {
                alert(comboText.getAttribute("restrictionMessage"));
                comboText.select();
                return;
                }
            else if(comboText.value != "" && parseInt(getObj(ctlID + "_SelectedIndex").value) == -1) {
                getObj(ctlID).value = comboText.value;
                if(getObj(ctlID).autopostback == true && (document.activeElement.id.indexOf(ctlID) < 0) && (getObj(ctlID).value != getObj(ctlID + "_old").value)) {
                    __doPostBack(getObj(ctlID).getAttribute("postbackname"), '');
                    }
                }
            else {
                if(getObj(ctlID).autopostback == true && (document.activeElement.id.indexOf(ctlID) < 0) && (getObj(ctlID).value != getObj(ctlID + "_old").value)) {
                    __doPostBack(getObj(ctlID).getAttribute("postbackname"), '');
                    }
                }
            comboText.style.color = ELB_ForeColor[ctlID]
            }
        else {
            oHeadText.style.color = ELB_ForeColor[ctlID];
            oHeadText.parentNode.parentNode.style.background = ELB_BackColor[ctlID];
            }

        if(displayMode != "listbox" && ELB_listMode[ctlID] == 0) {
            ELB_retractList(ctlID);
            }
		if(elbTip) { elbTip.style.visibility = "hidden"; }
        }
    else if(theID.indexOf("_sC") >= 0 && ELB_listMode[ctlID] != 1 && getObj(ctlID + "_listHead").getAttribute("displayMode") != "listbox") {
        ELB_SelectItem(ctlID, document.activeElement.parentNode.id);
        }
    }

function ELB_checkChange() {
    var sName = event.propertyName;
    var sID = "ELB";

    switch(sName) {
        case "selectedIndex":
            break;
        default:
            break;
        }
    }

function ELB_alterList(ctlID, e) { // Expand or retract the list, depending on its current state
    if(binIE) {
        if(getObj(ctlID + "_comboText")) {
            //if(getObj(ctlID + "_comboText").hasFocus) {
            if(document.activeElement.id == ctlID + "_comboText") { ELB_expandList(ctlID); return;}
            }
        }
    if(ELB_listState[ctlID] == 0) {
        ELB_expandList(ctlID);
        }
    else {
        ELB_retractList(ctlID);
        }
    }

function ELB_expandList(ctlID, e) {
    var listID = ctlID + "_dropList";
    var headID = ctlID + "_listHead";

    var oHead = getObj(headID);
    var oList = getObj(listID);
    var selIndex = parseInt(getObj(ctlID + "_SelectedIndex").value);

    var topCorrection = 0;
    var oParent = oHead.offsetParent;

    if(oParent.tagName == "TD" || 1==1) {
        if(oParent.vAlign == "bottom" || 1==1) {
            topCorrection = oHead.offsetHeight;
            if(oParent.style.borderTopWidth) {
                 topCorrection += parseInt(oParent.style.borderTopWidth)*2 + parseInt(oHead.style.borderWidth);
                 }
            }
        }

    oList.style.left = oHead.offsetLeft;
    oList.style.pixelTop = oHead.offsetTop + oHead.offsetHeight + topCorrection;
    oList.style.width = oHead.offsetWidth;
    oList.style.background = oHead.style.background;
    oList.style.zIndex = 999; //oHead.style.zIndex + 100;

    var theParent = oHead;
    var posX = oHead.offsetLeft;
    var posY = oHead.offsetTop;
	//alert("Adding " + oHead.offsetTop + "; total " + posY);

    while(theParent.offsetParent) {
        theParent = theParent.offsetParent;
        if(theParent.style.position.toLowerCase() == "relative") {
            posX += theParent.offsetLeft/2; - theParent.scrollLeft;
            posY += theParent.offsetTop/2; - theParent.scrollTop;
//            alert("relative: " + theParent.tagName);
            }
        else {
            posX += theParent.offsetLeft; // - theParent.scrollLeft/2;
            posY += theParent.offsetTop; // - theParent.scrollTop/2;
            }
        }
    oList.style.pixelTop = posY + oHead.offsetHeight;

    oList.style.pixelLeft = posX;
// */
	var iHeight = binIE?oList.style.pixelHeight:parseInt(oList.style.height);

	var listBottom = oList.style.pixelTop + iHeight;

    var screenBottom = parseInt(document.body.clientHeight) + parseInt(document.body.scrollTop);
    var listTop;
	if(binIE) {listTop = -(parseInt(oHead.offsetTop) - parseInt(oList.style.height));}

	//alert(listBottom + " " + screenBottom + " " + listTop);

    if((listBottom > screenBottom) && (listTop < (listBottom - screenBottom))) {
		oList.style.pixelTop = oHead.offsetTop - (oHead.offsetHeight + iHeight) + topCorrection;
		//oList.style.pixelTop = listTop + topCorrection - (iHeight);
        }

    if(selIndex > 0 && getObj(ctlID).getAttribute("useajax") != "true") {
        var selectedRow = getObj(ctlID + "_dropListTable").rows[selIndex];
        var selectedTop = selectedRow.offsetTop;
        var selectedHeight = parseInt(selectedRow.offsetHeight);
        oList.scrollTop = ((selIndex - 1) * selectedHeight);
        }
	if(binIE) {
		ELB_Backer[ctlID].style.width = oList.offsetWidth;
	    ELB_Backer[ctlID].style.height = oList.offsetHeight;

	    ELB_Backer[ctlID].style.top = parseInt(oList.style.top);
	    ELB_Backer[ctlID].style.left = parseInt(oList.style.left);

	    ELB_Backer[ctlID].style.zIndex = oList.style.zIndex - 1;
	    ELB_Backer[ctlID].style.display = "block";
		}
	oList.top = oList.style.pixelTop + "px";
	//alert(oList.top);
    oList.style.visibility = "visible";
	if(!binIE && getObj(ctlID + "_listHead").getAttribute("displayMode") == "dropdown") { getObj(ctlID + "_listHeadFocus").select(); }

    ELB_listState[ctlID] = 1;
    }

function ELB_retractList(ctlID) {
    var listID = ctlID + "_dropList";
    var oList = getObj(listID);
    oList.style.zIndex = 0;
    if(binIE) { ELB_Backer[ctlID].style.display = "none"; }
    oList.style.visibility = "hidden";
    ELB_listState[ctlID] = 0;
	if(binIE) {
	    if(document.activeElement.id.indexOf(ctlID) > -1 && document.activeElement.id.indexOf("_listHead") > 0) {
	        var displayMode = getObj(ctlID + "_listHead").displayMode;
	        if(displayMode == "dropdown") { getObj(ctlID + "_listHeadText").focus() }
	        else if(displayMode == "combo") { getObj(ctlID + "_comboText").select() }
	        }
		}
    }

function ELB_BeginDrag(ctlID, rowTemp) {
    if(binIE) {document.selection.empty();}
    if(binIE && !rowTemp) { rowTemp = window.event.srcElement; }
    if(rowTemp.id.indexOf("_sC") >= 0) { rowTemp = rowTemp.parentNode; }
    if(!ctlID) { ctlID = rowTemp.id.substring(0, rowTemp.id.indexOf("_")); }
    if(ELB_listMode[ctlID] == 1) {
        aCurrentDrag = new Array();
        sCurrentDrag = ctlID;
        iCurrentDrag = rowTemp.rowIndex;
        aCurrentDrag[iCurrentDrag] = true;
        }
    }

function ELB_EndDrag(ctlID) {
    sCurrentDrag = "";
    iCurrentDrag = -1;
    aCurrentDrag = new Array();
    }

function ELB_DoDrag(ctlID, rowStyle) {
	if(!binIE) { return; }
    aCurrentDrag[parseInt(rowStyle.getAttribute("rowIndex"))] = true;
    if(!rowStyle.getAttribute("selected")) { ELB_SelectItem(ctlID, rowStyle.id) }
    aCurrentDrag[window.event.srcElement.rowIndex] = true;
    var upRow = getObj(ctlID + "_Row" + (rowStyle.rowIndex + 1));
    var downRow = getObj(ctlID + "_Row" + (rowStyle.rowIndex - 1));
    if(rowStyle.rowIndex == iCurrentDrag) { // It's the row where we started
        if(upRow && upRow.getAttribute("selected")) {
            ELB_SelectItem(ctlID, upRow.id);
            }
        else if(downRow && downRow.getAttribute("selected")) {
            ELB_SelectItem(ctlID, downRow.id);
            }
        }
    else if(rowStyle.rowIndex > iCurrentDrag) { // We're below the start row on the page
        for(var i=iCurrentDrag;i<rowStyle.rowIndex;i++) {
            var rowTemp = getObj(ctlID + "_Row" + i);
            if(!rowTemp.getAttribute("selected") && rowTemp.selectvalue.indexOf(sNoSelect) < 0) {
                ELB_SelectItem(ctlID, rowTemp.id);
                ELB_Rollover(rowTemp, true);
                aCurrentDrag[rowTemp.rowIndex] = true;
                }
            }
        if(upRow && upRow.getAttribute("selected")) {
            if(aCurrentDrag[upRow.rowIndex]) {
                ELB_SelectItem(ctlID, upRow.id);
                aCurrentDrag[upRow.rowIndex] = false;
                }
            }
        }
    else if(rowStyle.rowIndex < iCurrentDrag) { // We're above the start row on the page
        for(var i=iCurrentDrag;i>rowStyle.rowIndex;i--) {
            var rowTemp = getObj(ctlID + "_Row" + i);
            if(!rowTemp.getAttribute("selected") && rowTemp.selectvalue.indexOf(sNoSelect) < 0) {
                ELB_SelectItem(ctlID, rowTemp.id);
                ELB_Rollover(rowTemp, true);
                aCurrentDrag[rowTemp.rowIndex] = true;
                }
            }
        if(downRow && downRow.getAttribute("selected")) {
            if(aCurrentDrag[downRow.rowIndex]) {
                ELB_SelectItem(ctlID, downRow.id);
                aCurrentDrag[downRow.rowIndex] = false;
                }
            }
        }
    ELB_checkScroll(ctlID);
    }

var iScrollCount = 0;

function ELB_checkScroll(ctlID) {
	if(!binIE) { return; }
    if(sCurrentDrag == ctlID) {
        var oHead = getObj(ctlID + "_listHead");
        var oList = getObj(ctlID + "_dropList");
        var oTable = getObj(ctlID + "_dropListTable");

        var iToScrollUp = oTable.rows[1].offsetHeight;
        var iToScrollDown = (oList.style.pixelHeight - oTable.rows[1].offsetHeight);
        if(ELB_listMode[ctlID] == 1 && oHead.style.display != "none") {
            iToScrollUp = oList.style.pixelTop - 2*oHead.offsetHeight + parseInt(document.body.scrollTop);
            iToScrollDown = oList.style.pixelTop + oList.style.pixelHeight - oHead.offsetHeight - parseInt(document.body.scrollTop);
            }
//        window.status = "top: " + iToScrollUp + "; bottom: " + iToScrollDown + "; mouse: " + window.event.y;
        if(window.event.y < iToScrollUp) {
            if(iScrollCount > -1) { iScrollCount = -1; }
            else { oList.doScroll("scrollBarUp"); iScrollCount = 0; }
            }
        else if(window.event.y > iToScrollDown) {
            if(iScrollCount < 1) { iScrollCount = 1; }
            else { oList.doScroll("scrollBarDown"); iScrollCount = 0; }
            }
        if(binIE) { document.selection.empty(); }
        }
    }

function ELB_Rollover(rowStyle, binRedundant) {
    if(binIE && !rowStyle) { rowStyle = window.event.srcElement; }
	while(rowStyle.id.indexOf("_Row") < 0) { rowStyle = rowStyle.parentNode; }
    var ctlID = rowStyle.parentNode.parentNode.id.replace("_dropListTable", "");

    if((ctlID == sCurrentDrag) && !binRedundant) { ELB_DoDrag(ctlID, rowStyle) }

    rowStyle.style.background = ELB_BackColorRoll[ctlID];
	var j=0;
    for(var iRow=0;iRow<=3;iRow++) {
        if(binIE && rowStyle.cells[iRow]) {
            rowStyle.cells[iRow].style.color = ELB_ForeColorRoll[ctlID];
			}
		else if(!binIE && rowStyle.cells[iRow]) {
			j=0;
			while(rowStyle.cells[iRow].childNodes[j]) { rowStyle.cells[iRow].childNodes[j].style.color = ELB_ForeColorRoll[ctlID]; j++; }
            }
        }
    if(binIE && event) { window.event.cancelBubble = true; }
    //else if(e) { e.stopPropagation() }
    }

function ELB_Rollout(rowStyle, e) { // Restore an option after highlighting
    if(!rowStyle) { rowStyle = window.event.srcElement; }
    while(rowStyle.id.indexOf("_Row") < 0) { rowStyle = rowStyle.parentNode; }
    var ctlID = rowStyle.parentNode.parentNode.id.replace("_dropListTable", "");
    var iCurrent = rowStyle.id.replace(ctlID + "_Row");
    var oHead = getObj(ctlID + "_listHead");

    if(!rowStyle.getAttribute("selected") || oHead.getAttribute("showCheckBoxes") == "true") {
        rowStyle.style.background = ELB_BackColor[ctlID];
        for(var iRow=0;iRow<=3;iRow++) {
	        if(binIE && rowStyle.cells[iRow]) {
	            rowStyle.childNodes[iRow].style.color = ELB_ForeColor[ctlID];
				}
			else if(!binIE && rowStyle.cells[iRow]) {
				j=0;
				while(rowStyle.cells[iRow].childNodes[j]) { rowStyle.cells[iRow].childNodes[j].style.color = ELB_ForeColor[ctlID]; j++; }
 	           }
            }
        }
    if(window.event) { window.event.cancelBubble = true; }
	else if(e) { if(e.stopPropagation) { e.stopPropagation(); } }
    }

function ELB_ScrollList(oList) {
    if(window.event.wheelDelta < 0) { // Scroll the list by the wheel
        oList.doScroll("scrollBarDown");
        }
    else {
        oList.doScroll("scrollBarUp");
        }
    window.event.returnValue = false; // Keep from scrolling the window
    }

// End EasyListBox script
//</script>
