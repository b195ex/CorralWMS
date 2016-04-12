<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="CorralWMS.SignIn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/><meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <script src="Scripts/jquery-2.2.2.min.js" type="text/javascript"></script>
    <script src="Scripts/bootstrap.min.js" type="text/javascript"></script>
    <title>Sign In - Lynx WMS</title>
    <style>
        body {
            padding-top: 40px;
            padding-bottom: 40px;
            background-color: #eee;
        }
        .form-signin {
            max-width: 330px;
            padding: 15px;
            margin: 0 auto;
        }
        .form-signin .form-signin-heading,.form-signin .checkbox {
            margin-bottom: 10px;
        }
        .form-signin .checkbox {
            font-weight: normal;
        }
        .form-signin .form-control {
            position: relative;
            height: auto;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
            padding: 10px;
            font-size: 16px;
        }
        .form-signin .form-control:focus {
            z-index: 2;
        }
        .form-signin input[type="email"] {
            margin-bottom: -1px;
            border-bottom-right-radius: 0;
            border-bottom-left-radius: 0;
        }
        .form-signin input[type="password"] {
            margin-bottom: 10px;
            border-top-left-radius: 0;
            border-top-right-radius: 0;
        }
    </style>
</head>
<body>
    <div class="container">
        <form id="form1" runat="server" class="form-signin">
            <script type="text/javascript">
                $(document).ready(function () {
                    $(".close").click(function () {
                        $(this).parent().hide()
                    });
                });
            </script>
            <h2 class="form-signin-heading">Please sign in</h2>
            <label for="inputEmail" class="sr-only">User Name</label>
            <asp:TextBox ID="inputEmail" runat="server" CssClass="form-control" placeholder="User name" required autofocus></asp:TextBox>
            <label for="inputPassword" class="sr-only">Password</label>
            <asp:TextBox ID="inputPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password" required></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Sign in" CssClass="btn btn-lg btn-primary btn-block" OnClick="Button1_Click" />
            <div class="alert alert-warning alert-dismissable collapse" role="alert" id="SignInFailed" runat="server">
                <button type="button" class="close" aria-label="Close">
                    <span aria-hidden="true" id="CloseFailAlert" runat="server">&times;</span>
                </button>
                <span>Sign in Failed.</span>
            </div>
            <div class="alert alert-danger alert-dismissable collapse" role="alert" id="LoginException" runat="server">
                <button type="button" class="close" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
                <span>
                    <asp:Label ID="ErrorLabel" runat="server" Text="Label"></asp:Label>
                </span>
            </div>
        </form>
    </div>
</body>
</html>
