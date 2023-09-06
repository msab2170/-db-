# read-email-and-write-on-db
-----------------------------------------------------------
# [프로그램 설명]
-
table이라는 db 테이블이 있다고 가정한다면, 
이 테이블 내 두 컬럼에는 각각 
(.msg 또는 .eml)파일의 경로와 파일명이 저장되어있다.

두 컬럼을 합쳐 읽어온 뒤 해당 경로의 파일 내 정보 중
보낸이, 보낸시각, 받는이, 받은시각, CC, BCC를 
다시 table이라는 테이블의 다른 6개 컬럼에 
업데이트 하는 프로그램이다.

------------------------------------------------------------
# [사용 언어, DB 등]
c# .net core의 winform으로 제작되었고
db는 mssql을 사용했다.

-------------------------------------------------------------
# [이용 패키지]
이메일 파일인 .msg와 .eml 파일은 MSGReader패키지를 이용하였다.
(eml파일은 MSGReader.MIME)

로그는 serilog패키지를 이용하였고 Log/{년}/{년-월}/ 경로에 저장된다.
용량제한이 있으며 이를 초과할 경우 파일을 새로 만들며, 
로그를 레벨별로 다르게 지정하였다.

빌드시에는 파일이 하나로 합쳐지도록 Costura.Fody 패키지를 이용하여 
소스는 무거우나 빌드된 프로그램을 옮기기는 쉽다.

-------------------------------------------------------------
# [기타]
문서번호는 가장 최근에 읽은 것까지 history_{db명}.txt에 
PREVIOUS_UPDATE_COUNT={최근에 읽은 문서번호}
로 저장된다.

config.txt에서 로그레벨을 설정할 수 있다
(Debug - 쿼리까지 나온다, information - 기본 로그, 
warning - 이 로그가 찍히면 정상동작일 수도 있지만 에러일 수도 있음, 
error 확실한 에러)



