è6
aD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application\Clients\QueryCustomerDataClient.cs
	namespace 	
SendSPFT
 
. 
Application 
. 
Clients &
{ 
public 

class #
QueryCustomerDataClient (
(( )
ILogger) 0
<0 1#
QueryCustomerDataClient1 H
>H I
serilogJ Q
,Q R
IConfigurationS a
configurationb o
,o p

HttpClientq {

httpClient	| Ü
)
Ü á
:
à â&
IQueryCustomerDataClient
ä ¢
{ 
private 
static 
readonly !
JsonSerializerOptions  5
_jsonOptions6 B
=C D
new !
JsonSerializerOptions &
{' ('
PropertyNameCaseInsensitive) D
=E F
trueG K
}L M
;M N
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
?A B
>B C 
_logResponseReceivedD X
=Y Z
LoggerMessage 
. 
Define 
< 
string "
>" #
(# $
LogLevel 
. 
Information 
,  
new 
EventId 
( 
$num 
, 
nameof #
(# $ 
_logResponseReceived$ 8
)8 9
)9 :
,: ;
$str .
). /
;/ 0
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
?A B
>B C
_logHttpErrorD Q
=R S
LoggerMessage   
.   
Define    
<    !
string  ! '
>  ' (
(  ( )
LogLevel!! 
.!! 
Error!! 
,!! 
new"" 
EventId"" 
("" 
$num""  
,""  !
nameof""" (
(""( )
_logHttpError"") 6
)""6 7
)""7 8
,""8 9
$str## R
)##R S
;##S T
private%% 
static%% 
readonly%% 
Action%%  &
<%%& '
ILogger%%' .
,%%. /
string%%0 6
,%%6 7
	Exception%%8 A
?%%A B
>%%B C
_logUnexpectedError%%D W
=%%X Y
LoggerMessage&& 
.&& 
Define&&  
<&&  !
string&&! '
>&&' (
(&&( )
LogLevel'' 
.'' 
Error'' 
,'' 
new(( 
EventId(( 
((( 
$num((  
,((  !
nameof((" (
(((( )
_logUnexpectedError(() <
)((< =
)((= >
,((> ?
$str)) O
)))O P
;))P Q
public,, 
async,, 
Task,, 
<,, 
Cliente,, !
>,,! "
GetDataClient,,# 0
(,,0 1
TdAttachmentDto,,1 @
dto,,A D
),,D E
{-- 	
try00 
{11 
var33 
	requestWS33 
=33 
new33  #
ReqConsulta33$ /
{44 
nroIdentificacion55 %
=55& '
dto55( +
.55+ ,
TdCedula55, 4
,554 5
tipoIdentificacion66 &
=66' (
dto66) ,
.66, -
TdTipoIdent66- 8
,668 9

conFormato77 
=77  
configuration77! .
.77. /

GetSection77/ 9
(779 :
$str77: T
)77T U
.77U V
Value77V [
}88 
;88 
string:: 
jsonPayload:: "
=::# $
JsonSerializer::% 3
.::3 4
	Serialize::4 =
(::= >
	requestWS::> G
)::G H
;::H I
string== 
url== 
=== 
configuration== *
[==* +
$str==+ E
]==E F
??==G I
string==J P
.==P Q
Empty==Q V
;==V W
var@@ 
requestMessage@@ "
=@@# $
new@@% (
HttpRequestMessage@@) ;
(@@; <

HttpMethod@@< F
.@@F G
Post@@G K
,@@K L
url@@M P
)@@P Q
{AA 
ContentBB 
=BB 
newBB !
StringContentBB" /
(BB/ 0
jsonPayloadBB0 ;
,BB; <
EncodingBB= E
.BBE F
UTF8BBF J
,BBJ K
$strBBL ^
)BB^ _
}CC 
;CC 
HttpResponseMessageFF #
responseFF$ ,
=FF- .
awaitFF/ 4

httpClientFF5 ?
.FF? @
	SendAsyncFF@ I
(FFI J
requestMessageFFJ X
)FFX Y
;FFY Z
responseGG 
.GG #
EnsureSuccessStatusCodeGG 0
(GG0 1
)GG1 2
;GG2 3
stringLL 
responseContentLL &
=LL' (
awaitLL) .
responseLL/ 7
.LL7 8
ContentLL8 ?
.LL? @
ReadAsStringAsyncLL@ Q
(LLQ R
)LLR S
;LLS T 
_logResponseReceivedMM $
(MM$ %
serilogMM% ,
,MM, -
responseContentMM. =
,MM= >
nullMM? C
)MMC D
;MMD E
varQQ 
clientesQQ 
=QQ 
JsonSerializerQQ -
.QQ- .
DeserializeQQ. 9
<QQ9 :
ListQQ: >
<QQ> ?
ClienteQQ? F
>QQF G
>QQG H
(QQH I
responseContentQQI X
,QQX Y
_jsonOptionsQQZ f
)QQf g
;QQg h
returnSS 
clientesSS 
?SS  
.SS  !
FirstOrDefaultSS! /
(SS/ 0
)SS0 1
??SS2 4
newSS5 8
ClienteSS9 @
(SS@ A
)SSA B
;SSB C
}UU 
catchVV 
(VV  
HttpRequestExceptionVV '
httpExVV( .
)VV. /
{WW 
_logHttpErrorXX 
(XX 
serilogXX %
,XX% &
httpExXX' -
.XX- .
MessageXX. 5
,XX5 6
nullXX7 ;
)XX; <
;XX< =
throwYY 
;YY 
}ZZ 
catch[[ 
([[ 
	Exception[[ 
ex[[ 
)[[  
{\\ 
_logHttpError]] 
(]] 
serilog]] %
,]]% &
ex]]' )
.]]) *
Message]]* 1
,]]1 2
null]]3 7
)]]7 8
;]]8 9
throw^^ 
;^^ 
}__ 
}aa 	
}ff 
}gg ˝Y
`D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application\Clients\SftpFileUploaderClient.cs
	namespace 	
SendSPFT
 
. 
Application 
. 
Clients &
{ 
public 

class "
SftpFileUploaderClient '
:( )
ISftpFileUploader* ;
{ 
private 
readonly 
ILogger  
<  !"
SftpFileUploaderClient! 7
>7 8
_logger9 @
;@ A
private 
readonly 
SftpSettings %
	_settings& /
;/ 0
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
?A B
>B C
_logSuccessD O
=P Q
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel) 1
.1 2
Information2 =
,= >
new? B
EventIdC J
(J K
$numK O
,O P
nameofQ W
(W X
_logSuccessX c
)c d
)d e
,e f
$str F
)F G
;G H
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
	Exception0 9
?9 :
>: ;
	_logError< E
=F G
LoggerMessage 
. 
Define  
(  !
LogLevel! )
.) *
Error* /
,/ 0
new1 4
EventId5 <
(< =
$num= A
,A B
nameofC I
(I J
	_logErrorJ S
)S T
)T U
,U V
$strW u
)u v
;v w
public "
SftpFileUploaderClient %
(% &
ILogger& -
<- ."
SftpFileUploaderClient. D
>D E
loggerF L
,L M
IConfigurationN \
configuration] j
)j k
{ 	
_logger 
= 
logger 
?? 
throw  %
new& )!
ArgumentNullException* ?
(? @
nameof@ F
(F G
loggerG M
)M N
)N O
;O P
	_settings   
=   
configuration   %
.  % &

GetSection  & 0
(  0 1
$str  1 ?
)  ? @
.  @ A
Get  A D
<  D E
SftpSettings  E Q
>  Q R
(  R S
)  S T
??!! 
throw!!  
new!!! $
ArgumentException!!% 6
(!!6 7
$str!!7 a
)!!a b
;!!b c
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
SftpUploadResult$$ *
>$$* +
UploadFileAsync$$, ;
($$; <
string$$< B
format$$C I
,$$I J
string$$K Q
base64Content$$R _
,$$_ `
string$$a g

reportName$$h r
)$$r s
{%% 	
Validate&& 
(&& 
format&& 
,&& 
base64Content&& *
)&&* +
;&&+ ,
return'' 
await'' #
UploadFileInternalAsync'' 0
(''0 1
format''1 7
,''7 8
base64Content''9 F
,''F G

reportName''H R
)''R S
;''S T
}(( 	
public** 
async** 
Task** 
<** 
SftpUploadResult** *
>*** +#
UploadFileInternalAsync**, C
(**C D
string**D J
format**K Q
,**Q R
string**S Y
base64Content**Z g
,**g h
string**i o

reportName**p z
)**z {
{++ 	
var,, 
result,, 
=,, 
new,, 
SftpUploadResult,, -
(,,- .
),,. /
;,,/ 0
var-- 
tempFilePath-- 
=-- 
string-- %
.--% &
Empty--& +
;--+ ,
try// 
{00 
var11 
	fileBytes11 
=11 
Convert11  '
.11' (
FromBase64String11( 8
(118 9
base64Content119 F
)11F G
;11G H
format44 
=44 
format44 
.44  
ToLower44  '
(44' (
CultureInfo44( 3
.443 4
InvariantCulture444 D
)44D E
;44E F
tempFilePath77 
=77 
Path77 #
.77# $
Combine77$ +
(77+ ,
Path77, 0
.770 1
GetTempPath771 <
(77< =
)77= >
,77> ?
$"77@ B
{77B C
Guid77C G
.77G H
NewGuid77H O
(77O P
)77P Q
}77Q R
$str77R S
{77S T

reportName77T ^
}77^ _
"77_ `
)77` a
;77a b
await88 
File88 
.88 
WriteAllBytesAsync88 -
(88- .
tempFilePath88. :
,88: ;
	fileBytes88< E
)88E F
;88F G
using:: 
var:: 

sftpClient:: $
=::% &
new::' *

SftpClient::+ 5
(::5 6
	_settings::6 ?
.::? @
Host::@ D
,::D E
	_settings::F O
.::O P
Port::P T
,::T U
	_settings::V _
.::_ `
Username::` h
,::h i
	_settings::j s
.::s t
Password::t |
)::| }
;::} ~

sftpClient;; 
.;; 
Connect;; "
(;;" #
);;# $
;;;$ %
var== 
remoteFilePath== "
===# $
Path==% )
.==) *
Combine==* 1
(==1 2
	_settings==2 ;
.==; <

RemotePath==< F
,==F G
Path==H L
.==L M
GetFileName==M X
(==X Y
tempFilePath==Y e
)==e f
)==f g
;==g h
await?? 
Task?? 
.?? 
Run?? 
(?? 
(??  
)??  !
=>??" $

sftpClient??% /
.??/ 0

UploadFile??0 :
(??: ;
File??; ?
.??? @
OpenRead??@ H
(??H I
tempFilePath??I U
)??U V
,??V W
remoteFilePath??X f
)??f g
)??g h
;??h i
resultBB 
.BB 
SuccessBB 
=BB  
awaitBB! &
TaskBB' +
.BB+ ,
RunBB, /
(BB/ 0
(BB0 1
)BB1 2
=>BB3 5

sftpClientBB6 @
.BB@ A
ExistsBBA G
(BBG H
remoteFilePathBBH V
)BBV W
)BBW X
;BBX Y
resultCC 
.CC 
RemoteFilePathCC %
=CC& '
remoteFilePathCC( 6
;CC6 7

sftpClientEE 
.EE 

DisconnectEE %
(EE% &
)EE& '
;EE' (
_logSuccessGG 
(GG 
_loggerGG #
,GG# $
remoteFilePathGG% 3
,GG3 4
nullGG5 9
)GG9 :
;GG: ;
}HH 
catchII 
(II 
	ExceptionII 
exII 
)II  
{JJ 
	_logErrorKK 
(KK 
_loggerKK !
,KK! "
exKK# %
)KK% &
;KK& '
resultLL 
.LL 
SuccessLL 
=LL  
falseLL! &
;LL& '
resultMM 
.MM 
ErrorMessageMM #
=MM$ %
exMM& (
.MM( )
MessageMM) 0
;MM0 1
tryOO 
{PP 
stringQQ 
errorFileNameQQ (
=QQ) *
$"QQ+ -
$strQQ- 3
{QQ3 4
GuidQQ4 8
.QQ8 9
NewGuidQQ9 @
(QQ@ A
)QQA B
}QQB C
$strQQC G
"QQG H
;QQH I
stringRR 
errorFilePathRR (
=RR) *
PathRR+ /
.RR/ 0
CombineRR0 7
(RR7 8
PathRR8 <
.RR< =
GetTempPathRR= H
(RRH I
)RRI J
,RRJ K
errorFileNameRRL Y
)RRY Z
;RRZ [
awaitSS 
FileSS 
.SS 
WriteAllTextAsyncSS 0
(SS0 1
errorFilePathSS1 >
,SS> ?
$"SS@ B
$strSSB R
{SSR S
formatSSS Y
}SSY Z
$strSSZ b
{SSb c
exSSc e
}SSe f
"SSf g
)SSg h
;SSh i
usingUU 
varUU 

sftpClientUU (
=UU) *
newUU+ .

SftpClientUU/ 9
(UU9 :
	_settingsUU: C
.UUC D
HostUUD H
,UUH I
	_settingsUUJ S
.UUS T
PortUUT X
,UUX Y
	_settingsUUZ c
.UUc d
UsernameUUd l
,UUl m
	_settingsUUn w
.UUw x
Password	UUx Ä
)
UUÄ Å
;
UUÅ Ç

sftpClientVV 
.VV 
ConnectVV &
(VV& '
)VV' (
;VV( )
stringXX 
errorRemotePathXX *
=XX+ ,
PathXX- 1
.XX1 2
CombineXX2 9
(XX9 :
	_settingsXX: C
.XXC D
	ErrorPathXXD M
,XXM N
errorFileNameXXO \
)XX\ ]
;XX] ^
awaitYY 
TaskYY 
.YY 
RunYY "
(YY" #
(YY# $
)YY$ %
=>YY& (

sftpClientYY) 3
.YY3 4

UploadFileYY4 >
(YY> ?
FileYY? C
.YYC D
OpenReadYYD L
(YYL M
errorFilePathYYM Z
)YYZ [
,YY[ \
errorRemotePathYY] l
)YYl m
)YYm n
;YYn o

sftpClient[[ 
.[[ 

Disconnect[[ )
([[) *
)[[* +
;[[+ ,
}\\ 
catch]] 
(]] 
	Exception]]  
innerEx]]! (
)]]( )
{^^ 
	_logError__ 
(__ 
_logger__ %
,__% &
innerEx__' .
)__. /
;__/ 0
}`` 
}aa 
returncc 
resultcc 
;cc 
}dd 	
publicff 
staticff 
voidff 
Validateff #
(ff# $
stringff$ *
formatff+ 1
,ff1 2
stringff3 9
base64Contentff: G
)ffG H
{gg 	
ifhh 
(hh 
stringhh 
.hh 
IsNullOrWhiteSpacehh )
(hh) *
formathh* 0
)hh0 1
)hh1 2
throwii 
newii 
ArgumentExceptionii +
(ii+ ,
$strii, D
,iiD E
nameofiiF L
(iiL M
formatiiM S
)iiS T
)iiT U
;iiU V
ifkk 
(kk 
stringkk 
.kk 
IsNullOrWhiteSpacekk )
(kk) *
base64Contentkk* 7
)kk7 8
)kk8 9
throwll 
newll 
ArgumentExceptionll +
(ll+ ,
$strll, L
,llL M
nameofllN T
(llT U
base64ContentllU b
)llb c
)llc d
;lld e
}mm 	
}nn 
}oo û!
ZD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application\Services\SendSftpService.cs
	namespace 	
SendSPFT
 
. 
Application 
. 
Services '
{ 
public 

sealed 
class 
SendSftpService '
(' (
ILogger 
< 
SendSftpService 
>  
logger! '
,' (
ISelectRegistryBD 
selectRecord &
,& '
IProcessRecord   
processRecord   $
)  $ %
:  & '
ISendSftpService  ( 8
{!! 
private$$ 
static$$ 
readonly$$ 
Action$$  &
<$$& '
ILogger$$' .
,$$. /
string$$0 6
,$$6 7
	Exception$$8 A
?$$A B
>$$B C
_logInfo$$D L
=$$M N
LoggerMessage%% 
.%% 
Define%%  
<%%  !
string%%! '
>%%' (
(%%( )
LogLevel%%) 1
.%%1 2
Information%%2 =
,%%= >
new%%? B
EventId%%C J
(%%J K
$num%%K L
,%%L M
nameof%%N T
(%%T U
_logInfo%%U ]
)%%] ^
)%%^ _
,%%_ `
$str%%a l
)%%l m
;%%m n
private'' 
static'' 
readonly'' 
Action''  &
<''& '
ILogger''' .
,''. /
string''0 6
,''6 7
	Exception''8 A
?''A B
>''B C
_logWarning''D O
=''P Q
LoggerMessage(( 
.(( 
Define((  
<((  !
string((! '
>((' (
(((( )
LogLevel(() 1
.((1 2
Warning((2 9
,((9 :
new((; >
EventId((? F
(((F G
$num((G H
,((H I
nameof((J P
(((P Q
_logWarning((Q \
)((\ ]
)((] ^
,((^ _
$str((` k
)((k l
;((l m
private** 
static** 
readonly** 
Action**  &
<**& '
ILogger**' .
,**. /
	Exception**0 9
?**9 :
>**: ;
	_logError**< E
=**F G
LoggerMessage++ 
.++ 
Define++  
(++  !
LogLevel++! )
.++) *
Error++* /
,++/ 0
new++1 4
EventId++5 <
(++< =
$num++= >
,++> ?
nameof++@ F
(++F G
	_logError++G P
)++P Q
)++Q R
,++R S
$str++T s
)++s t
;++t u
public.. 
async.. 
Task.. 
ExecuteServiceAsync.. -
(..- .
CancellationToken... ?
stoppingToken..@ M
)..M N
{// 	
_logInfo00 
(00 
logger00 
,00 
$str00 :
,00: ;
null00< @
)00@ A
;00A B
try22 
{33 
var44 
Records44 
=44 
await44 #
selectRecord44$ 0
.440 1
SelectRecordAsync441 B
(44B C
)44C D
;44D E
if66 
(66 
Records66 
==66 
null66 #
||66$ &
Records66' .
.66. /
Count66/ 4
==665 7
$num668 9
)669 :
{77 
_logInfo88 
(88 
logger88 #
,88# $
$str88% O
,88O P
null88Q U
)88U V
;88V W
return99 
;99 
}:: 
foreach<< 
(<< 
var<< 
record<< #
in<<$ &
Records<<' .
)<<. /
{== 
await>> 
processRecord>> '
.>>' (
ProcessRecordAsync>>( :
(>>: ;
record>>; A
)>>A B
;>>B C
}?? 
}@@ 
catchAA 
(AA 
	ExceptionAA 
exAA 
)AA  
{BB 
	_logErrorCC 
(CC 
loggerCC  
,CC  !
exCC" $
)CC$ %
;CC% &
}DD 
}EE 	
}HH 
}JJ 