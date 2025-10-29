›'
QD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess\Context\BdSybase.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Context %
{ 
public 

class 
BdSybase 
: 
IDisposable '
,' (
	IBdSybase) 2
{ 
private 
readonly 
string 
_cadenaConexion  /
;/ 0
private 
readonly 
ILogger  
<  !
BdSybase! )
>) *
_log+ /
;/ 0
private 
bool 
	_disposed 
; 
private 
static 
readonly 
Action  &
<& '
ILogger' .
<. /
BdSybase/ 7
>7 8
,8 9
string: @
,@ A
	ExceptionB K
>K L
_logErrorDelegateM ^
=_ `
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel 
. 
Error 
, 
new 
EventId 
( 
$num  
,  !
nameof" (
(( )
IsHealthyAsync) 7
)7 8
)8 9
,9 :
$str Q
) 
; 
public   
BdSybase   
(   
IConfiguration   &
configuration  ' 4
,  4 5
ICrypto  6 =
crypto  > D
,  D E
ILogger  F M
<  M N
BdSybase  N V
>  V W
log  X [
)  [ \
{!! 	!
ArgumentNullException"" !
.""! "
ThrowIfNull""" -
(""- .
configuration"". ;
)""; <
;""< =!
ArgumentNullException## !
.##! "
ThrowIfNull##" -
(##- .
crypto##. 4
)##4 5
;##5 6!
ArgumentNullException$$ !
.$$! "
ThrowIfNull$$" -
($$- .
log$$. 1
)$$1 2
;$$2 3
_log&& 
=&& 
log&& 
;&& 
string)) 
cadenaEncriptada)) #
=))$ %
configuration))& 3
.))3 4

GetSection))4 >
())> ?
$str))? ^
)))^ _
.))_ `
Value))` e
??** 
throw** 
new** %
InvalidOperationException** 6
(**6 7
$str**7 f
)**f g
;**g h
_cadenaConexion,, 
=,, 
crypto,, $
.,,$ %
Decrypt,,% ,
(,,, -
cadenaEncriptada,,- =
),,= >
;,,> ?
}-- 	
public22 
void22 
Dispose22 
(22 
)22 
{33 	
Dispose44 
(44 
	disposing44 
:44 
true44 #
)44# $
;44$ %
GC55 
.55 
SuppressFinalize55 
(55  
this55  $
)55$ %
;55% &
}66 	
	protected88 
virtual88 
void88 
Dispose88 &
(88& '
bool88' +
	disposing88, 5
)885 6
{99 	
if:: 
(:: 
	_disposed:: 
):: 
return;; 
;;; 
	_disposed== 
=== 
true== 
;== 
}>> 	
publicBB 
asyncBB 
TaskBB 
<BB 
SybaseConnectionBB *
>BB* +!
CreateConnectionAsyncBB, A
(BBA B
)BBB C
{CC 	
varDD 
conexionDD 
=DD 
newDD 
SybaseConnectionDD /
(DD/ 0
_cadenaConexionDD0 ?
)DD? @
;DD@ A
awaitEE 
conexionEE 
.EE 
	OpenAsyncEE $
(EE$ %
)EE% &
;EE& '
returnFF 
conexionFF 
;FF 
}GG 	
publicII 
asyncII 
TaskII 
<II 
boolII 
>II 
IsHealthyAsyncII  .
(II. /
)II/ 0
{JJ 	
tryKK 
{LL 
awaitMM 
usingMM 
varMM 
conexionMM  (
=MM) *
awaitMM+ 0!
CreateConnectionAsyncMM1 F
(MMF G
)MMG H
;MMH I
returnNN 
conexionNN 
.NN  
StateNN  %
==NN& (
ConnectionStateNN) 8
.NN8 9
OpenNN9 =
;NN= >
}OO 
catchPP 
(PP 
SybaseExceptionPP "
exPP# %
)PP% &
{QQ 
_logErrorDelegateRR !
(RR! "
_logRR" &
,RR& '
exRR( *
.RR* +
MessageRR+ 2
,RR2 3
exRR4 6
)RR6 7
;RR7 8
returnSS 
falseSS 
;SS 
}TT 
}UU 	
}VV 
}WW °.
XD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess\Context\SendSftpContext.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Context %
{ 
public 

class 
SendSftpContext  
:! "
ISendSftpContext# 3
,3 4
IDisposable5 @
{ 
private 
readonly 
string 
_cadenaConexion  /
;/ 0
private 
NpgsqlConnection  
?  !
	_conexion" +
;+ ,
private 
readonly 
ILogger  
<  !
SendSftpContext! 0
>0 1
_log2 6
;6 7
private 
bool 
	_disposed 
; 
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
?A B
>B C
_logErrorConnectionD W
=X Y
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel 
. 
Error 
, 
new 
EventId 
( 
$num 
, 
nameof %
(% &
_logErrorConnection& 9
)9 :
): ;
,; <
$str G
) 
; 
public 
SendSftpContext 
( 
IConfiguration -
configuration. ;
,; <
ICrypto= D
cryptoE K
,K L
ILoggerM T
<T U
SendSftpContextU d
>d e
logf i
)i j
{ 	!
ArgumentNullException   !
.  ! "
ThrowIfNull  " -
(  - .
configuration  . ;
)  ; <
;  < =!
ArgumentNullException!! !
.!!! "
ThrowIfNull!!" -
(!!- .
crypto!!. 4
)!!4 5
;!!5 6!
ArgumentNullException"" !
.""! "
ThrowIfNull""" -
(""- .
log"". 1
)""1 2
;""2 3
_log$$ 
=$$ 
log$$ 
;$$ 
string&& 
cadenaEncriptada&& #
=&&$ %
configuration&&& 3
.&&3 4
GetConnectionString&&4 G
(&&G H
$str&&H \
)&&\ ]
??''& (
throw'') .
new''/ 2%
InvalidOperationException''3 L
(''L M
$str''M p
)''p q
;''q r
_cadenaConexion)) 
=)) 
crypto)) $
.))$ %
Decrypt))% ,
()), -
cadenaEncriptada))- =
)))= >
;))> ?
}** 	
public,, 
async,, 
Task,, 
<,, 
NpgsqlConnection,, *
>,,* +!
CreateConnectionAsync,,, A
(,,A B
),,B C
{-- 	
ThrowIfDisposed.. 
(.. 
).. 
;.. 
	_conexion00 
=00 
new00 
NpgsqlConnection00 ,
(00, -
_cadenaConexion00- <
)00< =
;00= >
await11 
	_conexion11 
.11 
	OpenAsync11 %
(11% &
)11& '
;11' (
return22 
	_conexion22 
;22 
}33 	
public55 
async55 
Task55 
<55 
bool55 
>55 
IsHealthyAsync55  .
(55. /
)55/ 0
{66 	
ThrowIfDisposed77 
(77 
)77 
;77 
try99 
{:: 
await;; 
using;; 
var;; 
conexion;;  (
=;;) *
await;;+ 0!
CreateConnectionAsync;;1 F
(;;F G
);;G H
;;;H I
return<< 
	_conexion<<  
?<<  !
.<<! "
State<<" '
==<<( *
ConnectionState<<+ :
.<<: ;
Open<<; ?
;<<? @
}== 
catch>> 
(>> 
NpgsqlException>> "
ex>># %
)>>% &
{?? 
_logErrorConnection@@ #
(@@# $
_log@@$ (
,@@( )
ex@@* ,
.@@, -
Message@@- 4
,@@4 5
ex@@6 8
)@@8 9
;@@9 :
returnAA 
falseAA 
;AA 
}BB 
}CC 	
	protectedEE 
voidEE 
ThrowIfDisposedEE &
(EE& '
)EE' (
{FF 	#
ObjectDisposedExceptionGG #
.GG# $
ThrowIfGG$ +
(GG+ ,
	_disposedGG, 5
,GG5 6
nameofGG7 =
(GG= >
SendSftpContextGG> M
)GGM N
)GGN O
;GGO P
}HH 	
	protectedJJ 
virtualJJ 
voidJJ 
DisposeJJ &
(JJ& '
boolJJ' +
	disposingJJ, 5
)JJ5 6
{KK 	
ifLL 
(LL 
!LL 
	_disposedLL 
)LL 
{MM 
ifNN 
(NN 
	disposingNN 
)NN 
{OO 
	_conexionPP 
?PP 
.PP 
DisposePP &
(PP& '
)PP' (
;PP( )
}QQ 
	_disposedRR 
=RR 
trueRR  
;RR  !
}SS 
}TT 	
publicVV 
voidVV 
DisposeVV 
(VV 
)VV 
{WW 	
DisposeXX 
(XX 
	disposingXX 
:XX 
trueXX #
)XX# $
;XX$ %
GCYY 
.YY 
SuppressFinalizeYY 
(YY  
thisYY  $
)YY$ %
;YY% &
}ZZ 	
}[[ 
}\\ îP
[D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess\Exec\Func\SelectRegistryBD.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Exec "
." #
Func# '
{ 
public 

class 
SelectRegistryBD !
:" #
ISelectRegistryBD$ 5
{ 
private 
readonly 
ISendSftpContext )

_pgContext* 4
;4 5
private 
readonly 
ILogger  
<  !
SelectRegistryBD! 1
>1 2
_log3 7
;7 8
private 
readonly 
IConfiguration '
_configuration( 6
;6 7
private 
static 
readonly 
Action  &
<& '
ILogger' .
<. /
SelectRegistryBD/ ?
>? @
,@ A
stringB H
,H I
	ExceptionJ S
?S T
>T U
LogInfoV ]
=^ _
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel 
. 
Information $
,$ %
new 
EventId 
( 
$num  
,  !
nameof" (
(( )
SelectRegistryBD) 9
)9 :
): ;
,; <
$str 8
)8 9
;9 :
private 
static 
readonly 
Action  &
<& '
ILogger' .
<. /
SelectRegistryBD/ ?
>? @
,@ A
intB E
,E F
	ExceptionG P
?P Q
>Q R
LogInfoCountS _
=` a
LoggerMessage 
. 
Define  
<  !
int! $
>$ %
(% &
LogLevel 
. 
Information $
,$ %
new   
EventId   
(   
$num    
,    !
nameof  " (
(  ( )
SelectRegistryBD  ) 9
)  9 :
)  : ;
,  ; <
$str!! .
)!!. /
;!!/ 0
private## 
static## 
readonly## 
Action##  &
<##& '
ILogger##' .
<##. /
SelectRegistryBD##/ ?
>##? @
,##@ A
string##B H
,##H I
	Exception##J S
