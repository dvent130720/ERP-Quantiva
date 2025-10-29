É
ZD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Config\AppConfiguration.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Config  &
{ 
public 

class 
AppConfiguration !
{ 
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
AppType 
{ 
get  #
;# $
set% (
;( )
}* +
}		 
}

 è
[D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Config\HealthCheckConfig.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Config  &
{ 
public 

static 
class 
HealthCheckConfig )
{ 
public 
static 
IApplicationBuilder )
AddRegistration* 9
(9 :
this: >
IApplicationBuilder? R
appS V
)V W
{ 	
app 
. 
UseHealthChecks 
(  
$str  )
,) *
new+ .
HealthCheckOptions/ A
{ 
ResponseWriter 
=  
async! &
(' (
context( /
,/ 0
report1 7
)7 8
=>9 ;
{ 
context 
. 
Response $
.$ %
ContentType% 0
=1 2
$str3 E
;E F
var 
response  
=! "
new# &
HealthCheckResponse' :
{ 
Status 
=  
report! '
.' (
Status( .
.. /
ToString/ 7
(7 8
)8 9
,9 :
HealthChecks $
=% &
report' -
.- .
Entries. 5
.5 6
Select6 <
(< =
x= >
=>? A
newB E)
IndividualHealthCheckResponseF c
{ 
	Component %
=& '
x( )
.) *
Key* -
,- .
Status "
=# $
x% &
.& '
Value' ,
., -
Status- 3
.3 4
ToString4 <
(< =
)= >
,> ?
Description   '
=  ( )
x  * +
.  + ,
Value  , 1
.  1 2
Description  2 =
}!! 
)!! 
,!! 
HealthCheckDuration"" +
="", -
report"". 4
.""4 5
TotalDuration""5 B
.""B C
ToString""C K
(""K L
)""L M
}## 
;## 
await$$ 
context$$ !
.$$! "
Response$$" *
.$$* +

WriteAsync$$+ 5
($$5 6
JsonSerializer$$6 D
.$$D E
	Serialize$$E N
($$N O
response$$O W
)$$W X
)$$X Y
;$$Y Z
}%% 
}&& 
)&& 
;&& 
return(( 
app(( 
;(( 
}** 	
}++ 
},, ô
gD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Healthchecks\HttpEndpointHealthCheck.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Healthchecks  ,
{ 
public 

class #
HttpEndpointHealthCheck (
() *
IHttpClientFactory* <
httpClientFactory= N
,N O
stringP V
urlW Z
)Z [
:\ ]
IHealthCheck^ j
{ 
public 
async 
Task 
< 
HealthCheckResult +
>+ ,
CheckHealthAsync- =
(= >
HealthCheckContext 
context &
,& '
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
try 
{ 
var 
client 
= 
httpClientFactory .
.. /
CreateClient/ ;
(; <
)< =
;= >
var 
response 
= 
await $
client% +
.+ ,
GetAsync, 4
(4 5
url5 8
,8 9
cancellationToken: K
)K L
;L M
if 
( 
response 
. 
IsSuccessStatusCode 0
)0 1
{ 
return 
HealthCheckResult ,
., -
Healthy- 4
(4 5
$str5 O
)O P
;P Q
} 
return 
HealthCheckResult (
.( )
	Unhealthy) 2
(2 3
$"3 5
$str5 J
{J K
responseK S
.S T

StatusCodeT ^
}^ _
"_ `
)` a
;a b
} 
catch 
( 
	Exception 
ex 
)  
{   
return!! 
HealthCheckResult!! (
.!!( )
	Unhealthy!!) 2
(!!2 3
$"!!3 5
$str!!5 @
{!!@ A
ex!!A C
.!!C D
Message!!D K
}!!K L
"!!L M
)!!M N
;!!N O
}"" 
}## 	
}$$ 
}&& ¥
mD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Helpers\Contracts\IRequestContextExtractor.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Helpers  '
.' (
	Contracts( 1
{ 
public		 

	interface		 +
IRequestContextHeadersExtractor		 4
{

 !
RequestContextHeaders 
Extract %
(% &
HttpContext& 1
httpContext2 =
)= >
;> ?
} 
} –
cD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Helpers\Contracts\IRequestLogger.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Helpers  '
.' (
	Contracts( 1
{ 
public

 

	interface

 
IRequestLogger

 #
{ 
void 

LogRequest 
( 
object 
body #
,# $!
RequestContextHeaders% :
context; B
,B C
ILoggerD K
loggerL R
)R S
;S T
void 

LogSuccess 
( !
RequestContextHeaders -
context. 5
)5 6
;6 7
void 
LogError 
( !
RequestContextHeaders +
context, 3
,3 4
	Exception5 >
ex? A
)A B
;B C
} 
} ê	
lD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Helpers\Contracts\IServiceResponseFactory.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Helpers  '
.' (
	Contracts( 1
{ 
public 

	interface #
IServiceResponseFactory ,
{ 
ServiceResponse 
< 
T 
> 
Success "
<" #
T# $
>$ %
(% &
T& '
data( ,
,, -!
RequestContextHeaders. C
ctxD G
)G H
whereI N
TO P
:Q R
classS X
;X Y
ServiceResponse 
< 
T 
> 
Fail 
<  
T  !
>! "
(" #
string# )
code* .
,. /
string0 6
message7 >
,> ?!
RequestContextHeaders@ U
ctxV Y
)Y Z
where[ `
Ta b
:c d
classe j
;j k
} 
} ∆
bD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Helpers\RequestContextExtractor.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Helpers  '
{ 
public 

class *
RequestContextHeadersExtractor /
(/ 0
IVoucherNumber0 >
voucherGenerator? O
)O P
:Q R+
IRequestContextHeadersExtractorS r
{ 
public !
RequestContextHeaders $
Extract% ,
(, -
HttpContext- 8
httpContext9 D
)D E
{ 	
string 
	GetHeader 
( 
string #
key$ '
)' (
=>) +
httpContext, 7
.7 8
Request8 ?
.? @
Headers@ G
.G H
TryGetValueH S
(S T
keyT W
,W X
outY \
var] `
valuea f
)f g
?h i
valuej o
.o p
ToStringp x
(x y
)y z
:{ |
string	} É
.
É Ñ
Empty
Ñ â
;
â ä
return 
new !
RequestContextHeaders ,
{ 
	SessionId 
= 
	GetHeader %
(% &
$str& 3
)3 4
,4 5
TransactionId 
= 
	GetHeader  )
() *
$str* ;
); <
,< =
	ChannelId 
= 
	GetHeader %
(% &
$str& 3
)3 4
,4 5
I18n 
= 
	GetHeader  
(  !
$str! )
)) *
,* +
	ServiceId 
= 
	GetHeader %
(% &
$str& 3
)3 4
,4 5
VoucherNumber 
= 
voucherGenerator  0
.0 1!
GenerateVoucherNumber1 F
(F G
)G H
?H I
.I J
NumeroComprobanteJ [
??\ ^
$str_ b
} 
; 
} 	
}   
}!! « 
XD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Helpers\RequestLogger.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Helpers  '
{ 
public 

class 
RequestLogger 
( 
IBitacoraLog +
bitacoraLog, 7
)7 8
:9 :
IRequestLogger; I
{ 
private 
const 
string 
Producto %
=& '
$str( .
;. /
private 
const 
string 
Usuario $
=% &
$str' J
;J K
public 
void 

LogRequest 
( 
object %
body& *
,* +!
RequestContextHeaders, A
contextB I
,I J
ILoggerK R
loggerS Y
)Y Z
{ 	
bitacoraLog 
. 
	Registrar !
(! "
	CreateLog" +
(+ ,
context, 3
,3 4
$str5 I
,I J
$strK N
,N O
$strP T
)T U
)U V
;V W
} 	
public 
void 

LogSuccess 
( !
RequestContextHeaders 4
context5 <
)< =
{ 	
bitacoraLog   
.   
	Registrar   !
(  ! "
	CreateLog  " +
(  + ,
context  , 3
,  3 4
$str  5 A
,  A B
$str  C F
,  F G
$str  H L
)  L M
)  M N
;  N O
}!! 	
public## 
void## 
LogError## 
(## !
RequestContextHeaders## 2
context##3 :
,##: ;
	Exception##< E
ex##F H
)##H I
{$$ 	
bitacoraLog%% 
.%% 
	Registrar%% !
(%%! "
	CreateLog%%" +
(%%+ ,
context%%, 3
,%%3 4
$str%%5 <
,%%< =
$str%%> D
,%%D E
ex%%F H
.%%H I
Message%%I P
)%%P Q
)%%Q R
;%%R S
}&& 	
private(( 
static(( 
BitacoraLogDto(( %
	CreateLog((& /
(((/ 0!
RequestContextHeaders((0 E
context((F M
,((M N
string((O U
estado((V \
,((\ ]
string((^ d
codError((e m
,((m n
string((o u
	descError((v 
)	(( Ä
{)) 	
return** 
new** 
BitacoraLogDto** %
(**% &
)**& '
.++ 
WithProducto++ 
(++ 
Producto++ &
)++& '
.,, 
WithIdServicio,, 
(,,  
$str,,  )
),,) *
.-- 
WithIdTransaccion-- "
(--" #
context--# *
.--* +
TransactionId--+ 8
)--8 9
... #
WithIdentificadorSesion.. (
(..( )
context..) 0
...0 1
	SessionId..1 :
)..: ;
.// 
	WithCanal// 
(// 
context// "
.//" #
	ChannelId//# ,
)//, -
.00 
WithUsuario00 
(00 
Usuario00 $
)00$ %
.11 
WithNumComprobante11 #
(11# $
context11$ +
.11+ ,
VoucherNumber11, 9
)119 :
.22 
WithCodError22 
(22 
codError22 &
)22& '
.33 
WithDescError33 
(33 
	descError33 (
)33( )
.44 

WithEstado44 
(44 
estado44 "
)44" #
.55 
	WithFecha55 
(55 
DateTime55 #
.55# $
Now55$ '
.55' (
ToString55( 0
(550 1
$str551 F
,55F G
CultureInfo55H S
.55S T
InvariantCulture55T d
)55d e
)55e f
;55f g
}66 	
}77 
}88 ÷
aD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Helpers\ServiceResponseFactory.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Helpers  '
{ 
public 

class "
ServiceResponseFactory (
:) *#
IServiceResponseFactory+ B
{ 
public 
ServiceResponse 
<  
T  !
>! "
Success# *
<* +
T+ ,
>, -
(- .
T. /
data0 4
,4 5!
RequestContextHeaders6 K
ctxL O
)O P
whereQ V
TW X
:Y Z
class[ `
{ 	
return 
new 
ServiceResponse &
<& '
T' (
>( )
{ 
Data 
= 
data 
, 
	Succeeded 
= 
true  
,  !
	SessionId 
= 
ctx 
.  
	SessionId  )
,) *
TransactionId 
= 
ctx  #
.# $
TransactionId$ 1
,1 2
Errors 
= 
new 
List !
<! "
ErrorDetail" -
>- .
{ 
new 
ErrorDetail 
{  !
Code" &
=' (
$str) ,
,, -
Message. 5
=6 7
$str8 <
}= >
} 
} 
; 
} 	
public 
ServiceResponse 
<  
T  !
>! "
Fail# '
<' (
T( )
>) *
(* +
string+ 1
code2 6
,6 7
string8 >
message? F
,F G!
RequestContextHeadersH ]
ctx^ a
)a b
wherec h
Ti j
:k l
classm r
{   	
return!! 
new!! 
ServiceResponse!! &
<!!& '
T!!' (
>!!( )
{"" 
	Succeeded## 
=## 
false## !
,##! "
	SessionId$$ 
=$$ 
ctx$$ 
.$$  
	SessionId$$  )
,$$) *
TransactionId%% 
=%% 
ctx%%  #
.%%# $
TransactionId%%$ 1
,%%1 2
Errors&& 
=&& 
new&& 
List&& !
<&&! "
ErrorDetail&&" -
>&&- .
{'' 
new(( 
ErrorDetail(( 
{((  !
Code((" &
=((' (
code(() -
,((- .
Message((/ 6
=((7 8
message((9 @
}((A B
})) 
}** 
;** 
}++ 	
},, 
}.. ì
^D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Helpers\SwaggerHeaderFilter.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Helpers  '
{ 
public 

class 
SwaggerHeaderFilter %
:& '
IOperationFilter( 8
{ 
public 
void 
Apply 
( 
OpenApiOperation *
	operation+ 4
,4 5"
OperationFilterContext6 L
contextM T
)T U
{ 	
	operation 
. 

Parameters  
??=! $
new% (
List) -
<- .
OpenApiParameter. >
>> ?
(? @
)@ A
;A B
	operation 
. 

Parameters  
.  !
Add! $
($ %
new% (
OpenApiParameter) 9
{ 
Name 
= 
$str %
,% &
In 
= 
ParameterLocation &
.& '
Header' -
,- .
Required 
= 
true 
,  
Schema 
= 
new 
OpenApiSchema *
{+ ,
Type- 1
=2 3
$str4 <
}= >
} 
) 
; 
	operation 
. 

Parameters  
.  !
Add! $
($ %
new% (
OpenApiParameter) 9
{ 
Name 
= 
$str )
,) *
In 
= 
ParameterLocation &
.& '
Header' -
,- .
Required 
= 
true 
,  
Schema   
=   
new   
OpenApiSchema   *
{  + ,
Type  - 1
=  2 3
$str  4 <
}  = >
}!! 
)!! 
;!! 
	operation## 
.## 

Parameters##  
.##  !
Add##! $
(##$ %
new##% (
OpenApiParameter##) 9
{$$ 
Name%% 
=%% 
$str%% "
,%%" #
In&& 
=&& 
ParameterLocation&& &
.&&& '
Header&&' -
,&&- .
Required'' 
='' 
true'' 
,''  
Schema(( 
=(( 
new(( 
OpenApiSchema(( *
{((+ ,
Type((- 1
=((2 3
$str((4 <
}((= >
})) 
))) 
;)) 
	operation++ 
.++ 

Parameters++  
.++  !
Add++! $
(++$ %
new++% (
OpenApiParameter++) 9
{,, 
Name-- 
=-- 
$str-- 
,--  
In.. 
=.. 
ParameterLocation.. &
...& '
Header..' -
,..- .
Required// 
=// 
false//  
,//  !
Schema00 
=00 
new00 
OpenApiSchema00 *
{00+ ,
Type00- 1
=002 3
$str004 <
}00= >
}11 
)11 
;11 
}22 	
}33 
}44 ‡%
ZD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Mapper\AttachmentMapper.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Mapper  &
{ 
public 

class 
AttachmentMapper !
(! "
IConfiguration" 0
config1 7
,7 8
IVoucherNumber9 G
voucherH O
)O P
:Q R
IAttachmentMapperS d
{ 
public 
FileProcDigDto 
MapToProcDig *
(* +
Cliente+ 2
datoscliente3 ?
,? @
SftpUploadResultA Q
resultR X
,X Y
TdAttachmentDtoZ i
recordj p
,p q%
GetDatesSyb12ResponseDto	r ä
dates
ã ê
)
ê ë
{ 	
var 
	operacion 
= 
config "
." #

GetSection# -
(- .
$str. <
)< =
.= >
Value> C
??D F
stringG M
.M N
EmptyN S
;S T
var 
codigoSistema 
= 
config  &
.& '
GetValue' /
</ 0
int0 3
>3 4
(4 5
$str5 B
)B C
;C D
var 
num 
= 
voucher 
. !
GenerateVoucherNumber 3
(3 4
)4 5
;5 6
var 
typeProduct 
= 
config $
.$ %

GetSection% /
(/ 0
$str0 >
)> ?
.? @
Value@ E
;E F
return 
new 
FileProcDigDto %
{ 
	Operacion 
= 
	operacion %
,% &
CodigoSistema 
= 
codigoSistema  -
,- .
IdSolicitud 
= 
num !
.! "
NumeroComprobante" 3
,3 4
User   
=   
datoscliente   #
.  # $
Identificacion  $ 2
,  2 3
FechaProceso!! 
=!! 
(!!  
DateTime!!  (
)!!( )
dates!!) .
.!!. /
FechaProceso!!/ ;
,!!; <
	FechaHora"" 
="" 
("" 
DateTime"" %
)""% &
dates""& +
.""+ ,
	FechaHora"", 5
,""5 6
FechaEjecucion## 
=##  
(##! "
DateTime##" *
)##* +
dates##+ 0
.##0 1
FechaDia##1 9
,##9 :
IdDoc$$ 
=$$ 
record$$ 
.$$ 
TdReportName$$ +
,$$+ ,
IdDocDef%% 
=%% 
num%% 
.%% 
NumeroComprobante%% 0
,%%0 1
NombreCompleto&& 
=&&  
$"&&! #
{&&# $
datoscliente&&$ 0
.&&0 1
PrimerNombre&&1 =
}&&= >
$str&&> ?
{&&? @
datoscliente&&@ L
.&&L M
SegundoNombre&&M Z
}&&Z [
"&&[ \
,&&\ ]
	Apellido1'' 
='' 
datoscliente'' (
.''( )
PrimerApellido'') 7
,''7 8
	Apellido2(( 
=(( 
datoscliente(( (
.((( )
SegundoApellido(() 8
,((8 9
RazonSocial)) 
=)) 
null)) "
,))" #
	TipoIdent** 
=** 
record** "
.**" #
TdTipoIdent**# .
,**. /
NumeroCedula++ 
=++ 
record++ %
.++% &
TdCedula++& .
,++. /
TipoPersona,, 
=,, 
datoscliente,, *
.,,* +
TipoCliente,,+ 6
,,,6 7
TipoProducto-- 
=-- 
typeProduct-- *
,--* +
NumeroOperacion.. 
=..  !
record.." (
...( )
TdIdOperation..) 6
,..6 7
AnioPolitica// 
=// 
null// #
,//# $

NumeroEnte11 
=11 
int11  
.11  !
Parse11! &
(11& '
datoscliente11' 3
.113 4
Ente114 8
,118 9
CultureInfo11: E
.11E F
InvariantCulture11F V
)11V W
,11W X
TipoDocumento33 
=33 
record33  &
.33& '
TdFormat33' /
,33/ 0
Estado44 
=44 
record44 
.44  
TdOperacion44  +
,44+ ,
FechaCreditoMovil55 !
=55" #
null55$ (
}66 
;66 
}77 	
}:: 
}<< ∏
[D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Mapper\IAttachmentMapper.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Mapper  &
{ 
public 

	interface 
IAttachmentMapper &
{ 
FileProcDigDto 
MapToProcDig #
(# $
Cliente$ +
datoscliente, 8
,8 9
SftpUploadResult: J
resultK Q
,Q R
TdAttachmentDtoS b
recordc i
,i j%
GetDatesSyb12ResponseDto	k É
dates
Ñ â
)
â ä
;
ä ã
} 
} á
ZD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Metrics\MetricCollector.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Metrics  '
{ 
public 

class 
MetricCollector  
{ 
private 
static 
readonly 
string  &
[& '
]' (
_labelNames) 4
=5 6
[7 8
$str8 E
,E F
$strG O
]O P
;P Q
private

 
readonly

 
Counter

  
_requestCounter

! 0
;

0 1
private 
readonly 
Counter  
_errorsEncountered! 3
;3 4
private 
readonly 
	Histogram ""
_responseTimeHistogram# 9
;9 :
public 
MetricCollector 
( 
)  
{ 	
_requestCounter 
= 

Prometheus (
.( )
Metrics) 0
.0 1
CreateCounter1 >
(> ?
$str *
,* +
$str D
)D E
;E F
_errorsEncountered 
=  

Prometheus! +
.+ ,
Metrics, 3
.3 4
CreateCounter4 A
(A B
$str 
, 
$str J
)J K
;K L"
_responseTimeHistogram "
=# $

Prometheus% /
./ 0
Metrics0 7
.7 8
CreateHistogram8 G
(G H
$str *
,* +
$str L
,L M
new "
HistogramConfiguration *
{ 
Buckets 
= 
	Histogram '
.' (
ExponentialBuckets( :
(: ;
$num; ?
,? @
$numA B
,B C
$numD F
)F G
,G H

LabelNames 
=  
_labelNames! ,
} 
) 
; 
}   	
public"" 
void"" 
RegisterRequest"" #
(""# $
)""$ %
=>""& (
_requestCounter"") 8
.""8 9
Inc""9 <
(""< =
)""= >
;""> ?
public$$ 
void$$ 
RegisterError$$ !
($$! "
)$$" #
=>$$$ &
_errorsEncountered$$' 9
.$$9 :
Inc$$: =
($$= >
)$$> ?
;$$? @
public&& 
void&&  
RegisterResponseTime&& (
(&&( )
int&&) ,

statusCode&&- 7
,&&7 8
string&&9 ?
method&&@ F
,&&F G
TimeSpan&&H P
elapsed&&Q X
)&&X Y
{'' 	"
_responseTimeHistogram(( "
.((" #
Labels((# )
((() *

statusCode((* 4
.((4 5
ToString((5 =
(((= >
CultureInfo((> I
.((I J
InvariantCulture((J Z
)((Z [
,(([ \
method((] c
)((c d
.((d e
Observe((e l
(((l m
elapsed((m t
.((t u
TotalSeconds	((u Å
)
((Å Ç
;
((Ç É
})) 	
}** 
}++ ≈
oD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Models\HealthCheckModels\HealthCheckResponse.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Models  &
.& '
HealthCheckModels' 8
{ 
public

 

class

 
HealthCheckResponse

 $
{ 
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
public 
IEnumerable 
< )
IndividualHealthCheckResponse 8
>8 9
HealthChecks: F
{G H
getI L
;L M
setN Q
;Q R
}S T
public 
string 
HealthCheckDuration )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
} 
} Ä
yD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Models\HealthCheckModels\IndividualHealthCheckResponse.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Models  &
.& '
HealthCheckModels' 8
{ 
public

 

class

 )
IndividualHealthCheckResponse

 .
{ 
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
	Component 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
Description !
{" #
get$ '
;' (
set) ,
;, -
}. /
} 
} ÷
dD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Subservices\FileProcessingService.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Subservices  +
{ 
public 

class !
FileProcessingService &
:' ("
IFileProcessingService) ?
{ 
private 
readonly 
ISftpFileUploader *
	_uploader+ 4
;4 5
private 
readonly 
IFileProcDigExecSP +

_spService, 6
;6 7
private 
readonly 
IAttachmentMapper *
_mapper+ 2
;2 3
private 
readonly 
IGetDatesSyb12 '
_sybase( /
;/ 0
public !
FileProcessingService $
($ %
ISftpFileUploader 
uploader &
,& '
IFileProcDigExecSP 
	spService (
,( )
IAttachmentMapper 
mapper $
,$ %
IGetDatesSyb12 
sybase !
)! "
{ 	
	_uploader 
= 
uploader  
;  !

_spService 
= 
	spService "
;" #
_mapper 
= 
mapper 
; 
_sybase   
=   
sybase   
;   
}!! 	
public## 
async## 
Task## 
ProcessAsync## &
(##& '
TdAttachmentDto##' 6
record##7 =
,##= >
Cliente##? F
data##G K
)##K L
{$$ 	
var%% 
result%% 
=%% 
await%% 
	_uploader%% (
.%%( )
UploadFileAsync%%) 8
(%%8 9
record%%9 ?
.%%? @
TdFormat%%@ H
,%%H I
record%%J P
.%%P Q
TdBase64%%Q Y
,%%Y Z
record%%[ a
.%%a b
TdReportName%%b n
)%%n o
;%%o p
if&& 
(&& 
result&& 
.&& 
Success&& 
)&& 
{'' 
var** 
dates** 
=** 
await** !
_sybase**" )
.**) *
ExecuteSpGetDates*** ;
(**; <
)**< =
;**= >
var++ 
procDto++ 
=++ 
_mapper++ %
.++% &
MapToProcDig++& 2
(++2 3
data++3 7
,++7 8
result++9 ?
,++? @
record++A G
,++G H
dates++I N
)++N O
;++O P
await,, 

_spService,,  
.,,  !#
ExecuteFileProcDigAsync,,! 8
(,,8 9
procDto,,9 @
),,@ A
;,,A B
}-- 
}.. 	
}// 
}00 ™
eD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Subservices\IFileProcessingService.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Subservices  +
{ 
public 

	interface "
IFileProcessingService +
{ 
Task 
ProcessAsync 
( 
TdAttachmentDto )
record* 0
,0 1
Cliente2 9
data: >
)> ?
;? @
} 
} Í
]D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Subservices\IProcessRecord.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Subservices  +
{ 
public 

	interface 
IProcessRecord #
{ 
Task 
ProcessRecordAsync 
(  
TdAttachmentDto  /
record0 6
)6 7
;7 8
} 
}		 ™%
\D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.CrossCutting\Subservices\ProcessRecord.cs
	namespace 	
SendSPFT
 
. 
CrossCutting 
.  
Subservices  +
{ 
public 

class 
ProcessRecord 
( 
ILogger 
< 
ProcessRecord 
> 
logger %
,% &$
IQueryCustomerDataClient  
queryCustomerData! 2
,2 3
IUpdateTableBD 
rds 
, "
IFileProcessingService 
fileProcessing -
,- .
IConfiguration 
config 
) 
:  
IProcessRecord! /
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
>A B
_logInfoC K
=L M
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
$num 
, 
nameof %
(% &
_logInfo& .
). /
)/ 0
,0 1
$str 
) 
; 
private 
static 
readonly 
Action  &
<& '
ILogger' .
,. /
string0 6
,6 7
	Exception8 A
>A B
_logWarningC N
=O P
LoggerMessage 
. 
Define  
<  !
string! '
>' (
(( )
LogLevel 
. 
Warning  
,  !
new   
EventId   
(   
$num   
,   
nameof   %
(  % &
_logWarning  & 1
)  1 2
)  2 3
,  3 4
$str!! 
)!! 
;!! 
private## 
static## 
readonly## 
Action##  &
<##& '
ILogger##' .
,##. /
	Exception##0 9
>##9 :
	_logError##; D
=##E F
LoggerMessage$$ 
.$$ 
Define$$  
($$  !
LogLevel%% 
.%% 
Error%% 
,%% 
new&& 
EventId&& 
(&& 
$num&& 
,&& 
nameof&& %
(&&% &
	_logError&&& /
)&&/ 0
)&&0 1
,&&1 2
$str'' /
)''/ 0
;''0 1
public** 
async** 
Task** 
ProcessRecordAsync** ,
(**, -
TdAttachmentDto**- <
record**= C
)**C D
{++ 	
try,, 
{-- 
_logInfo.. 
(.. 
logger.. 
,..  
$"..! #
$str..# 7
{..7 8
record..8 >
...> ?
TdId..? C
}..C D
"..D E
,..E F
null..G K
)..K L
;..L M
var00 
customerData00  
=00! "
await00# (
queryCustomerData00) :
.00: ;
GetDataClient00; H
(00H I
record00I O
)00O P
;00P Q
if11 
(11 
customerData11  
==11! #
null11$ (
)11( )
{22 
_logWarning33 
(33  
logger33  &
,33& '
$str33( Q
,33Q R
null33S W
)33W X
;33X Y
return44 
;44 
}55 
await88 
fileProcessing88 $
.88$ %
ProcessAsync88% 1
(881 2
record882 8
,888 9
customerData88: F
)88F G
;88G H
record99 
.99 
TdOperacion99 "
=99# $
config99% +
.99+ ,

GetSection99, 6
(996 7
$str997 G
)99G H
.99H I
Value99I N
;99N O
record:: 
.:: !
TdOperacionFechaEnvio:: ,
=::- .
DateTime::/ 7
.::7 8
Now::8 ;
;::; <
await;; 
rds;; 
.;; 
UpdateTableAsync;; *
(;;* +
record;;+ 1
);;1 2
;;;2 3
}<< 
catch== 
(== 
	Exception== 
ex== 
)==  
{>> 
	_logError?? 
(?? 
logger??  
,??  !
ex??" $
)??$ %
;??% &
}@@ 
}AA 	
}BB 
}CC 