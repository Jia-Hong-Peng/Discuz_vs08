var item_form = document.forms["postform"];

/*******************************************************城市信息*********************************************************/
//数据文件详见javascript/locations.js

$("locus_1").onchange = function(e) {
    var length = 0;
    for(var i in locations) {
        if(locations[i].state == $("locus_1").value) {
            $("locus_2").options[length] = new Option(locations[i].city, locations[i].lid);
            length++;
        }
    }
    $("locus_2").options.length = length; 
}

function initstate() {
    $("locus_1").options.length = states.length+1; 
    $("locus_1").options[0] = new Option("----请选择省份----","-1");
    i = 1;
    for(var state in states) {
        $("locus_1").options[i] = new Option(states[i-1].state, states[i-1].state);
        i++;
    }
}

initstate();


/************************************************日期显示********************************************/

var _hourObject = $("_hour");
var _dateObject = $("_date");
var _minuteObject = $("_minute");

var serverTime = $("serverdatetime").value;

var clientTime = new Date();
//alert(clientTime.getTime() + " " +serverTime );
var offset = 0;
if (serverTime != '') {
	offset = serverTime - clientTime.getTime();
}
var maxDate = 14;
try {
	var now = new Date();
	now.setTime(now.getTime() + offset);
	var date = now.getFullYear()+'-'+(now.getMonth()+1)+'-'+now.getDate();
	var hour = now.getHours();
	var minute = now.getMinutes();
	//get last time
	var dateTime = serverTime;
	dateTime = dateTime.replace('-0', '-');
	var dateTimeArray = dateTime.split(' ');
	if (dateTimeArray[0]) {
		date = dateTimeArray[0];
	}
	if (dateTimeArray[1]) {
		var timeArray = dateTimeArray[1].split(':');
		if (timeArray[0]) {
			hour = timeArray[0];
		}
		if (timeArray[1]) {
			minute = timeArray[1];
		}
	}
	//format date
	date = date.replace('-0', '-');
	//delete old options
	for (var i = _dateObject.length - 1; i >= 0; --i) {
		_dateObject.remove(i);
	}
	//add new options
	var selected = 0;
	for (var i = 0; i <= maxDate; ++i) {
		var _now = new Date();
		_now.setTime(_now.getTime() + offset);
		_now.setDate(now.getDate() + i);
		var displayDate = _now.getFullYear()+'年'+(_now.getMonth()+1)+'月'+_now.getDate()+'日';
		var dateValue = _now.getFullYear()+'-'+(_now.getMonth()+1)+'-'+_now.getDate();
		_dateObject.options[i] = new Option(displayDate, dateValue);
		if (date == dateValue) {
			selected = i;
		}
	}
	if (_dateObject.options[selected]) {
		_dateObject.options[selected].selected = 'selected';
	}
	changeDate();
	//set now or set
	if (!dateTime || dateTime.length <= 2) {
		item_form.elements['_now'][0].checked = true;
		_dateObject.disabled = true;
		_hourObject.disabled = true;
		_minuteObject.disabled = true;
	}else {
		item_form.elements['_now'][1].checked = true;
		_dateObject.disabled = false;
		_hourObject.disabled = false;
		_minuteObject.disabled = false;
	}
	                		}catch (e) {
	alert(e.message + e.number);
}

function setStartTimeEnable(flag) {
	_dateObject.disabled = !flag;
	_hourObject.disabled = !flag;
	_minuteObject.disabled = !flag;
}

function changeDate() {
	try {
		//delete old hour
		for (var i = _hourObject.length - 1; i >= 0; --i) {
			_hourObject.remove(i);
		}
		var startDate = new Date();
		startDate.setTime(startDate.getTime() + offset);
		var endDate = new Date();
		endDate.setTime(endDate.getTime() + offset);
		endDate.setDate(endDate.getDate() + maxDate);
		var currDate = new Date();
		currDate.setTime(currDate.getTime() + offset);
		var currDateStr = _dateObject.options[_dateObject.selectedIndex].value;
		var dateArray = currDateStr.split('-');
		currDate.setFullYear(dateArray[0], dateArray[1] - 1, dateArray[2]);
		currDate.setSeconds(0);

		var startHour = 0;
		var endHour = 24;
		var key = 0;
		var selected = 0;
		for (var i = startHour; i < endHour; ++i) {
			currDate.setHours(i);
			currDate.setMinutes(59);
			if (startDate.getTime() > currDate.getTime()) {
				continue;
			}
			currDate.setMinutes(0);
			if (currDate.getTime() > endDate.getTime()) {
				break;
			}
			_hourObject.options[key] = new Option(i, i);
			if (hour == i) {
				selected = key;
			}
			key++;
		}
		if (_hourObject.options[selected]) {
			_hourObject.options[selected].selected = 'selected';
		}
		changeHour();
	}catch (e) {
		alert(e.message + e.number);
	}
}

