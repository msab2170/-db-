# read-email-and-write-on-db

table이라는 db 테이블이 있다고 가정하자, 
이 테이블 내 한 컬럼에 .msg 또는 .eml 파일의 경로 와 파일명을 불러와

해당 파일을 읽어 보낸이, 보낸시각, 받는이, 받은시각, CC, BCC를 
다시 table이라는 테이블의 다른 6개 컬럼에 업데이트 하는 프로그램이다.

이 프로그램 내에 db 관련이나 쿼리, 암호화 또는 복호화 관련 정보들은 모두 삭제했다.

문서번호는 가장 최근에 읽은 것까지 history_{db명}.txt에 
PREVIOUS_UPDATE_COUNT={최근에 읽은 문서번호}
로 저장되며

config.txt에서 로그레벨을 설정할 수 있다
(Debug - 쿼리까지 나온다, information - 기본 로그, warning - 이 로그가 찍히면 정상동작일 수도 있지만 에러일 수도 있음, error 확실한 에러)

c# .net core의 winform으로 제작되었고
mssql을 사용했었으나 db관련정보는 삭제했기때문에 의미는 떨어질 것으로 보인다.
로그는 serilog패키지를 이용하였고 Log/{년}/{년-월}/ 경로에 저장된다.
빌드시에는 파일이 하나로 합쳐지도록 Costura.Fody 패키지를 이용하였고 소스는 무거우나 빌드된 프로그램은 가볍다.

