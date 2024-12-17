# '마타유' 백엔드 서버 애플리케이션 레포지토리
'마타유' 백엔드 서버: .NET Web API  

# 데이터베이스

### MySQL DB 준비사항 :

스키마 이름: `MatajuApi`

~~~
-- SQL 예시:
-- 사용자 생성 및 권한 부여
CREATE USER 'matajuapi-user'@'localhost' IDENTIFIED BY '1234';


-- DB 생성 권한
GRANT CREATE ON *.* TO 'matajuapi-user'@'localhost';

-- MatajuApi 데이터베이스에 대해 ALTER, CREATE, SELECT, INSERT, UPDATE, DELETE 권한 부여
GRANT ALTER , CREATE, SELECT, INSERT, UPDATE, DELETE ON MatajuApi.* TO 'matajuapi-user'@'localhost';


-- 권한적용
FLUSH PRIVILEGES;
~~~

### 테이블 생성 with EF Core Tools

마이그레이션 update:

~~~
dotnet ef database update --project MatajuApi
~~~

주의: 프로젝트 닷넷 버전과 터미멀 EF Core Tools 버전을 맞추고 실행.

* CLI 툴 버전: EF Core Tools v 8.0.11