function changeHour() {
	try {
		//delete old minute
		for (var i = _minuteObject.length - 1; i >= 0; --i) {
			_minuteObject.remove(i);
		}
		
		var startDate = new Date();
		startDate.setTime(startDate.getTime() + offset);
		var endDate = new Date();
		endDate.setTime(endDate.getTime() + offset);
		endDate.setDate(endDate.getDate() + maxDate);
		var currDate = new Date();
		currDate.setTime(currDate.getTime() + offset);
		var currDateStr = _dateObject.options[_dateObject.selectedIndex].value;
		var dateArray = currDateStr.split('-');
		currDate.setFullYear(dateArray[0], dateArray[1] - 1, dateArray[2]);
		currDate.setHours(_hourObject.options[_hourObject.selectedIndex].value);
		currDate.setSeconds(0);
		hour = currDate.getHours();

		var startMinute = 0;
		var endMinute = 60;
		var step = 5;
		var key = 0;
		var selected = 0;
		for (var i = startMinute; i < endMinute; i+=5) {
			currDate.setMinutes(i);
			if (startDate.getTime() > currDate.getTime()) {
				continue;
			}
			if (currDate.getTime() > endDate.getTime()) {
				break;
			}
			_minuteObject.options[key] = new Option(i, i);
			if (Math.abs(minute - i) < step) {
				selected = key;
			}
			key++;
		}
		if (_minuteObject.options[selected]) {
			_minuteObject.options[selected].selected = 'selected';
		}
	}catch (e) {
		alert(e.message + e.number);
	}
}

function setMinute() {
	minute = _minuteObject.value;
}

function setTimeEnableAndInStock(bTime, bInStock){
	setStartTimeEnable(bTime);
	
//	if (bInStock && bInStock == 1){
//		item_form["dateline"].value=item_form['_date'].value + " " +item_form['_hour'].value + ":"+item_form['_minute'].value;
//	}
}

//function bindDateLine()
//{
//    //alert('d');
//    //item_form["dateline"].value=item_form['_date'].value + " " +item_form['_hour'].value + ":"+item_form['_minute'].value;
//    //return true;
//}




/********************************************** form validate *****************************************************/

lang["post_trade_amount_is_number"] = "对不起，商品总数必须为有效数字且不小于1。";
lang["post_trade_costprice_is_number"] = "对不起，商品原价必须为有效数字且不小于1。";
lang["post_trade_price_is_number"] = "对不起，商品现价必须为有效数字且不小于1。";
lang["post_trade_postage_is_number"] = "对不起，运费必须为有效数字或为空。";
lang["post_trade_repair_is_number"] = "对不起，保修期必须为有效数字或为空。";

//该方法绑定不能注释,否则会造成提交出现问题
$("postform").onsubmit = function() { 
    if(validate(this)){
         if(validategoods()) { 
            // if($("postsubmit").name == "editsubmit") 
                return true ;
         }    
    }
    return false;
};

function isPlus(number) {
    if(parseInt(number)<=0 || !parseInt(number)) {
        return false;
    }
    return true;
}