?##S T
>##T U
LogError##V ^
=##_ `
LoggerMessage$$ 
.$$ 
Define$$  
<$$  !
string$$! '
>$$' (
($$( )
LogLevel%% 
.%% 
Error%% 
,%% 
new&& 
EventId&& 
(&& 
$num&&  
,&&  !
nameof&&" (
(&&( )
SelectRegistryBD&&) 9
)&&9 :
)&&: ;
,&&; <
$str'' J
)''J K
;''K L
public)) 
SelectRegistryBD)) 
())  
ISendSftpContext** 
	pgContext** &
,**& '
ILogger++ 
<++ 
SelectRegistryBD++ $
>++$ %
log++& )
,++) *
IConfiguration,, 
configuration,, (
),,( )
{-- 	

_pgContext.. 
=.. 
	pgContext.. "
??..# %
throw..& +
new.., /!
ArgumentNullException..0 E
(..E F
nameof..F L
(..L M
	pgContext..M V
)..V W
)..W X
;..X Y
_log// 
=// 
log// 
??// 
throw// 
new//  #!
ArgumentNullException//$ 9
(//9 :
nameof//: @
(//@ A
log//A D
)//D E
)//E F
;//F G
_configuration00 
=00 
configuration00 *
??00+ -
throw00. 3
new004 7!
ArgumentNullException008 M
(00M N
nameof00N T
(00T U
configuration00U b
)00b c
)00c d
;00d e
}11 	
public33 
async33 
Task33 
<33 
List33 
<33 
TdAttachmentDto33 .
>33. /
>33/ 0
SelectRecordAsync331 B
(33B C
)33C D
{44 	
var55 
result55 
=55 
new55 
List55 !
<55! "
TdAttachmentDto55" 1
>551 2
(552 3
)553 4
;554 5
var66 
opRegistered66 
=66 
_configuration66 -
.66- .
GetValue66. 6
<666 7
string667 =
>66= >
(66> ?
$str66? U
)66U V
??66W Y
string66Z `
.66` a
Empty66a f
;66f g
const88 
string88 
sql88 
=88 
$str8J 
;JJ 
tryLL 
{MM 
LogInfoOO 
(OO 
_logOO 
,OO 
opRegisteredOO *
,OO* +
nullOO, 0
)OO0 1
;OO1 2
awaitQQ 
usingQQ 
varQQ 
connQQ  $
=QQ% &
awaitQQ' ,

_pgContextQQ- 7
.QQ7 8!
CreateConnectionAsyncQQ8 M
(QQM N
)QQN O
;QQO P
awaitRR 
usingRR 
varRR 
cmdRR  #
=RR$ %
newRR& )
NpgsqlCommandRR* 7
(RR7 8
sqlRR8 ;
,RR; <
connRR= A
)RRA B
;RRB C
cmdSS 
.SS 

ParametersSS 
.SS 
AddWithValueSS +
(SS+ ,
$strSS, 4
,SS4 5
opRegisteredSS6 B
)SSB C
;SSC D
awaitUU 
usingUU 
varUU 
readerUU  &
=UU' (
awaitUU) .
cmdUU/ 2
.UU2 3
ExecuteReaderAsyncUU3 E
(UUE F
)UUF G
;UUG H
whileVV 
(VV 
awaitVV 
readerVV #
.VV# $
	ReadAsyncVV$ -
(VV- .
)VV. /
)VV/ 0
{WW 
resultXX 
.XX 
AddXX 
(XX 
MapReaderToDtoXX -
(XX- .
readerXX. 4
)XX4 5
)XX5 6
;XX6 7
}YY 
LogInfoCount\\ 
(\\ 
_log\\ !
,\\! "
result\\# )
.\\) *
Count\\* /
,\\/ 0
null\\1 5
)\\5 6
;\\6 7
}]] 
catch^^ 
(^^ 
NpgsqlException^^ "
ex^^# %
)^^% &
{__ 
LogError`` 
(`` 
_log`` 
,`` 
ex`` !
.``! "
Message``" )
,``) *
ex``+ -
)``- .
;``. /
throwaa 
;aa 
}bb 
returndd 
resultdd 
;dd 
}ee 	
publicgg 
TdAttachmentDtogg 
MapReaderToDtogg -
(gg- .
NpgsqlDataReadergg. >
readergg? E
)ggE F
{hh 	
stringii 
GetStringSafeii  
(ii  !
intii! $
indexii% *
)ii* +
=>ii, .
readerii/ 5
.ii5 6
IsDBNullii6 >
(ii> ?
indexii? D
)iiD E
?iiF G
stringiiH N
.iiN O
EmptyiiO T
:iiU V
readeriiW ]
.ii] ^
	GetStringii^ g
