과제 제출 프로젝트에 대한 간단한 설명 첨부합니다.

샘플 Ticker, OrderBook, TradeHistory 생성 개수는 TradeDataRepository 상단에 constants 값 수정하시면 됩니다.

테스트용으로 자동 매매 시뮬레이션(TradeSimulator)이 있는데 Program.cs에 해당 주석을 해제하고 테스트해보실 수 있습니다.

백엔드의 TradeHubs와 프론트엔드에 SignalRService를 통해 SignalR로 통신하여 데이터 송수신을 하고
WindowService를 통해 프론트엔드의 UI 호출 및 관리를 했습니다.

감사합니다.