/* ************************************************************************************************
 ＜チェック系共通関数＞
 【関数名】                               【概要】
    chkDaisyouHikaku()                      大小比較チェック
    chkDaisyouHikaku2()                     大小比較チェック2
    chkErrMojiretu()                        エラー文字列チェック
    chkHankaku()                            半角文字チェック(,"<>'&は除く)
    chkHankakuEiji()                        半角英字チェック
    chkHankakuEijiKomoji()                  半角英字(小文字)チェック
    chkHankakuEisuuji()                     半角英数字チェック
    chkHankakuKanaEisuuji()                 半角カナ英数字チェック(半角カナ英数字以外の場合エラー表示)
    chkHankakuSuuji()                       半角数字チェック
    chkHiduke()                             日付チェック(yyyy/mm/dd)
    chkHidukeHaninai()                      日付範囲以内チェック
    chkHidukeYYYYMD()                       日付チェック(yyyy/mm/dd、yyyy/m/d)
    chkJikan()                              時間チェック(hh:mm)
    chkKaigyou()                            改行エラーチェック
    chkKakoHiduke()                         過去日付チェック
    chkKinsiMoji()                          禁止文字チェック、(禁止文字入力の場合エラー表示)
    chkLineCd()                             先頭半角スペース/半角スペース・半角カナ・半角英数/指定バイト数チェック
    chkMailAddress()                        半角英数、@、.、_、-チェック/指定内バイト数チェック
    chkMiraiHiduke()                        未来日付チェック
    chkGaiji()                              外字チェック
    chkNengetu()                            年月チェック(yyyy/mm)
    chkNyuuryokuHissu()                     入力必須チェック(テキスト用)
    chkSeisuu()                             整数チェック
    chkSeisuuSiteiHanni()                   整数指定範囲チェック
    chkSentakuHissu()                       選択必須チェック(選択リスト用)
    chkSentouSpace()                        先頭スペースチェック
    chkSiteiByte()                          指定バイト数チェック
    chkSiteiijyouByte()                     指定内バイト数チェック
	chkSiteinaiMoji()						指定内文字数チェック
    chkSiteiMojiretu()                      指定文字列チェック
    chkSiteinaiByte()                       指定内バイト数チェック
    chkSyousuu()                            小数チェック(-もＯＫ)
    chkTelFaxNo()                           半角数字ハイフンチェック
    chkUserID()                             半角英数、.、_、-チェック(左記以外の場合エラー表示)
    fnChkVal()                              フォーム内のコントロール全てのchkvalをチェックし、チェック処理を行う
    fnDisposeChk()                          指定されたパラメータのチェック処理を呼び出す
    chkZengin()                             全銀文字チェック
    chkZenginZenkaku()                      全銀全角文字チェック
    chkZenkakuKana()                        全角カタカナチェック
    chkZenkakuKanaEisuuji()                 全角カナ英数字チェック
    chkZipCd()                              郵便番号チェック
    chkMailAddressMst()                     メールアドレスチェック(マスタ用)
 【変更履歴】
    2004/02/10  吉田 兼三       チェック系関数を当ファイルに纏める
    2004/04/15  宇梶			CHKVAL関数でｴﾗｰになったｵﾌﾞｼﾞｪｸﾄ以外の指定ｵﾌﾞｼﾞｪｸﾄにﾌｫｰｶｽがあてられるよう修正
************************************************************************************************ */
/* ************************************************************************************************
	関数名 chkDaisyouHikaku()
		作成日 : 2000/03/23
		引数   : 1番目      -> 比較項目の下限
		引数   : 2番目      -> 比較項目の上限
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 大小比較チェック
				 比較項目の下限が上限以上の場合エラー
************************************************************************************************ */
function chkDaisyouHikaku(start,end) {
	var wkstart = start;
	var wkend = end;
	if (wkstart > wkend){
		return false;
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkDaisyouHikaku2()
		作成日 : 2000/07/06
		引数   : 1番目      -> 比較項目の下限
		引数   : 2番目      -> 比較項目の上限
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 大小比較チェック
				 比較項目の下限が上限以上の場合エラー
************************************************************************************************ */
function chkDaisyouHikaku2(start,end) {
	var wkstart = start;
	var wkend = end;
	if (wkstart >= wkend){
		return false;
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkErrMojiretu()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : エラー文字列チェック
				 比較条件文字列と違う場合エラー
************************************************************************************************ */
function chkErrMojiretu(str,moji) {
	var wkstr = str;
	var wkmoji = moji;
	if (wkstr != wkmoji){
		return false;
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkHankaku()
		作成日 : 2000/07/06
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角文字チェック(,"<>'&は除く)
				 半角文字以外、,"<>'&と違う場合は、エラー
************************************************************************************************ */
function chkHankaku(str) {
	var ch1;
	var ch2;
	var wkstr = str;
	if (wkstr != "") {
		for (i = 0; i < wkstr.length; i++) {
			ch1 = wkstr.substring(i,i+1);
			ch2 = wkstr.charCodeAt(i);
			if (!((ch2 >= " ".charCodeAt(0) && ch2 <= "~".charCodeAt(0)) || (ch2 >= "?.charCodeAt(0) && ch2 <= "?.charCodeAt(0)))) {
				return false;
			}
			if (ch2 == 44 || ch2 == 39 || ch2 == 34 || ch2 == 60 || ch2 == 62) {
				return false;
			}
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkHankakuEiji()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角英字チェック
				 半角英字(a～z、A～Z)以外エラー
************************************************************************************************ */
function chkHankakuEiji(str) {
	var ch;
	var wkstr = str;
	for (var i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if (!((ch >= "A" && ch <= "Z") || (ch >= "a" && ch <= "z"))) {
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkHankakuEijiKomoji()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角英小文字チェック
				 半角英小文字(a～z)はエラー
************************************************************************************************ */
function chkHankakuEijiKomoji(str) {
	var ch;
	var wkstr = str;
	for (var i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if ((ch >= "ａ" && ch <= "ｚ") || (ch >= "a" && ch <= "z")) {
			return false;
		}
	}
	return true;
}

/* ************************************************************************************************
	関数名 chkHankakuEisuuji()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角英数字チェック
				 半角英数字(a～z、A～Z、0～9)以外エラー
************************************************************************************************ */
function chkHankakuEisuuji(str) {
	var ch;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if (!((ch >= "0" &&  ch <= "9") || (ch >= "A" && ch <= "Z") || (ch >= "a" && ch <= "z"))){
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkHankakuKanaEisuuji()
		作成日 : 2000/06/27
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角カナ英数字チェック(半角カナ英数字以外の場合エラー表示)
************************************************************************************************ */
function chkHankakuKanaEisuuji(str) {
	var ch;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if (!((ch >= "0" &&  ch <= "9") || (ch >= "A" && ch <= "Z") || (ch >= "a" && ch <= "z") || (ch >= "? && ch <= "?))){
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkHankakuSuuji()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角数字チェック
				 半角数字(0～9)以外ならエラー表示
************************************************************************************************ */
function chkHankakuSuuji(str) {
	var ch;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if  (!(ch >= "0" &&  ch <= "9")){
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkHiduke()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : 0 or メッセージ文字(エラーの場合メッセージ文字を返す)
		概要   : 日付チェック(yyyy/mm/dd)
				 年月日の値が異常、(yyyy/mm/dd)の形式でない場合エラー
************************************************************************************************ */
function chkHiduke(str) {
	var str1 = str;
	var yValue = str1.substring(0,4);
	var mValue = str1.substring(5,7);
	var dValue = str1.substring(8,10);
	
	if (str!=""){
		if (str1.substring(4,5) != "/" || str1.substring(7,8) != "/" || str1.length != 10) {
			var err = "を正しく入力して下さい。(yyyy/mm/dd)";
			return err;
		}else{
			if (!chkHankakuSuuji(yValue)){
				var err = "の年は半角数字で入力して下さい。";
				return err;
			}
			if (!chkHankakuSuuji(mValue)){
	 			var err = "の月は半角数字で入力して下さい。";
				return err;
			}
			if (!chkHankakuSuuji(dValue)){
				var err = "の日は半角数字で入力して下さい。";
				return err;
			}
			if (parseInt(yValue) < 1900){
					var err = "の年を正しく入力して下さい。";
					return err;
			}
			if ((mValue > 12) || (mValue < 1)) {
				var err = "の月を正しく入力して下さい。";
				return err;
			}
			if ((mValue=="01") || (mValue=="03") || (mValue=="05") || (mValue=="07") || (mValue=="08") || (mValue=="10") || (mValue=="12")) {
				if (dValue > 31 || dValue < 1) {
					var err = "の日を正しく入力して下さい。";
					return err;
				}
			}
			if ((mValue=="04") || (mValue=="06") || (mValue=="09") || (mValue == "11")) {
				if ((dValue > 30) || (dValue < 1)) {
					var err = "の日を正しく入力して下さい。";
					return err;
				}
			}
			if (mValue=="02") {
				if (yValue % 4 == 0) {
					if (dValue > 29 || dValue < 1) {
						var err = "の日を正しく入力して下さい。";
						return err;
					}
				} else {
					if ((dValue > 28) || (dValue < 1)) {
						var err = "の日を正しく入力して下さい。";
						return err;
					}
				}
			}
		}
	}
	var err = "0";
	return err;
}
/* ************************************************************************************************
	関数名 chkHidukeHaninai()
		作成日 : 2000/04/21
		引数   : 1番目      -> 日付形式の開始日
		引数   : 2番目      -> 日付形式の終了日
		引数   : 3番目      -> 検索月数(何か月か)
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 日付範囲以内チェック
				 終了日が開始日から指定期間未満の場合OK
				 ※このチェックを使用する前に日付形式チェックをすること
				 ※範囲指定は、月単位で行うこと
************************************************************************************************ */
function chkHidukeHaninai(start,end,hani) {
	var wkstart = start;
	var wkend = end;
	if (wkstart > wkend) {
		return false;
	}
	var wkhani = parseInt(hani,10);
	var wkstart_nen = parseInt(wkstart.substring(0,4),10);
	var wkstart_tuki = parseInt(wkstart.substring(5,7),10)-1;
	var wkstart_hi = parseInt(wkstart.substring(8,10),10);
	var wkend_nen = parseInt(wkend.substring(0,4),10);
	var wkend_tuki = parseInt(wkend.substring(5,7),10)-1;
	var wkend_hi = parseInt(wkend.substring(8,10),10);
	var hikaku = new Date(wkstart_nen,wkstart_tuki+wkhani,wkstart_hi);
	var owari = new Date(wkend_nen,wkend_tuki,wkend_hi);
	if (hikaku <= owari) {
		return false;
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkHidukeYYYYMD()
		作成日 : 2000/08/16
		引数   : 1番目      -> チェックする文字列
		戻り値 : 0 or メッセージ文字(エラーの場合メッセージ文字を返す)
		概要   : 日付チェック(yyyy/mm/dd、yyyy/m/d)
				 年月日の値が異常、(yyyy/mm/dd、yyyy/m/d)の形式でない場合エラー
		変更履歴：
************************************************************************************************ */
function chkHidukeYYYYMD(str) {
	var str1 = str;

	// 日付を / で分割する
	var dateArray = new Array(3);
	dateArray  = str1.split("/");

	// 分割した日付の配列数が3以外なら年月日が揃っていないため、エラーとする
	if (dateArray.length != 3) {
		var err = "を正しく入力して下さい。(yyyy/mm/dd)";
		return err;
	}

	// 分割した日付をチェック用に別の変数に代入する
	var yValue = dateArray[0];
	var mValue = dateArray[1];
	var dValue = dateArray[2];

	//月と日に3桁以上の値を入力させない
	if ( mValue.length > 2) {
		var err = "の月は２桁以内で入力して下さい。";
		return err;
	}
	if ( dValue.length > 2) {
		var err = "の日は２桁以内で入力して下さい。";
		return err;
	}

	if (!chkHankakuSuuji(yValue)){
		var err = "の年は半角数字で入力して下さい。";
		return err;
	}
	if (!chkHankakuSuuji(mValue)){
 			var err = "の月は半角数字で入力して下さい。";
		return err;
	}
	if (!chkHankakuSuuji(dValue)){
		var err = "の日は半角数字で入力して下さい。";
		return err;
	}
	if (parseInt(yValue) < 1900){
			var err = "の年を正しく入力して下さい。";
			return err;
	}
	if ((mValue > 12) || (mValue < 1)) {
		var err = "の月を正しく入力して下さい。";
		return err;
	}
	if ((mValue==1) || (mValue==3) || (mValue==5) || (mValue==7) || (mValue==8) || (mValue==10) || (mValue==12)) {
		if (dValue > 31 || dValue < 1) {
			var err = "の日を正しく入力して下さい。";
			return err;
		}
	}
	if ((mValue==4) || (mValue==6) || (mValue==9) || (mValue==11)) {
		if ((dValue > 30) || (dValue < 1)) {
			var err = "の日を正しく入力して下さい。";
			return err;
		}
	}
	if (mValue==2) {
		if (yValue % 4 == 0) {
			if (dValue > 29 || dValue < 1) {
				var err = "の日を正しく入力して下さい。";
				return err;
			}
		} else {
			if ((dValue > 28) || (dValue < 1)) {
				var err = "の日を正しく入力して下さい。";
				return err;
			}
		}
	}

	var err = "0";
	return err;
}
/* ************************************************************************************************
	関数名 chkJikan()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : 0 or メッセージ文字(エラーの場合メッセージ文字を返す)
		概要   : 時間チェック(hh:mm)
				 時、分の値が異常、(hh:mm)の形式でない場合エラー
************************************************************************************************ */
function chkJikan(str) {
	var str1 = str;
	var hValue = str1.substring(0,2);
	var mValue = str1.substring(3,5);
	if (str1.substring(2,3) != ":" || str1.length != 5) {
		var err = "を正しく入力して下さい。(hh:mm)";
		return err;
	}else{
		if (!chkHankakuSuuji(hValue)){
			var err = "の時は半角数字で入力して下さい。";
			return err;
		}
		if (!chkHankakuSuuji(mValue)){
 			var err = "の分は半角数字で入力して下さい。";
			return err;
		}
		if (hValue > 23) {
			var err = "の時を正しく入力して下さい。";
			return err;
		}
		if (mValue > 59) {
			var err = "の分を正しく入力して下さい。";
			return err;
		}
	}
	var err = "0";
	return err;
}
/* ************************************************************************************************
	関数名 chkKaigyou()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 改行エラーチェック
				 改行がされている場合エラー
************************************************************************************************ */
function chkKaigyou(str) {
	var ch;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i, i + 1);
		if  (ch == "\n") {
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkKakoHiduke()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件日付
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 過去日付チェック
				 比較条件日付より未来の日付の場合エラー(比較条件日付以前の日付ＯＫ)
		注意   : 当日日付で比べる場合は、サーバー日付を渡すこと！
************************************************************************************************ */
function chkKakoHiduke(str,hikaku) {
	var wkstr = str;
	var wkhikaku = hikaku;
	if (wkstr != "") {
		if (wkstr > wkhikaku){
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkKinsiMoji()
		作成日 : 2000/03/22
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 禁止文字チェック、(禁止文字入力の場合エラー表示)
				 Uniコード 34=" 38=& 39=' 44=, 60=< 62=>
		修正日：2002/03/04  外崎 美岐  半角\も禁止文字に追加
************************************************************************************************ */
function chkKinsiMoji(str) {
	var ch1;
	var ch2;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch1 = wkstr.charCodeAt(i);
		ch2 = wkstr.substring(i,i+1);
		if (ch1 == 44 || ch1 == 39 || ch1 == 34 || ch1 == 60 || ch1 == 62 || ch1 == 92) {
			return false;
/*
		//①～⑳
		}else if(ch1 >= 9312 && ch1 <= 9331){
			return false;
		//CJK Compatibility  ㍉ ～㎡
		}else if(ch1 >= 13056 && ch1 <= 13311){
			return false;
		//Box Drawing ─ ～ ╋
		}else if(ch1 >= 9472 && ch1 <= 9599){
			return false;
		//㍻
		}else if(ch1 == 13179){
			return false;
		//〝
		}else if(ch1 == 12317){
			return false;
		//〟
		}else if(ch1 == 12319){
			return false;
		//㏍
		}else if(ch1 == 13261){
			return false;
		//℡
		}else if(ch1 == 8481){
			return false;
		//㊤
		}else if(ch1 == 12964){
			return false;
		//㊥
		}else if(ch1 == 12965){
			return false;
		//㊦
		}else if(ch1 == 12966){
			return false;
		//㊧
		}else if(ch1 == 12967){
			return false;
		//㊨
		}else if(ch1 == 12968){
			return false;
		//㈲
		}else if(ch1 == 12850){
			return false;
		//㈹
		}else if(ch1 == 12857){
			return false;
		//㍾
		}else if(ch1 == 13182){
			return false;
		//㍽
		}else if(ch1 == 13181){
			return false;
		//㍼
		}else if(ch1 == 13180){
			return false;
*/
		}else{
			if (ch2 >= "? && ch2 <= "?) {
				return false;
			}
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkLineCd()
		作成日 : 2000/06/27
		修正日 : 2001/03/19(6桁→7桁チェックに修正)
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件の桁数
		戻り値 : 0 or メッセージ文字(エラーの場合メッセージ文字を返す)
		概要   : 先頭半角スペースチェック(先頭半角スペースの場合エラー)
				 半角スペース・半角カナ・半角英数チェック(半角スペースカナ英数字以外の場合エラー表示)
				 指定バイト数チェック(比較条件の桁数と違う場合エラー)
************************************************************************************************ */
function chkLineCd(str) {
	var str1 = str;
	if (str1 != "") {
		//先頭スペースチェック
		var ch = str1.substring(0,1);
		if (ch == " ") {
			var err = "は一文字目にスペースを入力しないで下さい。";
			return err;
		}
		//半角スペース・半角カナ・半角英数チェック
		for (i = 0; i < str1.length; i++) {
			ch = str1.substring(i,i + 1);
			if (!((ch >= "0" &&  ch <= "9") || (ch >= "A" && ch <= "Z") || (ch >= "a" && ch <= "z") || (ch >= "? && ch <= "?) || (ch == " "))){
				var err = "は半角文字で入力して下さい。";
				return err;
			}
		}
		//指定バイト数チェック
		if (!chkSiteiByte(str1,7)){
			var err = "は7桁で入力して下さい。";
			return err;
		}
	}
	var err = "0";
	return err;
}
/* ************************************************************************************************
	関数名 chkMailAddress()
		作成日 : 2000/07/04
		引数   : 1番目      -> チェックする文字列
		戻り値 : 0 or メッセージ文字(エラーの場合メッセージ文字を返す)
		概要   : 半角英数、@、.、_、-チェック(左記以外の場合エラー表示)
				 @１つチェック(@２つ以上、@なしの場合エラー)
				 指定内バイト数チェック(比較条件の桁数と違う場合エラー)
************************************************************************************************ */
function chkMailAddress(str) {
	var str1 = str;
	var kazu = 0;
	if (str1 != "") {
		//半角半角英数、@、.、_、-チェック
		for (i = 0; i < str1.length; i++) {
			ch = str1.substring(i,i + 1);
			if (!((ch >= "0" &&  ch <= "9") || (ch >= "A" && ch <= "Z") || (ch >= "a" && ch <= "z") || (ch == "@") || (ch == ".") || (ch == "_") || (ch == "-"))){
				var err = "を正しく入力して下さい。";
				return err;
			}else{
				if (ch == "@") {
					kazu = kazu + 1
				}
			}
		}
		//@一つチェック
		if (kazu != 1) {
			var err = "を正しく入力して下さい。";
			return err;
		}
		//指定内バイト数チェック
		if (!chkSiteinaiByte(str1,40)){
			var err = "は40桁以内で入力して下さい。";
			return err;
		}
	}
	var err = "0";
	return err;
}
/* ************************************************************************************************
	関数名 chkMiraiHiduke()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件日付
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 未来日付チェック
				 比較条件日付より過去の日付の場合エラー(比較条件日付以降の日付ＯＫ)
		注意   : 当日日付で比べる場合は、サーバー日付を渡すこと！
************************************************************************************************ */
function chkMiraiHiduke(str,hikaku) {
	var wkstr = str;
	var wkhikaku = hikaku;
	if (wkstr != "") {
		if (wkstr < wkhikaku){
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkGaiji()
		作成日 : 2002/10/10
		引数   : 1番目      -> 外字チェックする文字列
		戻り値 : null or String(エラーの場合外字文字を返す)
		概要   : 外字チェック
************************************************************************************************ */
function chkGaiji(text)
{
// Del Nishiwaki 2004/3/30 No.41 対応
//	var	temp1,temp2;
//	var	chkstr = "①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ㍉㌔㌢㍍㌘㌧㌃㌶㍑㍗㌍㌦㌣㌫㍊㌻㎜㎝㎞㎎㎏㏄㎡㍻〝〟№㏍℡㊤㊥㊦㊧㊨㈱㈲㈹㍾㍽㍼≒≡∫∮∑√⊥∠∟⊿∵∩∪";
//	for ( i = 0; i < text.length; i++ )
//	{
//		temp1 = ""+text.charAt(i);
//		for ( j = 0; j < chkstr.length; j++ )
//		{
//			temp2 = "" + chkstr.charAt(j)
//			if ( temp1 == temp2 )
//			{
//				return temp2;
//			}
//		}
//	}
	return "";
}


/* ************************************************************************************************
	関数名 chkKinsiMojiReceipt()
		作成日 : 2014/05/01
		引数   : 1番目      -> 禁止文字チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 外字チェック
************************************************************************************************ */
function chkKinsiMojiReceipt(text)
{
	var	temp1,temp2;
	var	chkstr = "ПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя─│┌┐┘└├┬┤┴┼━┃┏┓┛┗┣┳┫┻╋┠┯┨┷┿┝┰┥┸╂①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ㍉㌔㌢㍍㌘㌧㌃㌶㍑㍗㌍㌦㌣㌫㍊㌻㎜㎝㎞㎎㎏㏄㎡㍻〝〟№㏍℡㊤㊥㊦㊧㊨㈱㈲㈹㍾㍽㍼≒≡∫∮∑√⊥∠∟⊿∵∩∪";
	for ( i = 0; i < text.length; i++ ){
		temp1 = "" + text.charAt(i);
		for ( j = 0; j < chkstr.length; j++ ){
			temp2 = "" + chkstr.charAt(j)
			if ( temp1 == temp2 ){
				return false;
			}
		}
	}
}


/* ************************************************************************************************
	関数名 chkNengetu()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : 0 or メッセージ文字(エラーの場合メッセージ文字を返す)
		概要   : 年月チェック(yyyy/mm)
				 年月の値が異常、(yyyy/mm)の形式でない場合エラー
************************************************************************************************ */
function chkNengetu(str) {
	var str1 = str;
	var yValue = str1.substring(0,4);
	var mValue = str1.substring(5,7);
	if (str1.substring(4,5) != "/" || str1.length != 7) {
		var err = "を正しく入力して下さい。(yyyy/mm)";
		return err;
	}else{
		if (!chkHankakuSuuji(yValue)){
			var err = "の年は半角数字で入力して下さい。";
			return err;
		}
		if (!chkHankakuSuuji(mValue)){
 			var err = "の月は半角数字で入力して下さい。";
			return err;
		}
		if (parseInt(yValue) < 1900){
				var err = "の年を正しく入力して下さい。";
				return err;
		}
		if ((mValue > 12) || (mValue < 1)) {
			var err = "の月を正しく入力して下さい。";
			return err;
		}
	}
	var err = "0";
	return err;
}
/* ************************************************************************************************
	関数名 chkNyuuryokuHissu()
		作成日 : 2000/03/22
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 入力必須チェック(テキスト用)
				 未入力、スペースのみならエラー表示
		修正日 : 2002/12/19
		修正者 : MIT/吉田
		修正内容 : 全角スペースをエラーとする。

************************************************************************************************ */
function chkNyuuryokuHissu(str) {
	var wkflg = 0;
	var wkdata = str;

	for (i = 0; i < wkdata.length; i++) {
		if ((wkdata.charAt(i) != " ") && (wkdata.charAt(i) != "　")) {
			wkflg = 1
		}
	}
	if (wkflg == 0) {
		return false;
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkSeisuu()
		作成日 : 2000/04/20
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 整数チェック
				 ２桁以上数値を入力した時、頭１桁目が0の場合エラー
************************************************************************************************ */
function chkSeisuu(str) {
	var ch;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if  (!(ch >= "0" &&  ch <= "9")){
			return false;
		}
	}
	if (wkstr.length > 1) {
		ch = wkstr.substring(0,1);
		if (ch == 0) {
			return false;
		}else{
			return true;
		}
	}else{
		return true;
	}
}
/* ************************************************************************************************
	関数名 chkSeisuuSiteiHanni()
		作成日 : 2000/04/20
		引数   : 1番目      -> チェック値
		引数   : 2番目      -> 下限
		引数   : 3番目      -> 上限
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 整数チェック
				 ２桁以上数値を入力した時、頭１桁目が0の場合エラー
************************************************************************************************ */
function chkSeisuuSiteiHanni(chkVal,lower,heiger){
	//数値チェック
	if(isNaN(chkVal)==true){
		return false;
	}
	if(isNaN(lower)==true){
		return false;
	}
	if(isNaN(heiger)==true){
		return false;
	}
	if((parseInt(lower,10)<=parseInt(chkVal,10))&&(parseInt(chkVal,10)<=parseInt(heiger,10))){
		return true;
	}
	return false;
}
/* ************************************************************************************************
	関数名 chkSentakuHissu()
		作成日 : 2000/03/22
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 選択必須チェック(選択リスト用)
				 リスト未選択ならエラー表示
************************************************************************************************ */
function chkSentakuHissu(str) {
	var wkflg = 0;
	var wkdata = str;
	for (i = 0; i < wkdata.length; i++) {
		if (wkdata.charAt(i) != " ") {
			wkflg = 1
		}
	}
	if (wkflg == 0) {
		return false;
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkSentouSpace()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 先頭スペースチェック
				 先頭１桁目が半角または全角スペースならエラー表示
************************************************************************************************ */
function chkSentouSpace(str) {
	var wkstr = str;
	if (wkstr.substring(0,1) == " " || wkstr.substring(0,1) == "　") {
		return false;
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkSiteiByte()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件の桁数
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 指定バイト数チェック
				 比較条件の桁数と違う場合は、エラー
************************************************************************************************ */
function chkSiteiByte(str,keta) {
	var iLength = 0;
	var i;
	// Netscape Navigatorの場合
	if (navigator.appName.indexOf("Netscape") >= 0) {
		iLength = chkSiteiByte.arguments[0].length;
	}
	// Microsoft Internet Explorerの場合
	if (navigator.appName.lastIndexOf("Explorer") >= 0) {
		if (chkSiteiByte.arguments[0] == "") {
			return true;
		}
		for (i = 0; i < chkSiteiByte.arguments[0].length; i++) {
			sBuffer = chkSiteiByte.arguments[0].substring(i, i+1);
			if ((sBuffer >= " " && sBuffer <= "~") || (sBuffer >= "? && sBuffer <= "?)) {
				iLength++;
			}else{
				iLength += 2;
			}
		}
	}
	if (keta != iLength) {
		return false;
	}else{
		return true;
	}
}
/* ************************************************************************************************
	関数名 chkSiteiijyouByte()
		作成日 : 2000/06/23
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件の桁数
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 指定内バイト数チェック
				 比較条件の桁数より小さい場合は、エラー(比較条件の桁数以上の場合ＯＫ)
************************************************************************************************ */
function chkSiteiijyouByte(str,keta) {
	var iLength = 0;
	var i;
	// Netscape Navigatorの場合
	if (navigator.appName.indexOf("Netscape") >= 0) {
		iLength = chkSiteiijyouByte.arguments[0].length;
	}
	// Microsoft Internet Explorerの場合
	if (navigator.appName.lastIndexOf("Explorer") >= 0) {
		if (chkSiteiijyouByte.arguments[0] == "") {
			return true;
		}
		for (i = 0; i < chkSiteiijyouByte.arguments[0].length; i++) {
			sBuffer = chkSiteiijyouByte.arguments[0].substring(i, i+1);
			if ((sBuffer >= " " && sBuffer <= "~") || (sBuffer >= "? && sBuffer <= "?)) {
				iLength++;
			}else{
				iLength += 2;
			}
		}
	}
	if (keta > iLength) {
		return false;
	}else{
		return true;
	}
}
/* ************************************************************************************************
	関数名 chkSiteiMojiretu()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 指定文字列チェック
				 比較条件文字列と同じ場合エラー
************************************************************************************************ */
function chkSiteiMojiretu(str,moji) {
	var wkstr = str;
	var wkmoji = moji;
	if (wkstr == wkmoji){
		return false;
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkSiteinaiByte()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件の桁数
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 指定内バイト数チェック
				 比較条件の桁数より大きい場合は、エラー(比較条件の桁数以下の場合ＯＫ)
************************************************************************************************ */
function chkSiteinaiByte(str,keta) {
	var iLength = 0;
	var i;
	// Netscape Navigatorの場合
	if (navigator.appName.indexOf("Netscape") >= 0) {
		iLength = chkSiteinaiByte.arguments[0].length;
	}
	// Microsoft Internet Explorerの場合
	if (navigator.appName.lastIndexOf("Explorer") >= 0) {
		if (chkSiteinaiByte.arguments[0] == "") {
			return true;
		}
		for (i = 0; i < chkSiteinaiByte.arguments[0].length; i++) {
			sBuffer = chkSiteinaiByte.arguments[0].substring(i, i+1);
			if ((sBuffer >= " " && sBuffer <= "~") || (sBuffer >= "? && sBuffer <= "?)) {
				iLength++;
			}else{
				iLength += 2;
			}
		}
	}
	if (keta < iLength) {
		return false;
	}else{
		return true;
	}
}
/* ************************************************************************************************
	関数名 chkSiteinaiMoji()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 比較条件の桁数
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 指定内文字数チェック
				 比較条件の文字数より大い場合は、エラー(比較条件の桁数以下の場合ＯＫ)
************************************************************************************************ */
function chkSiteinaiMoji(str,keta) {

	var iLength;
	iLength = str.length;

	if (parseInt(keta,10) < parseInt(iLength,10)) {
		return false;
	}else{
		return true;
	}
}
/* ************************************************************************************************
	関数名 chkSyousuu()
		作成日 : 2000/04/21
		引数   : 1番目      -> チェックする文字列
		引数   : 2番目      -> 整数部桁数
		引数   : 3番目      -> 小数部桁数
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 小数チェック(-もＯＫ)
				 引数の整数部、引数の小数部の小数範囲外の場合エラー
************************************************************************************************ */
function chkSyousuu(str,seisuu,syousuu) {
	var wkstr = str;
	var wkatai1 = parseInt(seisuu,10);
	var wkatai2 = parseInt(syousuu,10);
	var kazu = 0;
	var seisuu_bu = 0;
	var syousuu_bu = 0;
	var hajime = 0;
	//頭-の場合、半角数字チェックの開始位置をずらす
	ch = wkstr.substring(0,1);
	if (ch == "-") {
		hajime = 1;
	}
	//半角数字または.のチェック
	for (i = hajime; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if (i == hajime || i == (wkstr.length - 1)) {
			if  (!(ch >= "0" &&  ch <= "9")) {
				return false;
			}
		}else{
			if  (!(ch >= "0" &&  ch <= "9" || ch == ".")) {
				return false;
			}
		}
		if (ch == ".") {
			kazu = kazu + 1
		}else{
			if (kazu == "") {
				seisuu_bu = seisuu_bu + 1
			}else{
				syousuu_bu = syousuu_bu + 1
			}
		}
	}
	//.が２以上ないかのチェック
	if (kazu >= 2) {
		return false;
	}
	//整数部の桁数チェック
	if (seisuu_bu > wkatai1) {
		return false;
	}
	//小数部の桁数チェック
	if (syousuu_bu > wkatai2) {
		return false;
	}
	//整数部頭２桁以上の場合頭一桁目0以外のチェック
	if (seisuu_bu >= 2) {
		ch = wkstr.substring(hajime,hajime+1);
		if (ch == "0") {
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkTelFaxNo()
		作成日 : 2001/07/25
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角数字ハイフンチェック
				 半角数字(0～9)、半角ハイフン(-)以外ならエラー表示
************************************************************************************************ */
function chkTelFaxNo(str) {
	var ch;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if  ((ch >= "0" &&  ch <= "9") || (ch == "-")) {
		} else {
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkUserID()
		作成日 : 2000/11/20
		引数   : 1番目      -> チェックする文字列
		戻り値 : 0 or メッセージ文字(エラーの場合メッセージ文字を返す)
		概要   : 半角英数、.、_、-チェック(左記以外の場合エラー表示)
************************************************************************************************ */
function chkUserID(str) {
	var str1 = str;
	var kazu = 0;
	if (str1 != "") {
		//半角半角英数、.、_、-チェック
		for (i = 0; i < str1.length; i++) {
			ch = str1.substring(i,i + 1);
			if (!((ch >= "0" &&  ch <= "9") || (ch >= "A" && ch <= "Z") || (ch >= "a" && ch <= "z") || (ch == ".") || (ch == "_") || (ch == "-"))){
				var err = "を正しく入力して下さい。";
				return err;
			}
		}
	}
	var err = "0";
	return err;
}

/* ************************************************************************************************
	関数名 fnChkVal(objFrm)
		作成日 :  2003/10/008
		引数   : 1番目      -> フォームオブジェクト
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : フォーム内のコントロール全てのchkvalをチェックし、チェック処理を行う
			 CHKVALの値について
			 以下の値をカンマ区切りで連結する。以下のセットをさらに"@"(アットマーク)で連結する事が可能
				 ①チェック番号
				 ②チェック処理パラメータ1
				 ③チェック処理パラメータ2
				 ④チェック処理パラメータ3
				 ⑤メッセージ番号
				 ⑥メッセージパラメータ1
				 ⑦メッセージパラメータ2
				 ⑧メッセージパラメータ3
				 ⑨メッセージパラメータ4
************************************************************************************************ */
function fnChkVal(frm)
{
	var iObjCount;
	var iChkCount;
	var iChkSuu;
	var arryAtsplit;
	var arryLineSplit;
	var arryCommasplit1;
	var arryCommasplit2;
	var ret;
	var objObject				//<-- 2004/08/04 add (要望No113)  ukaji
	
	iChkSuu = frm.elements.length;

    for(iObjCount=0; iObjCount < iChkSuu; iObjCount++){
		if (frm.elements[iObjCount].CHKVAL != null)
		{
	  		if (frm.elements[iObjCount].CHKVAL != "") {
	  			//arryAtsplitに@が含まれる場合
				arryAtsplit = frm.elements[iObjCount].CHKVAL.split("@")
				//作成された配列文ループ
				for(iChkCount=0; iChkCount < arryAtsplit.length; iChkCount++){
					arryLineSplit =  arryAtsplit[iChkCount].split("|");
					//チェック処理とメッセージ処理がセットになっていない場合(開発者向けメッセージ)
					if(arryLineSplit.length!=2){
						alert("CHKVALの値が不正です。確認して下さい。(" + frm.elements[iObjCount].name + ")");
						return false;
					}
					arryConnmasplit1 = arryLineSplit[0].split(",");
					arryConnmasplit2 = arryLineSplit[1].split(",");
					//チェック処理
					ret = fnDisposeChk(arryConnmasplit1[0],frm.elements[iObjCount].value,arryConnmasplit1[1],arryConnmasplit1[2],arryConnmasplit1[3]);
					if(ret!="")
					{
						fnMsg(arryConnmasplit2[0],arryConnmasplit2[1],arryConnmasplit2[2],arryConnmasplit2[3],arryConnmasplit2[4],ret)
						if(frm.elements[iObjCount].type=="select-one"||frm.elements[iObjCount].type=="button"||frm.elements[iObjCount].type=="text"||frm.elements[iObjCount].type=="checkbox"||frm.elements[iObjCount].type=="radio"||frm.elements[iObjCount].type=="submit"||frm.elements[iObjCount].type=="file"||frm.elements[iObjCount].type=="reset"||frm.elements[iObjCount].type=="password"||frm.elements[iObjCount].type=="select-multiple"||frm.elements[iObjCount].type=="textarea"){
							frm.elements[iObjCount].disabled = false;
							if (frm.elements[iObjCount].CHKVALRETURNOBJECT == null){
								frm.elements[iObjCount].focus();
							}else{
								frm.item(frm.elements[iObjCount].CHKVALRETURNOBJECT).focus();
							}
						}else{
							if (frm.elements[iObjCount].CHKVALRETURNOBJECT != null){
								objObject = frm.item(frm.elements[iObjCount].CHKVALRETURNOBJECT);
								if(objObject.type=="select-one"||objObject.type=="button"||objObject.type=="text"||objObject.type=="checkbox"||objObject.type=="radio"||objObject.type=="submit"||objObject.type=="file"||objObject.type=="reset"||objObject.type=="password"||objObject.type=="select-multiple"||objObject.type=="textarea"){
									objObject.focus();
								}
							}
						}
						return false;
					}
				}
	   		}
		}
	}

	return true;

}

/* ************************************************************************************************
	関数名 fnDisposeChk()
		作成日 : 2003/10/08
		引数   : 1番目      -> チェック番号
		引数   : 2番目      -> チェックする値
		引数   : 3番目      -> パラメータ①
		引数   : 4番目      -> パラメータ②
		引数   : 5番目      -> パラメータ③
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 指定されたパラメータのチェック処理を呼び出す
************************************************************************************************ */
function fnDisposeChk(chkNo,val,param1,param2,param3)
{
	var ret = "";
	
	switch(chkNo)
	{
		case "101":	//半角数値チェック
			if(chkHankakuSuuji(val)==false)
			{
				ret = "err";
			}
		break;
		case "102":	//半角英数字チェック(大文字～小文字)
			if(chkHankakuEisuuji(val)==false)
			{
				ret = "err";
			}
		break;
		case "103":	//半角英字チェック(大文字～小文字)
			if(chkHankakuEiji(val)==false)
			{
				ret = "err";
			}
		break;
		case "104":	//外字チェック
			ret = chkGaiji(val);
		break;
		case "105":	//半角文字チェック
			if(chkHankaku(val)==false)
			{
				ret = "err";
			}
		break;
		case "106":	//指定内バイト数チェック
			if(chkSiteiByte(val,param1)==false)
			{
				ret = "err";
			}
		break;
		case "107":	//日付チェック(yyyy/mm/dd)
			ret = chkHiduke(val);
			if(ret == "0")
			{
				ret ="";
			}
		break;
		case "108":	//年月チェック(yyyy/mm)
			ret = chkNengetu(val);
			if(ret == "0")
			{
				ret ="";
			}
		break;
		case "109":	//時間チェック(hh:mm)
			ret = chkJikan(val);
			if(ret == "0")
			{
				ret = "";
			}
		break;
		case "110":	//改行エラーチェック
			if(chkKaigyou(val)==false)
			{
				ret = "err";
			}
		break;
		case "111":	//大小比較チェック （ >= 比較）
			if(trim(param1)!="")
			{
				if(chkDaisyouHikaku(parseFloat(param1),parseFloat(val))==false)
				{
					ret = "err";
				}
			}else{
				if(chkDaisyouHikaku(parseFloat(val),parseFloat(param2))==false)
				{
					ret = "err";
				}
			}
		break;
		case "112":	//大小比較チェック2 （ > 比較）
			if(trim(param1)!="")
			{
				if(chkDaisyouHikaku2(parseFloat(param1),parseFloat(val))==false)
				{
					ret = "err";
				}
			}else{
				if(chkDaisyouHikaku2(parseFloat(val),parseFloat(param2))==false)
				{
					ret = "err";
				}
			}
		break;
		case "113":	//エラー文字列チェック
			if(chkErrMojiretu(val,trim(param1))==false)
			{
				ret = "err";
			}
		break;
		case "114":	//指定文字列チェック
			if(chkSiteiMojiretu(val,trim(param1))==false)
			{
				ret = "err";
			}
		break;
		case "115":	//未来日付チェック
			if(chkMiraiHiduke(val,trim(param1))==false)
			{
				ret = "err";
			}
		break;
		case "116":	//過去日付チェック
			if(chkKakoHiduke(val,trim(param1))==false)
			{
				ret = "err";
			}
		break;
		case "117":	//整数チェック OK
			if(chkSeisuu(val)==false)
			{
				ret = "err";
			}
		break;
		case "118":	//日付範囲以内チェック
			if(chkHidukeHaninai(trim(param1),val,trim(param2))==false)
			{
				ret = "err";
			}
		break;
		case "119":	//小数チェック
			if(chkSyousuu(val,trim(param1),trim(param2))==false)
			{
				ret = "err";
			}
		break;
		case "120":	//指定バイト数以上チェック
			if(chkSiteiijyouByte(val,trim(param1))==false)
			{
				ret = "err";
			}
		break;
		case "121":	//半角カナ英数字チェック
			if(chkHankakuKanaEisuuji(val)==false)
			{
				ret = "err";
			}
		break;
		case "122":	//ラインコードチェック
			ret = chkLineCd(val);
			if(ret=="0")
			{
				ret = "";
			}
		break;
		case "123":	//メールアドレスチェック
			ret = chkMailAddress(val);
			if(ret=="0")
			{
				ret = "";
			}
		break;
		case "124":	//半角文字チェック(,"<>'&を除く)
			if(chkHankaku(val)==false)
			{
				ret = "err";
			}
		break;
		case "125":	//全銀文字チェック
			if(chkZengin(val)==false)
			{
				ret = "err";
			}
		break;
		case "126":	//全銀文字(全角カナバージョン)チェック
			if(chkZenginZenkaku(val)==false)
			{
				ret = "err";
			}
		break;
/*		case "127":	//全角→半角置換え(数字、．、／のみ)
			if(chgHankakuSuuji(val)==false)
			{
				ret = "err";
			}
		break;
		case "128":	//全角→半角置換え(数字、大･小文字英字のみ)
			if(chgHankakuEisuuji(val)==false)
			{
				ret = "err";
			}
		break;
		case "129":	//半角0を入れる(例：2000/1→2000/01)
			if(insZeroNengetu(val)==false)
			{
				ret = "err";
			}
		break;
		case "130":	//カンマをはずす
			if(chgCommaNasi(val)==false)
			{
				ret = "err";
			}
		break;
*/
		case "131":	//ユーザーIDチェック
			ret = chkUserID(val);
			if(ret=="0")
			{
				ret = "";
			}
		break;
		case "132":	//郵便番号チェック
			ret=chkZipCd(val);
			if(ret=="ok")
			{
				ret = "";
			}
		break;
		case "133":	//電話番号・FAX番号チェック
			if(chkTelFaxNo(val)==false)
			{
				ret = "err";
			}
		break;
/*
		case "134":	//全角→半角置換え(数字、ハイフンのみ)
			if(chgHankakuSuujiHyphen(val)==false)
			{
				ret = "err";
			}
		break;
		case "135":	//半角0を入れる(例：2000/1/1→2000/01/01)
			if(insZeroNengappi(val)==false)
			{
				ret = "err";
			}
		break;
*/
		case "136":	//リストボックス必須選択
			if(chkSentakuHissu(val)==false)
			{
				ret = "err";
			}
		break;
		case "137":	//必須入力
			if(chkNyuuryokuHissu(val)==false)
			{
				ret = "err";
			}
		break;
		case "138":	//禁止文字
			if(chkKinsiMoji(val)==false)
			{
				ret = "err";
			}
		break;
		case "139":	//指定内バイト数チェック
			if(chkSiteinaiByte(val,param1)==false)
			{
				ret = "err";
			}
		break;
		case "140":	//全角カナチェック(英数字チェックとセットで行う)
			if(chkZenkakuKanaEisuuji(val)==false)
			{
				ret = "err";
			}
			if(chkZenkakuKana(val)==false)
			{
				ret = "err";
			}
		break;
		case "141":	//全角かな英数字チェック
			if(chkZenkakuKanaEisuuji(val)==false)
			{
				ret = "err";
			}
		break;
		case "142":	//整数指定範囲内チェック
			if(chkSeisuuSiteiHanni(val,param1,param2)==false)
			{
				ret = "err";
			}
		break;
		case "143":	//メールアドレスチェック(マスタ用)
			ret = chkMailAddressMst(val);
			if(ret=="0")
			{
				ret = "";
			}
		break;
		case "144":	//禁止文字チェック(マスタ用)
			ret = chkKinsiMojiKanaOK(val);
			if(chkKinsiMojiKanaOK(val)==false)
			{
				ret =  "err";
			}else{
				ret =  "";
			}
		break;
		case "145":	//半角数字チェック(マイナス許容)
			ret = chkHankakuSuujiMinusOK(val);
			if(chkHankakuSuujiMinusOK(val)==false)
			{
				ret =  "err";
			}else{
				ret =  "";
			}
		break;
		case "146":	//全角カナチェック
			if ( chkZenkakuKana(val) == false ){
				ret = "err";
			}
		break;
		case "147":	//JANコードチェック
			ret = fncChkJanCode(val,trim(param1));
		break;
		case "148":	//全角チェック(全角文字以外入力不可)
			if (fncChkZenkaku(val) == false ){
				ret = "err";
			}
		break;
		case "149":	//ITFコードチェック
			ret = fncChkItfCode(val);
		break;
		case "150":	//半角英字小文字チェック
			if (chkHankakuEijiKomoji(val) == false ){
				ret = "err";
			}
		break;
		case "151":	//指定内文字数チェック
			if(chkSiteinaiMoji(val,param1)==false)
			{
				ret = "err";
			}
		break;
		case "152":	//禁止文字チェック(マスタ用)
			if(chkZenkakuKinsiMoji(val)==false)
			{
				ret =  "err";
			} 
		default:
		break;
		case "153":	//全半角カナチェック
			if ( chkZenHankakuKana2(val) == false ){
				ret = "err";
			}
		break;
		case "154":
			if(chkKinsiMojiHost(val)==false)
			{
				ret =  "err";
			}else{
				ret =  "";
			}
		break;
		// 2014/05/01 add wadak7
		case "155":	//禁止文字チェック
			if ( chkKinsiMojiReceipt(val) == false ){
				ret = "err";
			}
			break;
	}
	return ret;
}
/* ************************************************************************************************
	関数名 chkZengin()
		作成日 : 2000/09/22
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 全銀文字チェック
				 ・半角数字："0"～"9"
				 ・半角英字："A"～"Z"(大文字のみ)
				 ・半角カナ："?～"?(大文字のみ)
				 ・濁点　　："?、"?
				 ・記号　　："\"、"."、"?、"?、"-"、"/"、"("、")"
				 ・その他　：" "(半角スペース)
				以外は、エラー
				※本来","(半角カンマ)も含まれるが入力禁止文字なのでエラーとする
************************************************************************************************ */
function chkZengin(str) {
	var ch1;
	var wkstr = str;
	if (wkstr != "") {
		for (i = 0; i < wkstr.length; i++) {
			ch1 = wkstr.substring(i,i+1);
			if (!(ch1 == "(" || ch1 == ")" || ch1 == "-" || ch1 == "." || ch1 == "/" || (ch1 >= "0" && ch1 <= "9") || (ch1 >= "A" && ch1 <= "Z") || ch1 == "\\" || ch1 == "? || ch1 == "? || ch1 == " " || ch1 == "? || (ch1 >= "? && ch1 <= "?))) {
				return false;
			}
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkZenginZenkaku()
		作成日 : 2000/09/25
		変更日 : 2004/03/24 (全銀チェックのバグ修正 Nishiwaki)
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 全銀文字チェック
				 ・数字："0"～"9"、"０"～"９"
				 ・英字："A"～"Z"、"Ａ"～"Ｚ"(大文字のみ)
				 ・全角カナ："ア"～"ン"(大文字のみ)
				 ・記号　　："\"、"￥"、"."、"．"、"?、"「"、"?、"」"、"-"、"－"、"/"、"／"、"("、"（"、")"、"）"
				 ・その他　：" "(半角スペース)、"　"(全角スペース)
				以外は、エラー
				※本来","(半角カンマ)も含まれるが入力禁止文字なのでエラーとする
************************************************************************************************ */
function chkZenginZenkaku(str) {
	var ch1;
	var wkstr = str;
	if (wkstr != "") {
		for (i = 0; i < wkstr.length; i++) {
			ch1 = wkstr.substring(i,i+1);
			if (!(ch1 == "(" || ch1 == ")" || ch1 == "-" || ch1 == "." || ch1 == "/" || (ch1 >= "0" && ch1 <= "9") || (ch1 >= "A" && ch1 <= "Z") || ch1 == "\\" || ch1 == "? || ch1 == "? || ch1 == " " || (ch1 >= "ア" && ch1 <= "ヴ") || ch1 == "（" || ch1 == "）" || ch1 == "－" || ch1 == "．" || ch1 == "／" || (ch1 >= "０" && ch1 <= "９") || (ch1 >= "Ａ" && ch1 <= "Ｚ") || ch1 == "￥" || ch1 == "「" || ch1 == "」" || ch1 == "　")) {
				return false;
			}
			if (ch1 == "ァ" || ch1 == "ィ" ||ch1 == "ゥ" ||ch1 == "ェ" ||ch1 == "ォ" ||ch1 == "ヵ" ||ch1 == "ヶ" ||ch1 == "ッ" ||ch1 == "ャ" ||ch1 == "ュ" ||ch1 == "ョ" ||ch1 == "ヮ" ||ch1 == "ヰ" ||ch1 == "ヱ") {
				return false;
			}
		}
	}
	return true;
}
/* ************************************************************************************************
関数名 chkZenkakuKana()
	作成日 : 2002/07/18
	引数   : 1番目      -> チェックする文字列
	戻り値 : true or false(エラーの場合falseを返す)
	概要   : 全角カタカナチェック
	注意   : Shift_Jis 限定
	       : ２バイト判定を行っていないので、半角チェックを先に行う事
	       : 記号が使われることも考慮し、アルファベット、ひらがな、漢字を除外している
************************************************************************************************ */
function chkZenkakuKana(text)
{
  var temp;
  for( i = 0; i < text.length; i++ )
  {
	temp = text.charAt(i);
    if( (temp >= 'Ａ' && temp <= 'ｚ') )	//ShiftJis アルファベット除外
    {
	    return false;
    }
    if( (temp >= 'ぁ' && temp <= 'ん') )	//ShiftJis ひらがな除外
    {
	    return false;
    }
	if( (temp >= '亜' && temp <= '黑') )	//ShiftJis 漢字除外
	{
		return false;
	}
  }
  return true;
}
/* ************************************************************************************************
関数名 chkZenkakuKana()
	作成日 : 2002/07/18
	引数   : 1番目      -> チェックする文字列
	戻り値 : true or false(エラーの場合falseを返す)
	概要   : 全角カタカナチェック
	注意   : Shift_Jis 限定
	       : ２バイト判定を行っていないので、半角チェックを先に行う事
	       : 記号が使われることも考慮し、アルファベット、ひらがな、漢字を除外している
************************************************************************************************ */
function chkZenHankakuKana2(text)
{
	var temp;
	var i;

	for (i = 0; i < text.length; i++) {
		temp = text.charAt(i);
		//全角スペースは半角スペースに置き換え(特別措置)
		temp = (temp == "　") ? " " : temp;
		//全角チェック
		if((chkZenkakuKanaEisuuji(temp)==true)||(chkHankaku(temp)==true)){
		}else{
			return false;
		}
	}
	return true;
}

/* ****************************************************************************
	関数名 chkZenkakuKanaEisuuji()

		作成日 : 2002/11/08
		引数   : 1番目   -> チェックする文字列
		戻り値 : true    -> 全角カナ 全角英数 全角の。「」、・゛゜（）－はＯＫ
				 false   -> true以外の時

**************************************************************************** */
function chkZenkakuKanaEisuuji(sKana) {
	var i;
	var sBuffer
	for (i = 0; i < sKana.length; i++) {
		sBuffer = sKana.substring(i, i+1);
		switch( sBuffer){
		case "。":
		case "「":
		case "」":
		case "、":
		case "・":
		case "ヲ":
		case "ァ":
		case "ィ":
		case "ゥ":
		case "ェ":
		case "ォ":
		case "ャ":
		case "ュ":
		case "ョ":
		case "ッ":
		case "ー":
		case "ア":
		case "イ":
		case "ウ":
		case "エ":
		case "オ":
		case "カ":
		case "キ":
		case "ク":
		case "ケ":
		case "コ":
		case "サ":
		case "シ":
		case "ス":
		case "セ":
		case "ソ":
		case "タ":
		case "チ":
		case "ツ":
		case "テ":
		case "ト":
		case "ナ":
		case "ニ":
		case "ヌ":
		case "ネ":
		case "ノ":
		case "ハ":
		case "ヒ":
		case "フ":
		case "ヘ":
		case "ホ":
		case "マ":
		case "ミ":
		case "ム":
		case "メ":
		case "モ":
		case "ヤ":
		case "ユ":
		case "ヨ":
		case "ラ":
		case "リ":
		case "ル":
		case "レ":
		case "ロ":
		case "ワ":
		case "ン":
		case "゛":
		case "゜":
		case "ヴ":
		case "ガ":
		case "ギ":
		case "グ":
		case "ゲ":
		case "ゴ":
		case "ザ":
		case "ジ":
		case "ズ":
		case "ゼ":
		case "ゾ":
		case "ダ":
		case "ヂ":
		case "ヅ":
		case "デ":
		case "ド":
		case "バ":
		case "ビ":
		case "ブ":
		case "ベ":
		case "ボ":
		case "パ":
		case "ピ":
		case "プ":
		case "ペ":
		case "ポ":
		case "（":
		case "）":
		case "－":
		case "０":
		case "１":
		case "２":
		case "３":
		case "４":
		case "５":
		case "６":
		case "７":
		case "８":
		case "９":
		case "Ａ":
		case "Ｂ":
		case "Ｃ":
		case "Ｄ":
		case "Ｅ":
		case "Ｆ":
		case "Ｇ":
		case "Ｈ":
		case "Ｉ":
		case "Ｊ":
		case "Ｋ":
		case "Ｌ":
		case "Ｍ":
		case "Ｎ":
		case "Ｏ":
		case "Ｐ":
		case "Ｑ":
		case "Ｒ":
		case "Ｓ":
		case "Ｔ":
		case "Ｕ":
		case "Ｖ":
		case "Ｗ":
		case "Ｘ":
		case "Ｙ":
		case "Ｚ":
		case "ａ":
		case "ｂ":
		case "ｃ":
		case "ｄ":
		case "ｅ":
		case "ｆ":
		case "ｇ":
		case "ｈ":
		case "ｉ":
		case "ｊ":
		case "ｋ":
		case "ｌ":
		case "ｍ":
		case "ｎ":
		case "ｏ":
		case "ｐ":
		case "ｑ":
		case "ｒ":
		case "ｓ":
		case "ｔ":
		case "ｕ":
		case "ｖ":
		case "ｗ":
		case "ｘ":
		case "ｙ":
		case "ｚ":
		case "　":
			break;
		default:
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkZipCd()
		作成日 : 2001/07/24
		引数   : 1番目      -> チェックする文字列
		戻り値 : チェックＯＫの時、"ok" or チェックＮＧの時、エラーメッセージ
		概要   : ①８桁チェック
				 ②先頭３桁半角数字チェック
				 ③４桁目ハイフンチェック
				 ④５～８桁半角数字チェック
************************************************************************************************ */
function chkZipCd(prmChkMoji) {
	var strChkMoji = prmChkMoji;
	var strChk1Byte;
	var strErrMsg;
	var numIndex;

	//①８桁チェック
	if (!chkSiteiByte(strChkMoji,8)) {
		strErrMsg = "は半角8桁で入力して下さい。";
		return strErrMsg;
	}

	//②先頭３桁半角数字チェック
	for (numIndex = 0; numIndex < 3; numIndex++) {
		strChk1Byte = strChkMoji.substr(numIndex,1);
		if  (!(strChk1Byte >= "0" &&  strChk1Byte <= "9")) {
			strErrMsg = "は半角数字で入力して下さい。";
			return strErrMsg;
		}
	}

	//③４桁目ハイフンチェック
	strChk1Byte = strChkMoji.substr(3,1);
	if (strChk1Byte != "-") {
		strErrMsg = "の4桁目はハイフンを入力して下さい。";
		return strErrMsg;
	}

	//④５～８桁半角数字チェック
	for (numIndex = 4; numIndex < strChkMoji.length; numIndex++) {
		strChk1Byte = strChkMoji.substr(numIndex,1);
		if  (!(strChk1Byte >= "0" &&  strChk1Byte <= "9")) {
			strErrMsg = "は半角数字で入力して下さい。";
			return strErrMsg;
		}
	}

	strErrMsg = "ok";
	return strErrMsg;

}

/* ************************************************************************************************
	関数名 chkMailAddressMst()
		作成日 : 2004/02/25
		引数   : 1番目      -> チェックする文字列
		戻り値 : 0 or メッセージ文字(エラーの場合メッセージ文字を返す)
		概要   : マスタメンテ用(指定内バイトチェックがない)
				 半角英数、@、.、_、-チェック(左記以外の場合エラー表示)
				 @１つチェック(@２つ以上、@なしの場合エラー)
************************************************************************************************ */
function chkMailAddressMst(str) {
	var str1 = str;
	var kazu = 0;
	if (str1 != "") {
		//半角半角英数、@、.、_、-チェック
		for (i = 0; i < str1.length; i++) {
			ch = str1.substring(i,i + 1);
			if (!((ch >= "0" &&  ch <= "9") || (ch >= "A" && ch <= "Z") || (ch >= "a" && ch <= "z") || (ch == "@") || (ch == ".") || (ch == "_") || (ch == "-"))){
				var err = "を正しく入力して下さい。";
				return err;
			}else{
				if (ch == "@") {
					kazu = kazu + 1
				}
			}
		}
		//@一つチェック
		if (kazu != 1) {
			var err = "を正しく入力して下さい。";
			return err;
		}
	}
	var err = "0";
	return err;
}
/* ************************************************************************************************
	関数名 chkKinsiMojiKanaOK()
		作成日 : 2004/04/06  寺田和世
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 禁止文字チェック、ただし半角カナは可(禁止文字入力の場合エラー表示)
				 Uniコード 34=" 38=& 39=' 44=, 60=< 62=> \=92
		修正日：
************************************************************************************************ */
function chkKinsiMojiKanaOK(str) {
	var ch1;
	var ch2;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch1 = wkstr.charCodeAt(i);
		ch2 = wkstr.substring(i,i+1);
		if (ch1 == 44 || ch1 == 39 || ch1 == 34 || ch1 == 60 || ch1 == 62 || ch1 == 92) {
			return false;
/*
		}else if(ch1 >= 9312 && ch1 <= 9331){
			return false;
		//㍉ ～㎡
		}else if(ch1 >= 13120 && ch1 <= 13217){
			return false;
		//─ ～ ╋
		}else if(ch1 >= 9472 && ch1 <= 9547){
			return false;
		//㍻
		}else if(ch1 == 13179){
			return false;
		//〝
		}else if(ch1 == 12317){
			return false;
		//〟
		}else if(ch1 == 12319){
			return false;
		//㏍
		}else if(ch1 == 13261){
			return false;
		//℡
		}else if(ch1 == 8481){
			return false;
		//㊤
		}else if(ch1 == 12964){
			return false;
		//㊥
		}else if(ch1 == 12965){
			return false;
		//㊦
		}else if(ch1 == 12966){
			return false;
		//㊧
		}else if(ch1 == 12967){
			return false;
		//㊨
		}else if(ch1 == 12968){
			return false;
		//㈲
		}else if(ch1 == 12850){
			return false;
		//㈹
		}else if(ch1 == 12857){
			return false;
		//㍾
		}else if(ch1 == 13182){
			return false;
		//㍽
		}else if(ch1 == 13181){
			return false;
		//㍼
		}else if(ch1 == 13180){
			return false;
*/
		}
	}
	return true;
}

/* ************************************************************************************************
	関数名 chkKinsiMojiHost()
		作成日 : 2005/04/25
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 禁止文字チェック、ただし半角カナは可(禁止文字入力の場合エラー表示)
				 Uniコード 34=" 38=& 39=' 44=, 60=< 62=> \=92
		修正日：
************************************************************************************************ */
function chkKinsiMojiHost(str) {
	var ch1;
	var ch2;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch1 = wkstr.charCodeAt(i);
		ch2 = wkstr.substring(i,i+1);
		if (ch1 == 44 || ch1 == 39 || ch1 == 34 || ch1 == 60 || ch1 == 62 || ch1 == 92) {
			return false;
		}else if(ch1 >= 9312 && ch1 <= 9331){
			return false;
		//㍉ ～㎡
		}else if(ch1 >= 13120 && ch1 <= 13217){
			return false;
		//─ ～ ╋
		}else if(ch1 >= 9472 && ch1 <= 9547){
			return false;
		//㍻
		}else if(ch1 == 13179){
			return false;
		//〝
		}else if(ch1 == 12317){
			return false;
		//〟
		}else if(ch1 == 12319){
			return false;
		//㏍
		}else if(ch1 == 13261){
			return false;
		//℡
		}else if(ch1 == 8481){
			return false;
		//㊤
		}else if(ch1 == 12964){
			return false;
		//㊥
		}else if(ch1 == 12965){
			return false;
		//㊦
		}else if(ch1 == 12966){
			return false;
		//㊧
		}else if(ch1 == 12967){
			return false;
		//㊨
		}else if(ch1 == 12968){
			return false;
		//㈲
		}else if(ch1 == 12850){
			return false;
		//㈹
		}else if(ch1 == 12857){
			return false;
		//㍾
		}else if(ch1 == 13182){
			return false;
		//㍽
		}else if(ch1 == 13181){
			return false;
		//㍼
		}else if(ch1 == 13180){
			return false;
		}
	}
	return true;
}
/* ************************************************************************************************
	関数名 chkZenkakuKinsiMoji()
		作成日 : 2004/04/06  寺田和世
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 禁止文字チェック(全角バージョン)、ただし半角カナは可(禁止文字入力の場合エラー表示)
				 Uniコード 8221=" 65286=& 8217=' 65292=, 65308=＜ ＞=65310 \=65509
		修正日：
************************************************************************************************ */
function chkZenkakuKinsiMoji(str) {
	var ch1;
	var ch2;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch1 = parseFloat(wkstr.charCodeAt(i));
		ch2 = wkstr.substring(i,i+1);
		if (ch1 == 8221 || ch1 == 65286 || ch1 == 8217 || ch1 == 65292 || ch1 == 65308 || ch1 == 65310 || ch1 == 65509) {
			return false;
		}
	}
	return true;
}

/* ************************************************************************************************
	関数名 chkHankakuSuujiMinusOK()
		作成日 : 2004/04/22
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角数字チェック(マイナス値許容)
				 半角数字(0～9)以外ならエラー表示
************************************************************************************************ */
function chkHankakuSuujiMinusOK(str) {
	var ch;
	var wkstr = str;

	//数値として認識できるか？
//	if (isNaN(num = parseInt(str))) {
//		return false;
//	}

	for (i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if (i == 0 && ch == "-"){
			//先頭文字のみ"-"を許容
		}else if (!(ch >= "0" &&  ch <= "9")){
			return false;
		}
	}
	return true;
}


/* ****************************************************************************
	関数名 fncChkZenkaku()

		作成日 : 2002/11/08
		引数   : 1番目   -> チェックする文字列
		戻り値 : true    -> 全角カナ 全角英数 全角の。「」、・゛゜（）－はＯＫ
				 false   -> true以外の時
**************************************************************************** */
function fncChkZenkaku(val)
{
	//ブラウザの処理をチェック
	chkType = ("あ".length);
	var i;
	var ch;
	//NetScape等
	if(chkType == "1"){
		for (i = 0; i < val.length;i++){
			ch = val.charAt(i);
			if ((ch >= " " && ch <= "~")||(ch >= "? && ch <= "?)){
				return false;
			}
		}
	}
	//IE
	if(chkType == "2"){
		for (i=0; i<val.length;i=i+2){
			ch = val.charAt(i);
			if ((ch >= " " && ch <= "~")||(ch >= "? && ch <= "?)){
				return false;
			}
		}
	}
}



/* ************************************************************************************************
	関数名 fncChkItfCode()
		作成日 : 2004/12/21
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : ITFコードのチェック
************************************************************************************************ */
function fncChkItfCode(val)
{
	var msg;
	var tmp;
	var ChkDigit;
	var Itflen;

	if(trim(val)=="")
	{
		return ""
	}

	if(isNaN(val))
	{
		return "ITFコードは半角数値で入力してください。";
	}

	Itflen = val.length;
	//チェックデジットの抽出
	ChkDigit = val.slice(Itflen - 1, Itflen);
	//コード部分の抽出
	tmp = val.slice(0, Itflen - 1);

	if(Itflen != 14 && Itflen != 16)
	{
		return "ITFコードの桁数に過不足があります。14桁もしくは16桁の数字を入力してください。";
	}

	factor = 3;
  	sum = 0;
	for (index = tmp.length; index > 0; --index)
	{
		sum = sum + tmp.substring (index-1, index) * factor;
		factor = 4 - factor;
  	}
  	if(ChkDigit != ((1000 - sum) % 10)){
  		return "ITFコードのチェックデジットを確認して下さい。";
  	}else{
  		return "";
  	}

}
/* ************************************************************************************************
	関数名 fncChkJanCode()
		作成日 : 2004/12/21
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : JANコードのチェック
************************************************************************************************ */
function fncChkJanCode(val,flg)
{
	var msg;
	var tmp;
	var ChkDigit;
	var Janlen;
	
	if(isNaN(val))
	{
		return "JANコードは半角数値で入力してください。";
	}

	if(trim(val)=="")
	{
		return ""
	}

	Janlen = val.length;

	if(Janlen != 8 && Janlen != 13)
	{
		return "JANコードの桁数に過不足があります。8桁もしくは13桁の数字を入力してください。";
	}
	
	//国番号チェック
	if(flg==1){
		if(parseInt(val.substring(0,2),10)>=20&&parseInt(val.substring(0,2),10)<=29){
			return "インストアコード(先頭2桁が20～29)での入力は出来ません。";
		}
	}
	
	//チェックデジットの抽出
	ChkDigit = val.slice(Janlen - 1, Janlen);
	//コード部分の抽出
	tmp = val.slice(0, Janlen - 1);

	//チェックデジット整合チェック
	factor = 3;
  	sum = 0;
	for (index = tmp.length; index > 0; --index)
	{
		sum = sum + tmp.substring (index-1, index) * factor;
		factor = 4 - factor;
  	}
  	if(ChkDigit != ((1000 - sum) % 10)){
  		return "JANコードのチェックデジットを確認して下さい。";
  	}else{
  		return "";
  	}

}
/* ************************************************************************************************
	関数名 chkHankakuKanaEisuuji()
		作成日 : 2009/02/19 Siriporn B.
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角カナ英数字チェック(半角カナ英数字以外の場合エラー表示) || space
************************************************************************************************ */
function chkHankakuKanaEisuuji_space(str) {
	var ch;
	var wkstr = str;
	for (i = 0; i < wkstr.length; i++) {
		ch = wkstr.substring(i,i + 1);
		if (!((ch >= "0" &&  ch <= "9") || (ch >= "A" && ch <= "Z") || (ch >= "a" && ch <= "z") || (ch >= "? && ch <= "?) || (ch == " ") || (ch == "+") || (ch == "-") || (ch == "(")|| (ch == ")") || (ch == "_"))){
			return false;
		}
	}
	return true;
}
