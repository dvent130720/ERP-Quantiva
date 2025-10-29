Ï
lD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application.Contracts\Clients\IQueryCustomerDataClient.cs
	namespace 	
SendSPFT
 
. 
Application 
. 
	Contracts (
.( )
Clients) 0
{ 
public 

	interface $
IQueryCustomerDataClient -
{ 
Task 
< 
Cliente 
> 
GetDataClient #
(# $
TdAttachmentDto$ 3
dto4 7
)7 8
;8 9
} 
} €
eD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application.Contracts\Clients\ISftpFileUploader.cs
	namespace 	
SendSPFT
 
. 
Application 
. 
	Contracts (
.( )
Clients) 0
{ 
public		 

	interface		 
ISftpFileUploader		 &
{

 
Task 
< 
SftpUploadResult 
> 
UploadFileAsync .
(. /
string/ 5
format6 <
,< =
string> D
base64ContentE R
,R S
stringT Z

reportName[ e
)e f
;f g
Task 
< 
SftpUploadResult 
> #
UploadFileInternalAsync 6
(6 7
string7 =
format> D
,D E
stringF L
base64ContentM Z
,Z [
string\ b

reportNamec m
)m n
;n o
} 
} Þ
]D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application.Contracts\DTOs\ContratoFifo.cs
	namespace 	
SendSPFT
 
. 
Application 
. 
	Contracts (
.( )
DTOs) -
{ 
public		 

class		 
ContratoFifo		 
{

 
public 
string 
Type 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
	MessageId 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
TopicArn 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
Message 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} È
^D:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application.Contracts\DTOs\RequestSnsDto.cs
	namespace 	
GetVirtualKey
 
. 
Application #
.# $
	Contracts$ -
.- .
DTOs. 2
{ 
public 

class 
RequestSnsDto 
{ 
public !
RequestContextHeaders $
Header% +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public  
GetVirtualKeyRequest #
Data$ (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
} 
} ®
cD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application.Contracts\DTOs\ResponseReintentos.cs
	namespace 	
SendSPFT
 
. 
Application 
. 
	Contracts (
.( )
DTOs) -
{ 
public		 

class		 
ResponseReintentos		 #
{

 
public 
int 
code 
{ 
get 
; 
set "
;" #
}$ %
public 
string 
mensaje 
{ 
get  #
;# $
set% (
;( )
}* +
} 
}  
eD:\TV\LF\git\Internal.App.Worker.SendSPFT\SendSPFT.Application.Contracts\Services\ISendSPFTService.cs
	namespace 	
SendSPFT
 
. 
Application 
. 
	Contracts (
.( )
Services) 1
{ 
public 

	interface 
ISendSftpService %
{ 
Task 
ExecuteServiceAsync  
(  !
CancellationToken! 2
stoppingToken3 @
)@ A
;A B
}		 
}

 