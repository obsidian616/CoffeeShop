{前台功能}
--------------------------------------------------------------
程負責-首頁版面、購物車、留言板

[V]顯示商品清單
	寫js, 實作加入購物車功能
	只有在登入狀態下才能加入購物車
	將網站啟始頁面改為Products/Index
	修改 _Layout.cshtml裡，首頁的超連結

[V]add CartController, 實作將商品加入購物車
	[V] 顯示購物車資訊，實作增減數量的功能
	[V]實作 結帳作業

[V]留言板

--------------------------------------------------------------
PG負責-會員系統

[V] add 註冊新會員
	[V] 實作註冊功能

[V] 實作 新會員 Email 確認功能
	信裡的網址，為 https://.../Members/ActiveRegister?memberId=7&confirmCode=a07e6e4e7ecd4813aad0894fd258e610 (這裡的id和後面的confirmCode要和資料庫裡的資料對應)

[V] 實作登入登出功能
	只有帳密正確且開通會員才允許登入

[V]要做 Members / Index 會員中心頁，登入成功之後，導向此頁

[V]實作 修改個人基本資料
	不允許修改 email,password; 讓使用者修改姓名、性別、電話

[V] 實作 變更密碼
	要加[[Authorize]

[V] 實作 忘記密碼/重設密碼
	暫時設立 /files/ folder 用來存放 Email 內容

[V]生日優惠券	


==============================================================

{後台功能}

--------------------------------------------------------------

DD負責-後台版面、權限控管、使用者、登入登出

[V]建立EFModels
	add/Models/EFModels
	建立AppDbContext,Connection String,Entity Classes

[V]後台首頁
	add backendstyle/Style.css
	修改BundleConfig
	add Img
	add header登出登入
	add SidebarController
	add Sidebar.cshtml
	add Models/Components/SidebarMenu.cs
	add sidebar-auto-collapse.js

[V]登出登入
	add LoginController
		add Logout
		add ProcessLogin(登入流程驗證票)
		建立認證票
	add Login.cshtml
	add LoginVm
	add ChangePasswordVm
	add ChangePasswordDto
	add ChangePassword(修改密碼)
	add ForgotPasswordVm
	add ForgotPassword(忘記密碼)
	add ResetPasswordVm
	add ResetPassword(重設密碼)
	add ProcessChangePassword(重設密碼確認email存不存在)
	add HandleChangePassword(變更密碼加鹽)
	
[V]權限控管
	add Global.asax Application_AuthenticateRequest
	add Web.config authentication
	add Authorize/MyAuthorizeAttribute(自定義授權)
	add Authorize/CustomPrincipal
	
	使用者Users
	add UsersController
	add ViewModels/UserVm
	add Dtos/UserDto
	add Repositories/UserRepositories
	add Interfaces/IUserRepository
	add Services/UserServices
	add Users/Index.cshtml員工清單
	add Users/Register.cshtml新增員工
	add Users/EditProfile.cshtml編輯員工
	add Users/ActiveRegister.cshtml成功啟用使用者
	add Users/RegisterConfirm確認開通
	add ActiveRegister 驗證確認功能
	
	群組Group(驗證最高權限管理群不能變更刪除)
	add GroupsController
	add ViewModels/GroupVm
	add Dtos/GroupDto
	add Repositories/GroupRepositories
	add Interfaces/IGroupRepository
	add Services/GroupServices
	add Group/Index.cshtml
	
	
	使用者對應群組UserGroups(驗證最高權限使用者不能變更刪除)
	add ViewModels/UserGroupVm
	add Dtos/UserGroupDto
	add Repositories/UserGroupRepositories
	add Interfaces/IUserGroupRepository
	add Services/UserGroupServices

	群組功能權限管理GroupFunctions(驗證最高權限群組功能不能變更刪除)
	add GroupFunctionsController
	add ViewModels/GroupFunctionVm
	add Dtos/GroupFunctionDto
	add Repositories/GroupFunctionRepositories
	add Interfaces/IGroupFunctionRepository
	add Services/GroupFunctionServices

--------------------------------------------------------------

PG負責-會員系統

[V] 會員檢視清單

--------------------------------------------------------------

程負責-通知、訂單、菜單、分析、留言板

[V] 通知

[V] 訂單

[V] 菜單

[V] 分析

[V] 留言板
==============================================================

DD 修改
修改菜單照片存放位置

ImageService存放照片專案

	appsettings.json動態構建路徑
	launchSettings.json關閉自動開啟瀏覽器
	add PhotosController(api)上傳刪除