(iig h
indexiih m
)iim n
;iin o
DateTimejj 
?jj 
GetDateTimeSafejj %
(jj% &
intjj& )
indexjj* /
)jj/ 0
=>jj1 3
readerjj4 :
.jj: ;
IsDBNulljj; C
(jjC D
indexjjD I
)jjI J
?jjK L
nulljjM Q
:jjR S
readerjjT Z
.jjZ [
GetDateTimejj[ f
(jjf g
indexjjg l
)jjl m
;jjm n
returnll 
newll 
TdAttachmentDtoll &
{mm 
TdIdnn 
=nn 
readernn 
.nn 
GetInt32nn &
(nn& '
$numnn' (
)nn( )
,nn) *
TdCedulaoo 
=oo 
GetStringSafeoo (
(oo( )
$numoo) *
)oo* +
,oo+ ,
TdTipoIdentpp 
=pp 
GetStringSafepp +
(pp+ ,
$numpp, -
)pp- .
,pp. /
TdIdOperationqq 
=qq 
GetStringSafeqq  -
(qq- .
$numqq. /
)qq/ 0
,qq0 1
TdModelorr 
=rr 
GetStringSaferr (
(rr( )
$numrr) *
)rr* +
,rr+ ,
TdApplicationIdss 
=ss  !
GetStringSafess" /
(ss/ 0
$numss0 1
)ss1 2
,ss2 3
TdDomainHosttt 
=tt 
GetStringSafett ,
(tt, -
$numtt- .
)tt. /
,tt/ 0

TdReportIduu 
=uu 
GetStringSafeuu *
(uu* +
$numuu+ ,
)uu, -
,uu- .
TdAdjIdOperationvv  
=vv! "
GetStringSafevv# 0
(vv0 1
$numvv1 2
)vv2 3
,vv3 4
TdFormatww 
=ww 
GetStringSafeww (
(ww( )
$numww) *
)ww* +
,ww+ ,
TdBase64xx 
=xx 
GetStringSafexx (
(xx( )
$numxx) +
)xx+ ,
,xx, -
TdOperacionyy 
=yy 
GetStringSafeyy +
(yy+ ,
$numyy, .
)yy. /
,yy/ 0$
TdOperacionFechaRegistrozz (
=zz) *
GetDateTimeSafezz+ :
(zz: ;
$numzz; =
)zz= >
,zz> ?!
TdOperacionFechaEnvio{{ %
={{& '
GetDateTimeSafe{{( 7
({{7 8
$num{{8 :
){{: ;
,{{; <
TdReportName|| 
=|| 
GetStringSafe|| ,
(||, -
$num||- /
)||/ 0
}}} 
;}} 
}~~ 	
} 
}ÄÄ ó9
XD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess\Exec\Func\UpdateTableBD.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Exec "
." #
Func# '
{ 
public 

class 
UpdateTableBD 
:  
IUpdateTableBD! /
{ 
private 
readonly 
ISendSftpContext )

_pgContext* 4
;4 5
private 
readonly 
ILogger  
<  !
UpdateTableBD! .
>. /
_log0 4
;4 5
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
int0 3
,3 4
	Exception5 >
?> ?
>? @
_logNoRowsUpdatedA R
=S T
LoggerMessage 
. 
Define  
<  !
int! $
>$ %
(% &
LogLevel 
. 
Warning  
,  !
new 
EventId 
( 
$num  
,  !
nameof" (
(( )
UpdateTableAsync) 9
)9 :
): ;
,; <
$str B
)B C
;C D
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
int0 3
,3 4
	Exception5 >
?> ?
>? @#
_logUpdatedSuccessfullyA X
=Y Z
LoggerMessage 
. 
Define  
<  !
int! $
>$ %
(% &
LogLevel 
. 
Information $
,$ %
new   
EventId   
(   
$num    
,    !
nameof  " (
(  ( )
UpdateTableAsync  ) 9
)  9 :
)  : ;
,  ; <
$str!! B
)!!B C
;!!C D
private## 
static## 
readonly## 
Action##  &
<##& '
ILogger##' .
,##. /
int##0 3
,##3 4
	Exception##5 >
>##> ?
_logNpgsqlError##@ O
=##P Q
LoggerMessage$$ 
.$$ 
Define$$  
<$$  !
int$$! $
>$$$ %
($$% &
LogLevel%% 
.%% 
Error%% 
,%% 
new&& 
EventId&& 
(&& 
$num&&  
,&&  !
nameof&&" (
(&&( )
UpdateTableAsync&&) 9
)&&9 :
)&&: ;
,&&; <
$str'' O
)''O P
;''P Q
private)) 
static)) 
readonly)) 
Action))  &
<))& '
ILogger))' .
,)). /
	Exception))0 9