function validategoods() {
   if($("amount").value == "" || !isPlus($("amount").value)) {
		alert(lang["post_trade_amount_is_number"]);
		$('amount').focus();
		return false;
   }
   
   if($("costprice").value == "" || !isPlus($("costprice").value)) {
		alert(lang["post_trade_costprice_is_number"]);
		$('costprice').focus();
		return false;
   }
   
   if($("price").value == "" || !isPlus($("price").value)) {
		alert(lang["post_trade_price_is_number"]);
		$("price").focus();
		return false;
   }

   if(item_form.elements["transport"][0].checked != true) {
        if($("postage_mail").value != "" && !isPlus($("postage_mail").value)) {
		    alert(lang["post_trade_postage_is_number"]);
		    $("postage_mail").focus();
		    return false;
        }
        
        if($("postage_express").value != "" && !isPlus($("postage_express").value)) {
		    alert(lang["post_trade_postage_is_number"]);
		    $("postage_express").focus();
		    return false;
        }
        
		if($("postage_ems").value != "" && !isPlus($("postage_ems").value)) {
		    alert(lang["post_trade_postage_is_number"]);
		    $("postage_ems").focus();
		    return false;
        }
   }
   
   if($("repair").value != "" && !isPlus($("repair").value)) {
		alert(lang["post_trade_repair_is_number"]);
		$("repair").focus();
		return false;
   }
   
   if($("tradetype_1").checked && $("account").value == '') {
		alert('请输入支付宝帐号信息');
		$('account').focus();
		return false;
   }
   
   return true;
}


