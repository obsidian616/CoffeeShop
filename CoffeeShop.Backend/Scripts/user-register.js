$(document).ready(function () {

    $('form').on('submit', function (e) {
        e.preventDefault(); 

        $('.text-danger').text('');


        var isValid = true;


        var account = $('#Account').val().trim();
        if (!account) {
            $('#Account').next('.text-danger').text('帳號必填');
            isValid = false;
        } else if (!isValidEmail(account)) {
            $('#Account').next('.text-danger').text('請輸入有效的電子郵件地址');
            isValid = false;
        }


        var password = $('#Password').val();
        if (!password) {
            $('#Password').next('.text-danger').text('密碼必填');
            isValid = false;
        } else if (password.length < 6) {
            $('#Password').next('.text-danger').text('密碼至少需要6個字符');
            isValid = false;
        }

   
        var confirmPassword = $('#ConfirmPassword').val();
        if (!confirmPassword) {
            $('#ConfirmPassword').next('.text-danger').text('確認密碼必填');
            isValid = false;
        } else if (password !== confirmPassword) {
            $('#ConfirmPassword').next('.text-danger').text('密碼不一致');
            isValid = false;
        }

   
        var name = $('#Name').val().trim();
        if (!name) {
            $('#Name').next('.text-danger').text('姓名必填');
            isValid = false;
        }


        var phone = $('#Phone').val().trim();
        if (!phone) {
            $('#Phone').next('.text-danger').text('聯絡電話必填');
            isValid = false;
        } else if (!/^\d{10}$/.test(phone)) {
            $('#Phone').next('.text-danger').text('聯絡電話必須是10位數字');
            isValid = false;
        }

        if (!isValid) {
            return;
        }

        $.ajax({
            url: $(this).attr('Register'), 
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
       
                    var modal = new bootstrap.Modal(document.getElementById('registerSuccessModal'), {});
                    modal.show();

          
                    document.getElementById('confirmRegisterBtn').addEventListener('click', function () {
                        window.location.href = response.redirectUrl;
                    });
                } else {
         
                    if (response.errors && Array.isArray(response.errors)) {
                        response.errors.forEach(function (error) {
                            $('.text-danger').append('<p>' + error + '</p>'); 
                        });
                    } else {
                        alert("發生錯誤，請稍後再試");
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error('Error: ', error);  
                console.error('Status: ', status);
                console.error('Response: ', xhr.responseText); 
                alert('發生錯誤，請稍後再試');
            }
        });
    });


    $('#Account').on('blur', function () {
        var account = $(this).val().trim();

  
        if (account) {
            checkAccountExist(account);
        }
    });

    $('#Account').on('input', function () {
        var email = $(this).val().trim();
        if (email && !isValidEmail(email)) {
            $('#Account').next('.text-danger').text('請輸入有效的電子郵件地址');
        } else {
            $('#Account').next('.text-danger').text('');
        }
    });

  
    $('#Password, #ConfirmPassword').on('input', function () {
        var password = $('#Password').val();
        var confirmPassword = $('#ConfirmPassword').val();


        if (password && password.length < 6) {
            $('#Password').next('.text-danger').text('密碼至少需要6個字符');
        } else {
            $('#Password').next('.text-danger').text('');
        }


        if (password && confirmPassword && password !== confirmPassword) {
            $('#ConfirmPassword').next('.text-danger').text('密碼不一致');
        } else {
            $('#ConfirmPassword').next('.text-danger').text('');
        }
    });

    $('#Phone').on('input', function () {
        var phone = $(this).val().trim();
        if (phone && !/^\d{10}$/.test(phone)) {
            $('#Phone').next('.text-danger').text('聯絡電話必須是10位數字');
        } else {
            $('#Phone').next('.text-danger').text('');
        }
    });

    function isValidEmail(email) {
        var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    }


    function checkAccountExist(account) {
        $.ajax({
            url: '/Users/IsAccountExist',
            type: 'GET',
            data: { account: account },
            success: function (response) {
                if (response.exists) {
                    $('#Account').next('.text-danger').text('帳號已存在');
                } else {
                    $('#Account').next('.text-danger').text(''); 
                }
            },
            error: function () {
                $('#Account').next('.text-danger').text('無法檢查帳號，請稍後再試');
            }
        });
    }
});
