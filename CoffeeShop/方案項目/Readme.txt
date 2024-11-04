{CSS}

[V]修改-所有css樣式

=====================================

{會員系統}

[Working on] add 註冊新會員
	[Working on] 實作註冊功能


[Working on] 實作 新會員 Email 確認功能
	信裡的網址，為 https://.../Members/ActiveRegister?memberId=7&confirmCode=a07e6e4e7ecd4813aad0894fd258e610 (這裡的id和後面的confirmCode要和資料庫裡的資料對應)


[Working on] 實作登入登出功能
	只有帳密正確且開通會員才允許登入

[Working on]要做 Members / Index 會員中心頁，登入成功之後，導向此頁


[Working on]實作 修改個人基本資料
	不允許修改 email,password; 讓使用者修改姓名、性別、電話


[Working on] 實作 變更密碼
	要加[[Authorize]

[Working on] 實作 忘記密碼/重設密碼
	暫時設立 /files/ folder 用來存放 Email 內容
	

=======================================
{前台購物車}

[Working on]顯示商品清單
	寫js, 實作加入購物車功能
	只有在登入狀態下才能加入購物車
	將網站啟始頁面改為Products/Index
	修改 _Layout.cshtml裡，首頁的超連結

[Working on]add CartController, 實作將商品加入購物車
	[Working on] 顯示購物車資訊，實作增減數量的功能
	[Working on]實作 結帳作業

=======================================
{後台功能}


=======================================
測試123456789