>))9 :
_logUnexpectedError)); N
=))O P
LoggerMessage** 
.** 
Define**  
(**  !
LogLevel++ 
.++ 
Error++ 
,++ 
new,, 
EventId,, 
(,, 
$num,,  
,,,  !
nameof,," (
(,,( )
UpdateTableAsync,,) 9
),,9 :
),,: ;
,,,; <
$str-- H
)--H I
;--I J
public// 
UpdateTableBD// 
(// 
ISendSftpContext// -
	pgContext//. 7
,//7 8
ILogger//9 @
<//@ A
UpdateTableBD//A N
>//N O
log//P S
)//S T
{00 	

_pgContext11 
=11 
	pgContext11 "
??11# %
throw11& +
new11, /!
ArgumentNullException110 E
(11E F
nameof11F L
(11L M
	pgContext11M V
)11V W
)11W X
;11X Y
_log22 
=22 
log22 
??22 
throw22 
new22  #!
ArgumentNullException22$ 9
(229 :
nameof22: @
(22@ A
log22A D
)22D E
)22E F
;22F G
}33 	
public55 
async55 
Task55 
UpdateTableAsync55 *
(55* +
TdAttachmentDto55+ :
record55; A
)55A B
{66 	
const77 
string77 
sql77 
=77 
$str7< &
;<<& '
try>> 
{?? 
await@@ 
using@@ 
var@@ 
conn@@  $
=@@% &
await@@' ,

_pgContext@@- 7
.@@7 8!
CreateConnectionAsync@@8 M
(@@M N
)@@N O
;@@O P
awaitAA 
usingAA 
varAA 
cmdAA  #
=AA$ %
newAA& )
NpgsqlCommandAA* 7
(AA7 8
sqlAA8 ;
,AA; <
connAA= A
)AAA B
;AAB C
cmdDD 
.DD 

ParametersDD 
.DD 
AddWithValueDD +
(DD+ ,
$strDD, ;
,DD; <
recordDD= C
.DDC D
TdOperacionDDD O
??DDP R
(DDS T
objectDDT Z
)DDZ [
DBNullDD[ a
.DDa b
ValueDDb g
)DDg h
;DDh i
cmdEE 
.EE 

ParametersEE 
.EE 
AddWithValueEE +
(EE+ ,
$strEE, G
,EEG H
recordEEI O
.EEO P!
TdOperacionFechaEnvioEEP e
??EEf h
(EEi j
objectEEj p
)EEp q
DBNullEEq w
.EEw x
ValueEEx }
)EE} ~
;EE~ 
cmdFF 
.FF 

ParametersFF 
.FF 
AddWithValueFF +
(FF+ ,
$strFF, 4
,FF4 5
recordFF6 <
.FF< =
TdIdFF= A
)FFA B
;FFB C
varHH 
rowsAffectedHH  
=HH! "
awaitHH# (
cmdHH) ,
.HH, - 
ExecuteNonQueryAsyncHH- A
(HHA B
)HHB C
;HHC D
ifJJ 
(JJ 
rowsAffectedJJ  
==JJ! #
$numJJ$ %
)JJ% &
{KK 
_logNoRowsUpdatedLL %
(LL% &
_logLL& *
,LL* +
recordLL, 2
.LL2 3
TdIdLL3 7
,LL7 8
nullLL9 =
)LL= >
;LL> ?
}MM 
elseNN 
{OO #
_logUpdatedSuccessfullyPP +
(PP+ ,
_logPP, 0
,PP0 1
recordPP2 8
.PP8 9
TdIdPP9 =
,PP= >
nullPP? C
)PPC D
;PPD E
}QQ 
}RR 
catchSS 
(SS 
NpgsqlExceptionSS "
exSS# %
)SS% &
{TT 
_logNpgsqlErrorUU 
(UU  
_logUU  $
,UU$ %
recordUU& ,
.UU, -
TdIdUU- 1
,UU1 2
exUU3 5
)UU5 6
;UU6 7
throwVV 
;VV 
}WW 
catchXX 
(XX 
	ExceptionXX 
exXX 
)XX  
{YY 
_logUnexpectedErrorZZ #
(ZZ# $
_logZZ$ (
,ZZ( )
exZZ* ,
)ZZ, -
;ZZ- .
throw[[ 
;[[ 
}\\ 
}]] 	
}^^ 
}aa ÆW
ZD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess\Exec\SP\FileProcDigExecSP.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Exec "
." #
SP# %
{ 
public 

class 
FileProcDigExecSP "
:# $
IFileProcDigExecSP% 7
{ 
private 
readonly 
IConfiguration '
_configuration( 6
;6 7
public 
FileProcDigExecSP  
(  !
IConfiguration! /
configuration0 =
)= >
{ 	
_configuration 
= 
configuration *
;* +
} 	
public 
async 
Task 
< 
bool 
> #
ExecuteFileProcDigAsync  7
(7 8
FileProcDigDto8 F
dtoG J
)J K
{ 	
var 
connectionString  
=! "
_configuration# 1
.1 2
GetConnectionString2 E
(E F
$strF Q
)Q R
;R S
await 
using 
var 

connection &
=' (
new) ,
SqlConnection- :
(: ;
connectionString; K
)K L
;L M
await 
using 
var 
command #
=$ %
new& )

SqlCommand* 4
(4 5
$str5 N
,N O

connectionP Z
)Z [
{ 
CommandType 
= 
CommandType )
.) *
StoredProcedure* 9
} 
; 
command!! 
.!! 

Parameters!! 
.!! 
AddWithValue!! +
(!!+ ,
$str!!, :
,!!: ;
dto!!< ?
.!!? @
	Operacion!!@ I
)!!I J
;!!J K
command"" 
."" 

Parameters"" 
."" 
AddWithValue"" +
(""+ ,
$str"", ?
,""? @
dto""A D
.""D E
CodigoSistema""E R
)""R S
;""S T
command## 
.## 

Parameters## 
.## 
AddWithValue## +
(##+ ,
$str##, =
,##= >
dto##? B
.##B C
IdSolicitud##C N
)##N O
;##O P
command$$ 
.$$ 

Parameters$$ 
.$$ 
AddWithValue$$ +
($$+ ,
$str$$, 5
,$$5 6
dto$$7 :
.$$: ;
User$$; ?
)$$? @
;$$@ A
command%% 
.%% 

Parameters%% 
.%% 
AddWithValue%% +
(%%+ ,
$str%%, >
,%%> ?
dto%%@ C
.%%C D
NumeroCedula%%D P
)%%P Q
;%%Q R
command(( 
.(( 

Parameters(( 
.(( 
AddWithValue(( +
(((+ ,
$str((, >
,((> ?
dto((@ C
.((C D
FechaProceso((D P
.((P Q
ToSqlDateTimeOrNull((Q d
(((d e
)((e f
)((f g
;((g h
command)) 
.)) 

Parameters)) 
.)) 
AddWithValue)) +
())+ ,
$str)), ;
,)); <
dto))= @
.))@ A
	FechaHora))A J
.))J K
ToSqlDateTimeOrNull))K ^
())^ _
)))_ `
)))` a
;))a b
command** 
.** 

Parameters** 
.** 
AddWithValue** +
(**+ ,
$str**, @
,**@ A
dto**B E
.**E F
FechaEjecucion**F T
.**T U
ToSqlDateTimeOrNull**U h
(**h i
)**i j
)**j k
;**k l
command++ 
.++ 

Parameters++ 
.++ 
AddWithValue++ +
(+++ ,
$str++, D
,++D E
dto++F I
.++I J
FechaCreditoMovil++J [
.++[ \
ToSqlDateTimeOrNull++\ o
(++o p
)++p q
)++q r
;++r s
command.. 
... 

Parameters.. 
... 
AddWithValue.. +
(..+ ,
$str.., 7
,..7 8
dto..9 <
...< =
IdDoc..= B
??..C E
(..F G
object..G M
)..M N
DBNull..N T
...T U
Value..U Z
)..Z [
;..[ \
command// 
.// 

Parameters// 
.// 
AddWithValue// +
(//+ ,
$str//, ;
,//; <
dto//= @
.//@ A
IdDocDef//A I
??//J L
(//M N
object//N T
)//T U
DBNull//U [
.//[ \
Value//\ a
)//a b
;//b c
command00 
.00 

Parameters00 
.00 
AddWithValue00 +
(00+ ,
$str00, @
,00@ A
dto00B E
.00E F
NombreCompleto00F T
??00U W
(00X Y
object00Y _
)00_ `
DBNull00` f
.00f g
Value00g l
)00l m
;00m n
command11 
.11 

Parameters11 
.11 
AddWithValue11 +
(11+ ,
$str11, :
,11: ;
dto11< ?
.11? @
	Apellido111@ I
??11J L
(11M N
object11N T
)11T U
DBNull11U [
.11[ \
Value11\ a
)11a b
;11b c
command22 
.22 

Parameters22 
.22 
AddWithValue22 +
(22+ ,
$str22, :
,22: ;
dto22< ?
.22? @
	Apellido222@ I
??22J L
(22M N
object22N T
)22T U
DBNull22U [
.22[ \
Value22\ a
)22a b
;22b c
command33 
.33 

Parameters33 
.33 
AddWithValue33 +
(33+ ,
$str33, =
,33= >
dto33? B
.33B C
RazonSocial33C N
??33O Q
(33R S
object33S Y
)33Y Z
DBNull33Z `
.33` a
Value33a f
)33f g
;33g h
command44 
.44 

Parameters44 
.44 
AddWithValue44 +
(44+ ,
$str44, ;
,44; <
dto44= @
.44@ A
	TipoIdent44A J
??44K M
(44N O
object44O U
)44U V
DBNull44V \
.44\ ]
Value44] b
)44b c
;44c d
command55 
.55 

Parameters55 
.55 
AddWithValue55 +
(55+ ,
$str55, =
,55= >
dto55? B
.55B C
TipoPersona55C N
??55O Q
(55R S
object55S Y
)55Y Z
DBNull55Z `
.55` a
Value55a f
)55f g
;55g h
command66 
.66 

Parameters66 
.66 
AddWithValue66 +
(66+ ,
$str66, >
,66> ?
dto66@ C
.66C D
TipoProducto66D P
??66Q S
(66T U
object66U [
)66[ \
DBNull66\ b
.66b c
Value66c h
)66h i
;66i j
command77 
.77 

Parameters77 
.77 
AddWithValue77 +
(77+ ,
$str77, A
,77A B
dto77C F
.77F G
NumeroOperacion77G V
??77W Y
(77Z [
object77[ a
)77a b
DBNull77b h
.77h i
Value77i n
)77n o
;77o p
command88 
.88 

Parameters88 
.88 
AddWithValue88 +
(88+ ,
$str88, >
,88> ?
dto88@ C
.88C D
AnioPolitica88D P
??88Q S
(88T U
object88U [
)88[ \
DBNull88\ b
.88b c
Value88c h
)88h i
;88i j
command99 
.99 

Parameters99 
.99 
AddWithValue99 +
(99+ ,
$str99, ?
,99? @
dto99A D
.99D E
TipoDocumento99E R
??99S U
(99V W
object99W ]
)99] ^
DBNull99^ d
.99d e
Value99e j
)99j k
;99k l
command<< 
.<< 

Parameters<< 
.<< 
AddWithValue<< +
(<<+ ,
$str<<, <
,<<< =
dto<<> A
.<<A B

NumeroEnte<<B L
)<<L M
;<<M N
command?? 
.?? 

Parameters?? 
.?? 
AddWithValue?? +
(??+ ,
$str??, 7
,??7 8
string??9 ?
.??? @
IsNullOrWhiteSpace??@ R
(??R S
dto??S V
.??V W
Estado??W ]
)??] ^
???_ `
$char??a d
:??e f
dto??g j
.??j k
Estado??k q
[??q r
$num??r s
]??s t
)??t u
;??u v
awaitAA 

connectionAA 
.AA 
	OpenAsyncAA &
(AA& '
)AA' (
;AA( )
varBB 
resultBB 
=BB 
awaitBB 
commandBB &
.BB& ' 
ExecuteNonQueryAsyncBB' ;
(BB; <
)BB< =
;BB= >
returnCC 
resultCC 
>CC 
$numCC 
;CC 
}DD 	
}EE 
publicGG 

staticGG 
classGG "
SqlParameterExtensionsGG .
{HH 
privateII 
staticII 
readonlyII 
DateTimeII  (

MinSqlDateII) 3
=II4 5
newII6 9
DateTimeII: B
(IIB C
$numIIC G
,IIG H
$numIII J
,IIJ K
$numIIL M
,IIM N
$numIIO P
,IIP Q
$numIIR S
,IIS T
$numIIU V
,IIV W
DateTimeKindIIX d
.IId e
UtcIIe h
)IIh i
;IIi j
publicLL 
staticLL 
objectLL 
ToSqlDateTimeOrNullLL 0
(LL0 1
thisLL1 5
DateTimeLL6 >
dateLL? C
)LLC D
{MM 	
returnNN 
dateNN 
<NN 

MinSqlDateNN $
?NN% &
DBNullNN' -
.NN- .
ValueNN. 3
:NN4 5
dateNN6 :
;NN: ;
}OO 	
publicQQ 
staticQQ 
objectQQ 
ToSqlDateTimeOrNullQQ 0
(QQ0 1
thisQQ1 5
DateTimeQQ6 >
?QQ> ?
dateQQ@ D
)QQD E
{RR 	
returnSS 
dateSS 
.SS 
HasValueSS  
&&SS! #
dateSS$ (
.SS( )
ValueSS) .
>=SS/ 1

MinSqlDateSS2 <
?SS= >
dateSS? C
.SSC D
ValueSSD I
:SSJ K
DBNullSSL R
.SSR S
ValueSSS X
;SSX Y
}TT 	
}UU 
}VV ﬁU
VD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess\Exec\SP\GetDatesSyb12.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Exec "
." #
SP# %
{ 
public 

class 
GetDatesSyb12 
:  
IGetDatesSyb12! /
{ 
private 
readonly 
	IBdSybase "
_bd# &
;& '
private 
readonly 
ILogger  
<  !
GetDatesSyb12! .
>. /
_logger0 7
;7 8
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
?A B
>B C$
LogDataMappingSuccessfulD \
=] ^
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel) 1
.1 2
Information2 =
,= >
new? B
EventIdC J
(J K
$numK O
,O P
nameofQ W
(W X
GetDatesSyb12X e
)e f
)f g
,g h
$str :
): ;
;; <
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
?A B
>B C
LogMappingErrorD S
=T U
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel) 1
.1 2
Error2 7
,7 8
new9 <
EventId= D
(D E
$numE I
,I J
nameofK Q
(Q R
GetDatesSyb12R _
)_ `
)` a
,a b
$str @
)@ A
;A B
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
?A B
>B C

LogSpErrorD N
=O P
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel) 1
.1 2
Error2 7
,7 8
new9 <
EventId= D
(D E
$numE I
,I J
nameofK Q
(Q R
GetDatesSyb12R _
)_ `
)` a
,a b
$str   C
)  C D
;  D E
public"" 
GetDatesSyb12"" 
("" 
	IBdSybase"" &
bd""' )
,"") *
ILogger""+ 2
<""2 3
GetDatesSyb12""3 @
>""@ A
logger""B H
)""H I
{## 	
_bd$$ 
=$$ 
bd$$ 
??$$ 
throw$$ 
new$$ !!
ArgumentNullException$$" 7
($$7 8
nameof$$8 >
($$> ?
bd$$? A
)$$A B
)$$B C
;$$C D
_logger%% 
=%% 
logger%% 
??%% 
throw%%  %
new%%& )!
ArgumentNullException%%* ?
(%%? @
nameof%%@ F
(%%F G
logger%%G M
)%%M N
)%%N O
;%%O P
}'' 	
public)) 
async)) 
Task)) 
<)) $
GetDatesSyb12ResponseDto)) 2
>))2 3
ExecuteSpGetDates))4 E
())E F
)))F G
{** 	
var++ 
dsResultado++ 
=++ 
new++ !
DataSet++" )
(++) *
)++* +
;+++ ,
var,, 

dataOrigin,, 
=,, 
$str,, +
;,,+ ,
try.. 
{// 
const00 
string00 
spName00 #
=00$ %
$str00& E
;00E F
using22 
var22 
cnn22 
=22 
await22  %
_bd22& )
.22) *!
CreateConnectionAsync22* ?
(22? @
)22@ A
;22A B
using55 
var55 
command55 !
=55" #
new55$ '
SybaseCommand55( 5
(555 6
spName556 <
,55< =
cnn55> A
)55A B
{66 
CommandType77 
=77  !
CommandType77" -
.77- .
StoredProcedure77. =
}88 
;88 
command:: 
.:: 

Parameters:: "
.::" #
Add::# &
(::& '
new::' *
SybaseParameter::+ :
(::: ;
$str::; G
,::G H
$num::I L
)::L M
)::M N
;::N O
command;; 
.;; 

Parameters;; "
.;;" #
Add;;# &
(;;& '
new;;' *
SybaseParameter;;+ :
(;;: ;
$str;;; H
,;;H I
$str;;J M
);;M N
);;N O
;;;O P
var== 
adapter== 
=== 
new== !
SybaseDataAdapter==" 3
(==3 4
command==4 ;
)==; <
;==< =
adapter>> 
.>> 
Fill>> 
(>> 
dsResultado>> (
)>>( )
;>>) *
return@@ 
	MapResult@@  
(@@  !
dsResultado@@! ,
,@@, -

dataOrigin@@. 8
)@@8 9
;@@9 :
}AA 
catchBB 
(BB 
	ExceptionBB 
exBB 
)BB  
{CC 

LogSpErrorDD 
(DD 
_loggerDD "
,DD" #

dataOriginDD$ .
,DD. /
exDD0 2
)DD2 3
;DD3 4
throwEE 
newEE %
InvalidOperationExceptionEE 3
(EE3 4
$"EE4 6
$strEE6 O
{EEO P

dataOriginEEP Z
}EEZ [
"EE[ \
,EE\ ]
exEE^ `
)EE` a
;EEa b
}FF 
}GG 	
publicII $
GetDatesSyb12ResponseDtoII '
	MapResultII( 1
(II1 2
DataSetII2 9
dsII: <
,II< =
stringII> D

dataOriginIIE O
)IIO P
{JJ 	
ifKK 
(KK 
dsKK 
==KK 
nullKK 
||KK 
dsKK  
.KK  !
TablesKK! '
.KK' (
CountKK( -
==KK. 0
$numKK1 2
||KK3 5
dsKK6 8
.KK8 9
TablesKK9 ?
[KK? @
$numKK@ A
]KKA B
.KKB C
RowsKKC G
.KKG H
CountKKH M
==KKN P
$numKKQ R
)KKR S
throwLL 
newLL %
InvalidOperationExceptionLL 3
(LL3 4
$strLL4 \
)LL\ ]
;LL] ^
tryNN 
{OO 
varPP 
rowPP 
=PP 
dsPP 
.PP 
TablesPP #
[PP# $
$numPP$ %
]PP% &
.PP& '
RowsPP' +
[PP+ ,
$numPP, -
]PP- .
;PP. /
varQQ 
resultQQ 
=QQ 
newQQ  $
GetDatesSyb12ResponseDtoQQ! 9
{RR 
FechaProcesoSS  
=SS! "
rowSS# &
[SS& '
$strSS' 5
]SS5 6
!=SS7 9
DBNullSS: @
.SS@ A
ValueSSA F
?SSG H
ConvertSSI P
.SSP Q

ToDateTimeSSQ [
(SS[ \
rowSS\ _
[SS_ `
$strSS` n
]SSn o
,SSo p
CultureInfoSSq |
.SS| }
InvariantCulture	SS} ç
)
SSç é
:
SSè ê
(
SSë í
DateTime
SSí ö
?
SSö õ
)
SSõ ú
null
SSú †
,
SS† °
FechaDiaTT 
=TT 
rowTT "
[TT" #
$strTT# -
]TT- .
!=TT/ 1
DBNullTT2 8
.TT8 9
ValueTT9 >
?TT? @
ConvertTTA H
.TTH I

ToDateTimeTTI S
(TTS T
rowTTT W
[TTW X
$strTTX b
]TTb c
,TTc d
CultureInfoTTe p
.TTp q
InvariantCulture	TTq Å
)
TTÅ Ç
:
TTÉ Ñ
(
TTÖ Ü
DateTime
TTÜ é
?
TTé è
)
TTè ê
null
TTê î
,
TTî ï
	FechaHoraUU 
=UU 
rowUU  #
[UU# $
$strUU$ /
]UU/ 0
!=UU1 3
DBNullUU4 :
.UU: ;
ValueUU; @
?UUA B
ConvertUUC J
.UUJ K

ToDateTimeUUK U
(UUU V
rowUUV Y
[UUY Z
$strUUZ e
]UUe f
,UUf g
CultureInfoUUh s
.UUs t
InvariantCulture	UUt Ñ
)
UUÑ Ö
:
UUÜ á
(
UUà â
DateTime
UUâ ë
?
UUë í
)
UUí ì
null
UUì ó
,
UUó ò
NumErrorVV 
=VV 
rowVV "
[VV" #
$strVV# .
]VV. /
!=VV0 2
DBNullVV3 9
.VV9 :
ValueVV: ?
?VV@ A
ConvertVVB I
.VVI J
ToInt32VVJ Q
(VVQ R
rowVVR U
[VVU V
$strVVV a
]VVa b
,VVb c
CultureInfoVVd o
.VVo p
InvariantCulture	VVp Ä
)
VVÄ Å
:
VVÇ É
$num
VVÑ Ö
,
VVÖ Ü
MsgErrorWW 
=WW 
rowWW "
[WW" #
$strWW# .
]WW. /
!=WW0 2
DBNullWW3 9
.WW9 :
ValueWW: ?
?WW@ A
ConvertWWB I
.WWI J
ToStringWWJ R
(WWR S
rowWWS V
[WWV W
$strWWW b
]WWb c
,WWc d
CultureInfoWWe p
.WWp q
InvariantCulture	WWq Å
)
WWÅ Ç
??
WWÉ Ö
string
WWÜ å
.
WWå ç
Empty
WWç í
:
WWì î
string
WWï õ
.
WWõ ú
Empty
WWú °
}XX 
;XX $
LogDataMappingSuccessfulZZ (
(ZZ( )
_loggerZZ) 0
,ZZ0 1

dataOriginZZ2 <
,ZZ< =
nullZZ> B
)ZZB C
;ZZC D
return[[ 
result[[ 
;[[ 
}\\ 
catch]] 
(]] 
	Exception]] 
ex]] 
)]]  
{^^ 
LogMappingError__ 
(__  
_logger__  '
,__' (

dataOrigin__) 3
,__3 4
ex__5 7
)__7 8
;__8 9
throw`` 
new`` %
InvalidOperationException`` 3
(``3 4
$"``4 6
$str``6 [
{``[ \

dataOrigin``\ f
}``f g
"``g h
,``h i
ex``j l
)``l m
;``m n
}aa 
}bb 	
}cc 
}dd 