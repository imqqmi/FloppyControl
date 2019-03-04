(function(){var k;function aa(a){var b=0;return function(){return b<a.length?{done:!1,value:a[b++]}:{done:!0}}}
function ba(a){var b="undefined"!=typeof Symbol&&Symbol.iterator&&a[Symbol.iterator];return b?b.call(a):{next:aa(a)}}
var ca="function"==typeof Object.create?Object.create:function(a){function b(){}
b.prototype=a;return new b},da;
if("function"==typeof Object.setPrototypeOf)da=Object.setPrototypeOf;else{var fa;a:{var ha={oa:!0},ia={};try{ia.__proto__=ha;fa=ia.oa;break a}catch(a){}fa=!1}da=fa?function(a,b){a.__proto__=b;if(a.__proto__!==b)throw new TypeError(a+" is not extensible");return a}:null}var ja=da;
function n(a,b){a.prototype=ca(b.prototype);a.prototype.constructor=a;if(ja)ja(a,b);else for(var c in b)if("prototype"!=c)if(Object.defineProperties){var d=Object.getOwnPropertyDescriptor(b,c);d&&Object.defineProperty(a,c,d)}else a[c]=b[c];a.A=b.prototype}
var ka="function"==typeof Object.defineProperties?Object.defineProperty:function(a,b,c){a!=Array.prototype&&a!=Object.prototype&&(a[b]=c.value)},la="undefined"!=typeof window&&window===this?this:"undefined"!=typeof global&&null!=global?global:this;
function ma(a,b){if(b){for(var c=la,d=a.split("."),e=0;e<d.length-1;e++){var f=d[e];f in c||(c[f]={});c=c[f]}d=d[d.length-1];e=c[d];f=b(e);f!=e&&null!=f&&ka(c,d,{configurable:!0,writable:!0,value:f})}}
function na(a,b,c){if(null==a)throw new TypeError("The 'this' value for String.prototype."+c+" must not be null or undefined");if(b instanceof RegExp)throw new TypeError("First argument to String.prototype."+c+" must not be a regular expression");return a+""}
ma("String.prototype.startsWith",function(a){return a?a:function(a,c){var b=na(this,a,"startsWith");a+="";for(var e=b.length,f=a.length,g=Math.max(0,Math.min(c|0,b.length)),h=0;h<f&&g<e;)if(b[g++]!=a[h++])return!1;return h>=f}});
ma("String.prototype.endsWith",function(a){return a?a:function(a,c){var b=na(this,a,"endsWith");a+="";void 0===c&&(c=b.length);for(var e=Math.max(0,Math.min(c|0,b.length)),f=a.length;0<f&&0<e;)if(b[--e]!=a[--f])return!1;return 0>=f}});
function oa(){oa=function(){};
la.Symbol||(la.Symbol=pa)}
function qa(a,b){this.b=a;ka(this,"description",{configurable:!0,writable:!0,value:b})}
qa.prototype.toString=function(){return this.b};
var pa=function(){function a(c){if(this instanceof a)throw new TypeError("Symbol is not a constructor");return new qa("jscomp_symbol_"+(c||"")+"_"+b++,c)}
var b=0;return a}();
function ra(){oa();var a=la.Symbol.iterator;a||(a=la.Symbol.iterator=la.Symbol("Symbol.iterator"));"function"!=typeof Array.prototype[a]&&ka(Array.prototype,a,{configurable:!0,writable:!0,value:function(){return sa(aa(this))}});
ra=function(){}}
function sa(a){ra();a={next:a};a[la.Symbol.iterator]=function(){return this};
return a}
function p(a,b){return Object.prototype.hasOwnProperty.call(a,b)}
var ta="function"==typeof Object.assign?Object.assign:function(a,b){for(var c=1;c<arguments.length;c++){var d=arguments[c];if(d)for(var e in d)p(d,e)&&(a[e]=d[e])}return a};
ma("Object.assign",function(a){return a||ta});
ma("WeakMap",function(a){function b(a){this.b=(g+=Math.random()+1).toString();if(a){a=ba(a);for(var b;!(b=a.next()).done;)b=b.value,this.set(b[0],b[1])}}
function c(){}
function d(a){p(a,f)||ka(a,f,{value:new c})}
function e(a){var b=Object[a];b&&(Object[a]=function(a){if(a instanceof c)return a;d(a);return b(a)})}
if(function(){if(!a||!Object.seal)return!1;try{var b=Object.seal({}),c=Object.seal({}),d=new a([[b,2],[c,3]]);if(2!=d.get(b)||3!=d.get(c))return!1;d["delete"](b);d.set(c,4);return!d.has(b)&&4==d.get(c)}catch(u){return!1}}())return a;
var f="$jscomp_hidden_"+Math.random();e("freeze");e("preventExtensions");e("seal");var g=0;b.prototype.set=function(a,b){d(a);if(!p(a,f))throw Error("WeakMap key fail: "+a);a[f][this.b]=b;return this};
b.prototype.get=function(a){return p(a,f)?a[f][this.b]:void 0};
b.prototype.has=function(a){return p(a,f)&&p(a[f],this.b)};
b.prototype["delete"]=function(a){return p(a,f)&&p(a[f],this.b)?delete a[f][this.b]:!1};
return b});
ma("Map",function(a){function b(){var a={};return a.previous=a.next=a.head=a}
function c(a,b){var c=a.b;return sa(function(){if(c){for(;c.head!=a.b;)c=c.previous;for(;c.next!=c.head;)return c=c.next,{done:!1,value:b(c)};c=null}return{done:!0,value:void 0}})}
function d(a,b){var c=b&&typeof b;"object"==c||"function"==c?f.has(b)?c=f.get(b):(c=""+ ++g,f.set(b,c)):c="p_"+b;var d=a.f[c];if(d&&p(a.f,c))for(var e=0;e<d.length;e++){var h=d[e];if(b!==b&&h.key!==h.key||b===h.key)return{id:c,list:d,index:e,o:h}}return{id:c,list:d,index:-1,o:void 0}}
function e(a){this.f={};this.b=b();this.size=0;if(a){a=ba(a);for(var c;!(c=a.next()).done;)c=c.value,this.set(c[0],c[1])}}
if(function(){if(!a||"function"!=typeof a||!a.prototype.entries||"function"!=typeof Object.seal)return!1;try{var b=Object.seal({x:4}),c=new a(ba([[b,"s"]]));if("s"!=c.get(b)||1!=c.size||c.get({x:4})||c.set({x:4},"t")!=c||2!=c.size)return!1;var d=c.entries(),e=d.next();if(e.done||e.value[0]!=b||"s"!=e.value[1])return!1;e=d.next();return e.done||4!=e.value[0].x||"t"!=e.value[1]||!d.next().done?!1:!0}catch(ea){return!1}}())return a;
ra();var f=new WeakMap;e.prototype.set=function(a,b){a=0===a?0:a;var c=d(this,a);c.list||(c.list=this.f[c.id]=[]);c.o?c.o.value=b:(c.o={next:this.b,previous:this.b.previous,head:this.b,key:a,value:b},c.list.push(c.o),this.b.previous.next=c.o,this.b.previous=c.o,this.size++);return this};
e.prototype["delete"]=function(a){a=d(this,a);return a.o&&a.list?(a.list.splice(a.index,1),a.list.length||delete this.f[a.id],a.o.previous.next=a.o.next,a.o.next.previous=a.o.previous,a.o.head=null,this.size--,!0):!1};
e.prototype.clear=function(){this.f={};this.b=this.b.previous=b();this.size=0};
e.prototype.has=function(a){return!!d(this,a).o};
e.prototype.get=function(a){return(a=d(this,a).o)&&a.value};
e.prototype.entries=function(){return c(this,function(a){return[a.key,a.value]})};
e.prototype.keys=function(){return c(this,function(a){return a.key})};
e.prototype.values=function(){return c(this,function(a){return a.value})};
e.prototype.forEach=function(a,b){for(var c=this.entries(),d;!(d=c.next()).done;)d=d.value,a.call(b,d[1],d[0],this)};
e.prototype[Symbol.iterator]=e.prototype.entries;var g=0;return e});
ma("Set",function(a){function b(a){this.b=new Map;if(a){a=ba(a);for(var b;!(b=a.next()).done;)this.add(b.value)}this.size=this.b.size}
if(function(){if(!a||"function"!=typeof a||!a.prototype.entries||"function"!=typeof Object.seal)return!1;try{var b=Object.seal({x:4}),d=new a(ba([b]));if(!d.has(b)||1!=d.size||d.add(b)!=d||1!=d.size||d.add({x:4})!=d||2!=d.size)return!1;var e=d.entries(),f=e.next();if(f.done||f.value[0]!=b||f.value[1]!=b)return!1;f=e.next();return f.done||f.value[0]==b||4!=f.value[0].x||f.value[1]!=f.value[0]?!1:e.next().done}catch(g){return!1}}())return a;
ra();b.prototype.add=function(a){a=0===a?0:a;this.b.set(a,a);this.size=this.b.size;return this};
b.prototype["delete"]=function(a){a=this.b["delete"](a);this.size=this.b.size;return a};
b.prototype.clear=function(){this.b.clear();this.size=0};
b.prototype.has=function(a){return this.b.has(a)};
b.prototype.entries=function(){return this.b.entries()};
b.prototype.values=function(){return this.b.values()};
b.prototype.keys=b.prototype.values;b.prototype[Symbol.iterator]=b.prototype.values;b.prototype.forEach=function(a,b){var c=this;this.b.forEach(function(d){return a.call(b,d,d,c)})};
return b});
ma("Object.is",function(a){return a?a:function(a,c){return a===c?0!==a||1/a===1/c:a!==a&&c!==c}});
ma("String.prototype.includes",function(a){return a?a:function(a,c){return-1!==na(this,a,"includes").indexOf(a,c||0)}});
(function(){function a(){function a(){}
Reflect.construct(a,[],function(){});
return new a instanceof a}
if("undefined"!=typeof Reflect&&Reflect.construct){if(a())return Reflect.construct;var b=Reflect.construct;return function(a,d,e){a=b(a,d);e&&Reflect.setPrototypeOf(a,e.prototype);return a}}return function(a,b,e){void 0===e&&(e=a);
e=ca(e.prototype||Object.prototype);return Function.prototype.apply.call(a,e,b)||e}})();
var q=this;function r(a){return void 0!==a}
function t(a){return"string"==typeof a}
function v(a,b,c){a=a.split(".");c=c||q;a[0]in c||"undefined"==typeof c.execScript||c.execScript("var "+a[0]);for(var d;a.length&&(d=a.shift());)!a.length&&r(b)?c[d]=b:c[d]&&c[d]!==Object.prototype[d]?c=c[d]:c=c[d]={}}
var ua=/^[\w+/_-]+[=]{0,2}$/,va=null;function w(a,b){for(var c=a.split("."),d=b||q,e=0;e<c.length;e++)if(d=d[c[e]],null==d)return null;return d}
function wa(){}
function xa(a){a.ca=void 0;a.getInstance=function(){return a.ca?a.ca:a.ca=new a}}
function ya(a){var b=typeof a;if("object"==b)if(a){if(a instanceof Array)return"array";if(a instanceof Object)return b;var c=Object.prototype.toString.call(a);if("[object Window]"==c)return"object";if("[object Array]"==c||"number"==typeof a.length&&"undefined"!=typeof a.splice&&"undefined"!=typeof a.propertyIsEnumerable&&!a.propertyIsEnumerable("splice"))return"array";if("[object Function]"==c||"undefined"!=typeof a.call&&"undefined"!=typeof a.propertyIsEnumerable&&!a.propertyIsEnumerable("call"))return"function"}else return"null";
else if("function"==b&&"undefined"==typeof a.call)return"object";return b}
function x(a){return"array"==ya(a)}
function za(a){var b=ya(a);return"array"==b||"object"==b&&"number"==typeof a.length}
function y(a){return"function"==ya(a)}
function z(a){var b=typeof a;return"object"==b&&null!=a||"function"==b}
var Aa="closure_uid_"+(1E9*Math.random()>>>0),Ba=0;function Ca(a,b,c){return a.call.apply(a.bind,arguments)}
function Da(a,b,c){if(!a)throw Error();if(2<arguments.length){var d=Array.prototype.slice.call(arguments,2);return function(){var c=Array.prototype.slice.call(arguments);Array.prototype.unshift.apply(c,d);return a.apply(b,c)}}return function(){return a.apply(b,arguments)}}
function A(a,b,c){Function.prototype.bind&&-1!=Function.prototype.bind.toString().indexOf("native code")?A=Ca:A=Da;return A.apply(null,arguments)}
function Ea(a,b){var c=Array.prototype.slice.call(arguments,1);return function(){var b=c.slice();b.push.apply(b,arguments);return a.apply(this,b)}}
var B=Date.now||function(){return+new Date};
function Fa(a,b){v(a,b,void 0)}
function C(a,b){function c(){}
c.prototype=b.prototype;a.A=b.prototype;a.prototype=new c;a.prototype.constructor=a;a.kb=function(a,c,f){for(var d=Array(arguments.length-2),e=2;e<arguments.length;e++)d[e-2]=arguments[e];return b.prototype[c].apply(a,d)}}
;var Ga=document,D=window;function E(a){if(Error.captureStackTrace)Error.captureStackTrace(this,E);else{var b=Error().stack;b&&(this.stack=b)}a&&(this.message=String(a))}
C(E,Error);E.prototype.name="CustomError";var Ha=Array.prototype.indexOf?function(a,b){return Array.prototype.indexOf.call(a,b,void 0)}:function(a,b){if(t(a))return t(b)&&1==b.length?a.indexOf(b,0):-1;
for(var c=0;c<a.length;c++)if(c in a&&a[c]===b)return c;return-1},F=Array.prototype.forEach?function(a,b,c){Array.prototype.forEach.call(a,b,c)}:function(a,b,c){for(var d=a.length,e=t(a)?a.split(""):a,f=0;f<d;f++)f in e&&b.call(c,e[f],f,a)},Ia=Array.prototype.filter?function(a,b){return Array.prototype.filter.call(a,b,void 0)}:function(a,b){for(var c=a.length,d=[],e=0,f=t(a)?a.split(""):a,g=0;g<c;g++)if(g in f){var h=f[g];
b.call(void 0,h,g,a)&&(d[e++]=h)}return d},Ja=Array.prototype.map?function(a,b){return Array.prototype.map.call(a,b,void 0)}:function(a,b){for(var c=a.length,d=Array(c),e=t(a)?a.split(""):a,f=0;f<c;f++)f in e&&(d[f]=b.call(void 0,e[f],f,a));
return d},Ka=Array.prototype.reduce?function(a,b,c){return Array.prototype.reduce.call(a,b,c)}:function(a,b,c){var d=c;
F(a,function(c,f){d=b.call(void 0,d,c,f,a)});
return d};
function La(a,b){a:{var c=a.length;for(var d=t(a)?a.split(""):a,e=0;e<c;e++)if(e in d&&b.call(void 0,d[e],e,a)){c=e;break a}c=-1}return 0>c?null:t(a)?a.charAt(c):a[c]}
function Ma(a,b){var c=Ha(a,b);0<=c&&Array.prototype.splice.call(a,c,1)}
function Na(a){var b=a.length;if(0<b){for(var c=Array(b),d=0;d<b;d++)c[d]=a[d];return c}return[]}
function Oa(a,b){for(var c=1;c<arguments.length;c++){var d=arguments[c];if(za(d)){var e=a.length||0,f=d.length||0;a.length=e+f;for(var g=0;g<f;g++)a[e+g]=d[g]}else a.push(d)}}
;var Pa=String.prototype.trim?function(a){return a.trim()}:function(a){return/^[\s\xa0]*([\s\S]*?)[\s\xa0]*$/.exec(a)[1]};
function Qa(a,b){if(b)a=a.replace(Ra,"&amp;").replace(Sa,"&lt;").replace(Ta,"&gt;").replace(Ua,"&quot;").replace(Va,"&#39;").replace(Wa,"&#0;");else{if(!Xa.test(a))return a;-1!=a.indexOf("&")&&(a=a.replace(Ra,"&amp;"));-1!=a.indexOf("<")&&(a=a.replace(Sa,"&lt;"));-1!=a.indexOf(">")&&(a=a.replace(Ta,"&gt;"));-1!=a.indexOf('"')&&(a=a.replace(Ua,"&quot;"));-1!=a.indexOf("'")&&(a=a.replace(Va,"&#39;"));-1!=a.indexOf("\x00")&&(a=a.replace(Wa,"&#0;"))}return a}
var Ra=/&/g,Sa=/</g,Ta=/>/g,Ua=/"/g,Va=/'/g,Wa=/\x00/g,Xa=/[\x00&<>"']/;function Ya(a){return a=Qa(a,void 0)}
function Za(a){for(var b=0,c=0;c<a.length;++c)b=31*b+a.charCodeAt(c)>>>0;return b}
;var $a;a:{var ab=q.navigator;if(ab){var bb=ab.userAgent;if(bb){$a=bb;break a}}$a=""}function G(a){return-1!=$a.indexOf(a)}
;function cb(a,b){for(var c in a)b.call(void 0,a[c],c,a)}
function db(a,b){var c=za(b),d=c?b:arguments;for(c=c?0:1;c<d.length;c++){if(null==a)return;a=a[d[c]]}return a}
function eb(a){var b=fb,c;for(c in b)if(a.call(void 0,b[c],c,b))return c}
function gb(a){for(var b in a)return!1;return!0}
function hb(a,b){if(null!==a&&b in a)throw Error('The object already contains the key "'+b+'"');a[b]=!0}
function ib(a,b){for(var c in a)if(!(c in b)||a[c]!==b[c])return!1;for(c in b)if(!(c in a))return!1;return!0}
function jb(a){var b={},c;for(c in a)b[c]=a[c];return b}
function kb(a){var b=ya(a);if("object"==b||"array"==b){if(y(a.clone))return a.clone();b="array"==b?[]:{};for(var c in a)b[c]=kb(a[c]);return b}return a}
var lb="constructor hasOwnProperty isPrototypeOf propertyIsEnumerable toLocaleString toString valueOf".split(" ");function mb(a,b){for(var c,d,e=1;e<arguments.length;e++){d=arguments[e];for(c in d)a[c]=d[c];for(var f=0;f<lb.length;f++)c=lb[f],Object.prototype.hasOwnProperty.call(d,c)&&(a[c]=d[c])}}
;function nb(a){nb[" "](a);return a}
nb[" "]=wa;var ob=G("Opera"),pb=G("Trident")||G("MSIE"),qb=G("Edge"),rb=G("Gecko")&&!(-1!=$a.toLowerCase().indexOf("webkit")&&!G("Edge"))&&!(G("Trident")||G("MSIE"))&&!G("Edge"),sb=-1!=$a.toLowerCase().indexOf("webkit")&&!G("Edge");function tb(){var a=q.document;return a?a.documentMode:void 0}
var ub;a:{var vb="",wb=function(){var a=$a;if(rb)return/rv:([^\);]+)(\)|;)/.exec(a);if(qb)return/Edge\/([\d\.]+)/.exec(a);if(pb)return/\b(?:MSIE|rv)[: ]([^\);]+)(\)|;)/.exec(a);if(sb)return/WebKit\/(\S+)/.exec(a);if(ob)return/(?:Version)[ \/]?(\S+)/.exec(a)}();
wb&&(vb=wb?wb[1]:"");if(pb){var xb=tb();if(null!=xb&&xb>parseFloat(vb)){ub=String(xb);break a}}ub=vb}var yb=ub,zb;var Ab=q.document;zb=Ab&&pb?tb()||("CSS1Compat"==Ab.compatMode?parseInt(yb,10):5):void 0;var Bb=null,Cb=null;function Db(a){this.b=a||{cookie:""}}
k=Db.prototype;k.isEnabled=function(){return navigator.cookieEnabled};
k.set=function(a,b,c,d,e,f){if(/[;=\s]/.test(a))throw Error('Invalid cookie name "'+a+'"');if(/[;\r\n]/.test(b))throw Error('Invalid cookie value "'+b+'"');r(c)||(c=-1);e=e?";domain="+e:"";d=d?";path="+d:"";f=f?";secure":"";c=0>c?"":0==c?";expires="+(new Date(1970,1,1)).toUTCString():";expires="+(new Date(B()+1E3*c)).toUTCString();this.b.cookie=a+"="+b+e+d+c+f};
k.get=function(a,b){for(var c=a+"=",d=(this.b.cookie||"").split(";"),e=0,f;e<d.length;e++){f=Pa(d[e]);if(0==f.lastIndexOf(c,0))return f.substr(c.length);if(f==a)return""}return b};
k.remove=function(a,b,c){var d=r(this.get(a));this.set(a,"",0,b,c);return d};
k.isEmpty=function(){return!this.b.cookie};
k.clear=function(){for(var a=(this.b.cookie||"").split(";"),b=[],c=[],d,e,f=0;f<a.length;f++)e=Pa(a[f]),d=e.indexOf("="),-1==d?(b.push(""),c.push(e)):(b.push(e.substring(0,d)),c.push(e.substring(d+1)));for(a=b.length-1;0<=a;a--)this.remove(b[a])};
var Eb=new Db("undefined"==typeof document?null:document);function Fb(a){var b=w("window.location.href");null==a&&(a='Unknown Error of type "null/undefined"');if(t(a))return{message:a,name:"Unknown error",lineNumber:"Not available",fileName:b,stack:"Not available"};var c=!1;try{var d=a.lineNumber||a.line||"Not available"}catch(f){d="Not available",c=!0}try{var e=a.fileName||a.filename||a.sourceURL||q.$googDebugFname||b}catch(f){e="Not available",c=!0}return!c&&a.lineNumber&&a.fileName&&a.stack&&a.message&&a.name?a:(b=a.message,null==b&&(a.constructor&&a.constructor instanceof
Function?(a.constructor.name?b=a.constructor.name:(b=a.constructor,Gb[b]?b=Gb[b]:(b=String(b),Gb[b]||(c=/function\s+([^\(]+)/m.exec(b),Gb[b]=c?c[1]:"[Anonymous]"),b=Gb[b])),b='Unknown Error of type "'+b+'"'):b="Unknown Error of unknown type"),{message:b,name:a.name||"UnknownError",lineNumber:d,fileName:e,stack:a.stack||"Not available"})}
var Gb={};function Hb(a){var b=!1,c;return function(){b||(c=a(),b=!0);return c}}
;var Ib=!pb||9<=Number(zb);function Jb(){this.b="";this.f=Kb}
Jb.prototype.J=!0;Jb.prototype.I=function(){return this.b};
Jb.prototype.ba=!0;Jb.prototype.Y=function(){return 1};
function Lb(a){if(a instanceof Jb&&a.constructor===Jb&&a.f===Kb)return a.b;ya(a);return"type_error:TrustedResourceUrl"}
var Kb={};function H(){this.b="";this.f=Mb}
H.prototype.J=!0;H.prototype.I=function(){return this.b};
H.prototype.ba=!0;H.prototype.Y=function(){return 1};
function Nb(a){if(a instanceof H&&a.constructor===H&&a.f===Mb)return a.b;ya(a);return"type_error:SafeUrl"}
var Ob=/^(?:(?:https?|mailto|ftp):|[^:/?#]*(?:[/?#]|$))/i;function Pb(a){if(a instanceof H)return a;a="object"==typeof a&&a.J?a.I():String(a);Ob.test(a)||(a="about:invalid#zClosurez");return Qb(a)}
var Mb={};function Qb(a){var b=new H;b.b=a;return b}
Qb("about:blank");function Rb(){this.b="";this.g=Sb;this.f=null}
Rb.prototype.ba=!0;Rb.prototype.Y=function(){return this.f};
Rb.prototype.J=!0;Rb.prototype.I=function(){return this.b};
var Sb={};function Tb(a,b){var c=new Rb;c.b=a;c.f=b;return c}
Tb("<!DOCTYPE html>",0);Tb("",0);Tb("<br>",0);function Ub(a,b){var c=b instanceof H?b:Pb(b);a.href=Nb(c)}
function Vb(a,b){a.src=Lb(b);if(null===va)b:{var c=q.document;if((c=c.querySelector&&c.querySelector("script[nonce]"))&&(c=c.nonce||c.getAttribute("nonce"))&&ua.test(c)){va=c;break b}va=""}c=va;c&&a.setAttribute("nonce",c)}
;function Wb(a,b){this.x=r(a)?a:0;this.y=r(b)?b:0}
k=Wb.prototype;k.clone=function(){return new Wb(this.x,this.y)};
k.equals=function(a){return a instanceof Wb&&(this==a?!0:this&&a?this.x==a.x&&this.y==a.y:!1)};
k.ceil=function(){this.x=Math.ceil(this.x);this.y=Math.ceil(this.y);return this};
k.floor=function(){this.x=Math.floor(this.x);this.y=Math.floor(this.y);return this};
k.round=function(){this.x=Math.round(this.x);this.y=Math.round(this.y);return this};function Xb(a,b){this.width=a;this.height=b}
k=Xb.prototype;k.clone=function(){return new Xb(this.width,this.height)};
k.aspectRatio=function(){return this.width/this.height};
k.isEmpty=function(){return!(this.width*this.height)};
k.ceil=function(){this.width=Math.ceil(this.width);this.height=Math.ceil(this.height);return this};
k.floor=function(){this.width=Math.floor(this.width);this.height=Math.floor(this.height);return this};
k.round=function(){this.width=Math.round(this.width);this.height=Math.round(this.height);return this};function Yb(a){var b=document;return t(a)?b.getElementById(a):a}
function Zb(a,b){cb(b,function(b,d){b&&"object"==typeof b&&b.J&&(b=b.I());"style"==d?a.style.cssText=b:"class"==d?a.className=b:"for"==d?a.htmlFor=b:$b.hasOwnProperty(d)?a.setAttribute($b[d],b):0==d.lastIndexOf("aria-",0)||0==d.lastIndexOf("data-",0)?a.setAttribute(d,b):a[d]=b})}
var $b={cellpadding:"cellPadding",cellspacing:"cellSpacing",colspan:"colSpan",frameborder:"frameBorder",height:"height",maxlength:"maxLength",nonce:"nonce",role:"role",rowspan:"rowSpan",type:"type",usemap:"useMap",valign:"vAlign",width:"width"};
function ac(a,b,c){var d=arguments,e=document,f=String(d[0]),g=d[1];if(!Ib&&g&&(g.name||g.type)){f=["<",f];g.name&&f.push(' name="',Ya(g.name),'"');if(g.type){f.push(' type="',Ya(g.type),'"');var h={};mb(h,g);delete h.type;g=h}f.push(">");f=f.join("")}f=e.createElement(f);g&&(t(g)?f.className=g:x(g)?f.className=g.join(" "):Zb(f,g));2<d.length&&bc(e,f,d);return f}
function bc(a,b,c){function d(c){c&&b.appendChild(t(c)?a.createTextNode(c):c)}
for(var e=2;e<c.length;e++){var f=c[e];!za(f)||z(f)&&0<f.nodeType?d(f):F(cc(f)?Na(f):f,d)}}
function cc(a){if(a&&"number"==typeof a.length){if(z(a))return"function"==typeof a.item||"string"==typeof a.item;if(y(a))return"function"==typeof a.item}return!1}
function dc(a,b){for(var c=0;a;){if(b(a))return a;a=a.parentNode;c++}return null}
;function ec(a){fc();var b=new Jb;b.b=a;return b}
var fc=wa;function gc(){var a=hc;try{var b;if(b=!!a&&null!=a.location.href)a:{try{nb(a.foo);b=!0;break a}catch(c){}b=!1}return b}catch(c){return!1}}
function ic(a){var b=jc;if(b)for(var c in b)Object.prototype.hasOwnProperty.call(b,c)&&a.call(void 0,b[c],c,b)}
function kc(){var a=[];ic(function(b){a.push(b)});
return a}
var jc={Wa:"allow-forms",Xa:"allow-modals",Ya:"allow-orientation-lock",Za:"allow-pointer-lock",ab:"allow-popups",bb:"allow-popups-to-escape-sandbox",cb:"allow-presentation",eb:"allow-same-origin",fb:"allow-scripts",gb:"allow-top-navigation",hb:"allow-top-navigation-by-user-activation"},lc=Hb(function(){return kc()});
function mc(){var a=document.createElement("IFRAME").sandbox,b=a&&a.supports;if(!b)return{};var c={};F(lc(),function(d){b.call(a,d)&&(c[d]=!0)});
return c}
;function nc(a){"number"==typeof a&&(a=Math.round(a)+"px");return a}
;var oc=!!window.google_async_iframe_id,hc=oc&&window.parent||window;var pc=/^(?:([^:/?#.]+):)?(?:\/\/(?:([^/?#]*)@)?([^/#?]*?)(?::([0-9]+))?(?=[/#?]|$))?([^?#]+)?(?:\?([^#]*))?(?:#([\s\S]*))?$/;function I(a){return a?decodeURI(a):a}
function J(a,b){return b.match(pc)[a]||null}
function qc(a,b,c){if(x(b))for(var d=0;d<b.length;d++)qc(a,String(b[d]),c);else null!=b&&c.push(a+(""===b?"":"="+encodeURIComponent(String(b))))}
function rc(a){var b=[],c;for(c in a)qc(c,a[c],b);return b.join("&")}
function sc(a,b){var c=rc(b);if(c){var d=a.indexOf("#");0>d&&(d=a.length);var e=a.indexOf("?");if(0>e||e>d){e=d;var f=""}else f=a.substring(e+1,d);d=[a.substr(0,e),f,a.substr(d)];e=d[1];d[1]=c?e?e+"&"+c:c:e;c=d[0]+(d[1]?"?"+d[1]:"")+d[2]}else c=a;return c}
var tc=/#|$/;function uc(a,b){var c=a.search(tc);a:{var d=0;for(var e=b.length;0<=(d=a.indexOf(b,d))&&d<c;){var f=a.charCodeAt(d-1);if(38==f||63==f)if(f=a.charCodeAt(d+e),!f||61==f||38==f||35==f)break a;d+=e+1}d=-1}if(0>d)return null;e=a.indexOf("&",d);if(0>e||e>c)e=c;d+=b.length+1;return decodeURIComponent(a.substr(d,e-d).replace(/\+/g," "))}
;var vc=null;function wc(){var a=q.performance;return a&&a.now&&a.timing?Math.floor(a.now()+a.timing.navigationStart):B()}
function xc(){var a=void 0===a?q:a;return(a=a.performance)&&a.now?a.now():null}
;function yc(a,b,c){this.label=a;this.type=b;this.value=c;this.duration=0;this.uniqueId=this.label+"_"+this.type+"_"+Math.random();this.slotId=void 0}
;var K=q.performance,zc=!!(K&&K.mark&&K.measure&&K.clearMarks),Ac=Hb(function(){var a;if(a=zc){var b;if(null===vc){vc="";try{a="";try{a=q.top.location.hash}catch(c){a=q.location.hash}a&&(vc=(b=a.match(/\bdeid=([\d,]+)/))?b[1]:"")}catch(c){}}b=vc;a=!!b.indexOf&&0<=b.indexOf("1337")}return a});
function Bc(){var a=Cc;this.events=[];this.f=a||q;var b=null;a&&(a.google_js_reporting_queue=a.google_js_reporting_queue||[],this.events=a.google_js_reporting_queue,b=a.google_measure_js_timing);this.b=Ac()||(null!=b?b:1>Math.random())}
Bc.prototype.disable=function(){this.b=!1;this.events!=this.f.google_js_reporting_queue&&(Ac()&&F(this.events,Dc),this.events.length=0)};
function Dc(a){a&&K&&Ac()&&(K.clearMarks("goog_"+a.uniqueId+"_start"),K.clearMarks("goog_"+a.uniqueId+"_end"))}
Bc.prototype.start=function(a,b){if(!this.b)return null;var c=xc()||wc();c=new yc(a,b,c);var d="goog_"+c.uniqueId+"_start";K&&Ac()&&K.mark(d);return c};
Bc.prototype.end=function(a){if(this.b&&"number"==typeof a.value){var b=xc()||wc();a.duration=b-a.value;b="goog_"+a.uniqueId+"_end";K&&Ac()&&K.mark(b);this.b&&this.events.push(a)}};if(oc&&!gc()){var Ec="."+Ga.domain;try{for(;2<Ec.split(".").length&&!gc();)Ga.domain=Ec=Ec.substr(Ec.indexOf(".")+1),hc=window.parent}catch(a){}gc()||(hc=window)}var Cc=hc,Fc=new Bc;if("complete"==Cc.document.readyState)Cc.google_measure_js_timing||Fc.disable();else if(Fc.b){var Gc=function(){Cc.google_measure_js_timing||Fc.disable()},Hc=Cc;
Hc.addEventListener&&Hc.addEventListener("load",Gc,!1)};var Ic=(new Date).getTime();function Jc(a){if(!a)return"";a=a.split("#")[0].split("?")[0];a=a.toLowerCase();0==a.indexOf("//")&&(a=window.location.protocol+a);/^[\w\-]*:\/\//.test(a)||(a=window.location.href);var b=a.substring(a.indexOf("://")+3),c=b.indexOf("/");-1!=c&&(b=b.substring(0,c));a=a.substring(0,a.indexOf("://"));if("http"!==a&&"https"!==a&&"chrome-extension"!==a&&"file"!==a&&"android-app"!==a&&"chrome-search"!==a&&"app"!==a)throw Error("Invalid URI scheme in origin: "+a);c="";var d=b.indexOf(":");if(-1!=d){var e=
b.substring(d+1);b=b.substring(0,d);if("http"===a&&"80"!==e||"https"===a&&"443"!==e)c=":"+e}return a+"://"+b+c}
;function Kc(){function a(){e[0]=1732584193;e[1]=4023233417;e[2]=2562383102;e[3]=271733878;e[4]=3285377520;u=l=0}
function b(a){for(var b=g,c=0;64>c;c+=4)b[c/4]=a[c]<<24|a[c+1]<<16|a[c+2]<<8|a[c+3];for(c=16;80>c;c++)a=b[c-3]^b[c-8]^b[c-14]^b[c-16],b[c]=(a<<1|a>>>31)&4294967295;a=e[0];var d=e[1],f=e[2],h=e[3],m=e[4];for(c=0;80>c;c++){if(40>c)if(20>c){var l=h^d&(f^h);var u=1518500249}else l=d^f^h,u=1859775393;else 60>c?(l=d&f|h&(d|f),u=2400959708):(l=d^f^h,u=3395469782);l=((a<<5|a>>>27)&4294967295)+l+m+u+b[c]&4294967295;m=h;h=f;f=(d<<30|d>>>2)&4294967295;d=a;a=l}e[0]=e[0]+a&4294967295;e[1]=e[1]+d&4294967295;e[2]=
e[2]+f&4294967295;e[3]=e[3]+h&4294967295;e[4]=e[4]+m&4294967295}
function c(a,c){if("string"===typeof a){a=unescape(encodeURIComponent(a));for(var d=[],e=0,g=a.length;e<g;++e)d.push(a.charCodeAt(e));a=d}c||(c=a.length);d=0;if(0==l)for(;d+64<c;)b(a.slice(d,d+64)),d+=64,u+=64;for(;d<c;)if(f[l++]=a[d++],u++,64==l)for(l=0,b(f);d+64<c;)b(a.slice(d,d+64)),d+=64,u+=64}
function d(){var a=[],d=8*u;56>l?c(h,56-l):c(h,64-(l-56));for(var g=63;56<=g;g--)f[g]=d&255,d>>>=8;b(f);for(g=d=0;5>g;g++)for(var m=24;0<=m;m-=8)a[d++]=e[g]>>m&255;return a}
for(var e=[],f=[],g=[],h=[128],m=1;64>m;++m)h[m]=0;var l,u;a();return{reset:a,update:c,digest:d,ra:function(){for(var a=d(),b="",c=0;c<a.length;c++)b+="0123456789ABCDEF".charAt(Math.floor(a[c]/16))+"0123456789ABCDEF".charAt(a[c]%16);return b}}}
;function Lc(a,b,c){var d=[],e=[];if(1==(x(c)?2:1))return e=[b,a],F(d,function(a){e.push(a)}),Mc(e.join(" "));
var f=[],g=[];F(c,function(a){g.push(a.key);f.push(a.value)});
c=Math.floor((new Date).getTime()/1E3);e=0==f.length?[c,b,a]:[f.join(":"),c,b,a];F(d,function(a){e.push(a)});
a=Mc(e.join(" "));a=[c,a];0==g.length||a.push(g.join(""));return a.join("_")}
function Mc(a){var b=Kc();b.update(a);return b.ra().toLowerCase()}
;function Nc(a){var b=Jc(String(q.location.href)),c=q.__OVERRIDE_SID;null==c&&(c=(new Db(document)).get("SID"));if(c&&(b=(c=0==b.indexOf("https:")||0==b.indexOf("chrome-extension:"))?q.__SAPISID:q.__APISID,null==b&&(b=(new Db(document)).get(c?"SAPISID":"APISID")),b)){c=c?"SAPISIDHASH":"APISIDHASH";var d=String(q.location.href);return d&&b&&c?[c,Lc(Jc(d),b,a||null)].join(" "):null}return null}
;function Oc(a,b){this.g=a;this.h=b;this.f=0;this.b=null}
Oc.prototype.get=function(){if(0<this.f){this.f--;var a=this.b;this.b=a.next;a.next=null}else a=this.g();return a};
function Pc(a,b){a.h(b);100>a.f&&(a.f++,b.next=a.b,a.b=b)}
;function Qc(a){q.setTimeout(function(){throw a;},0)}
var Rc;
function Sc(){var a=q.MessageChannel;"undefined"===typeof a&&"undefined"!==typeof window&&window.postMessage&&window.addEventListener&&!G("Presto")&&(a=function(){var a=document.createElement("IFRAME");a.style.display="none";a.src="";document.documentElement.appendChild(a);var b=a.contentWindow;a=b.document;a.open();a.write("");a.close();var c="callImmediate"+Math.random(),d="file:"==b.location.protocol?"*":b.location.protocol+"//"+b.location.host;a=A(function(a){if(("*"==d||a.origin==d)&&a.data==
c)this.port1.onmessage()},this);
b.addEventListener("message",a,!1);this.port1={};this.port2={postMessage:function(){b.postMessage(c,d)}}});
if("undefined"!==typeof a&&!G("Trident")&&!G("MSIE")){var b=new a,c={},d=c;b.port1.onmessage=function(){if(r(c.next)){c=c.next;var a=c.fa;c.fa=null;a()}};
return function(a){d.next={fa:a};d=d.next;b.port2.postMessage(0)}}return"undefined"!==typeof document&&"onreadystatechange"in document.createElement("SCRIPT")?function(a){var b=document.createElement("SCRIPT");
b.onreadystatechange=function(){b.onreadystatechange=null;b.parentNode.removeChild(b);b=null;a();a=null};
document.documentElement.appendChild(b)}:function(a){q.setTimeout(a,0)}}
;function Tc(){this.f=this.b=null}
var Vc=new Oc(function(){return new Uc},function(a){a.reset()});
Tc.prototype.add=function(a,b){var c=Vc.get();c.set(a,b);this.f?this.f.next=c:this.b=c;this.f=c};
Tc.prototype.remove=function(){var a=null;this.b&&(a=this.b,this.b=this.b.next,this.b||(this.f=null),a.next=null);return a};
function Uc(){this.next=this.scope=this.b=null}
Uc.prototype.set=function(a,b){this.b=a;this.scope=b;this.next=null};
Uc.prototype.reset=function(){this.next=this.scope=this.b=null};function Wc(a,b){Xc||Yc();Zc||(Xc(),Zc=!0);$c.add(a,b)}
var Xc;function Yc(){if(q.Promise&&q.Promise.resolve){var a=q.Promise.resolve(void 0);Xc=function(){a.then(ad)}}else Xc=function(){var a=ad;
!y(q.setImmediate)||q.Window&&q.Window.prototype&&!G("Edge")&&q.Window.prototype.setImmediate==q.setImmediate?(Rc||(Rc=Sc()),Rc(a)):q.setImmediate(a)}}
var Zc=!1,$c=new Tc;function ad(){for(var a;a=$c.remove();){try{a.b.call(a.scope)}catch(b){Qc(b)}Pc(Vc,a)}Zc=!1}
;function bd(){this.f=-1}
;function cd(){this.f=64;this.b=[];this.j=[];this.u=[];this.h=[];this.h[0]=128;for(var a=1;a<this.f;++a)this.h[a]=0;this.i=this.g=0;this.reset()}
C(cd,bd);cd.prototype.reset=function(){this.b[0]=1732584193;this.b[1]=4023233417;this.b[2]=2562383102;this.b[3]=271733878;this.b[4]=3285377520;this.i=this.g=0};
function dd(a,b,c){c||(c=0);var d=a.u;if(t(b))for(var e=0;16>e;e++)d[e]=b.charCodeAt(c)<<24|b.charCodeAt(c+1)<<16|b.charCodeAt(c+2)<<8|b.charCodeAt(c+3),c+=4;else for(e=0;16>e;e++)d[e]=b[c]<<24|b[c+1]<<16|b[c+2]<<8|b[c+3],c+=4;for(e=16;80>e;e++){var f=d[e-3]^d[e-8]^d[e-14]^d[e-16];d[e]=(f<<1|f>>>31)&4294967295}b=a.b[0];c=a.b[1];var g=a.b[2],h=a.b[3],m=a.b[4];for(e=0;80>e;e++){if(40>e)if(20>e){f=h^c&(g^h);var l=1518500249}else f=c^g^h,l=1859775393;else 60>e?(f=c&g|h&(c|g),l=2400959708):(f=c^g^h,l=
3395469782);f=(b<<5|b>>>27)+f+m+l+d[e]&4294967295;m=h;h=g;g=(c<<30|c>>>2)&4294967295;c=b;b=f}a.b[0]=a.b[0]+b&4294967295;a.b[1]=a.b[1]+c&4294967295;a.b[2]=a.b[2]+g&4294967295;a.b[3]=a.b[3]+h&4294967295;a.b[4]=a.b[4]+m&4294967295}
cd.prototype.update=function(a,b){if(null!=a){r(b)||(b=a.length);for(var c=b-this.f,d=0,e=this.j,f=this.g;d<b;){if(0==f)for(;d<=c;)dd(this,a,d),d+=this.f;if(t(a))for(;d<b;){if(e[f]=a.charCodeAt(d),++f,++d,f==this.f){dd(this,e);f=0;break}}else for(;d<b;)if(e[f]=a[d],++f,++d,f==this.f){dd(this,e);f=0;break}}this.g=f;this.i+=b}};
cd.prototype.digest=function(){var a=[],b=8*this.i;56>this.g?this.update(this.h,56-this.g):this.update(this.h,this.f-(this.g-56));for(var c=this.f-1;56<=c;c--)this.j[c]=b&255,b/=256;dd(this,this.j);for(c=b=0;5>c;c++)for(var d=24;0<=d;d-=8)a[b]=this.b[c]>>d&255,++b;return a};function L(){this.f=this.f;this.u=this.u}
L.prototype.f=!1;L.prototype.dispose=function(){this.f||(this.f=!0,this.l())};
function ed(a,b){a.f?r(void 0)?b.call(void 0):b():(a.u||(a.u=[]),a.u.push(r(void 0)?A(b,void 0):b))}
L.prototype.l=function(){if(this.u)for(;this.u.length;)this.u.shift()()};
function fd(a){a&&"function"==typeof a.dispose&&a.dispose()}
function gd(a){for(var b=0,c=arguments.length;b<c;++b){var d=arguments[b];za(d)?gd.apply(null,d):fd(d)}}
;function hd(a){if(a.classList)return a.classList;a=a.className;return t(a)&&a.match(/\S+/g)||[]}
function id(a,b){if(a.classList)var c=a.classList.contains(b);else c=hd(a),c=0<=Ha(c,b);return c}
function jd(){var a=document.body;a.classList?a.classList.remove("inverted-hdpi"):id(a,"inverted-hdpi")&&(a.className=Ia(hd(a),function(a){return"inverted-hdpi"!=a}).join(" "))}
;var kd="StopIteration"in q?q.StopIteration:{message:"StopIteration",stack:""};function ld(){}
ld.prototype.next=function(){throw kd;};
ld.prototype.F=function(){return this};
function md(a){if(a instanceof ld)return a;if("function"==typeof a.F)return a.F(!1);if(za(a)){var b=0,c=new ld;c.next=function(){for(;;){if(b>=a.length)throw kd;if(b in a)return a[b++];b++}};
return c}throw Error("Not implemented");}
function nd(a,b){if(za(a))try{F(a,b,void 0)}catch(c){if(c!==kd)throw c;}else{a=md(a);try{for(;;)b.call(void 0,a.next(),void 0,a)}catch(c){if(c!==kd)throw c;}}}
function od(a){if(za(a))return Na(a);a=md(a);var b=[];nd(a,function(a){b.push(a)});
return b}
;function pd(a,b){this.g={};this.b=[];this.h=this.f=0;var c=arguments.length;if(1<c){if(c%2)throw Error("Uneven number of arguments");for(var d=0;d<c;d+=2)this.set(arguments[d],arguments[d+1])}else if(a)if(a instanceof pd)for(c=qd(a),d=0;d<c.length;d++)this.set(c[d],a.get(c[d]));else for(d in a)this.set(d,a[d])}
function qd(a){rd(a);return a.b.concat()}
k=pd.prototype;k.equals=function(a,b){if(this===a)return!0;if(this.f!=a.f)return!1;var c=b||sd;rd(this);for(var d,e=0;d=this.b[e];e++)if(!c(this.get(d),a.get(d)))return!1;return!0};
function sd(a,b){return a===b}
k.isEmpty=function(){return 0==this.f};
k.clear=function(){this.g={};this.h=this.f=this.b.length=0};
k.remove=function(a){return Object.prototype.hasOwnProperty.call(this.g,a)?(delete this.g[a],this.f--,this.h++,this.b.length>2*this.f&&rd(this),!0):!1};
function rd(a){if(a.f!=a.b.length){for(var b=0,c=0;b<a.b.length;){var d=a.b[b];Object.prototype.hasOwnProperty.call(a.g,d)&&(a.b[c++]=d);b++}a.b.length=c}if(a.f!=a.b.length){var e={};for(c=b=0;b<a.b.length;)d=a.b[b],Object.prototype.hasOwnProperty.call(e,d)||(a.b[c++]=d,e[d]=1),b++;a.b.length=c}}
k.get=function(a,b){return Object.prototype.hasOwnProperty.call(this.g,a)?this.g[a]:b};
k.set=function(a,b){Object.prototype.hasOwnProperty.call(this.g,a)||(this.f++,this.b.push(a),this.h++);this.g[a]=b};
k.forEach=function(a,b){for(var c=qd(this),d=0;d<c.length;d++){var e=c[d],f=this.get(e);a.call(b,f,e,this)}};
k.clone=function(){return new pd(this)};
k.F=function(a){rd(this);var b=0,c=this.h,d=this,e=new ld;e.next=function(){if(c!=d.h)throw Error("The map has changed since the iterator was created");if(b>=d.b.length)throw kd;var e=d.b[b++];return a?e:d.g[e]};
return e};function td(a){var b=[];ud(new vd,a,b);return b.join("")}
function vd(){}
function ud(a,b,c){if(null==b)c.push("null");else{if("object"==typeof b){if(x(b)){var d=b;b=d.length;c.push("[");for(var e="",f=0;f<b;f++)c.push(e),ud(a,d[f],c),e=",";c.push("]");return}if(b instanceof String||b instanceof Number||b instanceof Boolean)b=b.valueOf();else{c.push("{");e="";for(d in b)Object.prototype.hasOwnProperty.call(b,d)&&(f=b[d],"function"!=typeof f&&(c.push(e),wd(d,c),c.push(":"),ud(a,f,c),e=","));c.push("}");return}}switch(typeof b){case "string":wd(b,c);break;case "number":c.push(isFinite(b)&&
!isNaN(b)?String(b):"null");break;case "boolean":c.push(String(b));break;case "function":c.push("null");break;default:throw Error("Unknown type: "+typeof b);}}}
var xd={'"':'\\"',"\\":"\\\\","/":"\\/","\b":"\\b","\f":"\\f","\n":"\\n","\r":"\\r","\t":"\\t","\x0B":"\\u000b"},yd=/\uffff/.test("\uffff")?/[\\"\x00-\x1f\x7f-\uffff]/g:/[\\"\x00-\x1f\x7f-\xff]/g;function wd(a,b){b.push('"',a.replace(yd,function(a){var b=xd[a];b||(b="\\u"+(a.charCodeAt(0)|65536).toString(16).substr(1),xd[a]=b);return b}),'"')}
;function zd(a){if(!a)return!1;try{return!!a.$goog_Thenable}catch(b){return!1}}
;function M(a){this.b=0;this.u=void 0;this.h=this.f=this.g=null;this.i=this.j=!1;if(a!=wa)try{var b=this;a.call(void 0,function(a){Ad(b,2,a)},function(a){Ad(b,3,a)})}catch(c){Ad(this,3,c)}}
function Bd(){this.next=this.context=this.onRejected=this.f=this.b=null;this.g=!1}
Bd.prototype.reset=function(){this.context=this.onRejected=this.f=this.b=null;this.g=!1};
var Cd=new Oc(function(){return new Bd},function(a){a.reset()});
function Ed(a,b,c){var d=Cd.get();d.f=a;d.onRejected=b;d.context=c;return d}
function Fd(a){return new M(function(b,c){c(a)})}
M.prototype.then=function(a,b,c){return Gd(this,y(a)?a:null,y(b)?b:null,c)};
M.prototype.$goog_Thenable=!0;function Hd(a,b){return Gd(a,null,b,void 0)}
M.prototype.cancel=function(a){0==this.b&&Wc(function(){var b=new Id(a);Jd(this,b)},this)};
function Jd(a,b){if(0==a.b)if(a.g){var c=a.g;if(c.f){for(var d=0,e=null,f=null,g=c.f;g&&(g.g||(d++,g.b==a&&(e=g),!(e&&1<d)));g=g.next)e||(f=g);e&&(0==c.b&&1==d?Jd(c,b):(f?(d=f,d.next==c.h&&(c.h=d),d.next=d.next.next):Kd(c),Ld(c,e,3,b)))}a.g=null}else Ad(a,3,b)}
function Md(a,b){a.f||2!=a.b&&3!=a.b||Nd(a);a.h?a.h.next=b:a.f=b;a.h=b}
function Gd(a,b,c,d){var e=Ed(null,null,null);e.b=new M(function(a,g){e.f=b?function(c){try{var e=b.call(d,c);a(e)}catch(l){g(l)}}:a;
e.onRejected=c?function(b){try{var e=c.call(d,b);!r(e)&&b instanceof Id?g(b):a(e)}catch(l){g(l)}}:g});
e.b.g=a;Md(a,e);return e.b}
M.prototype.w=function(a){this.b=0;Ad(this,2,a)};
M.prototype.B=function(a){this.b=0;Ad(this,3,a)};
function Ad(a,b,c){if(0==a.b){a===c&&(b=3,c=new TypeError("Promise cannot resolve to itself"));a.b=1;a:{var d=c,e=a.w,f=a.B;if(d instanceof M){Md(d,Ed(e||wa,f||null,a));var g=!0}else if(zd(d))d.then(e,f,a),g=!0;else{if(z(d))try{var h=d.then;if(y(h)){Od(d,h,e,f,a);g=!0;break a}}catch(m){f.call(a,m);g=!0;break a}g=!1}}g||(a.u=c,a.b=b,a.g=null,Nd(a),3!=b||c instanceof Id||Pd(a,c))}}
function Od(a,b,c,d,e){function f(a){h||(h=!0,d.call(e,a))}
function g(a){h||(h=!0,c.call(e,a))}
var h=!1;try{b.call(a,g,f)}catch(m){f(m)}}
function Nd(a){a.j||(a.j=!0,Wc(a.m,a))}
function Kd(a){var b=null;a.f&&(b=a.f,a.f=b.next,b.next=null);a.f||(a.h=null);return b}
M.prototype.m=function(){for(var a;a=Kd(this);)Ld(this,a,this.b,this.u);this.j=!1};
function Ld(a,b,c,d){if(3==c&&b.onRejected&&!b.g)for(;a&&a.i;a=a.g)a.i=!1;if(b.b)b.b.g=null,Qd(b,c,d);else try{b.g?b.f.call(b.context):Qd(b,c,d)}catch(e){Rd.call(null,e)}Pc(Cd,b)}
function Qd(a,b,c){2==b?a.f.call(a.context,c):a.onRejected&&a.onRejected.call(a.context,c)}
function Pd(a,b){a.i=!0;Wc(function(){a.i&&Rd.call(null,b)})}
var Rd=Qc;function Id(a){E.call(this,a)}
C(Id,E);Id.prototype.name="cancel";function N(a){L.call(this);this.j=1;this.h=[];this.i=0;this.b=[];this.g={};this.m=!!a}
C(N,L);k=N.prototype;k.subscribe=function(a,b,c){var d=this.g[a];d||(d=this.g[a]=[]);var e=this.j;this.b[e]=a;this.b[e+1]=b;this.b[e+2]=c;this.j=e+3;d.push(e);return e};
function Sd(a,b,c,d){if(b=a.g[b]){var e=a.b;(b=La(b,function(a){return e[a+1]==c&&e[a+2]==d}))&&a.L(b)}}
k.L=function(a){var b=this.b[a];if(b){var c=this.g[b];0!=this.i?(this.h.push(a),this.b[a+1]=wa):(c&&Ma(c,a),delete this.b[a],delete this.b[a+1],delete this.b[a+2])}return!!b};
k.K=function(a,b){var c=this.g[a];if(c){for(var d=Array(arguments.length-1),e=1,f=arguments.length;e<f;e++)d[e-1]=arguments[e];if(this.m)for(e=0;e<c.length;e++){var g=c[e];Td(this.b[g+1],this.b[g+2],d)}else{this.i++;try{for(e=0,f=c.length;e<f;e++)g=c[e],this.b[g+1].apply(this.b[g+2],d)}finally{if(this.i--,0<this.h.length&&0==this.i)for(;c=this.h.pop();)this.L(c)}}return 0!=e}return!1};
function Td(a,b,c){Wc(function(){a.apply(b,c)})}
k.clear=function(a){if(a){var b=this.g[a];b&&(F(b,this.L,this),delete this.g[a])}else this.b.length=0,this.g={}};
k.l=function(){N.A.l.call(this);this.clear();this.h.length=0};function Ud(a){this.b=a}
Ud.prototype.set=function(a,b){r(b)?this.b.set(a,td(b)):this.b.remove(a)};
Ud.prototype.get=function(a){try{var b=this.b.get(a)}catch(c){return}if(null!==b)try{return JSON.parse(b)}catch(c){throw"Storage: Invalid value was encountered";}};
Ud.prototype.remove=function(a){this.b.remove(a)};function Vd(a){this.b=a}
C(Vd,Ud);function Wd(a){this.data=a}
function Xd(a){return!r(a)||a instanceof Wd?a:new Wd(a)}
Vd.prototype.set=function(a,b){Vd.A.set.call(this,a,Xd(b))};
Vd.prototype.f=function(a){a=Vd.A.get.call(this,a);if(!r(a)||a instanceof Object)return a;throw"Storage: Invalid value was encountered";};
Vd.prototype.get=function(a){if(a=this.f(a)){if(a=a.data,!r(a))throw"Storage: Invalid value was encountered";}else a=void 0;return a};function Yd(a){this.b=a}
C(Yd,Vd);Yd.prototype.set=function(a,b,c){if(b=Xd(b)){if(c){if(c<B()){Yd.prototype.remove.call(this,a);return}b.expiration=c}b.creation=B()}Yd.A.set.call(this,a,b)};
Yd.prototype.f=function(a){var b=Yd.A.f.call(this,a);if(b){var c=b.creation,d=b.expiration;if(d&&d<B()||c&&c>B())Yd.prototype.remove.call(this,a);else return b}};function Zd(){}
;function $d(){}
C($d,Zd);$d.prototype.clear=function(){var a=od(this.F(!0)),b=this;F(a,function(a){b.remove(a)})};function ae(a){this.b=a}
C(ae,$d);k=ae.prototype;k.isAvailable=function(){if(!this.b)return!1;try{return this.b.setItem("__sak","1"),this.b.removeItem("__sak"),!0}catch(a){return!1}};
k.set=function(a,b){try{this.b.setItem(a,b)}catch(c){if(0==this.b.length)throw"Storage mechanism: Storage disabled";throw"Storage mechanism: Quota exceeded";}};
k.get=function(a){a=this.b.getItem(a);if(!t(a)&&null!==a)throw"Storage mechanism: Invalid value was encountered";return a};
k.remove=function(a){this.b.removeItem(a)};
k.F=function(a){var b=0,c=this.b,d=new ld;d.next=function(){if(b>=c.length)throw kd;var d=c.key(b++);if(a)return d;d=c.getItem(d);if(!t(d))throw"Storage mechanism: Invalid value was encountered";return d};
return d};
k.clear=function(){this.b.clear()};
k.key=function(a){return this.b.key(a)};function be(){var a=null;try{a=window.localStorage||null}catch(b){}this.b=a}
C(be,ae);function ce(a,b){this.f=a;this.b=null;if(pb&&!(9<=Number(zb))){de||(de=new pd);this.b=de.get(a);this.b||(b?this.b=document.getElementById(b):(this.b=document.createElement("userdata"),this.b.addBehavior("#default#userData"),document.body.appendChild(this.b)),de.set(a,this.b));try{this.b.load(this.f)}catch(c){this.b=null}}}
C(ce,$d);var ee={".":".2E","!":".21","~":".7E","*":".2A","'":".27","(":".28",")":".29","%":"."},de=null;function fe(a){return"_"+encodeURIComponent(a).replace(/[.!~*'()%]/g,function(a){return ee[a]})}
k=ce.prototype;k.isAvailable=function(){return!!this.b};
k.set=function(a,b){this.b.setAttribute(fe(a),b);ge(this)};
k.get=function(a){a=this.b.getAttribute(fe(a));if(!t(a)&&null!==a)throw"Storage mechanism: Invalid value was encountered";return a};
k.remove=function(a){this.b.removeAttribute(fe(a));ge(this)};
k.F=function(a){var b=0,c=this.b.XMLDocument.documentElement.attributes,d=new ld;d.next=function(){if(b>=c.length)throw kd;var d=c[b++];if(a)return decodeURIComponent(d.nodeName.replace(/\./g,"%")).substr(1);d=d.nodeValue;if(!t(d))throw"Storage mechanism: Invalid value was encountered";return d};
return d};
k.clear=function(){for(var a=this.b.XMLDocument.documentElement,b=a.attributes.length;0<b;b--)a.removeAttribute(a.attributes[b-1].nodeName);ge(this)};
function ge(a){try{a.b.save(a.f)}catch(b){throw"Storage mechanism: Quota exceeded";}}
;function he(a,b){this.f=a;this.b=b+"::"}
C(he,$d);he.prototype.set=function(a,b){this.f.set(this.b+a,b)};
he.prototype.get=function(a){return this.f.get(this.b+a)};
he.prototype.remove=function(a){this.f.remove(this.b+a)};
he.prototype.F=function(a){var b=this.f.F(!0),c=this,d=new ld;d.next=function(){for(var d=b.next();d.substr(0,c.b.length)!=c.b;)d=b.next();return a?d.substr(c.b.length):c.f.get(d)};
return d};function ie(){this.f=[];this.b=-1}
ie.prototype.set=function(a,b){b=void 0===b?!0:b;0<=a&&52>a&&0===a%1&&this.f[a]!=b&&(this.f[a]=b,this.b=-1)};
ie.prototype.get=function(a){return!!this.f[a]};
function je(a){-1==a.b&&(a.b=Ka(a.f,function(a,c,d){return c?a+Math.pow(2,d):a},0));
return a.b}
;function ke(a,b){if(1<b.length)a[b[0]]=b[1];else{var c=b[0],d;for(d in c)a[d]=c[d]}}
var O=window.performance&&window.performance.timing&&window.performance.now?function(){return window.performance.timing.navigationStart+window.performance.now()}:function(){return(new Date).getTime()};var le=window.yt&&window.yt.config_||window.ytcfg&&window.ytcfg.data_||{};v("yt.config_",le,void 0);function P(a){ke(le,arguments)}
function Q(a,b){return a in le?le[a]:b}
function me(a){return Q(a,void 0)}
function ne(){return Q("PLAYER_CONFIG",{})}
;function oe(){var a=pe;w("yt.ads.biscotti.getId_")||v("yt.ads.biscotti.getId_",a,void 0)}
function qe(a){v("yt.ads.biscotti.lastId_",a,void 0)}
;function re(a){return a&&window.yterr?function(){try{return a.apply(this,arguments)}catch(b){R(b)}}:a}
function R(a,b,c,d,e){var f=w("yt.logging.errors.log");f?f(a,b,c,d,e):(f=Q("ERRORS",[]),f.push([a,b,c,d,e]),P("ERRORS",f))}
function se(a){R(a,"WARNING",void 0,void 0,void 0)}
;function te(a,b){var c=S(a);return void 0===c&&void 0!==b?b:Number(c||0)}
function S(a){return Q("EXPERIMENT_FLAGS",{})[a]}
;function ue(){var a=ve(),b=[];cb(a,function(a,d){var c=encodeURIComponent(String(d)),f;x(a)?f=a:f=[a];F(f,function(a){""==a?b.push(c):b.push(c+"="+encodeURIComponent(String(a)))})});
return b.join("&")}
function we(a){"?"==a.charAt(0)&&(a=a.substr(1));a=a.split("&");for(var b={},c=0,d=a.length;c<d;c++){var e=a[c].split("=");if(1==e.length&&e[0]||2==e.length)try{var f=decodeURIComponent((e[0]||"").replace(/\+/g," ")),g=decodeURIComponent((e[1]||"").replace(/\+/g," "));f in b?x(b[f])?Oa(b[f],g):b[f]=[b[f],g]:b[f]=g}catch(m){if(S("catch_invalid_url_components")){var h=Error("Error decoding URL component.");h.params="key: "+e[0]+", value: "+e[1];R(h)}else throw m;}}return b}
function xe(a,b,c){var d=a.split("#",2);a=d[0];d=1<d.length?"#"+d[1]:"";var e=a.split("?",2);a=e[0];e=we(e[1]||"");for(var f in b)!c&&null!==e&&f in e||(e[f]=b[f]);return sc(a,e)+d}
;function ve(a){var b=ye;a=void 0===a?w("yt.ads.biscotti.lastId_")||"":a;b=Object.assign(ze(b),Ae(b));b.ca_type="image";a&&(b.bid=a);return b}
function ze(a){var b={};b.dt=Ic;b.flash="0";a:{try{var c=a.b.top.location.href}catch(f){a=2;break a}a=c?c===a.f.location.href?0:1:2}b=(b.frm=a,b);b.u_tz=-(new Date).getTimezoneOffset();var d=void 0===d?D:d;try{var e=d.history.length}catch(f){e=0}b.u_his=e;b.u_java=!!D.navigator&&"unknown"!==typeof D.navigator.javaEnabled&&!!D.navigator.javaEnabled&&D.navigator.javaEnabled();D.screen&&(b.u_h=D.screen.height,b.u_w=D.screen.width,b.u_ah=D.screen.availHeight,b.u_aw=D.screen.availWidth,b.u_cd=D.screen.colorDepth);
D.navigator&&D.navigator.plugins&&(b.u_nplug=D.navigator.plugins.length);D.navigator&&D.navigator.mimeTypes&&(b.u_nmime=D.navigator.mimeTypes.length);return b}
function Ae(a){var b=a.b;try{var c=b.screenX;var d=b.screenY}catch(ea){}try{var e=b.outerWidth;var f=b.outerHeight}catch(ea){}try{var g=b.innerWidth;var h=b.innerHeight}catch(ea){}b=[b.screenLeft,b.screenTop,c,d,b.screen?b.screen.availWidth:void 0,b.screen?b.screen.availTop:void 0,e,f,g,h];c=a.b.top;try{var m=(c||window).document,l="CSS1Compat"==m.compatMode?m.documentElement:m.body;var u=(new Xb(l.clientWidth,l.clientHeight)).round()}catch(ea){u=new Xb(-12245933,-12245933)}m=u;u={};l=new ie;q.SVGElement&&
q.document.createElementNS&&l.set(0);c=mc();c["allow-top-navigation-by-user-activation"]&&l.set(1);c["allow-popups-to-escape-sandbox"]&&l.set(2);q.crypto&&q.crypto.subtle&&l.set(3);l=je(l);u.bc=l;u.bih=m.height;u.biw=m.width;u.brdim=b.join();a=a.f;return u.vis={visible:1,hidden:2,prerender:3,preview:4,unloaded:5}[a.visibilityState||a.webkitVisibilityState||a.mozVisibilityState||""]||0,u.wgl=!!D.WebGLRenderingContext,u}
var ye=new function(){var a=window.document;this.b=window;this.f=a};
v("yt.ads.signals.getAdSignalsString",function(){return ue()},void 0);B();var Be=r(XMLHttpRequest)?function(){return new XMLHttpRequest}:r(ActiveXObject)?function(){return new ActiveXObject("Microsoft.XMLHTTP")}:null;
function Ce(){if(!Be)return null;var a=Be();return"open"in a?a:null}
function De(a){switch(a&&"status"in a?a.status:-1){case 200:case 201:case 202:case 203:case 204:case 205:case 206:case 304:return!0;default:return!1}}
;function T(a,b){y(a)&&(a=re(a));return window.setTimeout(a,b)}
function U(a){window.clearTimeout(a)}
;var Ee={Authorization:"AUTHORIZATION","X-Goog-Visitor-Id":"SANDBOXED_VISITOR_ID","X-YouTube-Client-Name":"INNERTUBE_CONTEXT_CLIENT_NAME","X-YouTube-Client-Version":"INNERTUBE_CONTEXT_CLIENT_VERSION","X-Youtube-Identity-Token":"ID_TOKEN","X-YouTube-Page-CL":"PAGE_CL","X-YouTube-Page-Label":"PAGE_BUILD_LABEL","X-YouTube-Variants-Checksum":"VARIANTS_CHECKSUM"},Fe="app debugcss debugjs expflag force_ad_params force_viral_ad_response_params forced_experiments internalcountrycode internalipoverride absolute_experiments conditional_experiments sbb sr_bns_address".split(" "),
Ge=!1;
function He(a,b){b=void 0===b?{}:b;if(!c)var c=window.location.href;var d=J(1,a),e=I(J(3,a));d&&e?(d=c,c=a.match(pc),d=d.match(pc),c=c[3]==d[3]&&c[1]==d[1]&&c[4]==d[4]):c=e?I(J(3,c))==e&&(Number(J(4,c))||null)==(Number(J(4,a))||null):!0;d=!!S("web_ajax_ignore_global_headers_if_set");for(var f in Ee)e=Q(Ee[f]),!e||!c&&!Ie(a,f)||d&&void 0!==b[f]||(b[f]=e);if(c||Ie(a,"X-YouTube-Utc-Offset"))b["X-YouTube-Utc-Offset"]=-(new Date).getTimezoneOffset();S("pass_biscotti_id_in_header_ajax")&&(c||Ie(a,"X-YouTube-Ad-Signals"))&&
(b["X-YouTube-Ad-Signals"]=ue());return b}
function Je(a){var b=window.location.search,c=I(J(3,a)),d=I(J(5,a));d=(c=c&&c.endsWith("youtube.com"))&&d&&d.startsWith("/api/");if(!c||d)return a;var e=we(b),f={};F(Fe,function(a){e[a]&&(f[a]=e[a])});
return xe(a,f||{},!1)}
function Ie(a,b){var c=Q("CORS_HEADER_WHITELIST")||{},d=I(J(3,a));return d?(c=c[d])?0<=Ha(c,b):!1:!0}
function Ke(a,b){if(window.fetch&&"XML"!=b.format){var c={method:b.method||"GET",credentials:"same-origin"};b.headers&&(c.headers=b.headers);a=Le(a,b);var d=Me(a,b);d&&(c.body=d);b.withCredentials&&(c.credentials="include");var e=!1,f;fetch(a,c).then(function(a){if(!e){e=!0;f&&U(f);var c=a.ok,d=function(d){d=d||{};var e=b.context||q;c?b.onSuccess&&b.onSuccess.call(e,d,a):b.onError&&b.onError.call(e,d,a);b.da&&b.da.call(e,d,a)};
"JSON"==(b.format||"JSON")&&(c||400<=a.status&&500>a.status)?a.json().then(d,function(){d(null)}):d(null)}});
b.ha&&0<b.timeout&&(f=T(function(){e||(e=!0,U(f),b.ha.call(b.context||q))},b.timeout))}else Ne(a,b)}
function Ne(a,b){var c=b.format||"JSON";a=Le(a,b);var d=Me(a,b),e=!1,f,g=Oe(a,function(a){if(!e){e=!0;f&&U(f);var d=De(a),g=null,h=400<=a.status&&500>a.status,ea=500<=a.status&&600>a.status;if(d||h||ea)g=Pe(c,a,b.mb);if(d)a:if(a&&204==a.status)d=!0;else{switch(c){case "XML":d=0==parseInt(g&&g.return_code,10);break a;case "RAW":d=!0;break a}d=!!g}g=g||{};h=b.context||q;d?b.onSuccess&&b.onSuccess.call(h,a,g):b.onError&&b.onError.call(h,a,g);b.da&&b.da.call(h,a,g)}},b.method,d,b.headers,b.responseType,
b.withCredentials);
b.M&&0<b.timeout&&(f=T(function(){e||(e=!0,g.abort(),U(f),b.M.call(b.context||q,g))},b.timeout));
return g}
function Le(a,b){b.wa&&(a=document.location.protocol+"//"+document.location.hostname+(document.location.port?":"+document.location.port:"")+a);var c=Q("XSRF_FIELD_NAME",void 0),d=b.Va;d&&(d[c]&&delete d[c],a=xe(a,d||{},!0));return a}
function Me(a,b){var c=Q("XSRF_FIELD_NAME",void 0),d=Q("XSRF_TOKEN",void 0),e=b.postBody||"",f=b.C,g=me("XSRF_FIELD_NAME"),h;b.headers&&(h=b.headers["Content-Type"]);b.nb||I(J(3,a))&&!b.withCredentials&&I(J(3,a))!=document.location.hostname||"POST"!=b.method||h&&"application/x-www-form-urlencoded"!=h||b.C&&b.C[g]||(f||(f={}),f[c]=d);f&&t(e)&&(e=we(e),mb(e,f),e=b.ia&&"JSON"==b.ia?JSON.stringify(e):rc(e));f=e||f&&!gb(f);!Ge&&f&&"POST"!=b.method&&(Ge=!0,R(Error("AJAX request with postData should use POST")));
return e}
function Pe(a,b,c){var d=null;switch(a){case "JSON":a=b.responseText;b=b.getResponseHeader("Content-Type")||"";a&&0<=b.indexOf("json")&&(d=JSON.parse(a));break;case "XML":if(b=(b=b.responseXML)?Qe(b):null)d={},F(b.getElementsByTagName("*"),function(a){d[a.tagName]=Re(a)})}c&&Se(d);
return d}
function Se(a){if(z(a))for(var b in a){var c;(c="html_content"==b)||(c=b.length-5,c=0<=c&&b.indexOf("_html",c)==c);if(c){c=b;var d=Tb(a[b],null);a[c]=d}else Se(a[b])}}
function Qe(a){return a?(a=("responseXML"in a?a.responseXML:a).getElementsByTagName("root"))&&0<a.length?a[0]:null:null}
function Re(a){var b="";F(a.childNodes,function(a){b+=a.nodeValue});
return b}
function Te(a,b){b.method="POST";b.C||(b.C={});Ne(a,b)}
function Oe(a,b,c,d,e,f,g){function h(){4==(m&&"readyState"in m?m.readyState:0)&&b&&re(b)(m)}
c=void 0===c?"GET":c;d=void 0===d?"":d;var m=Ce();if(!m)return null;"onloadend"in m?m.addEventListener("loadend",h,!1):m.onreadystatechange=h;S("debug_forward_web_query_parameters")&&(a=Je(a));m.open(c,a,!0);f&&(m.responseType=f);g&&(m.withCredentials=!0);c="POST"==c&&(void 0===window.FormData||!(d instanceof FormData));if(e=He(a,e))for(var l in e)m.setRequestHeader(l,e[l]),"content-type"==l.toLowerCase()&&(c=!1);c&&m.setRequestHeader("Content-Type","application/x-www-form-urlencoded");m.send(d);
return m}
;var Ue={},Ve=0;
function We(a,b,c,d,e){e=void 0===e?"":e;a&&(c&&(c=$a,c=!(c&&0<=c.toLowerCase().indexOf("cobalt"))),c?a&&(a instanceof H||(a="object"==typeof a&&a.J?a.I():String(a),Ob.test(a)||(a="about:invalid#zClosurez"),a=Qb(a)),b=Nb(a),"about:invalid#zClosurez"===b?a="":(b instanceof Rb?a=b:(d="object"==typeof b,a=null,d&&b.ba&&(a=b.Y()),b=Qa(d&&b.J?b.I():String(b)),a=Tb(b,a)),a instanceof Rb&&a.constructor===Rb&&a.g===Sb?a=a.b:(ya(a),a="type_error:SafeHtml"),a=encodeURIComponent(String(td(a)))),/^[\s\xa0]*$/.test(a)||
(a=ac("IFRAME",{src:'javascript:"<body><img src=\\""+'+a+'+"\\"></body>"',style:"display:none"}),(9==a.nodeType?a:a.ownerDocument||a.document).body.appendChild(a))):e?Oe(a,b,"POST",e,d):Q("USE_NET_AJAX_FOR_PING_TRANSPORT",!1)||d?Oe(a,b,"GET","",d):((d=le.EXPERIMENT_FLAGS)&&d.web_use_beacon_api_for_ad_click_server_pings&&-1!=I(J(5,a)).indexOf("/aclk")&&"1"===uc(a,"ae")&&"1"===uc(a,"act")?Xe(a)?(b&&b(),d=!0):d=!1:d=!1,d||Ye(a,b)))}
function Xe(a,b){try{if(window.navigator&&window.navigator.sendBeacon&&window.navigator.sendBeacon(a,void 0===b?"":b))return!0}catch(c){}return!1}
function Ye(a,b){var c=new Image,d=""+Ve++;Ue[d]=c;c.onload=c.onerror=function(){b&&Ue[d]&&b();delete Ue[d]};
c.src=a}
;var Ze={},$e=0;
function af(a,b,c,d,e,f){f=f||{};f.name=c||Q("INNERTUBE_CONTEXT_CLIENT_NAME",1);f.version=d||Q("INNERTUBE_CONTEXT_CLIENT_VERSION",void 0);b=void 0===b?"ERROR":b;e=void 0===e?!1:e;b=void 0===b?"ERROR":b;e=window&&window.yterr||(void 0===e?!1:e)||!1;if(!(!a||!e||5<=$e||(e=a.stacktrace,c=a.columnNumber,a.hasOwnProperty("params")&&(d=String(JSON.stringify(a.params)),f.params=d.substr(0,500)),a=Fb(a),e=e||a.stack,d=a.lineNumber.toString(),isNaN(d)||isNaN(c)||(d=d+":"+c),window.yterr&&y(window.yterr)&&window.yterr(a),
Ze[a.message]||0<=e.indexOf("/YouTubeCenter.js")||0<=e.indexOf("/mytube.js")))){b={Va:{a:"logerror",t:"jserror",type:a.name,msg:a.message.substr(0,250),line:d,level:b,"client.name":f.name},C:{url:Q("PAGE_NAME",window.location.href),file:a.fileName},method:"POST"};f.version&&(b["client.version"]=f.version);e&&(b.C.stack=e);for(var g in f)b.C["client."+g]=f[g];if(g=Q("LATEST_ECATCHER_SERVICE_TRACKING_PARAMS",void 0))for(var h in g)b.C[h]=g[h];Ne(Q("ECATCHER_REPORT_HOST","")+"/error_204",b);Ze[a.message]=
!0;$e++}}
;var bf=window.yt&&window.yt.msgs_||window.ytcfg&&window.ytcfg.msgs||{};v("yt.msgs_",bf,void 0);function cf(a){ke(bf,arguments)}
;function df(a){a&&(a.dataset?a.dataset[ef("loaded")]="true":a.setAttribute("data-loaded","true"))}
function ff(a,b){return a?a.dataset?a.dataset[ef(b)]:a.getAttribute("data-"+b):null}
var gf={};function ef(a){return gf[a]||(gf[a]=String(a).replace(/\-([a-z])/g,function(a,c){return c.toUpperCase()}))}
;var hf=w("ytPubsubPubsubInstance")||new N;N.prototype.subscribe=N.prototype.subscribe;N.prototype.unsubscribeByKey=N.prototype.L;N.prototype.publish=N.prototype.K;N.prototype.clear=N.prototype.clear;v("ytPubsubPubsubInstance",hf,void 0);var jf=w("ytPubsubPubsubSubscribedKeys")||{};v("ytPubsubPubsubSubscribedKeys",jf,void 0);var kf=w("ytPubsubPubsubTopicToKeys")||{};v("ytPubsubPubsubTopicToKeys",kf,void 0);var lf=w("ytPubsubPubsubIsSynchronous")||{};v("ytPubsubPubsubIsSynchronous",lf,void 0);
function mf(a,b){var c=nf();if(c){var d=c.subscribe(a,function(){var c=arguments;var f=function(){jf[d]&&b.apply(window,c)};
try{lf[a]?f():T(f,0)}catch(g){R(g)}},void 0);
jf[d]=!0;kf[a]||(kf[a]=[]);kf[a].push(d);return d}return 0}
function of(a){var b=nf();b&&("number"==typeof a?a=[a]:t(a)&&(a=[parseInt(a,10)]),F(a,function(a){b.unsubscribeByKey(a);delete jf[a]}))}
function pf(a,b){var c=nf();c&&c.publish.apply(c,arguments)}
function qf(a){var b=nf();if(b)if(b.clear(a),a)rf(a);else for(var c in kf)rf(c)}
function nf(){return w("ytPubsubPubsubInstance")}
function rf(a){kf[a]&&(a=kf[a],F(a,function(a){jf[a]&&delete jf[a]}),a.length=0)}
;var sf=/\.vflset|-vfl[a-zA-Z0-9_+=-]+/,tf=/-[a-zA-Z]{2,3}_[a-zA-Z]{2,3}(?=(\/|$))/;function uf(a,b,c){c=void 0===c?null:c;if(window.spf){c="";if(a){var d=a.indexOf("jsbin/"),e=a.lastIndexOf(".js"),f=d+6;-1<d&&-1<e&&e>f&&(c=a.substring(f,e),c=c.replace(sf,""),c=c.replace(tf,""),c=c.replace("debug-",""),c=c.replace("tracing-",""))}spf.script.load(a,c,b)}else vf(a,b,c)}
function vf(a,b,c){c=void 0===c?null:c;var d=wf(a),e=document.getElementById(d),f=e&&ff(e,"loaded"),g=e&&!f;f?b&&b():(b&&(f=mf(d,b),b=""+(b[Aa]||(b[Aa]=++Ba)),xf[b]=f),g||(e=yf(a,d,function(){ff(e,"loaded")||(df(e),pf(d),T(Ea(qf,d),0))},c)))}
function yf(a,b,c,d){d=void 0===d?null:d;var e=document.createElement("SCRIPT");e.id=b;e.onload=function(){c&&setTimeout(c,0)};
e.onreadystatechange=function(){switch(e.readyState){case "loaded":case "complete":e.onload()}};
d&&e.setAttribute("nonce",d);Vb(e,ec(a));a=document.getElementsByTagName("head")[0]||document.body;a.insertBefore(e,a.firstChild);return e}
function zf(a){a=wf(a);var b=document.getElementById(a);b&&(qf(a),b.parentNode.removeChild(b))}
function Af(a,b){if(a&&b){var c=""+(b[Aa]||(b[Aa]=++Ba));(c=xf[c])&&of(c)}}
function wf(a){var b=document.createElement("a");Ub(b,a);a=b.href.replace(/^[a-zA-Z]+:\/\//,"//");return"js-"+Za(a)}
var xf={};function Bf(){}
function Cf(a,b){return Df(a,1,b)}
;function Ef(){}
n(Ef,Bf);function Df(a,b,c){isNaN(c)&&(c=void 0);var d=w("yt.scheduler.instance.addJob");return d?d(a,b,c):void 0===c?(a(),NaN):T(a,c||0)}
function Ff(a){if(!isNaN(a)){var b=w("yt.scheduler.instance.cancelJob");b?b(a):U(a)}}
Ef.prototype.start=function(){var a=w("yt.scheduler.instance.start");a&&a()};
Ef.prototype.pause=function(){var a=w("yt.scheduler.instance.pause");a&&a()};
xa(Ef);Ef.getInstance();var Gf=[],Hf=!1;function If(){if("1"!=db(ne(),"args","privembed")){var a=function(){Hf=!0;"google_ad_status"in window?P("DCLKSTAT",1):P("DCLKSTAT",2)};
uf("//static.doubleclick.net/instream/ad_status.js",a);Gf.push(Cf(function(){Hf||"google_ad_status"in window||(Af("//static.doubleclick.net/instream/ad_status.js",a),Hf=!0,P("DCLKSTAT",3))},5E3))}}
function Jf(){return parseInt(Q("DCLKSTAT",0),10)}
;function Kf(){this.f=!1;this.b=null}
Kf.prototype.initialize=function(a,b,c,d,e){var f=this;b?(this.f=!0,uf(b,function(){f.f=!1;if(window.botguard)Lf(f,c,d);else{zf(b);var a=Error("Unable to load Botguard");a.params="from "+b;se(a)}},e)):a&&(eval(a),window.botguard?Lf(this,c,d):se(Error("Unable to load Botguard from JS")))};
function Lf(a,b,c){try{a.b=new botguard.bg(b)}catch(d){se(d)}c&&c(b)}
Kf.prototype.dispose=function(){this.b=null};var Mf=new Kf,Nf=!1,Of=0,Pf="";function Qf(a){S("botguard_periodic_refresh")?Of=O():S("botguard_always_refresh")&&(Pf=a)}
function Rf(a){if(a){if(Mf.f)return!1;if(S("botguard_periodic_refresh"))return 72E5<O()-Of;if(S("botguard_always_refresh"))return Pf!=a}else return!1;return!Nf}
function Sf(){return!!Mf.b}
function Tf(a){a=void 0===a?{}:a;a=void 0===a?{}:a;return Mf.b?Mf.b.invoke(void 0,void 0,a):null}
;var Uf=0;v("ytDomDomGetNextId",w("ytDomDomGetNextId")||function(){return++Uf},void 0);var Vf={stopImmediatePropagation:1,stopPropagation:1,preventMouseEvent:1,preventManipulation:1,preventDefault:1,layerX:1,layerY:1,screenX:1,screenY:1,scale:1,rotation:1,webkitMovementX:1,webkitMovementY:1};
function Wf(a){this.type="";this.state=this.source=this.data=this.currentTarget=this.relatedTarget=this.target=null;this.charCode=this.keyCode=0;this.metaKey=this.shiftKey=this.ctrlKey=this.altKey=!1;this.clientY=this.clientX=0;this.changedTouches=this.touches=null;if(a=a||window.event){this.event=a;for(var b in a)b in Vf||(this[b]=a[b]);(b=a.target||a.srcElement)&&3==b.nodeType&&(b=b.parentNode);this.target=b;if(b=a.relatedTarget)try{b=b.nodeName?b:null}catch(c){b=null}else"mouseover"==this.type?
b=a.fromElement:"mouseout"==this.type&&(b=a.toElement);this.relatedTarget=b;this.clientX=void 0!=a.clientX?a.clientX:a.pageX;this.clientY=void 0!=a.clientY?a.clientY:a.pageY;this.keyCode=a.keyCode?a.keyCode:a.which;this.charCode=a.charCode||("keypress"==this.type?this.keyCode:0);this.altKey=a.altKey;this.ctrlKey=a.ctrlKey;this.shiftKey=a.shiftKey;this.metaKey=a.metaKey;this.b=a.pageX;this.f=a.pageY}}
function Xf(a){if(document.body&&document.documentElement){var b=document.body.scrollTop+document.documentElement.scrollTop;a.b=a.clientX+(document.body.scrollLeft+document.documentElement.scrollLeft);a.f=a.clientY+b}}
Wf.prototype.preventDefault=function(){this.event&&(this.event.returnValue=!1,this.event.preventDefault&&this.event.preventDefault())};
Wf.prototype.stopPropagation=function(){this.event&&(this.event.cancelBubble=!0,this.event.stopPropagation&&this.event.stopPropagation())};
Wf.prototype.stopImmediatePropagation=function(){this.event&&(this.event.cancelBubble=!0,this.event.stopImmediatePropagation&&this.event.stopImmediatePropagation())};var fb=w("ytEventsEventsListeners")||{};v("ytEventsEventsListeners",fb,void 0);var Yf=w("ytEventsEventsCounter")||{count:0};v("ytEventsEventsCounter",Yf,void 0);
function Zf(a,b,c,d){d=void 0===d?{}:d;a.addEventListener&&("mouseenter"!=b||"onmouseenter"in document?"mouseleave"!=b||"onmouseenter"in document?"mousewheel"==b&&"MozBoxSizing"in document.documentElement.style&&(b="MozMousePixelScroll"):b="mouseout":b="mouseover");return eb(function(e){var f="boolean"==typeof e[4]&&e[4]==!!d,g=z(e[4])&&z(d)&&ib(e[4],d);return!!e.length&&e[0]==a&&e[1]==b&&e[2]==c&&(f||g)})}
var $f=Hb(function(){var a=!1;try{var b=Object.defineProperty({},"capture",{get:function(){a=!0}});
window.addEventListener("test",null,b)}catch(c){}return a});
function V(a,b,c,d){d=void 0===d?{}:d;if(!a||!a.addEventListener&&!a.attachEvent)return"";var e=Zf(a,b,c,d);if(e)return e;e=++Yf.count+"";var f=!("mouseenter"!=b&&"mouseleave"!=b||!a.addEventListener||"onmouseenter"in document);var g=f?function(d){d=new Wf(d);if(!dc(d.relatedTarget,function(b){return b==a}))return d.currentTarget=a,d.type=b,c.call(a,d)}:function(b){b=new Wf(b);
b.currentTarget=a;return c.call(a,b)};
g=re(g);a.addEventListener?("mouseenter"==b&&f?b="mouseover":"mouseleave"==b&&f?b="mouseout":"mousewheel"==b&&"MozBoxSizing"in document.documentElement.style&&(b="MozMousePixelScroll"),$f()||"boolean"==typeof d?a.addEventListener(b,g,d):a.addEventListener(b,g,!!d.capture)):a.attachEvent("on"+b,g);fb[e]=[a,b,c,g,d];return e}
function ag(a){a&&("string"==typeof a&&(a=[a]),F(a,function(a){if(a in fb){var b=fb[a],d=b[0],e=b[1],f=b[3];b=b[4];d.removeEventListener?$f()||"boolean"==typeof b?d.removeEventListener(e,f,b):d.removeEventListener(e,f,!!b.capture):d.detachEvent&&d.detachEvent("on"+e,f);delete fb[a]}}))}
;function bg(a){this.w=a;this.b=null;this.i=0;this.m=null;this.j=0;this.g=[];for(a=0;4>a;a++)this.g.push(0);this.h=0;this.D=V(window,"mousemove",A(this.G,this));a=A(this.B,this);y(a)&&(a=re(a));this.H=window.setInterval(a,25)}
C(bg,L);bg.prototype.G=function(a){r(a.b)||Xf(a);var b=a.b;r(a.f)||Xf(a);this.b=new Wb(b,a.f)};
bg.prototype.B=function(){if(this.b){var a=O();if(0!=this.i){var b=this.m,c=this.b,d=b.x-c.x;b=b.y-c.y;d=Math.sqrt(d*d+b*b)/(a-this.i);this.g[this.h]=.5<Math.abs((d-this.j)/this.j)?1:0;for(c=b=0;4>c;c++)b+=this.g[c]||0;3<=b&&this.w();this.j=d}this.i=a;this.m=this.b;this.h=(this.h+1)%4}};
bg.prototype.l=function(){window.clearInterval(this.H);ag(this.D)};var cg={};
function dg(a){var b=void 0===a?{}:a;a=void 0===b.xa?!0:b.xa;b=void 0===b.Ka?!1:b.Ka;if(null==w("_lact",window)){var c=parseInt(Q("LACT"),10);c=isFinite(c)?B()-Math.max(c,0):-1;v("_lact",c,window);v("_fact",c,window);-1==c&&eg();V(document,"keydown",eg);V(document,"keyup",eg);V(document,"mousedown",eg);V(document,"mouseup",eg);a&&(b?V(window,"touchmove",function(){fg("touchmove",200)},{passive:!0}):(V(window,"resize",function(){fg("resize",200)}),V(window,"scroll",function(){fg("scroll",200)})));
new bg(function(){fg("mouse",100)});
V(document,"touchstart",eg,{passive:!0});V(document,"touchend",eg,{passive:!0})}}
function fg(a,b){cg[a]||(cg[a]=!0,Cf(function(){eg();cg[a]=!1},b))}
function eg(){null==w("_lact",window)&&dg();var a=B();v("_lact",a,window);-1==w("_fact",window)&&v("_fact",a,window);(a=w("ytglobal.ytUtilActivityCallback_"))&&a()}
function gg(){var a=w("_lact",window);return null==a?-1:Math.max(B()-a,0)}
;var hg=Math.pow(2,16)-1,ig=null,jg=0,kg={log_event:"events",log_interaction:"interactions"},lg=Object.create(null);lg.log_event="GENERIC_EVENT_LOGGING";lg.log_interaction="INTERACTION_LOGGING";var mg=new Set(["log_event"]),ng={},og=0,pg=0,W=w("ytLoggingTransportLogPayloadsQueue_")||{};v("ytLoggingTransportLogPayloadsQueue_",W,void 0);var qg=w("ytLoggingTransportTokensToCttTargetIds_")||{};v("ytLoggingTransportTokensToCttTargetIds_",qg,void 0);var rg=w("ytLoggingTransportDispatchedStats_")||{};
v("ytLoggingTransportDispatchedStats_",rg,void 0);v("ytytLoggingTransportCapturedTime_",w("ytLoggingTransportCapturedTime_")||{},void 0);function sg(){U(og);U(pg);pg=0;if(!gb(W)){for(var a in W){var b=ng[a];b&&(tg(a,b),delete W[a])}gb(W)||ug()}}
function ug(){S("web_gel_timeout_cap")&&!pg&&(pg=T(sg,3E4));U(og);og=T(sg,Q("LOGGING_BATCH_TIMEOUT",te("web_gel_debounce_ms",1E4)))}
function vg(a,b){b=void 0===b?"":b;W[a]=W[a]||{};W[a][b]=W[a][b]||[];return W[a][b]}
function tg(a,b){var c=kg[a],d=rg[a]||{};rg[a]=d;var e=Math.round(O());for(l in W[a]){var f=kb,g=b.b;g={client:{hl:g.Da,gl:g.Ca,clientName:g.Aa,clientVersion:g.Ba}};var h=window.devicePixelRatio;h&&1!=h&&(g.client.screenDensityFloat=String(h));Q("DELEGATED_SESSION_ID")&&!S("pageid_as_header_web")&&(g.user={onBehalfOfUser:Q("DELEGATED_SESSION_ID")});f=f({context:g});f[c]=vg(a,l);d.dispatchedEventCount=d.dispatchedEventCount||0;d.dispatchedEventCount+=f[c].length;if(g=qg[l])a:{var m=l;if(g.videoId)h=
"VIDEO";else if(g.playlistId)h="PLAYLIST";else break a;f.credentialTransferTokenTargetId=g;f.context=f.context||{};f.context.user=f.context.user||{};f.context.user.credentialTransferTokens=[{token:m,scope:h}]}delete qg[l];f.requestTimeMs=e;if(g=me("EVENT_ID"))h=(Q("BATCH_CLIENT_COUNTER",void 0)||0)+1,h>hg&&(h=1),P("BATCH_CLIENT_COUNTER",h),g={serializedEventId:g,clientCounter:h},f.serializedClientEventId=g,ig&&jg&&S("log_gel_rtt_web")&&(f.previousBatchInfo={serializedClientEventId:ig,roundtripMs:jg}),
ig=g,jg=0;wg(b,a,f,{retry:mg.has(a),onSuccess:A(xg,this,O())})}if(d.previousDispatchMs){c=e-d.previousDispatchMs;var l=d.diffCount||0;d.averageTimeBetweenDispatchesMs=l?(d.averageTimeBetweenDispatchesMs*l+c)/(l+1):c;d.diffCount=l+1}d.previousDispatchMs=e}
function xg(a){jg=Math.round(O()-a)}
;function yg(a,b,c,d,e){var f={};f.eventTimeMs=Math.round(d||O());f[a]=b;f.context={lastActivityMs:String(d?-1:gg())};e?(a={},e.videoId?a.videoId=e.videoId:e.playlistId&&(a.playlistId=e.playlistId),qg[e.token]=a,e=vg("log_event",e.token)):e=vg("log_event");e.push(f);c&&(ng.log_event=new c);e.length>=(te("web_logging_max_batch")||20)?sg():ug()}
;function zg(a,b,c){c=void 0===c?{}:c;var d={"X-Goog-Visitor-Id":c.visitorData||Q("VISITOR_DATA","")};if(b&&b.includes("www.youtube-nocookie.com"))return d;(b=c.jb||Q("AUTHORIZATION"))||(a?b="Bearer "+w("gapi.auth.getToken")().ib:b=Nc([]));b&&(d.Authorization=b,d["X-Goog-AuthUser"]=Q("SESSION_INDEX",0),S("pageid_as_header_web")&&(d["X-Goog-PageId"]=Q("DELEGATED_SESSION_ID")));return d}
function Ag(a){a=Object.assign({},a);delete a.Authorization;var b=Nc();if(b){var c=new cd;c.update(Q("INNERTUBE_API_KEY",void 0));c.update(b);b=c.digest();za(b);if(!Bb)for(Bb={},Cb={},c=0;65>c;c++)Bb[c]="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".charAt(c),Cb[c]="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.".charAt(c);c=Cb;for(var d=[],e=0;e<b.length;e+=3){var f=b[e],g=e+1<b.length,h=g?b[e+1]:0,m=e+2<b.length,l=m?b[e+2]:0,u=f>>2;f=(f&3)<<4|h>>4;h=(h&15)<<
2|l>>6;l&=63;m||(l=64,g||(h=64));d.push(c[u],c[f],c[h],c[l])}a.hash=d.join("")}return a}
;function Bg(a,b,c,d){Eb.set(""+a,b,c,"/",void 0===d?"youtube.com":d,!1)}
;function Cg(){var a=new be;(a=a.isAvailable()?new he(a,"yt.innertube"):null)||(a=new ce("yt.innertube"),a=a.isAvailable()?a:null);this.b=a?new Yd(a):null;this.f=document.domain||window.location.hostname}
Cg.prototype.set=function(a,b,c,d){c=c||31104E3;this.remove(a);if(this.b)try{this.b.set(a,b,B()+1E3*c);return}catch(f){}var e="";if(d)try{e=escape(td(b))}catch(f){return}else e=escape(b);Bg(a,e,c,this.f)};
Cg.prototype.get=function(a,b){var c=void 0,d=!this.b;if(!d)try{c=this.b.get(a)}catch(e){d=!0}if(d&&(c=Eb.get(""+a,void 0))&&(c=unescape(c),b))try{c=JSON.parse(c)}catch(e){this.remove(a),c=void 0}return c};
Cg.prototype.remove=function(a){this.b&&this.b.remove(a);var b=this.f;Eb.remove(""+a,"/",void 0===b?"youtube.com":b)};var Dg=new Cg;function Eg(a,b,c,d){if(d)return null;d=Dg.get("nextId",!0)||1;var e=Dg.get("requests",!0)||{};e[d]={method:a,request:b,authState:Ag(c),requestTime:Math.round(O())};Dg.set("nextId",d+1,86400,!0);Dg.set("requests",e,86400,!0);return d}
function Fg(a){var b=Dg.get("requests",!0)||{};delete b[a];Dg.set("requests",b,86400,!0)}
function Gg(a){var b=Dg.get("requests",!0);if(b){for(var c in b){var d=b[c];if(!(6E4>Math.round(O())-d.requestTime)){var e=d.authState,f=Ag(zg(!1));ib(e,f)&&(e=d.request,"requestTimeMs"in e&&(e.requestTimeMs=Math.round(O())),wg(a,d.method,e,{}));delete b[c]}}Dg.set("requests",b,86400,!0)}}
;function Hg(a){var b=this;this.b=a||{ya:me("INNERTUBE_API_KEY"),za:me("INNERTUBE_API_VERSION"),Aa:Q("INNERTUBE_CONTEXT_CLIENT_NAME","WEB"),Ba:me("INNERTUBE_CONTEXT_CLIENT_VERSION"),Da:me("INNERTUBE_CONTEXT_HL"),Ca:me("INNERTUBE_CONTEXT_GL"),Ea:me("INNERTUBE_HOST_OVERRIDE")||"",Fa:!!Q("INNERTUBE_USE_THIRD_PARTY_AUTH",!1)};Df(function(){Gg(b)},0,5E3)}
function wg(a,b,c,d){!Q("VISITOR_DATA")&&.01>Math.random()&&R(Error("Missing VISITOR_DATA when sending innertube request."),"WARNING");var e={headers:{"Content-Type":"application/json"},method:"POST",C:c,ia:"JSON",M:function(){d.M()},
ha:d.M,onSuccess:function(a,b){if(d.onSuccess)d.onSuccess(b)},
ga:function(a){if(d.onSuccess)d.onSuccess(a)},
onError:function(a,b){if(d.onError)d.onError(b)},
ob:function(a){if(d.onError)d.onError(a)},
timeout:d.timeout,withCredentials:!0},f="",g=a.b.Ea;g&&(f=g);g=a.b.Fa||!1;var h=zg(g,f,d);Object.assign(e.headers,h);e.headers.Authorization&&!f&&(e.headers["x-origin"]=window.location.origin);var m=""+f+("/youtubei/"+a.b.za+"/"+b)+"?alt=json&key="+a.b.ya,l;if(d.retry&&S("retry_web_logging_batches")&&"www.youtube-nocookie.com"!=f&&(l=Eg(b,c,h,g))){var u=e.onSuccess,ea=e.ga;e.onSuccess=function(a,b){Fg(l);u(a,b)};
c.ga=function(a,b){Fg(l);ea(a,b)}}try{S("use_fetch_for_op_xhr")?Ke(m,e):Te(m,e)}catch(Dd){if("InvalidAccessError"==Dd)l&&(Fg(l),l=0),R(Error("An extension is blocking network request."),"WARNING");
else throw Dd;}l&&Df(function(){Gg(a)},0,5E3)}
;var Ig=B().toString();
function Jg(){a:{if(window.crypto&&window.crypto.getRandomValues)try{var a=Array(16),b=new Uint8Array(16);window.crypto.getRandomValues(b);for(var c=0;c<a.length;c++)a[c]=b[c];var d=a;break a}catch(e){}d=Array(16);for(a=0;16>a;a++){b=B();for(c=0;c<b%23;c++)d[a]=Math.random();d[a]=Math.floor(256*Math.random())}if(Ig)for(a=1,b=0;b<Ig.length;b++)d[a%16]=d[a%16]^d[(a-1)%16]/4^Ig.charCodeAt(b),a++}a=[];for(b=0;b<d.length;b++)a.push("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_".charAt(d[b]&63));
return a.join("")}
;var Kg=w("ytLoggingTimeDocumentNonce_")||Jg();v("ytLoggingTimeDocumentNonce_",Kg,void 0);function Lg(a){this.b=a}
function Mg(a){var b={};void 0!==a.b.trackingParams?b.trackingParams=a.b.trackingParams:(b.veType=a.b.veType,null!=a.b.veCounter&&(b.veCounter=a.b.veCounter),null!=a.b.elementIndex&&(b.elementIndex=a.b.elementIndex));void 0!==a.b.dataElement&&(b.dataElement=Mg(a.b.dataElement));void 0!==a.b.youtubeData&&(b.youtubeData=a.b.youtubeData);return b}
Lg.prototype.toString=function(){return JSON.stringify(Mg(this))};
var Ng=1;function Og(a){a=void 0===a?0:a;return 0==a?"client-screen-nonce":"client-screen-nonce."+a}
function Pg(a){a=void 0===a?0:a;return 0==a?"ROOT_VE_TYPE":"ROOT_VE_TYPE."+a}
function Qg(a){return Q(Pg(void 0===a?0:a),void 0)}
v("yt_logging_screen.getRootVeType",Qg,void 0);function Rg(){var a=Qg(0),b;a?b=new Lg({veType:a,youtubeData:void 0}):b=null;return b}
function Sg(a){a=void 0===a?0:a;var b=Q(Og(a));b||0!=a||(b=Q("EVENT_ID"));return b?b:null}
v("yt_logging_screen.getCurrentCsn",Sg,void 0);function Tg(a,b,c){c=void 0===c?0:c;if(a!==Q(Og(c))||b!==Q(Pg(c)))P(Og(c),a),P(Pg(c),b),0==c&&(b=function(){setTimeout(function(){a&&yg("foregroundHeartbeatScreenAssociated",{clientDocumentNonce:Kg,clientScreenNonce:a},Hg)},0)},"requestAnimationFrame"in window?window.requestAnimationFrame(b):b())}
v("yt_logging_screen.setCurrentScreen",Tg,void 0);function Ug(a,b,c){b=void 0===b?{}:b;c=void 0===c?!1:c;var d=Q("EVENT_ID");d&&(b.ei||(b.ei=d));if(b){d=a;var e=Q("VALID_SESSION_TEMPDATA_DOMAINS",[]),f=I(J(3,window.location.href));f&&e.push(f);f=I(J(3,d));if(0<=Ha(e,f)||!f&&0==d.lastIndexOf("/",0))if(S("autoescape_tempdata_url")&&(e=document.createElement("a"),Ub(e,d),d=e.href),d){f=d.match(pc);d=f[5];e=f[6];f=f[7];var g="";d&&(g+=d);e&&(g+="?"+e);f&&(g+="#"+f);d=g;e=d.indexOf("#");if(d=0>e?d:d.substr(0,e)){if(b.itct||b.ved)b.csn=b.csn||Sg();if(h){var h=
parseInt(h,10);isFinite(h)&&0<h&&(d="ST-"+Za(d).toString(36),b=b?rc(b):"",Bg(d,b,h||5))}else h="ST-"+Za(d).toString(36),b=b?rc(b):"",Bg(h,b,5)}}}if(c)return!1;if((window.ytspf||{}).enabled)spf.navigate(a);else{var m=void 0===m?{}:m;var l=void 0===l?"":l;var u=void 0===u?window:u;c=u.location;a=sc(a,m)+l;a=a instanceof H?a:Pb(a);c.href=Nb(a)}return!0}
;function Vg(a,b,c){a={csn:a,parentVe:Mg(b),childVes:Ja(c,function(a){return Mg(a)})};
yg("visualElementAttached",a,Hg,void 0,void 0)}
;function Wg(a){a=a||{};var b={enablejsapi:1},c={};this.url=a.url||"";this.args=a.args||jb(b);this.assets=a.assets||{};this.attrs=a.attrs||jb(c);this.fallback=a.fallback||null;this.fallbackMessage=a.fallbackMessage||null;this.html5=!!a.html5;this.disable=a.disable||{};this.loaded=!!a.loaded;this.messages=a.messages||{}}
Wg.prototype.clone=function(){var a=new Wg,b;for(b in this)if(this.hasOwnProperty(b)){var c=this[b];"object"==ya(c)?a[b]=jb(c):a[b]=c}return a};function Xg(){L.call(this);this.b=[]}
n(Xg,L);Xg.prototype.l=function(){for(;this.b.length;){var a=this.b.pop();a.target.removeEventListener(a.name,a.lb)}L.prototype.l.call(this)};var Yg=/cssbin\/(?:debug-)?([a-zA-Z0-9_-]+?)(?:-2x|-web|-rtl|-vfl|.css)/;function Zg(a){a=a||"";if(window.spf){var b=a.match(Yg);spf.style.load(a,b?b[1]:"",void 0)}else $g(a)}
function $g(a){var b=ah(a),c=document.getElementById(b),d=c&&ff(c,"loaded");d||c&&!d||(c=bh(a,b,function(){ff(c,"loaded")||(df(c),pf(b),T(Ea(qf,b),0))}))}
function bh(a,b,c){var d=document.createElement("link");d.id=b;d.onload=function(){c&&setTimeout(c,0)};
a=ec(a);d.rel="stylesheet";d.href=Lb(a);(document.getElementsByTagName("head")[0]||document.body).appendChild(d);return d}
function ah(a){var b=document.createElement("A");a=Qb(a);Ub(b,a);b=b.href.replace(/^[a-zA-Z]+:\/\//,"//");return"css-"+Za(b)}
;function ch(a,b){L.call(this);this.j=this.T=a;this.D=b;this.m=!1;this.api={};this.P=this.B=null;this.G=new N;ed(this,Ea(fd,this.G));this.h={};this.N=this.R=this.g=this.X=this.b=null;this.H=!1;this.i=this.w=null;this.U={};this.ma=["onReady"];this.W=null;this.ea=NaN;this.O={};dh(this);this.V("WATCH_LATER_VIDEO_ADDED",this.Ha.bind(this));this.V("WATCH_LATER_VIDEO_REMOVED",this.Ia.bind(this));this.V("onAdAnnounce",this.qa.bind(this));this.na=new Xg(this);ed(this,Ea(fd,this.na))}
n(ch,L);k=ch.prototype;
k.loadNewVideoConfig=function(a){if(!this.f){a instanceof Wg||(a=new Wg(a));this.X=a;this.b=a.clone();this.g=this.b.attrs.id||this.g;"video-player"==this.g&&(this.g=this.D,this.b.attrs.id=this.D);this.j.id==this.g&&(this.g+="-player",this.b.attrs.id=this.g);this.b.args.enablejsapi="1";this.b.args.playerapiid=this.D;this.R||(this.R=eh(this,this.b.args.jsapicallback||"onYouTubePlayerReady"));this.b.args.jsapicallback=null;if(a=this.b.attrs.width)this.j.style.width=nc(Number(a)||a);if(a=this.b.attrs.height)this.j.style.height=
nc(Number(a)||a);fh(this);this.m&&gh(this)}};
k.ta=function(){return this.X};
function gh(a){a.b.loaded||(a.b.loaded=!0,"0"!=a.b.args.autoplay?a.api.loadVideoByPlayerVars(a.b.args):a.api.cueVideoByPlayerVars(a.b.args))}
function hh(a){var b=!0,c=ih(a);c&&a.b&&(a=a.b,b=ff(c,"version")==a.assets.js);return b&&!!w("yt.player.Application.create")}
function fh(a){if(!a.f&&!a.H){var b=hh(a);if(b&&"html5"==(ih(a)?"html5":null))a.N="html5",a.m||jh(a);else if(kh(a),a.N="html5",b&&a.i)a.T.appendChild(a.i),jh(a);else{a.b.loaded=!0;var c=!1;a.w=function(){c=!0;var b=a.b.clone();w("yt.player.Application.create")(a.T,b);jh(a)};
a.H=!0;b?a.w():(uf(a.b.assets.js,a.w),Zg(a.b.assets.css),lh(a)&&!c&&v("yt.player.Application.create",null,void 0))}}}
function ih(a){var b=Yb(a.g);!b&&a.j&&a.j.querySelector&&(b=a.j.querySelector("#"+a.g));return b}
function jh(a){if(!a.f){var b=ih(a),c=!1;b&&b.getApiInterface&&b.getApiInterface()&&(c=!0);c?(a.H=!1,b.isNotServable&&b.isNotServable(a.b.args.video_id)||mh(a)):a.ea=T(function(){jh(a)},50)}}
function mh(a){dh(a);a.m=!0;var b=ih(a);b.addEventListener&&(a.B=nh(a,b,"addEventListener"));b.removeEventListener&&(a.P=nh(a,b,"removeEventListener"));var c=b.getApiInterface();c=c.concat(b.getInternalApiInterface());for(var d=0;d<c.length;d++){var e=c[d];a.api[e]||(a.api[e]=nh(a,b,e))}for(var f in a.h)a.B(f,a.h[f]);gh(a);a.R&&a.R(a.api);a.G.K("onReady",a.api)}
function nh(a,b,c){var d=b[c];return function(){try{return a.W=null,d.apply(b,arguments)}catch(e){"sendAbandonmentPing"!=c&&(e.message+=" ("+c+")",a.W=e,se(e))}}}
function dh(a){a.m=!1;if(a.P)for(var b in a.h)a.P(b,a.h[b]);for(var c in a.O)U(parseInt(c,10));a.O={};a.B=null;a.P=null;for(var d in a.api)a.api[d]=null;a.api.addEventListener=a.V.bind(a);a.api.removeEventListener=a.Ma.bind(a);a.api.destroy=a.dispose.bind(a);a.api.getLastError=a.ua.bind(a);a.api.getPlayerType=a.va.bind(a);a.api.getCurrentVideoConfig=a.ta.bind(a);a.api.loadNewVideoConfig=a.loadNewVideoConfig.bind(a);a.api.isReady=a.Ga.bind(a)}
k.Ga=function(){return this.m};
k.V=function(a,b){var c=this,d=eh(this,b);if(d){if(!(0<=Ha(this.ma,a)||this.h[a])){var e=oh(this,a);this.B&&this.B(a,e)}this.G.subscribe(a,d);"onReady"==a&&this.m&&T(function(){d(c.api)},0)}};
k.Ma=function(a,b){if(!this.f){var c=eh(this,b);c&&Sd(this.G,a,c)}};
function eh(a,b){var c=b;if("string"==typeof b){if(a.U[b])return a.U[b];c=function(){var a=w(b);a&&a.apply(q,arguments)};
a.U[b]=c}return c?c:null}
function oh(a,b){var c="ytPlayer"+b+a.D;a.h[b]=c;q[c]=function(c){var d=T(function(){if(!a.f){a.G.K(b,c);var e=a.O,g=String(d);g in e&&delete e[g]}},0);
hb(a.O,String(d))};
return c}
k.qa=function(a){pf("a11y-announce",a)};
k.Ha=function(a){pf("WATCH_LATER_VIDEO_ADDED",a)};
k.Ia=function(a){pf("WATCH_LATER_VIDEO_REMOVED",a)};
k.va=function(){return this.N||(ih(this)?"html5":null)};
k.ua=function(){return this.W};
function kh(a){a.cancel();dh(a);a.N=null;a.b&&(a.b.loaded=!1);var b=ih(a);b&&(hh(a)||!lh(a)?a.i=b:(b&&b.destroy&&b.destroy(),a.i=null));for(a=a.T;b=a.firstChild;)a.removeChild(b)}
k.cancel=function(){this.w&&Af(this.b.assets.js,this.w);U(this.ea);this.H=!1};
k.l=function(){kh(this);if(this.i&&this.b&&this.i.destroy)try{this.i.destroy()}catch(b){R(b)}this.U=null;for(var a in this.h)q[this.h[a]]=null;this.X=this.b=this.api=null;delete this.T;delete this.j;L.prototype.l.call(this)};
function lh(a){return a.b&&a.b.args&&a.b.args.fflags?-1!=a.b.args.fflags.indexOf("player_destroy_old_version=true"):!1}
;var ph={},qh="player_uid_"+(1E9*Math.random()>>>0);function rh(a){var b="player";b=t(b)?Yb(b):b;var c=qh+"_"+(b[Aa]||(b[Aa]=++Ba)),d=ph[c];if(d)return d.loadNewVideoConfig(a),d.api;d=new ch(b,c);ph[c]=d;pf("player-added",d.api);ed(d,Ea(sh,d));T(function(){d.loadNewVideoConfig(a)},0);
return d.api}
function sh(a){delete ph[a.D]}
;function th(a,b,c){var d=Hg;Q("ytLoggingEventsDefaultDisabled",!1)&&Hg==Hg&&(d=null);yg(a,b,d,c,void 0)}
;var uh=w("ytLoggingLatencyUsageStats_")||{};v("ytLoggingLatencyUsageStats_",uh,void 0);var vh=0;function wh(a){uh[a]=uh[a]||{count:0};var b=uh[a];b.count++;b.time=O();vh||(vh=Df(xh,0,5E3));if(10<b.count){if(11==b.count){b=0==a.indexOf("info")?"WARNING":"ERROR";var c=Error("CSI data exceeded logging limit with key");c.params=a;af(c,b)}return!0}return!1}
function xh(){var a=O(),b;for(b in uh)6E4<a-uh[b].time&&delete uh[b];vh=0}
;function yh(a,b){this.version=a;this.args=b}
;function zh(a){this.topic=a}
zh.prototype.toString=function(){return this.topic};var Ah=w("ytPubsub2Pubsub2Instance")||new N;N.prototype.subscribe=N.prototype.subscribe;N.prototype.unsubscribeByKey=N.prototype.L;N.prototype.publish=N.prototype.K;N.prototype.clear=N.prototype.clear;v("ytPubsub2Pubsub2Instance",Ah,void 0);v("ytPubsub2Pubsub2SubscribedKeys",w("ytPubsub2Pubsub2SubscribedKeys")||{},void 0);v("ytPubsub2Pubsub2TopicToKeys",w("ytPubsub2Pubsub2TopicToKeys")||{},void 0);v("ytPubsub2Pubsub2IsAsync",w("ytPubsub2Pubsub2IsAsync")||{},void 0);
v("ytPubsub2Pubsub2SkipSubKey",null,void 0);function Bh(a,b){var c=w("ytPubsub2Pubsub2Instance");c&&c.publish.call(c,a.toString(),a,b)}
;var X=window.performance||window.mozPerformance||window.msPerformance||window.webkitPerformance||{};function Ch(){var a=Q("TIMING_TICK_EXPIRATION");a||(a={},P("TIMING_TICK_EXPIRATION",a));return a}
function Dh(){var a=Ch(),b;for(b in a)Ff(a[b]);P("TIMING_TICK_EXPIRATION",{})}
;function Eh(a,b){yh.call(this,1,arguments)}
n(Eh,yh);function Fh(a,b){yh.call(this,1,arguments)}
n(Fh,yh);var Gh=new zh("aft-recorded"),Hh=new zh("timing-sent");var Ih={vc:!0},Y={},Jh=(Y.ad_allowed="adTypesAllowed",Y.yt_abt="adBreakType",Y.ad_cpn="adClientPlaybackNonce",Y.ad_docid="adVideoId",Y.yt_ad_an="adNetworks",Y.ad_at="adType",Y.browse_id="browseId",Y.p="httpProtocol",Y.t="transportProtocol",Y.cpn="clientPlaybackNonce",Y.csn="clientScreenNonce",Y.docid="videoId",Y.is_continuation="isContinuation",Y.is_nav="isNavigation",Y.b_p="kabukiInfo.browseParams",Y.is_prefetch="kabukiInfo.isPrefetch",Y.is_secondary_nav="kabukiInfo.isSecondaryNav",Y.prev_browse_id=
"kabukiInfo.prevBrowseId",Y.query_source="kabukiInfo.querySource",Y.voz_type="kabukiInfo.vozType",Y.yt_lt="loadType",Y.yt_ad="isMonetized",Y.nr="webInfo.navigationReason",Y.ncnp="webInfo.nonPreloadedNodeCount",Y.paused="playerInfo.isPausedOnLoad",Y.yt_pt="playerType",Y.fmt="playerInfo.itag",Y.yt_pl="watchInfo.isPlaylist",Y.yt_pre="playerInfo.preloadType",Y.yt_ad_pr="prerollAllowed",Y.pa="previousAction",Y.yt_red="isRedSubscriber",Y.st="serverTimeMs",Y.aq="tvInfo.appQuality",Y.br_trs="tvInfo.bedrockTriggerState",
Y.label="tvInfo.label",Y.is_mdx="tvInfo.isMdx",Y.preloaded="tvInfo.isPreloaded",Y.query="unpluggedInfo.query",Y.upg_chip_ids_string="unpluggedInfo.upgChipIdsString",Y.yt_vst="videoStreamType",Y.vph="viewportHeight",Y.vpw="viewportWidth",Y.yt_vis="isVisible",Y),Kh="ap c cver cbrand cmodel ei srt yt_fss yt_li plid vpil vpni vpst yt_eil vpni2 vpil2 icrc icrt pa GetBrowse_rid GetPlayer_rid GetSearch_rid GetWatchNext_rid cmt d_vpct d_vpnfi d_vpni pc pfa pfeh pftr prerender psc rc start tcrt tcrc ssr vpr vps yt_abt yt_fn yt_fs yt_pft yt_pre yt_pt yt_pvis yt_ref yt_sts".split(" "),
Lh="isContinuation isNavigation kabukiInfo.isPrefetch kabukiInfo.isSecondaryNav isMonetized playerInfo.isPausedOnLoad prerollAllowed isRedSubscriber tvInfo.isMdx tvInfo.isPreloaded isVisible watchInfo.isPlaylist".split(" "),Mh={},Nh=(Mh.pa="LATENCY_ACTION_",Mh.yt_pt="LATENCY_PLAYER_",Mh.yt_vst="VIDEO_STREAM_TYPE_",Mh),Oh=!1;
function Ph(){var a=Qh().info.yt_lt="hot_bg";Rh().info_yt_lt=a;if(Sh())if("yt_lt"in Jh){var b=Jh.yt_lt;0<=Ha(Lh,b)&&(a=!!a);"yt_lt"in Nh&&(a=Nh.yt_lt+a.toUpperCase());var c=a;if(Sh()){a={};b=b.split(".");for(var d=a,e=0;e<b.length-1;e++)d[b[e]]=d[b[e]]||{},d=d[b[e]];d[b[b.length-1]]=c;c=Th();b=Object.keys(a).join("");wh("info_"+b+"_"+c)||(a.clientActionNonce=c,th("latencyActionInfo",a))}}else 0<=Ha(Kh,"yt_lt")||R(Error("Unknown label yt_lt logged with GEL CSI."))}
function Uh(){var a=Vh();if(a.aft)return a.aft;for(var b=Q("TIMING_AFT_KEYS",["ol"]),c=b.length,d=0;d<c;d++){var e=a[b[d]];if(e)return e}return NaN}
A(X.clearResourceTimings||X.webkitClearResourceTimings||X.mozClearResourceTimings||X.msClearResourceTimings||X.oClearResourceTimings||wa,X);function Th(){var a=Qh().nonce;a||(a=Jg(),Qh().nonce=a);return a}
function Vh(){return Qh().tick}
function Rh(){var a=Qh();"gel"in a||(a.gel={});return a.gel}
function Qh(){var a;(a=w("ytcsi.data_"))||(a={tick:{},info:{}},Fa("ytcsi.data_",a));return a}
function Wh(){var a=Vh(),b=a.pbr,c=a.vc;a=a.pbs;return b&&c&&a&&b<c&&c<a&&1==Qh().info.yt_pvis}
function Sh(){return!!S("csi_on_gel")||!!Qh().useGel}
function Xh(){Dh();if(!Sh()){var a=Vh(),b=Qh().info,c=a._start;for(f in a)if(0==f.lastIndexOf("_",0)&&x(a[f])){var d=f.slice(1);if(d in Ih){var e=Ja(a[f],function(a){return Math.round(a-c)});
b["all_"+d]=e.join()}delete a[f]}e=Q("CSI_SERVICE_NAME","youtube");var f={v:2,s:e,action:Q("TIMING_ACTION",void 0)};d=Ph.srt;void 0!==a.srt&&delete b.srt;if(b.h5jse){var g=window.location.protocol+w("ytplayer.config.assets.js");(g=X.getEntriesByName?X.getEntriesByName(g)[0]:null)?b.h5jse=Math.round(b.h5jse-g.responseEnd):delete b.h5jse}a.aft=Uh();Wh()&&"youtube"==e&&(Ph(),e=a.vc,g=a.pbs,delete a.aft,b.aft=Math.round(g-e));for(var h in b)"_"!=h.charAt(0)&&(f[h]=b[h]);a.ps=O();h={};e=[];for(var m in a)"_"!=
m.charAt(0)&&(g=Math.round(a[m]-c),h[m]=g,e.push(m+"."+g));f.rt=e.join(",");(a=w("ytdebug.logTiming"))&&a(f,h);Yh(f,!!b.ap);Bh(Hh,new Fh(h.aft+(d||0),void 0))}}
function Yh(a,b){if(S("debug_csi_data")){var c=w("yt.timing.csiData");c||(c=[],v("yt.timing.csiData",c,void 0));c.push({page:location.href,time:new Date,args:a})}c="";for(var d in a)c+="&"+d+"="+a[d];d="/csi_204?"+c.substring(1);if(window.navigator&&window.navigator.sendBeacon&&b){var e=void 0===e?"":e;Xe(d,e)||We(d,void 0,void 0,void 0,e)}else We(d);Fa("yt.timing.pingSent_",!0)}
;function Zh(a){return(0==a.search("cue")||0==a.search("load"))&&"loadModule"!=a}
function $h(a,b,c){t(a)&&(a={mediaContentUrl:a,startSeconds:b,suggestedQuality:c});b=/\/([ve]|embed)\/([^#?]+)/.exec(a.mediaContentUrl);a.videoId=b&&b[2]?b[2]:null;return ai(a)}
function ai(a,b,c){if(z(a)){b=["endSeconds","startSeconds","mediaContentUrl","suggestedQuality","videoId"];c={};for(var d=0;d<b.length;d++){var e=b[d];a[e]&&(c[e]=a[e])}return c}return{videoId:a,startSeconds:b,suggestedQuality:c}}
function bi(a,b,c,d){if(z(a)&&!x(a)){b="playlist list listType index startSeconds suggestedQuality".split(" ");c={};for(d=0;d<b.length;d++){var e=b[d];a[e]&&(c[e]=a[e])}return c}b={index:b,startSeconds:c,suggestedQuality:d};t(a)&&16==a.length?b.list="PL"+a:b.playlist=a;return b}
;function ci(a){L.call(this);this.b=a;this.b.subscribe("command",this.ja,this);this.g={};this.i=!1}
C(ci,L);k=ci.prototype;k.start=function(){this.i||this.f||(this.i=!0,di(this.b,"RECEIVING"))};
k.ja=function(a,b,c){if(this.i&&!this.f){var d=b||{};switch(a){case "addEventListener":t(d.event)&&(a=d.event,a in this.g||(c=A(this.Oa,this,a),this.g[a]=c,this.addEventListener(a,c)));break;case "removeEventListener":t(d.event)&&ei(this,d.event);break;default:this.h.isReady()&&this.h.isExternalMethodAvailable(a,c||null)&&(b=fi(a,b||{}),c=this.h.handleExternalCall(a,b,c||null),(c=gi(a,c))&&this.i&&!this.f&&di(this.b,a,c))}}};
k.Oa=function(a,b){this.i&&!this.f&&di(this.b,a,this.Z(a,b))};
k.Z=function(a,b){if(null!=b)return{value:b}};
function ei(a,b){b in a.g&&(a.removeEventListener(b,a.g[b]),delete a.g[b])}
k.l=function(){var a=this.b;a.f||Sd(a.b,"command",this.ja,this);this.b=null;for(var b in this.g)ei(this,b);ci.A.l.call(this)};function hi(a,b){ci.call(this,b);this.h=a;this.start()}
C(hi,ci);hi.prototype.addEventListener=function(a,b){this.h.addEventListener(a,b)};
hi.prototype.removeEventListener=function(a,b){this.h.removeEventListener(a,b)};
function fi(a,b){switch(a){case "loadVideoById":return b=ai(b),[b];case "cueVideoById":return b=ai(b),[b];case "loadVideoByPlayerVars":return[b];case "cueVideoByPlayerVars":return[b];case "loadPlaylist":return b=bi(b),[b];case "cuePlaylist":return b=bi(b),[b];case "seekTo":return[b.seconds,b.allowSeekAhead];case "playVideoAt":return[b.index];case "setVolume":return[b.volume];case "setPlaybackQuality":return[b.suggestedQuality];case "setPlaybackRate":return[b.suggestedRate];case "setLoop":return[b.loopPlaylists];
case "setShuffle":return[b.shufflePlaylist];case "getOptions":return[b.module];case "getOption":return[b.module,b.option];case "setOption":return[b.module,b.option,b.value];case "handleGlobalKeyDown":return[b.keyCode,b.shiftKey,b.ctrlKey,b.altKey,b.metaKey]}return[]}
function gi(a,b){switch(a){case "isMuted":return{muted:b};case "getVolume":return{volume:b};case "getPlaybackRate":return{playbackRate:b};case "getAvailablePlaybackRates":return{availablePlaybackRates:b};case "getVideoLoadedFraction":return{videoLoadedFraction:b};case "getPlayerState":return{playerState:b};case "getCurrentTime":return{currentTime:b};case "getPlaybackQuality":return{playbackQuality:b};case "getAvailableQualityLevels":return{availableQualityLevels:b};case "getDuration":return{duration:b};
case "getVideoUrl":return{videoUrl:b};case "getVideoEmbedCode":return{videoEmbedCode:b};case "getPlaylist":return{playlist:b};case "getPlaylistIndex":return{playlistIndex:b};case "getOptions":return{options:b};case "getOption":return{option:b}}}
hi.prototype.Z=function(a,b){switch(a){case "onReady":return;case "onStateChange":return{playerState:b};case "onPlaybackQualityChange":return{playbackQuality:b};case "onPlaybackRateChange":return{playbackRate:b};case "onError":return{errorCode:b}}return hi.A.Z.call(this,a,b)};
hi.prototype.l=function(){hi.A.l.call(this);delete this.h};function ii(a,b,c,d){L.call(this);this.g=b||null;this.w="*";this.h=c||null;this.sessionId=null;this.channel=d||null;this.D=!!a;this.m=A(this.B,this);window.addEventListener("message",this.m)}
n(ii,L);ii.prototype.B=function(a){if(!("*"!=this.h&&a.origin!=this.h||this.g&&a.source!=this.g)&&t(a.data)){try{var b=JSON.parse(a.data)}catch(c){return}if(!(null==b||this.D&&(this.sessionId&&this.sessionId!=b.id||this.channel&&this.channel!=b.channel))&&b)switch(b.event){case "listening":"null"!=a.origin&&(this.h=this.w=a.origin);this.g=a.source;this.sessionId=b.id;this.b&&(this.b(),this.b=null);break;case "command":this.i&&(!this.j||0<=Ha(this.j,b.func))&&this.i(b.func,b.args,a.origin)}}};
ii.prototype.sendMessage=function(a,b){var c=b||this.g;if(c){this.sessionId&&(a.id=this.sessionId);this.channel&&(a.channel=this.channel);try{var d=td(a);c.postMessage(d,this.w)}catch(e){R(e,"WARNING")}}};
ii.prototype.l=function(){window.removeEventListener("message",this.m);L.prototype.l.call(this)};function ji(a,b,c){ii.call(this,a,b,c||Q("POST_MESSAGE_ORIGIN",void 0)||window.document.location.protocol+"//"+window.document.location.hostname,"widget");this.j=this.b=this.i=null}
n(ji,ii);function ki(){var a=this.f=new ji(!!Q("WIDGET_ID_ENFORCE")),b=A(this.La,this);a.i=b;a.j=null;this.f.channel="widget";if(a=Q("WIDGET_ID"))this.f.sessionId=a;this.h=[];this.j=!1;this.i={}}
k=ki.prototype;k.La=function(a,b,c){"addEventListener"==a&&b?(a=b[0],this.i[a]||"onReady"==a||(this.addEventListener(a,li(this,a)),this.i[a]=!0)):this.la(a,b,c)};
k.la=function(){};
function li(a,b){return A(function(a){this.sendMessage(b,a)},a)}
k.addEventListener=function(){};
k.sa=function(){this.j=!0;this.sendMessage("initialDelivery",this.aa());this.sendMessage("onReady");F(this.h,this.ka,this);this.h=[]};
k.aa=function(){return null};
function mi(a,b){a.sendMessage("infoDelivery",b)}
k.ka=function(a){this.j?this.f.sendMessage(a):this.h.push(a)};
k.sendMessage=function(a,b){this.ka({event:a,info:void 0==b?null:b})};
k.dispose=function(){this.f=null};function ni(a){ki.call(this);this.b=a;this.g=[];this.addEventListener("onReady",A(this.Ja,this));this.addEventListener("onVideoProgress",A(this.Sa,this));this.addEventListener("onVolumeChange",A(this.Ta,this));this.addEventListener("onApiChange",A(this.Na,this));this.addEventListener("onPlaybackQualityChange",A(this.Pa,this));this.addEventListener("onPlaybackRateChange",A(this.Qa,this));this.addEventListener("onStateChange",A(this.Ra,this));this.addEventListener("onWebglSettingsChanged",A(this.Ua,
this))}
C(ni,ki);k=ni.prototype;k.la=function(a,b,c){if(this.b.isExternalMethodAvailable(a,c)){b=b||[];if(0<b.length&&Zh(a)){var d=b;if(z(d[0])&&!x(d[0]))d=d[0];else{var e={};switch(a){case "loadVideoById":case "cueVideoById":e=ai.apply(window,d);break;case "loadVideoByUrl":case "cueVideoByUrl":e=$h.apply(window,d);break;case "loadPlaylist":case "cuePlaylist":e=bi.apply(window,d)}d=e}b.length=1;b[0]=d}this.b.handleExternalCall(a,b,c);Zh(a)&&mi(this,this.aa())}};
k.Ja=function(){var a=A(this.sa,this);this.f.b=a};
k.addEventListener=function(a,b){this.g.push({eventType:a,listener:b});this.b.addEventListener(a,b)};
k.aa=function(){if(!this.b)return null;var a=this.b.getApiInterface();Ma(a,"getVideoData");for(var b={apiInterface:a},c=0,d=a.length;c<d;c++){var e=a[c],f=e;if(0==f.search("get")||0==f.search("is")){f=e;var g=0;0==f.search("get")?g=3:0==f.search("is")&&(g=2);f=f.charAt(g).toLowerCase()+f.substr(g+1);try{var h=this.b[e]();b[f]=h}catch(m){}}}b.videoData=this.b.getVideoData();b.currentTimeLastUpdated_=B()/1E3;return b};
k.Ra=function(a){a={playerState:a,currentTime:this.b.getCurrentTime(),duration:this.b.getDuration(),videoData:this.b.getVideoData(),videoStartBytes:0,videoBytesTotal:this.b.getVideoBytesTotal(),videoLoadedFraction:this.b.getVideoLoadedFraction(),playbackQuality:this.b.getPlaybackQuality(),availableQualityLevels:this.b.getAvailableQualityLevels(),currentTimeLastUpdated_:B()/1E3,playbackRate:this.b.getPlaybackRate(),mediaReferenceTime:this.b.getMediaReferenceTime()};this.b.getVideoUrl&&(a.videoUrl=
this.b.getVideoUrl());this.b.getVideoContentRect&&(a.videoContentRect=this.b.getVideoContentRect());this.b.getProgressState&&(a.progressState=this.b.getProgressState());this.b.getPlaylist&&(a.playlist=this.b.getPlaylist());this.b.getPlaylistIndex&&(a.playlistIndex=this.b.getPlaylistIndex());this.b.getStoryboardFormat&&(a.storyboardFormat=this.b.getStoryboardFormat());mi(this,a)};
k.Pa=function(a){mi(this,{playbackQuality:a})};
k.Qa=function(a){mi(this,{playbackRate:a})};
k.Na=function(){for(var a=this.b.getOptions(),b={namespaces:a},c=0,d=a.length;c<d;c++){var e=a[c],f=this.b.getOptions(e);b[e]={options:f};for(var g=0,h=f.length;g<h;g++){var m=f[g],l=this.b.getOption(e,m);b[e][m]=l}}this.sendMessage("apiInfoDelivery",b)};
k.Ta=function(){mi(this,{muted:this.b.isMuted(),volume:this.b.getVolume()})};
k.Sa=function(a){a={currentTime:a,videoBytesLoaded:this.b.getVideoBytesLoaded(),videoLoadedFraction:this.b.getVideoLoadedFraction(),currentTimeLastUpdated_:B()/1E3,playbackRate:this.b.getPlaybackRate(),mediaReferenceTime:this.b.getMediaReferenceTime()};this.b.getProgressState&&(a.progressState=this.b.getProgressState());mi(this,a)};
k.Ua=function(){var a={sphericalProperties:this.b.getSphericalProperties()};mi(this,a)};
k.dispose=function(){ni.A.dispose.call(this);for(var a=0;a<this.g.length;a++){var b=this.g[a];this.b.removeEventListener(b.eventType,b.listener)}this.g=[]};function oi(a){a=void 0===a?!1:a;L.call(this);this.b=new N(a);ed(this,Ea(fd,this.b))}
C(oi,L);oi.prototype.subscribe=function(a,b,c){return this.f?0:this.b.subscribe(a,b,c)};
oi.prototype.i=function(a,b){this.f||this.b.K.apply(this.b,arguments)};function pi(a,b,c){oi.call(this);this.g=a;this.h=b;this.j=c}
C(pi,oi);function di(a,b,c){if(!a.f){var d=a.g;d.f||a.h!=d.b||(a={id:a.j,command:b},c&&(a.data=c),d.b.postMessage(td(a),d.h))}}
pi.prototype.l=function(){this.h=this.g=null;pi.A.l.call(this)};function qi(a,b,c){L.call(this);this.b=a;this.h=c;this.i=V(window,"message",A(this.j,this));this.g=new pi(this,a,b);ed(this,Ea(fd,this.g))}
C(qi,L);qi.prototype.j=function(a){var b;if(b=!this.f)if(b=a.origin==this.h)a:{b=this.b;do{b:{var c=a.source;do{if(c==b){c=!0;break b}if(c==c.parent)break;c=c.parent}while(null!=c);c=!1}if(c){b=!0;break a}b=b.opener}while(null!=b);b=!1}if(b&&(b=a.data,t(b))){try{b=JSON.parse(b)}catch(d){return}b.command&&(c=this.g,c.f||c.i("command",b.command,b.data,a.origin))}};
qi.prototype.l=function(){ag(this.i);this.b=null;qi.A.l.call(this)};function ri(){var a=jb(si),b;return Hd(new M(function(c,d){a.onSuccess=function(a){De(a)?c(a):d(new ti("Request failed, status="+a.status,"net.badstatus",a))};
a.onError=function(a){d(new ti("Unknown request error","net.unknown",a))};
a.M=function(a){d(new ti("Request timed out","net.timeout",a))};
b=Ne("//googleads.g.doubleclick.net/pagead/id",a)}),function(a){a instanceof Id&&b.abort();
return Fd(a)})}
function ti(a,b){E.call(this,a+", errorCode="+b);this.errorCode=b;this.name="PromiseAjaxError"}
n(ti,E);function ui(){this.f=0;this.b=null}
ui.prototype.then=function(a,b,c){return 1===this.f&&a?(a=a.call(c,this.b),zd(a)?a:vi(a)):2===this.f&&b?(a=b.call(c,this.b),zd(a)?a:wi(a)):this};
ui.prototype.getValue=function(){return this.b};
ui.prototype.$goog_Thenable=!0;function wi(a){var b=new ui;a=void 0===a?null:a;b.f=2;b.b=void 0===a?null:a;return b}
function vi(a){var b=new ui;a=void 0===a?null:a;b.f=1;b.b=void 0===a?null:a;return b}
;function xi(a){E.call(this,a.message||a.description||a.name);this.isMissing=a instanceof yi;this.isTimeout=a instanceof ti&&"net.timeout"==a.errorCode;this.isCanceled=a instanceof Id}
n(xi,E);xi.prototype.name="BiscottiError";function yi(){E.call(this,"Biscotti ID is missing from server")}
n(yi,E);yi.prototype.name="BiscottiMissingError";var si={format:"RAW",method:"GET",timeout:5E3,withCredentials:!0},zi=null;function pe(){if("1"===db(ne(),"args","privembed"))return Fd(Error("Biscotti ID is not available in private embed mode"));zi||(zi=Hd(ri().then(Ai),function(a){return Bi(2,a)}));
return zi}
function Ai(a){a=a.responseText;if(0!=a.lastIndexOf(")]}'",0))throw new yi;a=JSON.parse(a.substr(4));if(1<(a.type||1))throw new yi;a=a.id;qe(a);zi=vi(a);Ci(18E5,2);return a}
function Bi(a,b){var c=new xi(b);qe("");zi=wi(c);0<a&&Ci(12E4,a-1);throw c;}
function Ci(a,b){T(function(){Hd(ri().then(Ai,function(a){return Bi(b,a)}),wa)},a)}
function Di(){try{var a=w("yt.ads.biscotti.getId_");return a?a():pe()}catch(b){return Fd(b)}}
;function Ei(a){if("1"!==db(ne(),"args","privembed")){a&&oe();try{Di().then(function(a){a=ve(a);a.bsq=Fi++;Te("//www.youtube.com/ad_data_204",{wa:!1,C:a,withCredentials:!0})},function(){}),T(Ei,18E5)}catch(b){R(b)}}}
var Fi=0;var Z=w("ytglobal.prefsUserPrefsPrefs_")||{};v("ytglobal.prefsUserPrefsPrefs_",Z,void 0);function Gi(){this.b=Q("ALT_PREF_COOKIE_NAME","PREF");var a=Eb.get(""+this.b,void 0);if(a){a=decodeURIComponent(a).split("&");for(var b=0;b<a.length;b++){var c=a[b].split("="),d=c[0];(c=c[1])&&(Z[d]=c.toString())}}}
k=Gi.prototype;k.get=function(a,b){Hi(a);Ii(a);var c=void 0!==Z[a]?Z[a].toString():null;return null!=c?c:b?b:""};
k.set=function(a,b){Hi(a);Ii(a);if(null==b)throw Error("ExpectedNotNull");Z[a]=b.toString()};
k.remove=function(a){Hi(a);Ii(a);delete Z[a]};
k.save=function(){Bg(this.b,this.dump(),63072E3)};
k.clear=function(){for(var a in Z)delete Z[a]};
k.dump=function(){var a=[],b;for(b in Z)a.push(b+"="+encodeURIComponent(String(Z[b])));return a.join("&")};
function Ii(a){if(/^f([1-9][0-9]*)$/.test(a))throw Error("ExpectedRegexMatch: "+a);}
function Hi(a){if(!/^\w+$/.test(a))throw Error("ExpectedRegexMismatch: "+a);}
function Ji(a){a=void 0!==Z[a]?Z[a].toString():null;return null!=a&&/^[A-Fa-f0-9]+$/.test(a)?parseInt(a,16):null}
xa(Gi);var Ki=null,Li=null,Mi=null,Ni={};function Oi(a){var b=a.id;a=a.ve_type;var c=Ng++;a=new Lg({veType:a,veCounter:c,elementIndex:void 0,dataElement:void 0,youtubeData:void 0});Ni[b]=a;b=Sg();c=Rg();b&&c&&Vg(b,c,[a])}
function Pi(a){var b=a.csn;a=a.root_ve_type;if(b&&a&&(Tg(b,a),a=Rg()))for(var c in Ni){var d=Ni[c];d&&Vg(b,a,[d])}}
function Qi(a){Ni[a.id]=new Lg({trackingParams:a.tracking_params})}
function Ri(a){var b=Sg();a=Ni[a.id];b&&a&&yg("visualElementGestured",{csn:b,ve:Mg(a),gestureType:"INTERACTION_LOGGING_GESTURE_TYPE_GENERIC_CLICK"},Hg,void 0,void 0)}
function Si(a){a=a.ids;var b=Sg();if(b)for(var c=0;c<a.length;c++){var d=Ni[a[c]];d&&yg("visualElementShown",{csn:b,ve:Mg(d),eventType:1},Hg,void 0,void 0)}}
;v("yt.setConfig",P,void 0);v("yt.config.set",P,void 0);v("yt.setMsg",cf,void 0);v("yt.msgs.set",cf,void 0);v("yt.logging.errors.log",af,void 0);
v("writeEmbed",function(){var a=Q("PLAYER_CONFIG",void 0);Ei(!0);"gvn"==a.args.ps&&(document.body.style.backgroundColor="transparent");var b=document.referrer,c=Q("POST_MESSAGE_ORIGIN");window!=window.top&&b&&b!=document.URL&&(a.args.loaderUrl=b);Q("LIGHTWEIGHT_AUTOPLAY")&&(a.args.autoplay="1");Ki=a=rh(a);a.addEventListener("onScreenChanged",Pi);a.addEventListener("onLogClientVeCreated",Oi);a.addEventListener("onLogServerVeCreated",Qi);a.addEventListener("onLogVeClicked",Ri);a.addEventListener("onLogVesShown",
Si);b=Q("POST_MESSAGE_ID","player");Q("ENABLE_JS_API")?Mi=new ni(a):Q("ENABLE_POST_API")&&t(b)&&t(c)&&(Li=new qi(window.parent,b,c),Mi=new hi(a,Li.g));c=me("BG_P");Rf(c)&&(Q("BG_I")||Q("BG_IU"))&&(Nf=!0,Mf.initialize(Q("BG_I",null),Q("BG_IU",null),c,Qf,void 0));If()},void 0);
v("yt.www.watch.ads.restrictioncookie.spr",function(a){We(a+"mac_204?action_fcts=1");return!0},void 0);
var Ti=re(function(){var a="ol";X.mark&&(0==a.lastIndexOf("mark_",0)||(a="mark_"+a),X.mark(a));a=Vh();var b=O();a.ol&&(a._ol=a._ol||[a.ol],a._ol.push(b));a.ol=b;a=Ch();if(b=a.ol)Ff(b),a.ol=0;Rh().tick_ol=void 0;O();Sh()?(a=Th(),wh("tick_ol_"+a)||th("latencyActionTicked",{tickName:"ol",clientActionNonce:a},void 0),a=!0):a=!1;if(a=!a)a=!w("yt.timing.pingSent_");if(a&&(b=Q("TIMING_ACTION",void 0),a=Vh(),w("ytglobal.timingready_")&&b&&a._start&&(b=Uh()))){Oh||(Bh(Gh,new Eh(Math.round(b-a._start),void 0)),
Oh=!0);b=!0;var c=Q("TIMING_WAIT",[]);if(c.length)for(var d=0,e=c.length;d<e;++d)if(!(c[d]in a)){b=!1;break}b&&Xh()}a=Gi.getInstance();c=!!((Ji("f"+(Math.floor(119/31)+1))||0)&67108864);b=1<window.devicePixelRatio;document.body&&id(document.body,"exp-invert-logo")&&(b&&!id(document.body,"inverted-hdpi")?(d=document.body,d.classList?d.classList.add("inverted-hdpi"):id(d,"inverted-hdpi")||(d.className+=0<d.className.length?" inverted-hdpi":"inverted-hdpi")):!b&&id(document.body,"inverted-hdpi")&&jd());
c!=b&&(c="f"+(Math.floor(119/31)+1),d=Ji(c)||0,d=b?d|67108864:d&-67108865,0==d?delete Z[c]:(b=d.toString(16),Z[c]=b.toString()),a.save())}),Ui=re(function(){var a=Ki;
a&&a.sendAbandonmentPing&&a.sendAbandonmentPing();Q("PL_ATT")&&Mf.dispose();a=0;for(var b=Gf.length;a<b;a++)Ff(Gf[a]);Gf.length=0;zf("//static.doubleclick.net/instream/ad_status.js");Hf=!1;P("DCLKSTAT",0);gd(Mi,Li);if(a=Ki)a.removeEventListener("onScreenChanged",Pi),a.removeEventListener("onLogClientVeCreated",Oi),a.removeEventListener("onLogServerVeCreated",Qi),a.removeEventListener("onLogVeClicked",Ri),a.removeEventListener("onLogVesShown",Si),a.destroy();Ni={}});
window.addEventListener?(window.addEventListener("load",Ti),window.addEventListener("unload",Ui)):window.attachEvent&&(window.attachEvent("onload",Ti),window.attachEvent("onunload",Ui));Fa("yt.abuse.player.botguardInitialized",w("yt.abuse.player.botguardInitialized")||Sf);Fa("yt.abuse.player.invokeBotguard",w("yt.abuse.player.invokeBotguard")||Tf);Fa("yt.abuse.dclkstatus.checkDclkStatus",w("yt.abuse.dclkstatus.checkDclkStatus")||Jf);
Fa("yt.player.exports.navigate",w("yt.player.exports.navigate")||Ug);Fa("yt.util.activity.init",w("yt.util.activity.init")||dg);Fa("yt.util.activity.getTimeSinceActive",w("yt.util.activity.getTimeSinceActive")||gg);Fa("yt.util.activity.setTimestamp",w("yt.util.activity.setTimestamp")||eg);}).call(this);
