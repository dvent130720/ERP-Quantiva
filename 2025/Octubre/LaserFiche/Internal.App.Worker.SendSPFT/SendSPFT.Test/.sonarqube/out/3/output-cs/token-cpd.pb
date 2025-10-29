Å
\D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess.Contracts\Context\IBdSybase.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
	Contracts '
.' (
Context( /
{ 
public

 

	interface

 
	IBdSybase

 
{ 
public 
Task 
< 
SybaseConnection $
>$ %!
CreateConnectionAsync& ;
(; <
)< =
;= >
public 
Task 
< 
bool 
> 
IsHealthyAsync (
(( )
)) *
;* +
} 
} Ó
cD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess.Contracts\Context\ISendSftpContext.cs
	namespace		 	
SendSPFT		
 
.		 

DataAccess		 
.		 
	Contracts		 '
.		' (
Context		( /
{

 
public 

	interface 
ISendSftpContext %
{ 
public 
Task 
< 
NpgsqlConnection $
>$ %!
CreateConnectionAsync& ;
(; <
)< =
;= >
public 
Task 
< 
bool 
> 
IsHealthyAsync (
(( )
)) *
;* +
} 
} …
fD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess.Contracts\Exec\Func\ISelectRegistryBD.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
	Contracts '
.' (
Exec( ,
., -
Func- 1
{ 
public 

	interface 
ISelectRegistryBD &
{ 
Task 
< 
List 
< 
TdAttachmentDto !
>! "
>" #
SelectRecordAsync$ 5
(5 6
)6 7
;7 8
TdAttachmentDto 
MapReaderToDto &
(& '
NpgsqlDataReader' 7
reader8 >
)> ?
;? @
} 
} ¬
cD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess.Contracts\Exec\Func\IUpdateTableBD.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
	Contracts '
.' (
Exec( ,
., -
Func- 1
{ 
public 

	interface 
IUpdateTableBD #
{ 
Task 
UpdateTableAsync 
( 
TdAttachmentDto -
record. 4
)4 5
;5 6
} 
} ã
eD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess.Contracts\Exec\SP\IFileProcDigExecSP.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
	Contracts '
.' (
Exec( ,
., -
SP- /
{ 
public 

	interface 
IFileProcDigExecSP '
{ 
Task 
< 
bool 
> #
ExecuteFileProcDigAsync *
(* +
FileProcDigDto+ 9
dto: =
)= >
;> ?
} 
} ê
aD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.DataAccess.Contracts\Exec\SP\IGetDatesSyb12.cs
	namespace 	
SendSPFT
 
. 

DataAccess 
. 
	Contracts '
.' (
Context( /
{ 
public 

	interface 
IGetDatesSyb12 #
{		 
Task

 
<

 $
GetDatesSyb12ResponseDto

 %
>

% &
ExecuteSpGetDates

' 8
(

8 9
)

9 :
;

: ;$
GetDatesSyb12ResponseDto  
	MapResult! *
(* +
DataSet+ 2
ds3 5
,5 6
string7 =

dataOrigin> H
)H I
;I J
} 
} 