˘
cD:\TV\temp\Core.SET.Web.Api.GetHolidays\Core.SET.Web.Api.GetHolidays.DataAccess\Context\BdSybase.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Context %
{ 
public 

class 
BdSybase 
( 
IConfiguration (
_configuration) 7
,7 8
ICrypto9 @
_cryptoA H
,H I
ILoggerJ Q
<Q R
BdSybaseR Z
>Z [
_log\ `
)` a
:b c
IDisposabled o
,o p
	IBdSybaseq z
{ 
private 
bool 
	_disposed 
; 
	protected 
virtual 
void 
Dispose &
(& '
bool' +
	disposing, 5
)5 6
{ 	
if 
( 
	_disposed 
) 
return !
;! "
	_disposed 
= 
true 
; 
} 	
public 
void 
Dispose 
( 
) 
{ 	
Dispose 
( 
true 
) 
; 
GC 
. 
SuppressFinalize 
(  
this  $
)$ %
;% &
} 	
~ 	
BdSybase	 
( 
) 
{ 	
Dispose 
( 
false 
) 
; 
} 	
public!! 
async!! 
Task!! 
<!! 
SybaseConnection!! *
>!!* +!
CreateConnectionAsync!!, A
(!!A B
)!!B C
{"" 	
string## 
cadenaEncriptada## #
=##$ %
_configuration##& 4
.##4 5

GetSection##5 ?
(##? @
$str##@ _
)##_ `
.##` a
Value##a f
??$$ 
throw$$ 
new$$ %
InvalidOperationException$$ 5
($$5 6
nameof$$6 <
($$< =
cadenaEncriptada$$= M
)$$M N
)$$N O
;$$O P
var%% 
cadenaConexion%% 
=%%  
_crypto%%! (
.%%( )
Decrypt%%) 0
(%%0 1
cadenaEncriptada%%1 A
)%%A B
;%%B C
var&& 
conexion&& 
=&& 
new&& 
SybaseConnection&& /
(&&/ 0
cadenaConexion&&0 >
)&&> ?
;&&? @
await'' 
conexion'' 
.'' 
	OpenAsync'' $
(''$ %
)''% &
;''& '
return(( 
conexion(( 
;(( 
})) 	
public++ 
async++ 
Task++ 
<++ 
bool++ 
>++ 
IsHealthyAsync++  .
(++. /
)++/ 0
{,, 	
try-- 
{.. 
using// 
var// 
conexion// "
=//# $
await//% *!
CreateConnectionAsync//+ @
(//@ A
)//A B
;//B C
return00 
conexion00 
.00  
State00  %
==00& (
ConnectionState00) 8
.008 9
Open009 =
;00= >
}11 
catch22 
(22 
SybaseException22 "
ex22# %
)22% &
{33 
_log44 
.44 
LogError44 
(44 
ex44  
,44  !
$str44" M
)44M N
;44N O
return55 
false55 
;55 
}66 
}77 	
}88 
}99 Ø
cD:\TV\temp\Core.SET.Web.Api.GetHolidays\Core.SET.Web.Api.GetHolidays.DataAccess\Context\DataBase.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Context %
{ 
public 

class 
DataBase 
{ 
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
StoredProcedure %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 
string 
ConnectionString &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
} 
}		 ï.
jD:\TV\temp\Core.SET.Web.Api.GetHolidays\Core.SET.Web.Api.GetHolidays.DataAccess\Context\SendSftpContext.cs
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
NpgsqlConnection  
	_conexion! *
;* +
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
>A B
_logErrorConnectionC V
=W X
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
}\\ ß7
mD:\TV\temp\Core.SET.Web.Api.GetHolidays\Core.SET.Web.Api.GetHolidays.DataAccess\Exec\Func\SelectRegistryBD.cs
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
public 

class 
SelectRegistryBD !
(! "
ISendSftpContext" 2

_pgContext3 =
,= >
ILogger 
< 
SelectRegistryBD $
>$ %
_log& *
,* +
IConfiguration 
_configuration )
)) *
:+ ,
ISelectRegistryBD- >
{ 
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
>A B
_logErrorSelectC R
=S T
LoggerMessage
 
. 
Define 
< 
string %
>% &
(& '
LogLevel 
. 
Error 
, 
new 
EventId 
( 
$num 
, 
nameof #
(# $
_logErrorSelect$ 3
)3 4
)4 5
,5 6
$str G
)
 
; 
public 
async 
Task 
< 
List 
< 
TdAttachmentDto .
>. /
>/ 0
SelectRecordAsync1 B
(B C
)C D
{ 	
var 
result 
= 
new 
List !
<! "
TdAttachmentDto" 1
>1 2
(2 3
)3 4
;4 5
var   
opRegistered   
=   
_configuration   -
.  - .
GetValue  . 6
<  6 7
string  7 =
>  = >
(  > ?
$str  ? U
)  U V
??  W Y
string  Z `
.  ` a
Empty  a f
;  f g
const"" 
string"" 
sql"" 
="" 
$str"4 	
;44	 

try66 
{77 
await88 
using88 
var88 
conn88  $
=88% &
await88' ,

_pgContext88- 7
.887 8!
CreateConnectionAsync888 M
(88M N
)88N O
;88O P
await99 
using99 
var99 
cmd99  #
=99$ %
new99& )
NpgsqlCommand99* 7
(997 8
sql998 ;
,99; <
conn99= A
)99A B
;99B C
cmd:: 
.:: 

Parameters:: 
.:: 
AddWithValue:: +
(::+ ,
$str::, 4
,::4 5
opRegistered::6 B
)::B C
;::C D
await<< 
using<< 
var<< 
reader<<  &
=<<' (
await<<) .
cmd<</ 2
.<<2 3
ExecuteReaderAsync<<3 E
(<<E F
)<<F G
;<<G H
while== 
(== 
await== 
reader== #
.==# $
	ReadAsync==$ -
(==- .
)==. /
)==/ 0
{>> 
result?? 
.?? 
Add?? 
(?? 
MapReaderToDto?? -
(??- .
reader??. 4
)??4 5
)??5 6
;??6 7
}@@ 
}AA 
catchBB 
(BB 
NpgsqlExceptionBB "
exBB# %
)BB% &
{CC 
_logErrorSelectDD 
(DD  
_logDD  $
,DD$ %
exDD& (
.DD( )
MessageDD) 0
,DD0 1
exDD2 4
)DD4 5
;DD5 6
throwFF 
;FF 
}GG 
returnII 
resultII 
;II 
}JJ 	
publicLL 
TdAttachmentDtoLL 
MapReaderToDtoLL  .
(LL. /
NpgsqlDataReaderLL/ ?
readerLL@ F
)LLF G
{MM 	
stringNN 
GetStringSafeNN  
(NN  !
intNN! $
indexNN% *
)NN* +
=>NN, .
readerNN/ 5
.NN5 6
IsDBNullNN6 >
(NN> ?
indexNN? D
)NND E
?NNF G
stringNNH N
.NNN O
EmptyNNO T
:NNU V
readerNNW ]
.NN] ^
	GetStringNN^ g
(NNg h
indexNNh m
)NNm n
;NNn o
DateTimeOO 
?OO 
GetDateTimeSafeOO %
(OO% &
intOO& )
indexOO* /
)OO/ 0
=>OO1 3
readerOO4 :
.OO: ;
IsDBNullOO; C
(OOC D
indexOOD I
)OOI J
?OOK L
nullOOM Q
:OOR S
readerOOT Z
.OOZ [
GetDateTimeOO[ f
(OOf g
indexOOg l
)OOl m
;OOm n
returnQQ 
newQQ 
TdAttachmentDtoQQ &
{RR 
TdIdSS 
=SS 
readerSS 
.SS 
GetInt32SS &
(SS& '
$numSS' (
)SS( )
,SS) *
TdCedulaTT 
=TT 
GetStringSafeTT (
(TT( )
$numTT) *
)TT* +
,TT+ ,
TdTipoIdentUU 
=UU 
GetStringSafeUU +
(UU+ ,
$numUU, -
)UU- .
,UU. /
TdIdOperationVV 
=VV 
GetStringSafeVV  -
(VV- .
$numVV. /
)VV/ 0
,VV0 1
TdModeloWW 
=WW 
GetStringSafeWW (
(WW( )
$numWW) *
)WW* +
,WW+ ,
TdApplicationIdXX 
=XX  !
GetStringSafeXX" /
(XX/ 0
$numXX0 1
)XX1 2
,XX2 3
TdDomainHostYY 
=YY 
GetStringSafeYY ,
(YY, -
$numYY- .
)YY. /
,YY/ 0

TdReportIdZZ 
=ZZ 
GetStringSafeZZ *
(ZZ* +
$numZZ+ ,
)ZZ, -
,ZZ- .
TdAdjIdOperation[[  
=[[! "
GetStringSafe[[# 0
([[0 1
$num[[1 2
)[[2 3
,[[3 4
TdFormat\\ 
=\\ 
GetStringSafe\\ (
(\\( )
$num\\) *
)\\* +
,\\+ ,
TdBase64]] 
=]] 
GetStringSafe]] (
(]]( )
$num]]) +
)]]+ ,
,]], -
TdOperacion^^ 
=^^ 
GetStringSafe^^ +
(^^+ ,
$num^^, .
)^^. /
,^^/ 0$
TdOperacionFechaRegistro__ (
=__) *
GetDateTimeSafe__+ :
(__: ;
$num__; =
)__= >
,__> ?!
TdOperacionFechaEnvio`` %
=``& '
GetDateTimeSafe``( 7
(``7 8
$num``8 :
)``: ;
,``; <
TdReportNameaa 
=aa 
GetStringSafeaa ,
(aa, -
$numaa- /
)aa/ 0
}bb 
;bb 
}cc 	
}dd 
}ff ≤2
jD:\TV\temp\Core.SET.Web.Api.GetHolidays\Core.SET.Web.Api.GetHolidays.DataAccess\Exec\Func\UpdateTableBD.cs
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
( 
ISendSftpContext /

_pgContext0 :
,: ;
ILogger< C
<C D
UpdateTableBDD Q
>Q R
_logS W
)W X
:Y Z
IUpdateTableBD[ i
{ 
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
int0 3
,3 4
	Exception5 >
>> ?
_logNoRowsUpdated@ Q
=R S
LoggerMessage 
. 
Define  
<  !
int! $
>$ %
(% &
LogLevel 
. 
Warning  
,  !
new 
EventId 
( 
$num  
,  !
nameof" (
(( )
UpdateTableAsync) 9
)9 :
): ;
,; <
$str ;
); <
;< =
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
int0 3
,3 4
	Exception5 >
>> ?#
_logUpdatedSuccessfully@ W
=X Y
LoggerMessage 
. 
Define  
<  !
int! $
>$ %
(% &
LogLevel 
. 
Information $
,$ %
new 
EventId 
( 
$num  
,  !
nameof" (
(( )
UpdateTableAsync) 9
)9 :
): ;
,; <
$str =
)= >
;> ?
private!! 
static!! 
readonly!! 
Action!!  &
<!!& '
ILogger!!' .
,!!. /
int!!0 3
,!!3 4
	Exception!!5 >