function bbcode2html(str) {

	str = trim(str);

	if(str == '') {
		return '';
	}

	if(!fetchCheckbox('bbcodeoff') && allowbbcode) {
		str= str.replace(/\s*\[code\]([\s\S]+?)\[\/code\]\s*/ig, function($1, $2) {return parsecode($2);});
	}

	if(!forumallowhtml && !(allowhtml && fetchCheckbox('htmlon'))) {
		str = str.replace(/</g, '&lt;');
		str = str.replace(/>/g, '&gt;');
		if(!fetchCheckbox('parseurloff')) {
			str = parseurl(str, 'html');
		}
	}

/* Discuz!NT  start */

	/*if(!fetchCheckbox('smileyoff') && allowsmilies) {
		for(id in smilies) {
			re = new RegExp(addslashes(smilies[id]['code']), "g");
			str = str.replace(re, '<img src="./images/smilies/' + smilies[id]['url'] + '" border="0" smilieid="' + id + '" alt="' + smilies[id]['code'] + '" />');
		}
	}*/
	if(!fetchCheckbox('smileyoff') && allowsmilies) {
		for(i in smilies_HASH) {
			for(id in smilies_HASH[i]) {
				re = new RegExp(addslashes(smilies_HASH[i][id]['code']), "g");
				str = str.replace(re, '<img src="editor/images/smilies/' + smilies_HASH[i][id]['url'] + '" border="0" smiliecode="' + smilies_HASH[i][id]['code'] + '" alt="" />');
			}
		}
	}
/* Discuz!NT  end */

	if(!fetchCheckbox('bbcodeoff') && allowbbcode) {
		str= str.replace(/\[url\]\s*(www.|https?:\/\/|ftp:\/\/|gopher:\/\/|news:\/\/|telnet:\/\/|rtsp:\/\/|mms:\/\/|callto:\/\/|bctp:\/\/|ed2k:\/\/){1}([^\[\"']+?)\s*\[\/url\]/ig, function($1, $2, $3) {return cuturl($2 + $3);});
		str= str.replace(/\[url=www.([^\[\"']+?)\](.+?)\[\/url\]/ig, '<a href="http://www.$1" target="_blank">$2</a>');
		str= str.replace(/\[url=(https?|ftp|gopher|news|telnet|rtsp|mms|callto|bctp|ed2k){1}:\/\/([^\[\"']+?)\]([\s\S]+?)\[\/url\]/ig, '<a href="$1://$2" target="_blank">$3</a>');
		str= str.replace(/\[email\](.*?)\[\/email\]/ig, '<a href="mailto:$1">$1</a>');
		str= str.replace(/\[email=(.[^\[]*)\](.*?)\[\/email\]/ig, '<a href="mailto:$1" target="_blank">$2</a>');
		str = str.replace(/\[color=([^\[\<]+?)\]/ig, '<font color="$1">');
		str = str.replace(/\[size=(\d+?)\]/ig, '<font size="$1">');
		str = str.replace(/\[size=(\d+(\.\d+)?(px|pt|in|cm|mm|pc|em|ex|%)+?)\]/ig, '<font style="font-size: $1">');
		str = str.replace(/\[font=([^\[\<]+?)\]/ig, '<font face="$1">');
		str = str.replace(/\[align=([^\[\<]+?)\]/ig, '<p align="$1">');
		str = str.replace(/\[float=([^\[\<]+?)\]/ig, '<br style="clear: both"><span style="float: $1;">');

		re = /\[table(?:=(\d{1,4}%?)(?:,([\(\)%,#\w ]+))?)?\]\s*([\s\S]+?)\s*\[\/table\]/ig;
		for (i = 0; i < 4; i++) {
			str = str.replace(re, function($1, $2, $3, $4) {return parsetable($2, $3, $4);});
		}

		str = preg_replace([
			'\\\[\\\/color\\\]', '\\\[\\\/size\\\]', '\\\[\\\/font\\\]', '\\\[\\\/align\\\]', '\\\[b\\\]', '\\\[\\\/b\\\]',
			'\\\[i\\\]', '\\\[\\\/i\\\]', '\\\[u\\\]', '\\\[\\\/u\\\]', '\\\[list\\\]', '\\\[list=1\\\]', '\\\[list=a\\\]',
			'\\\[list=A\\\]', '\\\[\\\*\\\]', '\\\[\\\/list\\\]', '\\\[indent\\\]', '\\\[\\\/indent\\\]', '\\\[\\\/float\\\]'
			], [
			'</font>', '</font>', '</font>', '</p>', '<b>', '</b>', '<i>',
			'</i>', '<u>', '</u>', '<ul>', '<ul type=1>', '<ul type=a>',
			'<ul type=A>', '<li>', '</ul>', '<blockquote>', '</blockquote>', '</span>'
			], str);
	}

	if(!fetchCheckbox('bbcodeoff')) {
		if(allowimgcode) {
			str = str.replace(/\[localimg=(\d{1,4}),(\d{1,4})\](\d+)\[\/localimg\]/ig, function ($1, $2, $3, $4) {if($('attach_' + $4)) {var src = $('attach_' + $4).value; if(src != '') return '<img style="filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=\'scale\',src=\'' + src + '\');width:' + $2 + ';height=' + $3 + '" src=\'images/common/none.gif\' border="0" aid="attach_' + $4 + '" alt="" />';}});
			str = str.replace(/\[img\]\s*([^\[\<\r\n]+?)\s*\[\/img\]/ig, '<img src="$1" border="0" alt="" />');
			//str = str.replace(/\[attachimg\](\d+)\[\/attachimg\]/ig, function ($1, $2) {eval('var attachimg = $(\'preview_' + $2 + '\')');return '<img src="' + attachimg.src + '" border="0" aid="attachimg_' + $2 + '" width="' + attachimg.clientWidth + '" alt="" />';});
			str = str.replace(/\[attachimg\](\d+)\[\/attachimg\]/ig, '<img src="attachment.aspx?goodsattach=yes&attachmentid=$1" border="0" aid="attachimg_$1" alt="" />');

			str = str.replace(/\[img=(\d{1,4})[x|\,](\d{1,4})\]\s*([^\[\<\r\n]+?)\s*\[\/img\]/ig, '<img width="$1" height="$2" src="$3" border="0" alt="" />');
		} else {
			str = str.replace(/\[img\]\s*([^\[\<\r\n]+?)\s*\[\/img\]/ig, '<a href="$1" target="_blank">$1</a>');
			str = str.replace(/\[img=(\d{1,4})[x|\,](\d{1,4})\]\s*([^\[\<\r\n]+?)\s*\[\/img\]/ig, '<a href="$1" target="_blank">$1</a>');
		}
	}

	for(var i = 0; i <= pcodecount; i++) {
		str = str.replace("[\tDISCUZ_CODE_" + i + "\t]", codehtml[i]);
	}

	if(!forumallowhtml && !(allowhtml && fetchCheckbox('htmlon'))) {
		str = preg_replace(['\t', '   ', '  ', '(\r\n|\n|\r)'], ['&nbsp; &nbsp; &nbsp; &nbsp; ', '&nbsp; &nbsp;', '&nbsp;&nbsp;', '<br />'], str);
	}

	return str;
}