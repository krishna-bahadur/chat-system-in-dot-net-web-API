using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTest;


[TestClass]
public class AuthenticationServiceTest
{
    [TestMethod]
    public async Task Login_ValidCredential_ReturnsToken()
    {
        var authServiceMock = new Mock<IAuthService>();

        var loginModel = new LoginModel
        {
            Username = "krishna",
            Password = "krishna"
        };

        var expectedResult = new ServiceResult<object>(true, new
        {
            Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoia3J" +
            "pc2huYSIsImp0aSI6IjVkNjQ4ODE1LTViYTEtNGU0MC1hMjc1LTVjZTMxMDUzNzM3YiIsInVpZCI6ImU2ZmMzMjkwLTdmMzUtNDA0NC1hN2M2LTQ3ZWZhOGJjNTkyYyIsImRlcGlkIjoiZDh" +
            "hYTIyOWEtMmU5OC00Y2NiLThmNGUtNmY4YzE1MzhhOWIzIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoidXNlciIsImV4cCI6MT" +
            "Y5NTkwMTUzMywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo0MjAwIn0.WWlCkoTGukzYr-2arEhjfCNBNN5756BkiSSEaxp3LhI",
            RefreshToken = "ki96EyQXA6IgZ5rtsh7ilg8FbKAkW8P/8eOAHVLKV7Gpj5BNevnWd8dtKzX8rhkNRw6pMk6IprQS6LBYE7Y5Gw==",
            Expiration = "2023-09-28T11:45:33Z"
        });

        authServiceMock.Setup(x => x.Login(loginModel)).ReturnsAsync(expectedResult);

        var authService = authServiceMock.Object;

        var result = await authService.Login(loginModel);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task Login_InvalidCredential_ReturnsError()
    {
        var authServiceMock = new Mock<IAuthService>();

        var loginModel = new LoginModel
        {
            Username = "krishna",
            Password = "wrongpassword"
        };

        var expectedResult = new ServiceResult<object>(false, errors: new[] { "Invalid username or password." });

        authServiceMock.Setup(x => x.Login(loginModel)).ReturnsAsync(expectedResult);

        var authService = authServiceMock.Object;

        var result = await authService.Login(loginModel);

        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Data);
    }

}