>!!> ?
_logNpgsqlError!!@ O
=!!P Q
LoggerMessage"" 
."" 
Define""  
<""  !
int""! $
>""$ %
(""% &
LogLevel## 
.## 
Error## 
,## 
new$$ 
EventId$$ 
($$ 
$num$$  
,$$  !
nameof$$" (
($$( )
UpdateTableAsync$$) 9
)$$9 :
)$$: ;
,$$; <
$str%% L
)%%L M
;%%M N
private'' 
static'' 
readonly'' 
Action''  &
<''& '
ILogger''' .
,''. /
	Exception''0 9
>''9 :
_logUnexpectedError''; N
=''O P
LoggerMessage(( 
.(( 
Define((  
(((  !
LogLevel)) 
.)) 
Error)) 
,)) 
new** 
EventId** 
(** 
$num**  
,**  !
nameof**" (
(**( )
UpdateTableAsync**) 9
)**9 :
)**: ;
,**; <
$str++ C
)++C D
;++D E
public// 
async// 
Task// 
UpdateTableAsync// *
(//* +
TdAttachmentDto//+ :
record//; A
)//A B
{00 	
const11 
string11 
sql11 
=11 
$str16 &
;66& '
try88 
{99 
await:: 
using:: 
var:: 
conn::  $
=::% &
await::' ,

_pgContext::- 7
.::7 8!
CreateConnectionAsync::8 M
(::M N
)::N O
;::O P
await;; 
using;; 
var;; 
cmd;;  #
=;;$ %
new;;& )
NpgsqlCommand;;* 7
(;;7 8
sql;;8 ;
,;;; <
conn;;= A
);;A B
;;;B C
cmd>> 
.>> 

Parameters>> 
.>> 
AddWithValue>> +
(>>+ ,
$str>>, ;
,>>; <
record>>= C
.>>C D
TdOperacion>>D O
??>>P R
(>>S T
object>>T Z
)>>Z [
DBNull>>[ a
.>>a b
Value>>b g
)>>g h
;>>h i
cmd?? 
.?? 

Parameters?? 
.?? 
AddWithValue?? +
(??+ ,
$str??, G
,??G H
record??I O
.??O P!
TdOperacionFechaEnvio??P e
????f h
(??i j
object??j p
)??p q
DBNull??q w
.??w x
Value??x }
)??} ~
;??~ 
cmd@@ 
.@@ 

Parameters@@ 
.@@ 
AddWithValue@@ +
(@@+ ,
$str@@, 4
,@@4 5
record@@6 <
.@@< =
TdId@@= A
)@@A B
;@@B C
varBB 
rowsAffectedBB  
=BB! "
awaitBB# (
cmdBB) ,
.BB, - 
ExecuteNonQueryAsyncBB- A
(BBA B
)BBB C
;BBC D
ifDD 
(DD 
rowsAffectedDD  
==DD! #
$numDD$ %
)DD% &
{EE 
_logNoRowsUpdatedFF %
(FF% &
_logFF& *
,FF* +
recordFF, 2
.FF2 3
TdIdFF3 7
,FF7 8
nullFF9 =
)FF= >
;FF> ?
}GG 
elseHH 
{II #
_logUpdatedSuccessfullyJJ +
(JJ+ ,
_logJJ, 0
,JJ0 1
recordJJ2 8
.JJ8 9
TdIdJJ9 =
,JJ= >
nullJJ? C
)JJC D
;JJD E
}KK 
}LL 
catchMM 
(MM 
NpgsqlExceptionMM "
exMM# %
)MM% &
{NN 
_logNpgsqlErrorOO 
(OO  
_logOO  $
,OO$ %
recordOO& ,
.OO, -
TdIdOO- 1
,OO1 2
exOO3 5
)OO5 6
;OO6 7
throwPP 
;PP 
}QQ 
catchRR 
(RR 
	ExceptionRR 
exRR 
)RR  
{SS 
_logUnexpectedErrorTT #
(TT# $
_logTT$ (
,TT( )
exTT* ,
)TT, -
;TT- .
throwUU 
;UU 
}VV 
}WW 	
}XX 
}[[ íW
lD:\TV\temp\Core.SET.Web.Api.GetHolidays\Core.SET.Web.Api.GetHolidays.DataAccess\Exec\SP\FileProcDigExecSP.cs
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
public 

class 
FileProcDigExecSP "
(# $
IConfiguration$ 2
_configuration3 A
,A B
ICryptoC J
_cryptoK R
)R S
:T U
IFileProcDigExecSPV h
{ 
private 
readonly 
string 
_connectionString  1
=2 3
_crypto4 ;
.; <
Decrypt< C
(C D
_configurationD R
.R S
GetConnectionStringS f
(f g
$strg r
)r s
)s t
.t u
Trimu y
(y z
)z {
;{ |
public 
async 
Task 
< 
bool 
> #
ExecuteFileProcDigAsync  7
(7 8
FileProcDigDto8 F
dtoG J
)J K
{ 	
await 
using 
var 

connection &
=' (
new) ,
SqlConnection- :
(: ;
_connectionString; L
)L M
;M N
await 
using 
var 
command #
=$ %
new& )

SqlCommand* 4
(4 5
$str5 N
,N O

connectionP Z
)Z [
{ 
CommandType 
= 
CommandType )
.) *
StoredProcedure* 9
} 
; 
command 
. 

Parameters 
. 
AddWithValue +
(+ ,
$str, :
,: ;
dto< ?
.? @
	Operacion@ I
)I J
;J K
command 
. 

Parameters 
. 
AddWithValue +
(+ ,
$str, ?
,? @
dtoA D
.D E
CodigoSistemaE R
)R S
;S T
command 
. 

Parameters 
. 
AddWithValue +
(+ ,
$str, =
,= >
dto? B
.B C
IdSolicitudC N
)N O
;O P
command   
.   

Parameters   
.   
AddWithValue   +
(  + ,
$str  , 5
,  5 6
dto  7 :
.  : ;
User  ; ?
)  ? @
;  @ A
command!! 
.!! 

Parameters!! 
.!! 
AddWithValue!! +
(!!+ ,
$str!!, >
,!!> ?
dto!!@ C
.!!C D
NumeroCedula!!D P
)!!P Q
;!!Q R
command$$ 
.$$ 

Parameters$$ 
.$$ 
AddWithValue$$ +
($$+ ,
$str$$, >
,$$> ?
dto$$@ C
.$$C D
FechaProceso$$D P
.$$P Q
ToSqlDateTimeOrNull$$Q d
($$d e
)$$e f
)$$f g
;$$g h
command%% 
.%% 

Parameters%% 
.%% 
AddWithValue%% +
(%%+ ,
$str%%, ;
,%%; <
dto%%= @
.%%@ A
	FechaHora%%A J
.%%J K
ToSqlDateTimeOrNull%%K ^
(%%^ _
)%%_ `
)%%` a
;%%a b
command&& 
.&& 

Parameters&& 
.&& 
AddWithValue&& +
(&&+ ,
$str&&, @
,&&@ A
dto&&B E
.&&E F
FechaEjecucion&&F T
.&&T U
ToSqlDateTimeOrNull&&U h
(&&h i
)&&i j
)&&j k
;&&k l
command'' 
.'' 

Parameters'' 
.'' 
AddWithValue'' +
(''+ ,
$str'', D
,''D E
dto''F I
.''I J
FechaCreditoMovil''J [
.''[ \
ToSqlDateTimeOrNull''\ o
(''o p
)''p q
)''q r
;''r s
command** 
.** 

Parameters** 
.** 
AddWithValue** +
(**+ ,
$str**, 7
,**7 8
dto**9 <
.**< =
IdDoc**= B
??**C E
(**F G
object**G M
)**M N
DBNull**N T
.**T U
Value**U Z
)**Z [
;**[ \
command++ 
.++ 

Parameters++ 
.++ 
AddWithValue++ +
(+++ ,
$str++, ;
,++; <
dto++= @
.++@ A
IdDocDef++A I
??++J L
(++M N
object++N T
)++T U
DBNull++U [
.++[ \
Value++\ a
)++a b
;++b c
command,, 
.,, 

Parameters,, 
.,, 
AddWithValue,, +
(,,+ ,
$str,,, @
,,,@ A
dto,,B E
.,,E F
NombreCompleto,,F T
??,,U W
(,,X Y
object,,Y _
),,_ `
DBNull,,` f
.,,f g
Value,,g l
),,l m
;,,m n
command-- 
.-- 

Parameters-- 
.-- 
AddWithValue-- +
(--+ ,
$str--, :
,--: ;
dto--< ?
.--? @
	Apellido1--@ I
??--J L
(--M N
object--N T
)--T U
DBNull--U [
.--[ \
Value--\ a
)--a b
;--b c
command.. 
... 

Parameters.. 
... 
AddWithValue.. +
(..+ ,
$str.., :
,..: ;
dto..< ?
...? @
	Apellido2..@ I
??..J L
(..M N
object..N T
)..T U
DBNull..U [
...[ \
Value..\ a
)..a b
;..b c
command// 
.// 

Parameters// 
.// 
AddWithValue// +
(//+ ,
$str//, =
,//= >
dto//? B
.//B C
RazonSocial//C N
??//O Q
(//R S
object//S Y
)//Y Z
DBNull//Z `
.//` a
Value//a f
)//f g
;//g h
command00 
.00 

Parameters00 
.00 
AddWithValue00 +
(00+ ,
$str00, ;
,00; <
dto00= @
.00@ A
	TipoIdent00A J
??00K M
(00N O
object00O U
)00U V
DBNull00V \
.00\ ]
Value00] b
)00b c
;00c d
command11 
.11 

Parameters11 
.11 
AddWithValue11 +
(11+ ,
$str11, =
,11= >
dto11? B
.11B C
TipoPersona11C N
??11O Q
(11R S
object11S Y
)11Y Z
DBNull11Z `
.11` a
Value11a f
)11f g
;11g h
command22 
.22 

Parameters22 
.22 
AddWithValue22 +
(22+ ,
$str22, >
,22> ?
dto22@ C
.22C D
TipoProducto22D P
??22Q S
(22T U
object22U [
)22[ \
DBNull22\ b
.22b c
Value22c h
)22h i
;22i j
command33 
.33 

Parameters33 
.33 
AddWithValue33 +
(33+ ,
$str33, A
,33A B
dto33C F
.33F G
NumeroOperacion33G V
??33W Y
(33Z [
object33[ a
)33a b
DBNull33b h
.33h i
Value33i n
)33n o
;33o p
command44 
.44 

Parameters44 
.44 
AddWithValue44 +
(44+ ,
$str44, >
,44> ?
dto44@ C
.44C D
AnioPolitica44D P
??44Q S
(44T U
object44U [
)44[ \
DBNull44\ b
.44b c
Value44c h
)44h i
;44i j
command55 
.55 

Parameters55 
.55 
AddWithValue55 +
(55+ ,
$str55, ?
,55? @
dto55A D
.55D E
TipoDocumento55E R
??55S U
(55V W
object55W ]
)55] ^
DBNull55^ d
.55d e
Value55e j
)55j k
;55k l
command88 
.88 

Parameters88 
.88 
AddWithValue88 +
(88+ ,
$str88, <
,88< =
dto88> A
.88A B

NumeroEnte88B L
)88L M
;88M N
command;; 
.;; 

Parameters;; 
.;; 
AddWithValue;; +
(;;+ ,
$str;;, 7
,;;7 8
string;;9 ?
.;;? @
IsNullOrWhiteSpace;;@ R
(;;R S
dto;;S V
.;;V W
Estado;;W ]
);;] ^
?;;_ `
$char;;a d
:;;e f
dto;;g j
.;;j k
Estado;;k q
[;;q r
$num;;r s
];;s t
);;t u
;;;u v
await== 

connection== 
.== 
	OpenAsync== &
(==& '
)==' (
;==( )
var>> 
result>> 
=>> 
await>> 
command>> &
.>>& ' 
ExecuteNonQueryAsync>>' ;
(>>; <
)>>< =
;>>= >
return?? 
result?? 
>?? 
$num?? 
;?? 
}@@ 	
}AA 
publicCC 

staticCC 
classCC "
SqlParameterExtensionsCC .
{DD 
privateEE 
staticEE 
readonlyEE 
DateTimeEE  (

MinSqlDateEE) 3
=EE4 5
newEE6 9
(EE9 :
$numEE: >
,EE> ?
$numEE@ A
,EEA B
$numEEC D
,EED E
$numEEF G
,EEG H
$numEEI J
,EEJ K
$numEEL M
,EEM N
DateTimeKindEEO [
.EE[ \
UtcEE\ _
)EE_ `
;EE` a
publicGG 
staticGG 
objectGG 
ToSqlDateTimeOrNullGG 0
(GG0 1
thisGG1 5
DateTimeGG6 >
dateGG? C
)GGC D
{HH 	
returnII 
dateII 
<II 

MinSqlDateII $
?II% &
DBNullII' -
.II- .
ValueII. 3
:II4 5
dateII6 :
;II: ;
}JJ 	
publicLL 
staticLL 
objectLL 
ToSqlDateTimeOrNullLL 0
(LL0 1
thisLL1 5
DateTimeLL6 >
?LL> ?
dateLL@ D
)LLD E
{MM 	
returnNN 
dateNN 
.NN 
HasValueNN  
&&NN! #
dateNN$ (
.NN( )
ValueNN) .
>=NN/ 1

MinSqlDateNN2 <
?NN= >
dateNN? C
.NNC D
ValueNND I
:NNJ K
DBNullNNL R
.NNR S
ValueNNS X
;NNX Y
}OO 	
}PP 
}QQ ∑*
pD:\TV\temp\Core.SET.Web.Api.GetHolidays\Core.SET.Web.Api.GetHolidays.DataAccess\Exec\SP\FileRegBitacoraExexSP.cs
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
public 

class !
FileRegBitacoraExexSP &
(& '
IConfiguration' 5
configuration6 C
)C D
:E F"
IFileRegBitacoraExexSPG ]
{ 
public 
async 
Task 
< 
FileRegBitacoraDto ,
>, -'
ExecuteFileRegBitacoraAsync. I
(I J
FileRegBitacoraDtoJ \
dto] `
)` a
{ 	
var 
connectionString  
=! "
configuration# 0
.0 1
GetConnectionString1 D
(D E
$strE P
)P Q
;Q R
await 
using 
var 

connection &
=' (
new) ,
SqlConnection- :
(: ;
connectionString; K
)K L
;L M
await 
using 
var 
command #
=$ %
new& )

SqlCommand* 4
(4 5
$str5 O
,O P

connectionQ [
)[ \
{ 
CommandType 
= 
CommandType )
.) *
StoredProcedure* 9
} 
; 
command 
. 

Parameters 
. 
AddWithValue +
(+ ,
$str, :
,: ;
dto< ?
.? @
	Operacion@ I
??J L
(M N
objectN T
)T U
DBNullU [
.[ \
Value\ a
)a b
;b c
command   
.   

Parameters   
.   
AddWithValue   +
(  + ,
$str  , 8
,  8 9
dto  : =
.  = >
Proceso  > E
??  F H
(  I J
object  J P
)  P Q
DBNull  Q W
.  W X
Value  X ]
)  ] ^
;  ^ _
command!! 
.!! 

Parameters!! 
.!! 
AddWithValue!! +
(!!+ ,
$str!!, 7
,!!7 8
dto!!9 <
.!!< =
Estado!!= C
??!!D F
(!!G H
object!!H N
)!!N O
DBNull!!O U
.!!U V
Value!!V [
)!![ \
;!!\ ]
command"" 
."" 

Parameters"" 
."" 
AddWithValue"" +
(""+ ,
$str"", ;
,""; <
dto""= @
.""@ A
	DescError""A J
??""K M
(""N O
object""O U
)""U V
DBNull""V \
.""\ ]
Value""] b
)""b c
;""c d
command## 
.## 

Parameters## 
.## 
AddWithValue## +
(##+ ,
$str##, >
,##> ?
dto##@ C
.##C D
FechaProceso##D P
)##P Q
;##Q R
var&& 
oError&& 
=&& 
new&& 
SqlParameter&& )
(&&) *
$str&&* 4
,&&4 5
	SqlDbType&&6 ?
.&&? @
Int&&@ C
)&&C D
{&&E F
	Direction&&G P
=&&Q R
ParameterDirection&&S e
.&&e f
Output&&f l
}&&m n
;&&n o
var'' 
	oMsgError'' 
='' 
new'' 
SqlParameter''  ,
('', -
$str''- ;
,''; <
	SqlDbType''= F
.''F G
VarChar''G N
,''N O
$num''P S
)''S T
{''U V
	Direction''W `
=''a b
ParameterDirection''c u
.''u v
Output''v |
}''} ~
;''~ 
command(( 
.(( 

Parameters(( 
.(( 
Add(( "
(((" #
oError((# )
)(() *
;((* +
command)) 
.)) 

Parameters)) 
.)) 
Add)) "
())" #
	oMsgError))# ,
))), -
;))- .
await++ 

connection++ 
.++ 
	OpenAsync++ &
(++& '
)++' (
;++( )
await,, 
command,, 
.,,  
ExecuteNonQueryAsync,, .
(,,. /
),,/ 0
;,,0 1
dto// 
.// 
Error// 
=// 
oError// 
.// 
Value// $
==//% '
DBNull//( .
.//. /
Value/// 4
?//5 6
null//7 ;
://< =
Convert//> E
.//E F
ToInt32//F M
(//M N
oError//N T
.//T U
Value//U Z
)//Z [
;//[ \
dto00 
.00 
MsgError00 
=00 
	oMsgError00 $
.00$ %
Value00% *
==00+ -
DBNull00. 4
.004 5
Value005 :
?00; <
null00= A
:00B C
	oMsgError00D M
.00M N
Value00N S
!00S T
.00T U
ToString00U ]
(00] ^
)00^ _
!00_ `
;00` a
return66 
dto66 
;66 
}77 	
}88 
}99 ÅO
hD:\TV\temp\Core.SET.Web.Api.GetHolidays\Core.SET.Web.Api.GetHolidays.DataAccess\Exec\SP\GetDatesSyb12.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
Exec "
." #
SP# %
{ 
public 

class 
GetDatesSyb12 
(  
	IBdSybase  )
_bd* -
,- .
ILogger/ 6
<6 7
GetDatesSyb127 D
>D E
_loggerF M
)M N
:O P
IGetDatesSyb12Q _
{ 
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
>A B$
LogDataMappingSuccessfulC [
=\ ]
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel) 1
.1 2
Information2 =
,= >
new? B
EventIdC J
(J K
$numK O
,O P
nameofQ W
(W X
GetDatesSyb12X e
)e f
)f g
,g h
$str :
): ;
;; <
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
>A B
LogMappingErrorC R
=S T
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel) 1
.1 2
Error2 7
,7 8
new9 <
EventId= D
(D E
$numE I
,I J
nameofK Q
(Q R
GetDatesSyb12R _
)_ `
)` a
,a b
$str @
)@ A
;A B
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
>A B

LogSpErrorC M
=N O
LoggerMessage   
.   
Define    
<    !
string  ! '
>  ' (
(  ( )
LogLevel  ) 1
.  1 2
Error  2 7
,  7 8
new  9 <
EventId  = D
(  D E
$num  E I
,  I J
nameof  K Q
(  Q R
GetDatesSyb12  R _
)  _ `
)  ` a
,  a b
$str!! C
)!!C D
;!!D E
public&& 
async&& 
Task&& 
<&& $
GetDatesSyb12ResponseDto&& 2
>&&2 3
ExecuteSpGetDates&&4 E
(&&E F
)&&F G
{'' 	
var(( 
dsResultado(( 
=(( 
new(( !
DataSet((" )
((() *
)((* +
;((+ ,
var)) 

dataOrigin)) 
=)) 
$str)) +
;))+ ,
try++ 
{,, 
const-- 
string-- 
spName-- #
=--$ %
$str--& E
;--E F
using// 
var// 
cnn// 
=// 
await//  %
_bd//& )
.//) *!
CreateConnectionAsync//* ?
(//? @
)//@ A
;//A B
using22 
var22 
command22 !
=22" #
new22$ '
SybaseCommand22( 5
(225 6
spName226 <
,22< =
cnn22> A
)22A B
{33 
CommandType44 
=44  !
CommandType44" -
.44- .
StoredProcedure44. =
}55 
;55 
command77 
.77 

Parameters77 "
.77" #
Add77# &
(77& '
new77' *
SybaseParameter77+ :
(77: ;
$str77; G
,77G H
$num77I L
)77L M
)77M N
;77N O
command88 
.88 

Parameters88 "
.88" #
Add88# &
(88& '
new88' *
SybaseParameter88+ :
(88: ;
$str88; H
,88H I
$str88J M
)88M N
)88N O
;88O P
var:: 
adapter:: 
=:: 
new:: !
SybaseDataAdapter::" 3
(::3 4
command::4 ;
)::; <
;::< =
adapter;; 
.;; 
Fill;; 
(;; 
dsResultado;; (
);;( )
;;;) *
return== 
	MapResult==  
(==  !
dsResultado==! ,
,==, -

dataOrigin==. 8
)==8 9
;==9 :
}>> 
catch?? 
(?? 
	Exception?? 
ex?? 
)??  
{@@ 

LogSpErrorAA 
(AA 
_loggerAA "
,AA" #

dataOriginAA$ .
,AA. /
exAA0 2
)AA2 3
;AA3 4
throwBB 
newBB %
InvalidOperationExceptionBB 3
(BB3 4
$"BB4 6
$strBB6 L
{BBL M

dataOriginBBM W
}BBW X
"BBX Y
,BBY Z
exBB[ ]
)BB] ^
;BB^ _
}CC 
}DD 	
publicFF $
GetDatesSyb12ResponseDtoFF '
	MapResultFF( 1
(FF1 2
DataSetFF2 9
dsFF: <
,FF< =
stringFF> D

dataOriginFFE O
)FFO P
{GG 	
ifHH 
(HH 
dsHH 
==HH 
nullHH 
||HH 
dsHH  
.HH  !
TablesHH! '
.HH' (
CountHH( -
==HH. 0
$numHH1 2
||HH3 5
dsHH6 8
.HH8 9
TablesHH9 ?
[HH? @
$numHH@ A
]HHA B
.HHB C
RowsHHC G
.HHG H
CountHHH M
==HHN P
$numHHQ R
)HHR S
throwII 
newII %
InvalidOperationExceptionII 3
(II3 4
$strII4 \
)II\ ]
;II] ^
tryKK 
{LL 
varMM 
rowMM 
=MM 
dsMM 
.MM 
TablesMM #
[MM# $
$numMM$ %
]MM% &
.MM& '
RowsMM' +
[MM+ ,
$numMM, -
]MM- .
;MM. /
varNN 
resultNN 
=NN 
newNN  $
GetDatesSyb12ResponseDtoNN! 9
{OO 
FechaProcesoPP  
=PP! "
rowPP# &
[PP& '
$strPP' 5
]PP5 6
!=PP7 9
DBNullPP: @
.PP@ A
ValuePPA F
?PPG H
ConvertPPI P
.PPP Q

ToDateTimePPQ [
(PP[ \
rowPP\ _
[PP_ `
$strPP` n
]PPn o
,PPo p
CultureInfoPPq |
.PP| }
InvariantCulture	PP} ç
)
PPç é
:
PPè ê
(
PPë í
DateTime
PPí ö
?
PPö õ
)
PPõ ú
null
PPú †
,
PP† °
FechaDiaQQ 
=QQ 
rowQQ "
[QQ" #
$strQQ# -
]QQ- .
!=QQ/ 1
DBNullQQ2 8
.QQ8 9
ValueQQ9 >
?QQ? @
ConvertQQA H
.QQH I

ToDateTimeQQI S
(QQS T
rowQQT W
[QQW X
$strQQX b
]QQb c
,QQc d
CultureInfoQQe p
.QQp q
InvariantCulture	QQq Å
)
QQÅ Ç
:
QQÉ Ñ
(
QQÖ Ü
DateTime
QQÜ é
?
QQé è
)
QQè ê
null
QQê î
,
QQî ï
	FechaHoraRR 
=RR 
rowRR  #
[RR# $
$strRR$ /
]RR/ 0
!=RR1 3
DBNullRR4 :
.RR: ;
ValueRR; @
?RRA B
ConvertRRC J
.RRJ K

ToDateTimeRRK U
(RRU V
rowRRV Y
[RRY Z
$strRRZ e
]RRe f
,RRf g
CultureInfoRRh s
.RRs t
InvariantCulture	RRt Ñ
)
RRÑ Ö
:
RRÜ á
(
RRà â
DateTime
RRâ ë
?
RRë í
)
RRí ì
null
RRì ó
,
RRó ò
NumErrorSS 
=SS 
rowSS "
[SS" #
$strSS# .
]SS. /
!=SS0 2
DBNullSS3 9
.SS9 :
ValueSS: ?
?SS@ A
ConvertSSB I
.SSI J
ToInt32SSJ Q
(SSQ R
rowSSR U
[SSU V
$strSSV a
]SSa b
,SSb c
CultureInfoSSd o
.SSo p
InvariantCulture	SSp Ä
)
SSÄ Å
:
SSÇ É
$num
SSÑ Ö
,
SSÖ Ü
MsgErrorTT 
=TT 
rowTT "
[TT" #
$strTT# .
]TT. /
!=TT0 2
DBNullTT3 9
.TT9 :
ValueTT: ?
?TT@ A
ConvertTTB I
.TTI J
ToStringTTJ R
(TTR S
rowTTS V
[TTV W
$strTTW b
]TTb c
,TTc d
CultureInfoTTe p
.TTp q
InvariantCulture	TTq Å
)
TTÅ Ç
??
TTÉ Ö
string
TTÜ å
.
TTå ç
Empty
TTç í
:
TTì î
string
TTï õ
.
TTõ ú
Empty
TTú °
}UU 
;UU $
LogDataMappingSuccessfulWW (
(WW( )
_loggerWW) 0
,WW0 1

dataOriginWW2 <
,WW< =
nullWW> B
)WWB C
;WWC D
returnXX 
resultXX 
;XX 
}YY 
catchZZ 
(ZZ 
	ExceptionZZ 
exZZ 
)ZZ  
{[[ 
LogMappingError\\ 
(\\  
_logger\\  '
,\\' (

dataOrigin\\) 3
,\\3 4
ex\\5 7
)\\7 8
;\\8 9
throw]] 
new]] %
InvalidOperationException]] 3
(]]3 4
$"]]4 6
$str]]6 U
{]]U V

dataOrigin]]V `
}]]` a
"]]a b
,]]b c
ex]]d f
)]]f g
;]]g h
}^^ 
}__ 	
}`` 
}aa 