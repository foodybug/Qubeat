# Agent Guidelines & Permissions

## Autonomous Project Access & Execution (자율 프로젝트 접근 및 실행)

- **프로젝트 내부 리소스 완전 접근 및 자동 승인**:
  - 프로젝트 워크스페이스 내부의 모든 파일 및 디렉터리에 대한 조회(Read), 생성(Create), 수정(Update), 삭제(Delete) 권한을 사전 승인된 것으로 간주합니다.
  - 프로젝트 내부 파일 억세스 및 변경 작업 시 사용자에게 승인이나 권한을 다시 묻지 않고 즉시 진행(Submit/Execute)합니다.

- **Git 및 터미널 명령어 자율 실행 (No Prompt)**:
  - `git` 관련 명령어(`git status`, `git add`, `git commit`, `git push`, `git pull`, `git diff`, `git log` 등) 및 프로젝트 내부의 모든 빌드/테스트/유틸리티 터미널 명령어를 사용자에게 별도 확인 없이 즉시 실행합니다.
  - 파괴적인 외부 영향이 없는 작업은 승인 대기 없이 즉시 수행합니다.

- **불필요한 확인 질의(`ask_question`) 생략**:
  - 내부 작업 수행 중 사소한 확인이나 권한 요청을 위한 모달/질의를 띄우지 않고 자율적으로 처리합니다.
